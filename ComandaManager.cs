using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace popus_pizzeria
{
    public static class ComandaManager
    {
        public static int ObtenerNumeroCorrelativo()
        {
            int numeroComanda = 1;
            DateTime fechaHoy = DateTime.Today;

            string querySelect = "SELECT UltimoNumero FROM ComandaCounter WHERE Fecha = @fecha";
            string queryInsert = "INSERT INTO ComandaCounter (Fecha, UltimoNumero) VALUES (@fecha, 1)";
            string queryUpdate = "UPDATE ComandaCounter SET UltimoNumero = UltimoNumero + 1 WHERE Fecha = @fecha";
            string querySelectAfterUpdate = "SELECT UltimoNumero FROM ComandaCounter WHERE Fecha = @fecha";

            try
            {
                if (MainClass.con.State == System.Data.ConnectionState.Closed)
                    MainClass.con.Open();

                // Verificar si ya hay una entrada para la fecha de hoy
                using (SqlCommand cmdSelect = new SqlCommand(querySelect, MainClass.con))
                {
                    cmdSelect.Parameters.AddWithValue("@fecha", fechaHoy);
                    var result = cmdSelect.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        // No existe, se crea con valor 1
                        using (SqlCommand cmdInsert = new SqlCommand(queryInsert, MainClass.con))
                        {
                            cmdInsert.Parameters.AddWithValue("@fecha", fechaHoy);
                            cmdInsert.ExecuteNonQuery();
                        }

                        numeroComanda = 1;
                    }
                    else
                    {
                        // Ya existe, incrementamos
                        using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, MainClass.con))
                        {
                            cmdUpdate.Parameters.AddWithValue("@fecha", fechaHoy);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        // Consultamos nuevamente el número actualizado
                        using (SqlCommand cmdFinal = new SqlCommand(querySelectAfterUpdate, MainClass.con))
                        {
                            cmdFinal.Parameters.AddWithValue("@fecha", fechaHoy);
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
