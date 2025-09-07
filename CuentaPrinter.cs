using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using popus_pizzeria.Properties;

namespace popus_pizzeria
{
    /// <summary>
    /// Clase para manejar la impresión de cuentas para el cliente.
    /// Diseñada para impresoras térmicas de 80mm.
    /// </summary>
    public static class CuentaPrinter
    {
        /// <summary>
        /// Imprime la cuenta completa del cliente.
        /// </summary>
        /// <param name="grid">El DataGridView que contiene los productos de la cuenta.</param>
        /// <param name="total">El texto del label que contiene el total.</param>
        /// <param name="infoHeaders">Un diccionario con la información de la mesa, el mesero y el cliente.</param>
        public static void ImprimirCuenta(DataGridView grid, string total, Dictionary<string, string> infoHeaders)
        {
            string nombreImpresora = ConfigurationManager.AppSettings["CuentaPrinter"];
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = nombreImpresora;

            pd.PrintPage += (sender, e) =>
            {
                Font fontTitle = new Font("Arial", 12, FontStyle.Bold);
                Font fontHeader = new Font("Arial", 10, FontStyle.Bold);
                Font fontBody = new Font("Arial", 9);
                Font fontTotal = new Font("Arial", 11, FontStyle.Bold);
                Font fontObservation = new Font("Arial", 8, FontStyle.Italic);

                Graphics g = e.Graphics;
                float yPos = 10; // Reducción del margen superior para empezar a imprimir más arriba.
                float leftMargin = 0;
                float rightMargin = e.PageBounds.Right;
                float lineWidth = rightMargin - leftMargin;

                // --- Dibuja la imagen del logo desde los recursos ---
                Image logo = Resources.popuss1;
                if (logo != null)
                {
                    float logoWidth = 150; // Ajusta el ancho de la imagen según necesites
                    float logoHeight = (float)logo.Height * logoWidth / (float)logo.Width;
                    float xPosImage = leftMargin + (lineWidth - logoWidth) / 2;

                    g.DrawImage(logo, xPosImage, yPos, logoWidth, logoHeight);
                    yPos += logoHeight + 5; // Reducción del espacio post-logo a la mitad
                }

                // --- Dibuja el Título del Recibo ---
                string title = "--- POPU´S PIZZERIA ---";
                SizeF titleSize = g.MeasureString(title, fontTitle);
                g.DrawString(title, fontTitle, Brushes.Black, leftMargin + (lineWidth - titleSize.Width) / 2, yPos);
                yPos += titleSize.Height + 5; // Reducción del espacio post-título a la mitad

                // --- Dibuja la Información General ---
                string billInfo = $"Mesa: {infoHeaders["Mesa"]}\nMozo: {infoHeaders["Mozo"]}\nFecha: {DateTime.Now.ToShortDateString()}\nHora: {DateTime.Now.ToShortTimeString()}";
                if (infoHeaders.ContainsKey("Cliente"))
                {
                    billInfo += $"\nCliente: {infoHeaders["Cliente"]}";
                }
                g.DrawString(billInfo, fontBody, Brushes.Black, leftMargin, yPos);
                yPos += g.MeasureString(billInfo, fontBody).Height + 5; // Reducción del espacio post-información a la mitad

                // --- Dibuja los Encabezados de la Lista de Productos ---
                // Se ajustan las coordenadas para distribuir el espacio de manera equitativa.
                float colQty = leftMargin;
                float colProduct = leftMargin + 30; // 30px de margen después de Cantidad
                float colUnitPrice = leftMargin + 180; // Posición fija para Precio Unitario
                float colTotal = rightMargin; // El total siempre se alinea al margen derecho

                g.DrawString("Cant.", fontHeader, Brushes.Black, colQty, yPos);
                g.DrawString("Producto", fontHeader, Brushes.Black, colProduct, yPos);
                g.DrawString("Precio U.", fontHeader, Brushes.Black, colUnitPrice, yPos);
                g.DrawString("Total", fontHeader, Brushes.Black, colTotal - g.MeasureString("Total", fontHeader).Width, yPos);

                yPos += g.MeasureString("Producto", fontHeader).Height + 1.5f; // Reducción del espaciado de línea
                g.DrawLine(Pens.Black, leftMargin, yPos, rightMargin, yPos);
                yPos += 1.5f; // Reducción del espacio post-línea

                // --- Dibuja los Detalles de Cada Producto ---
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;

                    string productName = row.Cells["dgvPName"].Value.ToString();
                    string quantity = row.Cells["dgvQty"].Value.ToString();
                    string price = double.Parse(row.Cells["dgvPrice"].Value.ToString()).ToString("N2");
                    string rowTotal = double.Parse(row.Cells["dgvAmount"].Value.ToString()).ToString("N2");

                    // Dibuja la cantidad
                    g.DrawString(quantity, fontBody, Brushes.Black, colQty, yPos);
                    // Dibuja el nombre del producto
                    g.DrawString(productName, fontBody, Brushes.Black, colProduct, yPos);
                    // Dibuja el precio unitario (alineado a la derecha de su columna)
                    g.DrawString(price, fontBody, Brushes.Black, colUnitPrice - g.MeasureString(price, fontBody).Width, yPos);
                    // Dibuja el total del producto (alineado a la derecha del ticket)
                    g.DrawString(rowTotal, fontBody, Brushes.Black, colTotal - g.MeasureString(rowTotal, fontBody).Width, yPos);

                    yPos += g.MeasureString(productName, fontBody).Height;
                    yPos += 1.5f; // Reducción del espaciado entre productos
                }

                // --- Dibuja Separador y Total ---
                g.DrawLine(Pens.Black, leftMargin, yPos, rightMargin, yPos);
                yPos += 2.5f; // Reducción del espacio post-línea a la mitad

                string totalText = "TOTAL:";
                string totalValue = total;

                SizeF totalTextSize = g.MeasureString(totalText, fontTotal);
                SizeF totalValueSize = g.MeasureString(totalValue, fontTotal);

                g.DrawString(totalText, fontTotal, Brushes.Black, leftMargin, yPos);
                g.DrawString(totalValue, fontTotal, Brushes.Black, rightMargin - totalValueSize.Width, yPos);

                yPos += fontTotal.Height + 5; // Reducción del espacio antes del pie de página a la mitad
                string footer = "  --- ¡DISFRUTA la VIDA! ---\n--- ¡CON CADA BOCADO! ---";
                SizeF footerSize = g.MeasureString(footer, fontBody);
                g.DrawString(footer, fontBody, Brushes.Black, leftMargin + (lineWidth - footerSize.Width) / 2, yPos);
            };

            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir la cuenta: " + ex.Message);
            }
        }
    }
}
