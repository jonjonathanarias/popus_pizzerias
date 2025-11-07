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

        // EN CLASE: frmAddCustomer.cs

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string qry = "";

            // **CLAVE:** Determinar si es una EDICIÓN (UPDATE) o un NUEVO REGISTRO (INSERT)
            if (CustomerID == 0)
            {
                // --- MODO AGREGAR: Ejecutar INSERT ---
                qry = @"INSERT INTO tblCustomers (Name, Phone, Address, Reference) 
                VALUES (@Name, @Phone, @Address, @Reference)";
            }
            else
            {
                // --- MODO EDITAR: Ejecutar UPDATE ---
                qry = @"UPDATE tblCustomers 
                SET Name = @Name, Phone = @Phone, Address = @Address, Reference = @Reference 
                WHERE CustomerID = @ID";
            }

            // 1. Configurar los parámetros comunes
            SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con);
            cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim()); // Asegúrate de usar los nombres de TextBoxes correctos
            cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
            cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@Reference", txtReference.Text.Trim());

            // 2. Añadir el parámetro @ID solo si es una EDICIÓN
            if (CustomerID > 0)
            {
                cmd.Parameters.AddWithValue("@ID", CustomerID);
            }

            // 3. Ejecutar la consulta
            try
            {
                if (MainClass.con.State != ConnectionState.Open) MainClass.con.Open();

                cmd.ExecuteNonQuery();

                if (CustomerID == 0)
                {
                    // Si fue un INSERT, recuperamos el nuevo ID para pasarlo de vuelta al formulario principal (opcional)
                    CustomerID = (int)MainClass.con.LastInsertRowId;
                }

                MainClass.con.Close();
                MessageBox.Show("Datos guardados exitosamente.");
                this.DialogResult = DialogResult.OK; // Esto cierra el formulario y recarga la lista en frmSearchCustomer
            }
            catch (Exception ex)
            {
                if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
                MessageBox.Show($"Error al guardar los datos del cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
