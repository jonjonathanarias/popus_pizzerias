using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.SqlClient;
using System.Data.SQLite;

namespace popus_pizzeria.Model
{
    public class ArqueoCaja
    {
        public DateTime Fecha { get; }

        public ArqueoCaja(DateTime fecha)
        {
            Fecha = fecha.Date;
        }

        private void CargarTotalesYConteo(string query, ArqueoResultado resultado)
        {
            //using (SqlCommand cmd = new SqlCommand(query, MainClass.con))
            // CAMBIO AQUI: Reemplazar SqlCommand por SQLiteCommand
            using (SQLiteCommand cmd = new SQLiteCommand(query, MainClass.con))
            {
                //cmd.Parameters.AddWithValue("@Fecha", Fecha);
                // LÍNEA CORREGIDA
                cmd.Parameters.AddWithValue("@Fecha", Fecha.ToString("yyyy-MM-dd"));

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                //using (SqlDataReader reader = cmd.ExecuteReader())
                // CAMBIO AQUI: Reemplazar SqlDataReader por SQLiteDataReader
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tipo = reader["orderType"].ToString().Trim().ToLower();
                        double total = Convert.ToDouble(reader["Total"]);
                        int cantidad = Convert.ToInt32(reader["Cantidad"]);

                        switch (tipo)
                        {
                            case "mesas":
                                resultado.TotalMesas = total;
                                resultado.CantMesas = cantidad;
                                break;
                            case "take away":
                            case "takeaway":
                                resultado.TotalTakeAway = total;
                                resultado.CantTakeAway = cantidad;
                                break;
                            case "delivery":
                                resultado.TotalDelivery = total;
                                resultado.CantDelivery = cantidad;
                                break;
                        }

                        resultado.TotalGeneral += total;
                        resultado.CantTotal += cantidad;
                    }
                }
                MainClass.con.Close();
            }
        }

        private void CargarTotalesPorMedioDePago(ArqueoResultado resultado)
        {
            /*string query = @"SELECT paymentMethod, SUM(total) AS Total, COUNT(*) AS Cantidad
                             FROM tblMain
                             WHERE CAST(aDate AS DATE) = @Fecha AND status = 'Pagado'
                             GROUP BY paymentMethod";*/
            // Corrección de la sintaxis SQL: CAST(aDate AS DATE) -> DATE(aDate)
            string query = @"SELECT paymentMethod, SUM(total) AS Total, COUNT(*) AS Cantidad
                             FROM tblMain
                             WHERE DATE(aDate) = @Fecha AND status = 'Pagado'
                             GROUP BY paymentMethod";

            //using (SqlCommand cmd = new SqlCommand(query, MainClass.con))
            // CAMBIO AQUI: Reemplazar SqlCommand por SQLiteCommand
            using (SQLiteCommand cmd = new SQLiteCommand(query, MainClass.con))
            {
                //cmd.Parameters.AddWithValue("@Fecha", Fecha);
                // LÍNEA CORREGIDA
                cmd.Parameters.AddWithValue("@Fecha", Fecha.ToString("yyyy-MM-dd"));

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                //using (SqlDataReader reader = cmd.ExecuteReader())
                // CAMBIO AQUI: Reemplazar SqlDataReader por SQLiteDataReader
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string medio = reader["paymentMethod"].ToString().Trim().ToLower();
                        double total = Convert.ToDouble(reader["Total"]);
                        int cantidad = Convert.ToInt32(reader["Cantidad"]);

                        switch (medio)
                        {
                            case "efectivo":
                                resultado.TotalEfectivo = total;
                                resultado.CantEfectivo = cantidad;
                                break;
                            case "transferencia":
                                resultado.TotalTransferencia = total;
                                resultado.CantTransferencia = cantidad;
                                break;
                            case "qr":
                                resultado.TotalQR = total;
                                resultado.CantQR = cantidad;
                                break;
                            case "tarjeta de credito":
                                resultado.TotalCredito = total;
                                resultado.CantCredito = cantidad;
                                break;
                            case "tarjeta de debito":
                                resultado.TotalDebito = total;
                                resultado.CantDebito = cantidad;
                                break;
                        }
                    }
                }
                MainClass.con.Close();
            }
        }

        public ArqueoResultado ObtenerArqueoParcial()
        {
            var resultado = new ArqueoResultado();

            /*string qry = @"SELECT orderType, SUM(total) AS Total, COUNT(*) AS Cantidad
                            FROM tblMain
                            WHERE CAST(aDate AS DATE) = @Fecha AND status = 'Pagado' 
                            GROUP BY orderType";*/
            string qry = @"SELECT orderType, SUM(total) AS Total, COUNT(*) AS Cantidad
                           FROM tblMain
                           WHERE DATE(aDate) = @Fecha AND status = 'Pagado'
                           GROUP BY orderType";

            CargarTotalesYConteo(qry, resultado);
            CargarTotalesPorMedioDePago(resultado);
            return resultado;
        }

        public ArqueoResultado ObtenerArqueoAcumulado()
        {
            var resultado = new ArqueoResultado();

            /*string qry = @"SELECT orderType, SUM(total) AS Total, COUNT(*) AS Cantidad
                            FROM tblMain
                            WHERE CAST(aDate AS DATE) = @Fecha AND status = 'Pagado'
                            GROUP BY orderType";*/

            string qry = @"SELECT orderType, SUM(total) AS Total, COUNT(*) AS Cantidad
                           FROM tblMain
                           WHERE DATE(aDate) = @Fecha AND status = 'Pagado'
                           GROUP BY orderType";

            CargarTotalesYConteo(qry, resultado);
            CargarTotalesPorMedioDePago(resultado);
            return resultado;
        }

        public ArqueoResultado RealizarArqueo()
        {
            var resultado = new ArqueoResultado();

            /*string qry = @"SELECT orderType, SUM(total) AS Total, COUNT(*) AS Cantidad
                            FROM tblMain
                            WHERE CAST(aDate AS DATE) = @Fecha AND status = 'Pagado' 
                            GROUP BY orderType";*/
            string qry = @"SELECT orderType, SUM(total) AS Total, COUNT(*) AS Cantidad
               FROM tblMain
               WHERE DATE(aDate) = @Fecha AND status = 'Pagado'  
               GROUP BY orderType";

            CargarTotalesYConteo(qry, resultado);
            CargarTotalesPorMedioDePago(resultado);
            return resultado;
        }

        public bool YaFueCerrado()
        {
            string qry = "SELECT COUNT(*) FROM tblArqueoCaja WHERE Fecha = @Fecha AND Cerrado = 1";
            //using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            // CAMBIO AQUI: Reemplazar SqlCommand por SQLiteCommand
            using (SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con))
            {
                //cmd.Parameters.AddWithValue("@Fecha", Fecha);
                // LÍNEA CORREGIDA
                cmd.Parameters.AddWithValue("@Fecha", Fecha.ToString("yyyy-MM-dd"));
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                MainClass.con.Close();
                return count > 0;
            }
        }

        public bool CerrarArqueo(ArqueoResultado resultado)
        {
            string insertQry = @"INSERT INTO tblArqueoCaja (Fecha, TotalMesas, TotalTakeAway, TotalDelivery, TotalGeneral, Cerrado)
                                VALUES (@Fecha, @Mesas, @TakeAway, @Delivery, @Total, 1)";

            //using (SqlCommand cmd = new SqlCommand(insertQry, MainClass.con))
            // CAMBIO AQUI: Reemplazar SqlCommand por SQLiteCommand
            using (SQLiteCommand cmd = new SQLiteCommand(insertQry, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@Fecha", Fecha);
                cmd.Parameters.AddWithValue("@Mesas", resultado.TotalMesas);
                cmd.Parameters.AddWithValue("@TakeAway", resultado.TotalTakeAway);
                cmd.Parameters.AddWithValue("@Delivery", resultado.TotalDelivery);
                cmd.Parameters.AddWithValue("@Total", resultado.TotalGeneral);

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                int rows = cmd.ExecuteNonQuery();
                MainClass.con.Close();

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
            /* string updateQry = @"UPDATE tblMain SET Arqueado = 1 
                                  WHERE CAST(aDate AS DATE) = @Fecha AND status = 'Pagado' AND Arqueado = 0";*/
            string updateQry = @"UPDATE tblMain SET Arqueado = 1 
                                 WHERE DATE(aDate) = @Fecha AND status = 'Pagado' AND Arqueado = 0";

            //using (SqlCommand cmd = new SqlCommand(updateQry, MainClass.con))
            // CAMBIO AQUI: Reemplazar SqlCommand por SQLiteCommand
            using (SQLiteCommand cmd = new SQLiteCommand(updateQry, MainClass.con))
            {
                //cmd.Parameters.AddWithValue("@Fecha", Fecha);
                // LÍNEA CORREGIDA
                cmd.Parameters.AddWithValue("@Fecha", Fecha.ToString("yyyy-MM-dd"));
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                cmd.ExecuteNonQuery();
                MainClass.con.Close();
            }
        }
    }

    public class ArqueoResultado
    {
        public double TotalMesas { get; set; }
        public int CantMesas { get; set; }
        public double TotalTakeAway { get; set; }
        public int CantTakeAway { get; set; }
        public double TotalDelivery { get; set; }
        public int CantDelivery { get; set; }
        public double TotalGeneral { get; set; }
        public int CantTotal { get; set; }

        public double TotalEfectivo { get; set; }
        public int CantEfectivo { get; set; }
        public double TotalTransferencia { get; set; }
        public int CantTransferencia { get; set; }
        public double TotalQR { get; set; }
        public int CantQR { get; set; }
        public double TotalCredito { get; set; }
        public int CantCredito { get; set; }
        public double TotalDebito { get; set; }
        public int CantDebito { get; set; }

        public override string ToString()
        {
            return $"--- Arqueo del Día ---\n" +
                   $"Mesas:      {TotalMesas:C2} ({CantMesas} pedidos)\n" +
                   $"Take Away:  {TotalTakeAway:C2} ({CantTakeAway} pedidos)\n" +
                   $"Delivery:   {TotalDelivery:C2} ({CantDelivery} pedidos)\n" +
                   $"----------------------------\n" +
                   $"TOTAL:      {TotalGeneral:C2} ({CantTotal} pedidos)\n" +
                   $"\n--- Medios de Pago ---\n" +
                   $"Efectivo:         {TotalEfectivo:C2} ({CantEfectivo} pagos)\n" +
                   $"Transferencia:    {TotalTransferencia:C2} ({CantTransferencia} pagos)\n" +
                   $"QR:               {TotalQR:C2} ({CantQR} pagos)\n" +
                   $"Crédito:          {TotalCredito:C2} ({CantCredito} pagos)\n" +
                   $"Débito:           {TotalDebito:C2} ({CantDebito} pagos)";
        }
    }
}
