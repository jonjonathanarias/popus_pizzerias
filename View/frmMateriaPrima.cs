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
    public partial class frmMateriaPrima : SampleAdd
    {
        SqlConnection con = new SqlConnection("Server=DESKTOP-FFIHDBP\\SQLEXPRESS;Database=popus_pizzeria;Trusted_Connection=True; TrustServerCertificate=True;");
        public frmMateriaPrima()
        {
            InitializeComponent();
        }


        private void frmMateriaPrima_Load(object sender, EventArgs e)
        {
            cmbUnidad.Items.AddRange(new string[] { "gr", "ml", "unidad" });
            CargarMateriaPrima();
        }

        private void CargarMateriaPrima()
        {
            string qry = "SELECT * FROM MateriaPrima";
            SqlDataAdapter da = new SqlDataAdapter(qry, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvMateriaPrima.DataSource = dt;
        }

        public override void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtCostoUnitario.Text;
            decimal costo = decimal.Parse(txtCostoUnitario.Text);
            string unidad = cmbUnidad.Text;

            string qry = "INSERT INTO MateriaPrima (nombre, unidad, costoUnitario) VALUES (@nombre, @unidad, @costo)";
            SqlCommand cmd = new SqlCommand(qry, con);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@unidad", unidad);
            cmd.Parameters.AddWithValue("@costo", costo);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            CargarMateriaPrima();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvMateriaPrima.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvMateriaPrima.SelectedRows[0].Cells["id"].Value);
                string qry = "DELETE FROM MateriaPrima WHERE id = @id";
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                CargarMateriaPrima();
            }
        }
    }
}
