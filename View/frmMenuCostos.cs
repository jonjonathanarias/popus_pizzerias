using System;
using System.Drawing;
using System.Windows.Forms;

namespace popus_pizzeria.View
{
    public partial class frmMenuCostos : Form
    {
        Button btnMateriaPrima;
        Button btnProductoMateria;
        Button btnCostoProducto;
        Button btnGestionarProducto; // NUEVO

        public frmMenuCostos()
        {
            InitializeComponent();

            btnMateriaPrima = new Button { Text = "Gestionar Materia Prima", Left = 50, Top = 30, Width = 200, Height = 40 };
            btnGestionarProducto = new Button { Text = "Gestionar Productos", Left = 50, Top = 80, Width = 200, Height = 40 }; // NUEVO
            btnProductoMateria = new Button { Text = "Asignar Insumos a Producto", Left = 50, Top = 130, Width = 200, Height = 40 };
            btnCostoProducto = new Button { Text = "Calcular Costo Producto", Left = 50, Top = 180, Width = 200, Height = 40 };

            btnMateriaPrima.Click += (s, e) => new frmMateriaPrima().ShowDialog();
            btnGestionarProducto.Click += (s, e) => new frmProducto().ShowDialog(); // NUEVO
            btnProductoMateria.Click += (s, e) => new frmProductoMateriaPrima().ShowDialog();
            btnCostoProducto.Click += (s, e) => new frmCostoProducto().ShowDialog();

            this.Controls.Add(btnMateriaPrima);
            this.Controls.Add(btnGestionarProducto); // NUEVO
            this.Controls.Add(btnProductoMateria);
            this.Controls.Add(btnCostoProducto);

            this.Text = "Menú de Costos";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 320;
            this.Height = 280;
        }
    }
}
