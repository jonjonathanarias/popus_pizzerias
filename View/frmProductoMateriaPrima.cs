using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.View
{
    public partial class frmProductoMateriaPrima : Form
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-FFIHDBP\\SQLEXPRESS;Database=popus_pizzeria;Trusted_Connection=True; TrustServerCertificate=True;");

        ComboBox cmbProducto;
        ComboBox cmbMateriaPrima;
        TextBox txtCantidad;
        Button btnAgregar;
        DataGridView dgvRelaciones;
        public frmProductoMateriaPrima()
        {
            InitializeComponent();
            cmbProducto = new ComboBox { Left = 20, Top = 20, Width = 200 };
            cmbMateriaPrima = new ComboBox { Left = 20, Top = 60, Width = 200 };
            txtCantidad = new TextBox { Left = 20, Top = 100, Width = 200, };
            btnAgregar = new Button { Text = "Agregar", Left = 240, Top = 100, Width = 100 };
            dgvRelaciones = new DataGridView { Left = 20, Top = 150, Width = 500, Height = 200 };

            btnAgregar.Click += btnAgregar_Click;
            cmbProducto.SelectedIndexChanged += (s, e) => CargarRelaciones();

            this.Controls.Add(cmbProducto);
            this.Controls.Add(cmbMateriaPrima);
            this.Controls.Add(txtCantidad);
            this.Controls.Add(btnAgregar);
            this.Controls.Add(dgvRelaciones);

            this.Load += frmProductoMateriaPrima_Load;
        }

        private void frmProductoMateriaPrima_Load(object sender, EventArgs e)
        {
            CargarProductos();
            CargarMateriasPrimas();
            CargarRelaciones();
        }

        private void CargarProductos()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT pID, pName FROM products", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbProducto.DataSource = dt;
            cmbProducto.DisplayMember = "pName";
            cmbProducto.ValueMember = "pID";
        }

        private void CargarMateriasPrimas()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT id, nombre FROM MateriaPrima", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbMateriaPrima.DataSource = dt;
            cmbMateriaPrima.DisplayMember = "nombre";
            cmbMateriaPrima.ValueMember = "id";
        }

        private void CargarRelaciones()
        {
            if (cmbProducto.SelectedValue == null) return;
            int productoID = (int)cmbProducto.SelectedValue;
            string qry = @"SELECT pm.id, mp.nombre, pm.cantidadNecesaria
                           FROM ProductoMateriaPrima pm
                           JOIN MateriaPrima mp ON mp.id = pm.materiaPrimaID
                           WHERE pm.productoID = @productoID";

            SqlDataAdapter da = new SqlDataAdapter(qry, con);
            da.SelectCommand.Parameters.AddWithValue("@productoID", productoID);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvRelaciones.DataSource = dt;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            int productoID = (int)cmbProducto.SelectedValue;
            int materiaPrimaID = (int)cmbMateriaPrima.SelectedValue;
            decimal cantidad = decimal.Parse(txtCantidad.Text);

            string qry = "INSERT INTO ProductoMateriaPrima (productoID, materiaPrimaID, cantidadNecesaria) VALUES (@pID, @mpID, @cantidad)";
            SqlCommand cmd = new SqlCommand(qry, con);
            cmd.Parameters.AddWithValue("@pID", productoID);
            cmd.Parameters.AddWithValue("@mpID", materiaPrimaID);
            cmd.Parameters.AddWithValue("@cantidad", cantidad);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            CargarRelaciones();
        }
    }
}
