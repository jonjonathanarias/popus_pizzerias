using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;



namespace popus_pizzeria
{
    public static class ComandaManager
    {
        public static int ObtenerNumeroCorrelativo()
        {
            int numeroComanda = 1;
            DateTime fechaHoy = DateTime.Today;
            // Convertimos la fecha a un formato de texto estándar para SQLite
            string fechaHoyStr = fechaHoy.ToString("yyyy-MM-dd");

            string querySelect = "SELECT UltimoNumero FROM ComandaCounter WHERE Fecha = @fecha";
            string queryInsert = "INSERT INTO ComandaCounter (Fecha, UltimoNumero) VALUES (@fecha, 1)";
            string queryUpdate = "UPDATE ComandaCounter SET UltimoNumero = UltimoNumero + 1 WHERE Fecha = @fecha";
            string querySelectAfterUpdate = "SELECT UltimoNumero FROM ComandaCounter WHERE Fecha = @fecha";

            try
            {
                if (MainClass.con.State == System.Data.ConnectionState.Closed)
                    MainClass.con.Open();

                // Verificar si ya hay una entrada para la fecha de hoy
                // using (SqlCommand cmdSelect = new SqlCommand(querySelect, MainClass.con))
                // 1. Verificar si ya hay una entrada para la fecha de hoy
                // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
                using (SQLiteCommand cmdSelect = new SQLiteCommand(querySelect, MainClass.con))
                {
                    cmdSelect.Parameters.AddWithValue("@fecha", fechaHoyStr);
                    var result = cmdSelect.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        // No existe, se crea con valor 1
                        //using (SqlCommand cmdInsert = new SqlCommand(queryInsert, MainClass.con))
                        // 2. No existe, se crea con valor 1
                        // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
                        using (SQLiteCommand cmdInsert = new SQLiteCommand(queryInsert, MainClass.con))
                        {
                            cmdInsert.Parameters.AddWithValue("@fecha", fechaHoyStr);
                            cmdInsert.ExecuteNonQuery();
                        }

                        numeroComanda = 1;
                    }
                    else
                    {
                        // Ya existe, incrementamos
                        //using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, MainClass.con))
                        // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
                        using (SQLiteCommand cmdUpdate = new SQLiteCommand(queryUpdate, MainClass.con))
                        {
                            cmdUpdate.Parameters.AddWithValue("@fecha", fechaHoyStr);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        // Consultamos nuevamente el número actualizado
                        //using (SqlCommand cmdFinal = new SqlCommand(querySelectAfterUpdate, MainClass.con))
                        // 4. Consultamos nuevamente el número actualizado
                        // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
                        using (SQLiteCommand cmdFinal = new SQLiteCommand(querySelectAfterUpdate, MainClass.con))
                        {
                            cmdFinal.Parameters.AddWithValue("@fecha", fechaHoyStr);
                            var nuevoValor = cmdFinal.ExecuteScalar();
                            numeroComanda = Convert.ToInt32(nuevoValor);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al obtener número de comanda:\n" + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                if (MainClass.con.State == System.Data.ConnectionState.Open)
                    MainClass.con.Close();
            }

            return numeroComanda;
        }
    }
}
