using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.View
{
    public partial class frmMenuCostos : Form
    {

        Button btnMateriaPrima;
        Button btnProductoMateria;
        Button btnCostoProducto;
        public frmMenuCostos()
        {
            InitializeComponent();

            // Renombrando para claridad, si btnGuardar era para Materia Prima
            btnMateriaPrima = new Button { Text = "Gestionar Materia Prima", Left = 50, Top = 30, Width = 200, Height = 40 };
            btnProductoMateria = new Button { Text = "Asignar Insumos a Producto", Left = 50, Top = 80, Width = 200, Height = 40 };
            btnCostoProducto = new Button { Text = "Calcular Costo Producto", Left = 50, Top = 130, Width = 200, Height = 40 };

            // Asignar eventos de click a los nuevos formularios
            btnMateriaPrima.Click += (s, e) => new frmMateriaPrima().ShowDialog();
            btnProductoMateria.Click += (s, e) => new frmProductoMateriaPrima().ShowDialog();
            btnCostoProducto.Click += (s, e) => new frmCostoProducto().ShowDialog(); // Ahora abriremos frmCostoProducto

            this.Controls.Add(btnMateriaPrima);
            this.Controls.Add(btnProductoMateria);
            this.Controls.Add(btnCostoProducto);

            this.Text = "Menú de Costos";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 320;
            this.Height = 240;
        }
    }
}
