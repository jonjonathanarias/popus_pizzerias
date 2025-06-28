using popus_pizzeria.Model;
using popus_pizzeria.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        //Metodos para agregar controles en frmMain

        public void agregarControles(Form f)
        {
            CenterPanel.Controls.Clear();
            f.Dock = DockStyle.Fill;
            f.TopLevel = false;
            CenterPanel.Controls.Add(f);
            f.Show();
        }


        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void guna2Button2_Click(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            lblUser.Text = MainClass.USER;
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            agregarControles(new frmHome());
        }

        private void btnCategorias_Click(object sender, EventArgs e)
        {
            agregarControles(new frmCategoriaView());
        }

        private void btnMesas_Click(object sender, EventArgs e)
        {
            agregarControles(new frmTableDesigner());
        }

        private void btnEmpleados_Click(object sender, EventArgs e)
        {
            agregarControles(new frmStaffView());
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            agregarControles(new frmProductsView());
        }

        private void btnPos_Click(object sender, EventArgs e)
        {
            frmPOS frm = new frmPOS();
            frm.Show();
        }

        private void btnCocina_Click(object sender, EventArgs e)
        {
            agregarControles(new frmKitchenView());
        }

        private void btnConfiguraciones_Click(object sender, EventArgs e)
        {
            agregarControles(new frmArqueoCaja(DateTime.Today));
        }

        private void DiseñoMesa_Click(object sender, EventArgs e)
        {
            
        }

        private void ExcelCosto_Click(object sender, EventArgs e)
        {
            agregarControles(new frmCostosExcel());
        }
    }
}
