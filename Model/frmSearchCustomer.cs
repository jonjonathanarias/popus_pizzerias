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
using System.Windows.Forms.VisualStyles;

namespace popus_pizzeria.Model
{
    public partial class frmSearchCustomer : SampleView
    {
        public frmSearchCustomer()
        {
            InitializeComponent();
        }

        public int CustomerID = 0;
        public string CustomerName = "", Phone = "", Address = "", Reference = "";
        public string OrderType = "Delivery";

        private void frmSearchCustomer_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            frmAddCustomer frm = new frmAddCustomer();
            frm.orderType = OrderType;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                CustomerID = frm.CustomerID;
            }
            LoadCustomers();
        }

        public override void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            LoadCustomers(txtBuscar.Text.Trim());
        }

        private void LoadCustomers(string search = "")
        {
            string qry = @"SELECT * FROM tblCustomers
                       WHERE Name LIKE @search OR Phone LIKE @search OR Address LIKE @search";

            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            cmd.Parameters.AddWithValue("@search", "%" + search + "%");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvCustomers.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                dgvCustomers.Rows.Add(row["CustomerID"], row["Name"], row["Phone"], row["Address"], row["Reference"]);
            }
        }

        private void dgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                CustomerID = Convert.ToInt32(dgvCustomers.Rows[e.RowIndex].Cells[0].Value);
                CustomerName = dgvCustomers.Rows[e.RowIndex].Cells[1].Value.ToString();
                Phone = dgvCustomers.Rows[e.RowIndex].Cells[2].Value.ToString();
                Address = dgvCustomers.Rows[e.RowIndex].Cells[3].Value.ToString();
                Reference = dgvCustomers.Rows[e.RowIndex].Cells[4].Value.ToString();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

    }
}
