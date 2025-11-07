using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using popus_pizzeria.View;

namespace popus_pizzeria.Model
{
    public partial class frmSearchCustomer : SampleView
    {
        public frmSearchCustomer()
        {
            InitializeComponent();
        }

        // Propiedades públicas para la selección
        public int CustomerID { get; set; } = 0;
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string Reference { get; set; } = "";
        public string OrderType { get; set; } = "Delivery";

        private void frmSearchCustomer_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            // Asegúrate de que este evento CellClick esté asociado en el diseñador o aquí.
            if (dgvCustomers.DataSource == null) // Evita agregar el manejador dos veces si ya está en el diseñador
            {
                dgvCustomers.CellClick += dgvCustomers_CellClick;
            }
        }

        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            // Resetear CustomerID para forzar el modo AGREGAR en frmAddCustomer
            CustomerID = 0;
            frmAddCustomer frm = new frmAddCustomer();
            frm.orderType = OrderType;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Se usa el ID retornado del formulario para selección, si es necesario
                CustomerID = frm.CustomerID;
            }
            LoadCustomers();
        }

        public override void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            LoadCustomers(txtBuscar.Text.Trim());
        }

        /// <summary>
        /// Carga los datos de los clientes en el DataGridView.
        /// </summary>
        private void LoadCustomers(string search = "")
        {
            string qry = @"SELECT CustomerID, Name, Phone, Address, Reference FROM tblCustomers
                           WHERE Name LIKE @search OR Phone LIKE @search OR Address LIKE @search OR Reference LIKE @search";

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con);
                cmd.Parameters.AddWithValue("@search", $"%{search}%"); // Interpolación de cadena para búsqueda

                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvCustomers.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    // Asumiendo que las columnas del DataGridView son: 
                    // [dgvCustomerID], [dgvName], [dgvPhone], [dgvAddress], [dgvReference], [dgveditCustomer], [dgvdeleteCustomer]
                    dgvCustomers.Rows.Add(
                        row["CustomerID"],
                        row["Name"],
                        row["Phone"],
                        row["Address"],
                        row["Reference"]
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento para seleccionar cliente haciendo doble clic (para seleccionar y cerrar)
        private void dgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // **Asegúrate que Cells[0] en la carga sea el CustomerID.**
                CustomerID = Convert.ToInt32(dgvCustomers.Rows[e.RowIndex].Cells[0].Value);
                CustomerName = dgvCustomers.Rows[e.RowIndex].Cells[1].Value.ToString();
                Phone = dgvCustomers.Rows[e.RowIndex].Cells[2].Value.ToString();
                Address = dgvCustomers.Rows[e.RowIndex].Cells[3].Value.ToString();
                Reference = dgvCustomers.Rows[e.RowIndex].Cells[4].Value.ToString();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Maneja los clics en las columnas de botones (Editar/Eliminar).
        /// </summary>
        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvCustomers.CurrentCell == null) return;

            // RECUPERACIÓN DEL ID USANDO EL NOMBRE DE COLUMNA ASUMIDO: dgvCustomerID
            int customerId = Convert.ToInt32(dgvCustomers.Rows[e.RowIndex].Cells["dgvCustomerID"].Value);
            string colName = dgvCustomers.CurrentCell.OwningColumn.Name;

            // ------------------------------------
            // 1. Lógica para EDITAR (dgveditCustomer)
            // ------------------------------------
            if (colName == "dgveditCustomer")
            {
                if (MessageBox.Show("¿Está seguro que desea editar el cliente?", "Confirmar Edición", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    frmAddCustomer frm = new frmAddCustomer();

                    // 1. Asignar el ID (Clave para forzar el modo UPDATE en frmAddCustomer)
                    frm.CustomerID = customerId;

                    // 2. Pre-llenar el formulario (asumiendo que las celdas 1-4 son Name, Phone, Address, Reference)
                    frm.txtName.Text = dgvCustomers.Rows[e.RowIndex].Cells[1].Value.ToString();
                    frm.txtPhone.Text = dgvCustomers.Rows[e.RowIndex].Cells[2].Value.ToString();
                    frm.txtAddress.Text = dgvCustomers.Rows[e.RowIndex].Cells[3].Value.ToString();
                    frm.txtReference.Text = dgvCustomers.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        LoadCustomers(); // Recargar la lista después de la edición
                    }
                }
            }

            // ------------------------------------
            // 2. Lógica para ELIMINAR (dgvdeleteCustomer)
            // ------------------------------------
            else if (colName == "dgvdeleteCustomer")
            {
                // Confirma la eliminación directamente con el usuario
                if (MessageBox.Show("¿Está seguro que desea eliminar el cliente permanentemente?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string qry = "DELETE FROM tblCustomers WHERE CustomerID = @id";

                    try
                    {
                        SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con);
                        cmd.Parameters.AddWithValue("@id", customerId);

                        // Abre la conexión si está cerrada
                        if (MainClass.con.State != ConnectionState.Open) MainClass.con.Open();

                        cmd.ExecuteNonQuery();

                        // Cierra la conexión
                        MainClass.con.Close();

                        MessageBox.Show("Cliente Eliminado Correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomers(); // Recarga la lista de clientes
                    }
                    catch (Exception ex)
                    {
                        // Asegura que la conexión se cierre en caso de error
                        if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
                        MessageBox.Show($"Error al eliminar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}