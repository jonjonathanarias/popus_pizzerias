using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        /// <param name="nombreImpresora">El nombre de la impresora de cuentas.</param>
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
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                float rightMargin = e.MarginBounds.Right;
                float lineWidth = rightMargin - leftMargin;

                // --- Draw the Bill Title ---
                string title = "--- POPU´S PIZZERIA ---";
                SizeF titleSize = g.MeasureString(title, fontTitle);
                g.DrawString(title, fontTitle, Brushes.Black, leftMargin + (lineWidth - titleSize.Width) / 2, yPos);
                yPos += titleSize.Height + 10;

                // --- Draw General Information ---
                string billInfo = $"Mesa: {infoHeaders["Mesa"]}\nMozo: {infoHeaders["Mozo"]}\nFecha: {DateTime.Now.ToShortDateString()}\nHora: {DateTime.Now.ToShortTimeString()}";
                if (infoHeaders.ContainsKey("Cliente"))
                {
                    billInfo += $"\nCliente: {infoHeaders["Cliente"]}";
                }
                g.DrawString(billInfo, fontBody, Brushes.Black, leftMargin, yPos);
                yPos += g.MeasureString(billInfo, fontBody).Height + 10;

                // --- Draw Product List Headers ---
                float colQty = leftMargin;
                float colProduct = leftMargin + 60;
                float colUnitPrice = leftMargin + 200;
                float colTotal = rightMargin;

                g.DrawString("Cant.", fontHeader, Brushes.Black, colQty, yPos);
                g.DrawString("Producto", fontHeader, Brushes.Black, colProduct, yPos);
                g.DrawString("Precio U.", fontHeader, Brushes.Black, colUnitPrice, yPos);
                g.DrawString("Total", fontHeader, Brushes.Black, colTotal - g.MeasureString("Total", fontHeader).Width, yPos);
                yPos += g.MeasureString("Producto", fontHeader).Height + 3;
                g.DrawLine(Pens.Black, leftMargin, yPos, rightMargin, yPos);
                yPos += 3;

                // --- Draw Individual Product Details ---
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;

                    string productName = row.Cells["dgvPName"].Value.ToString();
                    string quantity = row.Cells["dgvQty"].Value.ToString();
                    string price = double.Parse(row.Cells["dgvPrice"].Value.ToString()).ToString("N2");
                    string rowTotal = double.Parse(row.Cells["dgvAmount"].Value.ToString()).ToString("N2");
                   // string observation = row.Cells["dgvObs"].Value?.ToString() ?? string.Empty;

                    g.DrawString(quantity, fontBody, Brushes.Black, colQty, yPos);
                    g.DrawString(productName, fontBody, Brushes.Black, colProduct, yPos);
                    g.DrawString(price, fontBody, Brushes.Black, colUnitPrice, yPos);
                    g.DrawString(rowTotal, fontBody, Brushes.Black, colTotal - g.MeasureString(rowTotal, fontBody).Width, yPos);
                    yPos += g.MeasureString(productName, fontBody).Height;

                   // if (!string.IsNullOrEmpty(observation))
                   // {
                   //     g.DrawString($"  - Obs: {observation}", fontObservation, Brushes.Black, leftMargin + 10, yPos);
                   //     yPos += g.MeasureString("  - Obs:", fontObservation).Height;
                   // }
                    
                    yPos += 3;
                }

                // --- Draw Separator and Total ---
                g.DrawLine(Pens.Black, leftMargin, yPos, rightMargin, yPos);
                yPos += 5;

                string totalText = "TOTAL:";
                string totalValue = total;

                SizeF totalTextSize = g.MeasureString(totalText, fontTotal);
                SizeF totalValueSize = g.MeasureString(totalValue, fontTotal);

                g.DrawString(totalText, fontTotal, Brushes.Black, leftMargin, yPos);
                g.DrawString(totalValue, fontTotal, Brushes.Black, rightMargin - totalValueSize.Width, yPos);

                yPos += fontTotal.Height + 10;
                string footer = "--- ¡DISFRUTA DE LA VIDA! ---\n --- ¡CON CADA BOCADO! ---";
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
