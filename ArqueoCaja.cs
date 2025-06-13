using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace popus_pizzeria.Model
{
    public class ArqueoCaja
    {
        public DateTime Fecha { get; }

        public ArqueoCaja(DateTime fecha)
        {
            Fecha = fecha.Date;
        }

        public ArqueoResultado RealizarArqueo()
        {
            var resultado = new ArqueoResultado();

            string qry = @"SELECT orderType, SUM(total) AS Total
                       FROM tblMain
                       WHERE CAST(aDate AS DATE) = @Fecha AND status = 'Pagado' 
                       GROUP BY orderType";

            using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@Fecha", Fecha);

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tipo = reader["orderType"].ToString().Trim().ToLower();
                        double total = Convert.ToDouble(reader["Total"]);

                        Console.WriteLine($"Tipo detectado: {tipo} - Total: {total}");
                        switch (tipo)
                        {
                            case "mesas":
                                resultado.TotalMesas = total;
                                break;
                            case "take away":
                            case "takeaway":
                                resultado.TotalTakeAway = total;
                                break;
                            case "delivery":
                                resultado.TotalDelivery = total;
                                break;
                        }

                        resultado.TotalGeneral += total;
                    }
                }
                MainClass.con.Close();
            }


            return resultado;

        }

        public bool YaFueCerrado()
        {
            string qry = "SELECT COUNT(*) FROM tblArqueoCaja WHERE Fecha = @Fecha AND Cerrado = 1";
            using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@Fecha", Fecha);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                MainClass.con.Close();
                return count > 0;
            }
        }

        public bool CerrarArqueo(ArqueoResultado resultado)
        {
            Console.WriteLine("Intentando cerrar arqueo...");
            Console.WriteLine($"Fecha: {Fecha.ToShortDateString()}");
            Console.WriteLine($"Mesas: {resultado.TotalMesas}, TakeAway: {resultado.TotalTakeAway}, Delivery: {resultado.TotalDelivery}");

            string insertQry = @"INSERT INTO tblArqueoCaja (Fecha, TotalMesas, TotalTakeAway, TotalDelivery, TotalGeneral, Cerrado)
                         VALUES (@Fecha, @Mesas, @TakeAway, @Delivery, @Total, 1)";

            using (SqlCommand cmd = new SqlCommand(insertQry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@Fecha", Fecha);
                cmd.Parameters.AddWithValue("@Mesas", resultado.TotalMesas);
                cmd.Parameters.AddWithValue("@TakeAway", resultado.TotalTakeAway);
                cmd.Parameters.AddWithValue("@Delivery", resultado.TotalDelivery);
                cmd.Parameters.AddWithValue("@Total", resultado.TotalGeneral);

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                int rows = cmd.ExecuteNonQuery();
                MainClass.con.Close();

                Console.WriteLine($"Filas insertadas: {rows}");

                if (rows > 0)
                {
                    MarcarOrdenesComoArqueadas();
                    return true;
                }

                return false;
            }
        }


        private void MarcarOrdenesComoArqueadas()
        {
            string updateQry = @"UPDATE tblMain SET Arqueado = 1 WHERE CAST(aDate AS DATE) = @Fecha AND status = 'Pagado' AND Arqueado = 0";

            using (SqlCommand cmd = new SqlCommand(updateQry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@Fecha", Fecha);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                cmd.ExecuteNonQuery();
                MainClass.con.Close();
            }
        }
    }


    public class ArqueoResultado
    {
        public double TotalMesas { get; set; }
        public double TotalTakeAway { get; set; }
        public double TotalDelivery { get; set; }
        public double TotalGeneral { get; set; }

        public override string ToString()
        {
            return $"--- Arqueo del Día ---\n" +
                   $"Mesas:      {TotalMesas:C2}\n" +
                   $"Take Away:   {TotalTakeAway:C2}\n" +
                   $"Delivery:    {TotalDelivery:C2}\n" +
                   $"----------------------------\n" +
                   $"TOTAL:       {TotalGeneral:C2}";
        }
    }
}
