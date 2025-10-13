using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.Model
{
    public partial class frmAddCustomer : Form
    {
        public frmAddCustomer()
        {
            InitializeComponent();
        }

        public string orderType = "";
        public int CustomerID = 0;

        public string CustomerName = "", Phone = "", Address = "", Reference = "";

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            CustomerName = txtName.Text.Trim();
            Phone = txtPhone.Text.Trim();
            Address = txtAddress.Text.Trim();
            Reference = txtReference.Text.Trim();

            // Guardar nuevo o actualizar cliente
            // string qry = "INSERT INTO tblCustomers (Name, Phone, Address, Reference) VALUES (@Name, @Phone, @Address, @Reference); SELECT SCOPE_IDENTITY();";
           
            // Guardar nuevo o actualizar cliente
            // CAMBIAR: Usamos la función de SQLite last_insert_rowid() en lugar de SCOPE_IDENTITY()
            string qry = "INSERT INTO tblCustomers (Name, Phone, Address, Reference) VALUES (@Name, @Phone, @Address, @Reference); SELECT last_insert_rowid();";

            //SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            // CAMBIAR: SqlCommand -> SQLiteCommand
            SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con);
            cmd.Parameters.AddWithValue("@Name", CustomerName);
            cmd.Parameters.AddWithValue("@Phone", Phone);
            cmd.Parameters.AddWithValue("@Address", Address);
            cmd.Parameters.AddWithValue("@Reference", Reference);

            if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
            CustomerID = Convert.ToInt32(cmd.ExecuteScalar());
            if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    

        private void frmAddCustomer_Load(object sender, EventArgs e)
        {
            if (orderType == "Take Away")
            {
                // Ocultar campos no necesarios
                txtAddress.Visible = false;
                txtPhone.Visible = false;
                txtReference.Visible = false;

                // También ocultar las etiquetas si las hay
                lblAddress.Visible = false;
                lblPhone.Visible = false;
                lblReference.Visible = false;
            }
        }


    }
}
