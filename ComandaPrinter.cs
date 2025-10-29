using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace popus_pizzeria
{
    public static class ComandaPrinter
    {
        public static void ImprimirTexto(string texto)
        {
            // 1. Obtener la configuración de la impresora
            string nombreImpresora = ConfigurationManager.AppSettings["ComandaPrinter"];
            if (string.IsNullOrEmpty(nombreImpresora))
            {
                MessageBox.Show("Error: Nombre de impresora no configurado en App.config.");
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = nombreImpresora;

            // 2. Configura evento para imprimir el texto
            pd.PrintPage += (sender, ev) =>
            {
                // **AJUSTE CLAVE PARA 58MM (30 CARACTERES)**
                // Usamos Consolas o Courier New, fuentes monoespaciadas.
                // Tamaño 10 o 9 es apropiado para impresoras térmicas.
                Font font = new Font("Consolas", 9, FontStyle.Regular);

                float leftMargin = 2; // Margen izquierdo pequeño
                float topMargin = 5;  // Margen superior pequeño

                // Calcular el ancho de un carácter
                SizeF charSize = ev.Graphics.MeasureString("W", font);

                // Un ancho de 30-32 caracteres * el ancho de un carácter
                // 30 caracteres * ~5.5 pixeles/carácter = ~165 pixeles
                float requiredWidth = charSize.Width * 32;

                // Definimos un área de dibujo que limita explícitamente el ancho.
                // Esto asegura que DrawString respete los saltos de línea (\r\n)
                // y no intente expandir las líneas a un ancho de página completo.
                RectangleF printArea = new RectangleF(
                    leftMargin,
                    topMargin,
                    requiredWidth, // Anchura limitada a 32 caracteres
                    ev.PageBounds.Height * 2 // Altura amplia para que quepa todo
                );

                // Dibuja el texto dentro del área definida.
                ev.Graphics.DrawString(
                    texto,
                    font,
                    Brushes.Black,
                    printArea,
                    StringFormat.GenericTypographic // Mejor para texto preformateado
                );

                // Indica que no hay más páginas
                ev.HasMorePages = false;
            };

            // 3. Imprimir el documento
            try
            {
                pd.Print();
            }
            catch (InvalidPrinterException)
            {
                MessageBox.Show($"Error: La impresora '{nombreImpresora}' no es válida o no está conectada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general al imprimir la comanda: " + ex.Message);
            }
        }
    }
}