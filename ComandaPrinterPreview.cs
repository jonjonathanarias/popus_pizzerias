using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace popus_pizzeria
{
    public class ComandaPrinterPreview
    {
        public static void MostrarComandaComoImagen(string texto)
        {
            // Configurar ancho de 5.7cm (2.24 pulgadas) a 203 DPI (típico de impresora térmica)
            float cmToInch = 2.54f;
            float widthInch = 5.7f / cmToInch;
            int dpi = 203;
            int imageWidth = (int)(widthInch * dpi);
            Font font = new Font("Consolas", 10);

            // Medir altura necesaria
            Bitmap tempBitmap = new Bitmap(1, 1);
            Graphics tempGraphics = Graphics.FromImage(tempBitmap);
            SizeF textSize = tempGraphics.MeasureString(texto, font, imageWidth);
            int imageHeight = (int)Math.Ceiling(textSize.Height);

            // Crear imagen final
            Bitmap bmp = new Bitmap(imageWidth, imageHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(texto, font, Brushes.Black, new RectangleF(0, 0, imageWidth, imageHeight));
            }

            // Mostrar en un PictureBox dentro de un Form
            Form previewForm = new Form
            {
                Text = "Vista previa de Comanda",
                Width = imageWidth + 40,
                Height = Math.Min(imageHeight + 60, 800),
                AutoScroll = true
            };

            PictureBox pb = new PictureBox
            {
                Image = bmp,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            previewForm.Controls.Add(pb);
            previewForm.ShowDialog();
        }
    }
}
