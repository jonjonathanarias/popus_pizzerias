using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace popus_pizzeria
{
    public static class ComandaPrinter
    {
        public static void ImprimirTexto(string texto, string nombreImpresora = "ImpresoraZJ-58")
        {
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = nombreImpresora;

            // Configura evento para imprimir el texto
            pd.PrintPage += (sender, ev) =>
            {
                Font font = new Font("Consolas", 10); // Fuente monoespaciada para impresoras térmicas
                float leftMargin = 5;
                float topMargin = 5;

                ev.Graphics.DrawString(texto, font, Brushes.Black, new RectangleF(leftMargin, topMargin, ev.PageBounds.Width, ev.PageBounds.Height));
            };

            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message);
            }
        }
    }
}
