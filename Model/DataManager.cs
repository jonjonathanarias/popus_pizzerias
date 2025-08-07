using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace popus_pizzeria.Model
{
    public static class DataManager
    {
        public static List<Producto> GetProductos()
        {
            var lista = new List<Producto>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Productos", MainClass.con);

            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Producto(
                    (int)dr["Id"],
                    dr["Nombre"].ToString(),
                    dr["Descripcion"].ToString()
                ));
            }

            dr.Close();
            MainClass.con.Close();
            return lista;
        }

        public static List<MateriaPrima> GetMateriasPrimas()
        {
            var lista = new List<MateriaPrima>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM MateriasPrimas", MainClass.con);

            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new MateriaPrima(
                    (int)dr["Id"],
                    dr["Nombre"].ToString(),
                    dr["UnidadMedida"].ToString(),
                    (decimal)dr["CostoPorUnidad"]
                ));
            }

            dr.Close();
            MainClass.con.Close();
            return lista;
        }

        public static List<ProductoMateriaPrima> GetRecetasPorProducto(int productoId)
        {
            var lista = new List<ProductoMateriaPrima>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Recetas WHERE ProductoId = @ProductoId", MainClass.con);
            cmd.Parameters.AddWithValue("@ProductoId", productoId);

            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ProductoMateriaPrima(
                    (int)dr["ProductoId"],
                    (int)dr["MateriaPrimaId"],
                    (decimal)dr["CantidadUsada"]
                ));
            }

            dr.Close();
            MainClass.con.Close();
            return lista;
        }

        public static void AgregarMateriaPrima(MateriaPrima mp)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO MateriasPrimas (Nombre, UnidadMedida, CostoPorUnidad) VALUES (@n, @u, @c)", MainClass.con);
            cmd.Parameters.AddWithValue("@n", mp.Nombre);
            cmd.Parameters.AddWithValue("@u", mp.UnidadMedida);
            cmd.Parameters.AddWithValue("@c", mp.CostoPorUnidad);

            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            cmd.ExecuteNonQuery();
            MainClass.con.Close();
        }

        public static void AgregarReceta(ProductoMateriaPrima receta)
        {
            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            var exists = new SqlCommand("SELECT COUNT(*) FROM Recetas WHERE ProductoId = @p AND MateriaPrimaId = @m", MainClass.con);
            exists.Parameters.AddWithValue("@p", receta.ProductoId);
            exists.Parameters.AddWithValue("@m", receta.MateriaPrimaId);

            int count = (int)exists.ExecuteScalar();

            if (count == 0)
            {
                var insert = new SqlCommand("INSERT INTO Recetas (ProductoId, MateriaPrimaId, CantidadUsada) VALUES (@p, @m, @c)", MainClass.con);
                insert.Parameters.AddWithValue("@p", receta.ProductoId);
                insert.Parameters.AddWithValue("@m", receta.MateriaPrimaId);
                insert.Parameters.AddWithValue("@c", receta.CantidadUsada);
                insert.ExecuteNonQuery();
            }
            else
            {
                var update = new SqlCommand("UPDATE Recetas SET CantidadUsada = @c WHERE ProductoId = @p AND MateriaPrimaId = @m", MainClass.con);
                update.Parameters.AddWithValue("@p", receta.ProductoId);
                update.Parameters.AddWithValue("@m", receta.MateriaPrimaId);
                update.Parameters.AddWithValue("@c", receta.CantidadUsada);
                update.ExecuteNonQuery();
            }

            MainClass.con.Close();
        }

        public static decimal CalcularCostoProducto(int productoId)
        {
            decimal total = 0;
            SqlCommand cmd = new SqlCommand(@"
                SELECT r.CantidadUsada, m.CostoPorUnidad 
                FROM Recetas r 
                JOIN MateriasPrimas m ON r.MateriaPrimaId = m.Id 
                WHERE r.ProductoId = @id", MainClass.con);
            cmd.Parameters.AddWithValue("@id", productoId);

            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                decimal cantidad = (decimal)dr["CantidadUsada"];
                decimal costoUnidad = (decimal)dr["CostoPorUnidad"];
                total += cantidad * costoUnidad;
            }

            dr.Close();
            MainClass.con.Close();
            return total;
        }

        public static void EliminarIngredienteDeReceta(int productoId, int materiaPrimaId)
        {
            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM Recetas WHERE ProductoId = @p AND MateriaPrimaId = @m", MainClass.con);
            cmd.Parameters.AddWithValue("@p", productoId);
            cmd.Parameters.AddWithValue("@m", materiaPrimaId);
            cmd.ExecuteNonQuery();

            MainClass.con.Close();
        }

        public static void AgregarProducto(Producto producto)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Productos (Nombre, Descripcion) VALUES (@n, @d)", MainClass.con);
            cmd.Parameters.AddWithValue("@n", producto.Nombre);
            cmd.Parameters.AddWithValue("@d", producto.Descripcion);

            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            cmd.ExecuteNonQuery();
            MainClass.con.Close();
        }


    }
}
