using Guna.UI2.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guna.UI2.WinForms.Enums;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace popus_pizzeria.Model
{
    public partial class frmPOS : Form
    {
        public frmPOS()
        {
            InitializeComponent();


        }

        public int MainId = 0;
        public string OrderType = "";
        public int CustomerID = 0;

        private int itemSpacing = 5;
        private int itemWidth = 200; // Ajusta al ancho de tu ucProduct
        private int itemHeight = 180; // Ajusta al alto de tu ucProduct
        private int currentX = 0;
        private int currentY = 0;


        private void ClearForm()
        {
            MainId = 0;
            CustomerID = 0;
            OrderType = "";

            btnImprimirCuenta.Rows.Clear();
            lblTotal.Text = "0.00";


            lblTable.Text = "";
            lblTable.Visible = false;
            lblWaiter.Text = "";
            lblWaiter.Visible = false;


            lblCustomer.Text = "";
            lblCustomer.Visible = false;

            txtBuscar.Text = "";

            foreach (Control control in CategoryPanel.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2Button btn)
                {
                    if (btn.Text == "Todas las Categorias")
                    {
                        btn.Checked = true;
                        _Click(btn, EventArgs.Empty);
                        break;
                    }
                }
            }

            ProductPanel.Controls.Clear();
            currentX = 0; // Reset product panel layout positions
            currentY = 0;
            LoadProduct();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPOS_Load(object sender, EventArgs e)
        {
            btnImprimirCuenta.BorderStyle = BorderStyle.FixedSingle;
            SetupDataGridViewColumns(); // Call this method to set up columns
            AddCategory();

            ProductPanel.Controls.Clear();
            LoadProduct();
            originalWidth = this.Width;
            originalHeight = this.Height;
        }


        private void SetupDataGridViewColumns()
        {

            btnImprimirCuenta.Columns.Clear();
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "S/N", Name = "dgvSerial", Width = 50 });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "DetailID", Name = "dgvid", Visible = false });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ProdID", Name = "dgvProID", Visible = false });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nombre", Name = "dgvPName", Width = 150 });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cantidad", Name = "dgvQty", Width = 70 });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Precio", Name = "dgvPrice", Width = 80 });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Total", Name = "dgvAmount", Width = 100 });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Observacion", Name = "dgvObs", Width = 150 });
            btnImprimirCuenta.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "Enviado", Name = "dgvIsSent", Visible = false });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "QtyOriginal",
                Name = "dgvQtyOriginal",
                Visible = false
            });
            btnImprimirCuenta.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "QtyEnviado",
                Name = "dgvQtyEnviado",
                Visible = false
            });


            AddDeleteButtonColumn();
        }


        private void AddCategory()
        {
            string qry = "Select * from category";
            //SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);

            // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
            SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con);

            // CAMBIAR: Se reemplaza SqlDataAdapter por SQLiteDataAdapter
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            CategoryPanel.Controls.Clear();
            Guna.UI2.WinForms.Guna2Button b = null;

            int diff = 0;

            // Botón de "All Category"
            b = new Guna.UI2.WinForms.Guna2Button();
            b.Location = new Point(0, diff);
            b.FillColor = Color.FromArgb(50, 55, 89);
            b.Size = new Size(244, 58);
            b.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            b.Text = "Todas las Categorias";
            b.Click += new EventHandler(_Click);
            CategoryPanel.Controls.Add(b);
            diff += 70;

            // Resto de las categorías
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    b = new Guna.UI2.WinForms.Guna2Button();
                    b.Location = new Point(0, diff);
                    b.FillColor = Color.FromArgb(50, 55, 89);
                    b.Size = new Size(244, 58);
                    b.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
                    b.Text = row["catName"].ToString();
                    b.Click += new EventHandler(_Click);
                    CategoryPanel.Controls.Add(b);
                    diff += 70;
                }
            }
        }


        private void ReorganizeProducts()
        {
            int currentX = 0;
            int currentY = 0;

            foreach (Control ctrl in ProductPanel.Controls)
            {
                if (ctrl is ucProduct pro && pro.Visible)
                {
                    pro.Location = new Point(currentX, currentY);

                    currentX += itemWidth + itemSpacing;
                    if (currentX + itemWidth > ProductPanel.Width)
                    {
                        currentX = 0;
                        currentY += itemHeight + itemSpacing;
                    }
                }
            }
        }

        private void _Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button b = (Guna.UI2.WinForms.Guna2Button)sender;
            string selectedCategory = b.Text.Trim().ToLower();

            foreach (Control item in ProductPanel.Controls)
            {
                if (item is ucProduct pro)
                {
                    if (selectedCategory == "todas las categorias")
                    {
                        pro.Visible = true;
                    }
                    else
                    {
                        pro.Visible = pro.PCategory.ToLower().Contains(selectedCategory);
                    }
                }
            }

            ReorganizeProducts(); // reorganiza los visibles
        }
        private void MergePendingProductRows()
        {
            var groupedRows = new Dictionary<string, DataGridViewRow>();

            for (int i = btnImprimirCuenta.Rows.Count - 1; i >= 0; i--)
            {
                var row = btnImprimirCuenta.Rows[i];
                if (row.IsNewRow) continue;

                bool isSent = Convert.ToBoolean(row.Cells["dgvIsSent"]?.Value ?? false);
                if (isSent) continue; // Solo fusionamos filas no enviadas

                int prodID = Convert.ToInt32(row.Cells["dgvProID"].Value);
                string observation = (row.Cells["dgvObs"].Value?.ToString() ?? "").Trim().ToLower();

                string key = prodID + "|" + observation;

                if (groupedRows.ContainsKey(key))
                {
                    var existingRow = groupedRows[key];

                    int existingQty = Convert.ToInt32(existingRow.Cells["dgvQty"].Value);
                    int newQty = Convert.ToInt32(row.Cells["dgvQty"].Value);
                    double price = Convert.ToDouble(existingRow.Cells["dgvPrice"].Value);

                    existingRow.Cells["dgvQty"].Value = existingQty + newQty;
                    existingRow.Cells["dgvAmount"].Value = (existingQty + newQty) * price;

                    btnImprimirCuenta.Rows.RemoveAt(i); // Eliminar fila duplicada
                }
                else
                {
                    groupedRows[key] = row;
                }
            }
        }





        private void AddItems(string code, string proID, string name, string cat, string price, Image pimage)
        {
            var w = new ucProduct()
            {
                PName = name,
                PPrice = price,
                PCategory = cat,
                PImage = pimage,
                id = Convert.ToInt32(proID),
                Width = itemWidth,
                Height = itemHeight
            };
            w.Tag = code; // Código interno del producto

            if (currentX + itemWidth > ProductPanel.Width)
            {
                currentX = 0;
                currentY += itemHeight + itemSpacing;
            }

            w.Location = new Point(currentX, currentY);
            ProductPanel.Controls.Add(w);
            currentX += itemWidth + itemSpacing;

            w.onSelect += (ss, ee) =>
            {
                var wdg = (ucProduct)ss;
                bool found = false;

                foreach (DataGridViewRow item in btnImprimirCuenta.Rows)
                {
                    if (item.IsNewRow) continue;

                    int prodId = Convert.ToInt32(item.Cells["dgvProID"].Value);
                    string obs = (item.Cells["dgvObs"].Value?.ToString() ?? "").Trim().ToLower();
                    int qty = Convert.ToInt32(item.Cells["dgvQty"].Value);
                    // int qtyEnviado = Convert.ToInt32(item.Cells["dgvQtyEnviado"].Value ?? 0); // Este valor lo usaremos en KOT

                    string nuevaObs = ""; // o podés capturarla desde un TextBox de observación
                    if (prodId == wdg.id && obs == nuevaObs)
                    {
                        found = true;

                        item.Cells["dgvQty"].Value = qty + 1; // Incrementa la cantidad total en la grilla
                        item.Cells["dgvAmount"].Value = (qty + 1) * Convert.ToDouble(item.Cells["dgvPrice"].Value);

                        // Si el ítem estaba completamente enviado (gris/tachado), ahora tiene cantidad pendiente,
                        // así que resetear su estilo y el flag dgvIsSent.
                        if (Convert.ToBoolean(item.Cells["dgvIsSent"].Value ?? false))
                        {
                            item.Cells["dgvIsSent"].Value = false; // Marcar como no completamente enviado
                            item.ReadOnly = false; // Hacerlo editable de nuevo
                            item.DefaultCellStyle.ForeColor = Color.DarkGreen;
                            item.DefaultCellStyle.Font = new Font(btnImprimirCuenta.Font, FontStyle.Bold);
                        }
                        break;
                    }
                }

                // Si no existe fila previa para ese producto, agregar nueva
                if (!found)
                {
                    int n = btnImprimirCuenta.Rows.Add();

                    btnImprimirCuenta.Rows[n].Cells["dgvID"].Value = 0;
                    btnImprimirCuenta.Rows[n].Cells["dgvProID"].Value = wdg.id;
                    btnImprimirCuenta.Rows[n].Cells["dgvPName"].Value = wdg.PName;
                    btnImprimirCuenta.Rows[n].Cells["dgvQty"].Value = 1;
                    btnImprimirCuenta.Rows[n].Cells["dgvPrice"].Value = wdg.PPrice;
                    btnImprimirCuenta.Rows[n].Cells["dgvAmount"].Value = wdg.PPrice;
                    btnImprimirCuenta.Rows[n].Cells["dgvObs"].Value = "";
                    btnImprimirCuenta.Rows[n].Cells["dgvDelete"].Value = "Eliminar";
                    btnImprimirCuenta.Rows[n].Cells["dgvIsSent"].Value = false; // Nuevo ítem, no está enviado
                    btnImprimirCuenta.Rows[n].Cells["dgvQtyEnviado"].Value = 0; // Cantidad enviada es 0

                    btnImprimirCuenta.Rows[n].DefaultCellStyle.ForeColor = Color.DarkGreen;
                    btnImprimirCuenta.Rows[n].DefaultCellStyle.Font = new Font(btnImprimirCuenta.Font, FontStyle.Bold);
                }
                MergePendingProductRows(); // Esto debería seguir funcionando
                GetTotal(); // Actualiza el total
            };
        }


        private void LoadProduct()
        {
            string qry = "Select * From products inner join category on catID = categoryID";
            //SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);

            // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
            SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con);

            // CAMBIAR: Se reemplaza SqlDataAdapter por SQLiteDataAdapter
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                Byte[] imagearry = (byte[])item["pImage"];
                Byte[] imagebytearray = imagearry;

                AddItems(item["pCode"].ToString(), item["pID"].ToString(), item["pName"].ToString(), item["catName"].ToString(), item["pPrice"].ToString(), Image.FromStream(new MemoryStream(imagearry)));

            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text.Trim().ToLower();

            foreach (Control item in ProductPanel.Controls)
            {
                if (item is ucProduct pro)
                {
                    string nombre = pro.PName.ToLower();
                    string codigo = pro.Tag?.ToString().ToLower() ?? "";

                    pro.Visible = nombre.Contains(busqueda) || codigo.Contains(busqueda);
                }
            }


            ReorganizeProducts(); // reorganiza los visibles
        }


        //Metodo para agregar y quitar productos del dataGreid
        private void AddDeleteButtonColumn()
        {
            DataGridViewButtonColumn dgvDeleteButton = new DataGridViewButtonColumn();
            dgvDeleteButton.HeaderText = "Eliminar";
            dgvDeleteButton.Name = "dgvDelete";
            dgvDeleteButton.Text = "-";
            dgvDeleteButton.UseColumnTextForButtonValue = true;
            dgvDeleteButton.Width = 50;
            btnImprimirCuenta.Columns.Add(dgvDeleteButton);

        }
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == btnImprimirCuenta.Columns["dgvDelete"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = btnImprimirCuenta.Rows[e.RowIndex];
                int currentQty = Convert.ToInt32(row.Cells["dgvQty"].Value);

                if (currentQty > 1)
                {

                    row.Cells["dgvQty"].Value = currentQty - 1;

                    row.Cells["dgvAmount"].Value = (currentQty - 1) * Convert.ToDouble(row.Cells["dgvPrice"].Value);
                }
                else
                {

                    btnImprimirCuenta.Rows.RemoveAt(e.RowIndex);
                }


                GetTotal();
            }


            if (e.RowIndex >= 0 && btnImprimirCuenta.Columns[e.ColumnIndex].Name == "dgvObs")
            {

                string currentObservation = btnImprimirCuenta.Rows[e.RowIndex].Cells["dgvObs"].Value?.ToString() ?? string.Empty;

                using (frmObservation obsForm = new frmObservation(currentObservation))
                {
                    if (obsForm.ShowDialog() == DialogResult.OK)
                    {
                        btnImprimirCuenta.Rows[e.RowIndex].Cells["dgvObs"].Value = obsForm.Observation;
                    }
                }
            }
        }
        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // por serial
            int count = 0;

            foreach (DataGridViewRow row in btnImprimirCuenta.Rows)
            {

                count++;
                row.Cells[0].Value = count;
            }
        }

        private void GetTotal()
        {
            double total = 0;
            lblTotal.Text = "";

            foreach (DataGridViewRow item in btnImprimirCuenta.Rows)
            {
                total += double.Parse(item.Cells["dgvAmount"].Value.ToString());
            }

            lblTotal.Text = total.ToString("N2");
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            btnImprimirCuenta.Rows.Clear();
            MainId = 0;
            lblTotal.Text = "00";
            lblCustomer.Text = "";
            ClearForm();
        }

        private void btnDelivery_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Delivery";


            guna2MessageDialog1.Text = "¿Es un cliente existente?";
            guna2MessageDialog1.Caption = "Buscar cliente";
            guna2MessageDialog1.Parent = this;
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;

            DialogResult result = guna2MessageDialog1.Show();

            if (result == DialogResult.Yes)
            {
                frmSearchCustomer frm = new frmSearchCustomer();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CustomerID = frm.CustomerID;
                    lblCustomer.Visible = true;
                    lblCustomer.Text = $"CLIENTE: {frm.CustomerName}   TELEFONO: {frm.Phone}";
                    // Puedes mostrar sus datos si quieres
                }
            }
            else
            {
                frmAddCustomer frm = new frmAddCustomer();
                frm.orderType = OrderType;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CustomerID = frm.CustomerID;
                    lblCustomer.Visible = true;
                    lblCustomer.Text = $"CLIENTE: {frm.CustomerName}  TELEFONO: {frm.Phone}";
                }
            }

        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Take Away";
            lblCustomer.Text = "";

            guna2MessageDialog1.Text = "¿Es un cliente existente?";
            guna2MessageDialog1.Caption = "Buscar cliente";
            guna2MessageDialog1.Parent = this;
            guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
            guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;

            DialogResult result = guna2MessageDialog1.Show(); ;

            if (result == DialogResult.Yes)
            {
                frmSearchCustomer frm = new frmSearchCustomer();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CustomerID = frm.CustomerID;
                    lblCustomer.Visible = true;
                    lblCustomer.Text = $"CLIENTE: {frm.CustomerName}";
                }
            }
            else
            {
                frmAddCustomer frm = new frmAddCustomer();
                frm.orderType = OrderType;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CustomerID = frm.CustomerID;
                    lblCustomer.Visible = true;
                    lblCustomer.Text = $"CLIENTE: {frm.CustomerName}";

                }
            }

        }


        private void btnDin_Click(object sender, EventArgs e)
        {
            OrderType = "Mesas";
            frmTableSelect frm = new frmTableSelect();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                lblTable.Text = frm.TableName;
                lblTable.Visible = true;

                // 🔴 Marcar la mesa como ocupada
                string qry = "UPDATE tables SET status = 'Ocupada' WHERE tname = @tname";
                //using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
                // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
                using (SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con))
                {
                    cmd.Parameters.AddWithValue("@tname", frm.TableName);
                    if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                    cmd.ExecuteNonQuery();
                    if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
                }

                // ✅ Si hay pedido abierto, lo cargamos
                if (frm.PedidoMainID > 0)
                {
                    id = frm.PedidoMainID;
                    MainId = frm.PedidoMainID;
                    LoadEntries(); // ← ya lo tenés
                }
                else
                {
                    // 🔍 Solo pedimos mozo si no hay pedido
                    frmWaiterSelect frm2 = new frmWaiterSelect();
                    if (frm2.ShowDialog() == DialogResult.OK)
                    {
                        lblWaiter.Text = frm2.WaiterName;
                        lblWaiter.Visible = true;
                    }
                    else
                    {
                        // ❌ Si no selecciona mozo, cancelamos todo
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                        return;
                    }
                }
            }
            else
            {
                // ❌ Si no seleccionó ninguna mesa, salir también
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }


        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnKot_Click(object sender, EventArgs e)
        {
            // 1. Inicializar variables de pago
            double totalAmount = Convert.ToDouble(lblTotal.Text);
            double receivedAmount = 0;
            double changeAmount = 0;
            string finalStatus = "Pendiente";
            // 🛑 NUEVA VARIABLE para el método de pago (necesaria para el estatus descriptivo)
            string paymentMethodUsed = string.Empty;

            // 2. Guardar encabezado inicial (o actualizar con status 'Pendiente')
            string qryMain = MainId == 0
                ? @"INSERT INTO tblMain (aDate, aTime, TableName, WaiterName, status, orderType, total, received, change, CustomerID, discount)
            VALUES (@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @CustomerID, 0);"
                : @"UPDATE tblMain SET
            status = @status, total = @total, received = @received, change = @change, CustomerID = @CustomerID
            WHERE MainID = @ID";

            SQLiteCommand cmdMain = new SQLiteCommand(qryMain, MainClass.con);
            cmdMain.Parameters.AddWithValue("@ID", MainId);
            cmdMain.Parameters.AddWithValue("@aDate", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            cmdMain.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmdMain.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmdMain.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmdMain.Parameters.AddWithValue("@status", finalStatus); // Status inicial: Pendiente
            cmdMain.Parameters.AddWithValue("@orderType", OrderType);
            cmdMain.Parameters.AddWithValue("@total", totalAmount);
            cmdMain.Parameters.AddWithValue("@received", receivedAmount);
            cmdMain.Parameters.AddWithValue("@change", changeAmount);
            cmdMain.Parameters.AddWithValue("@CustomerID", CustomerID);

            if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
            try
            {
                if (MainId == 0)
                {
                    cmdMain.ExecuteNonQuery();
                    cmdMain.CommandText = "SELECT LAST_INSERT_ROWID();";
                    MainId = Convert.ToInt32(cmdMain.ExecuteScalar()); // Obtiene el nuevo MainId
                }
                else
                    cmdMain.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar encabezado: " + ex.Message);
                MainClass.con.Close();
                return;
            }
            finally
            {
                MainClass.con.Close();
            }

            // 3. Lógica de Pago Condicional y Construcción del Estatus Descriptivo
            bool isDeliveryOrTakeAway = (OrderType.ToLower() == "delivery" || OrderType.ToLower().Contains("take"));

            if (totalAmount > 0 && isDeliveryOrTakeAway)
            {
                using (popus_pizzeria.Model.Paymentform paymentForm = new popus_pizzeria.Model.Paymentform(totalAmount))
                {
                    if (paymentForm.ShowDialog() == DialogResult.OK)
                    {
                        receivedAmount = paymentForm.Recibido;
                        changeAmount = paymentForm.Cambio;
                        paymentMethodUsed = paymentForm.MetodoPago; // Capturamos el método

                        // 🛑 LÓGICA REFACTORIZADA PARA EL ESTATUS DESCRIPTIVO 🛑
                        if (paymentMethodUsed == "Efectivo")
                        {
                            // Si es efectivo y el cambio es >= 0, está "Pagado en Efectivo"
                            finalStatus = (changeAmount >= 0) ? "Pagado en Efectivo" : "Pendiente";
                        }
                        else
                        {
                            // Si es cualquier método electrónico, asumimos que fue exitoso
                            // y usamos el método para el estatus (Ej: "Pagado con Transferencia")
                            finalStatus = "Pagado con " + paymentMethodUsed;
                        }
                    }
                    // Si el usuario cancela (DialogResult.Cancel), finalStatus permanece como "Pendiente"
                }
            }

            // 4. Actualizar el encabezado con el Estatus y los datos de Pago (si se modificaron)
            // Usaremos solo la parte principal del estatus ("Pagado" o "Pendiente") para la DB si es necesario,
            // o el estatus completo si la columna 'status' en tblMain es lo suficientemente grande.
            // Usaremos el estatus COMPLETO para la comanda.

            // Solo actualizamos la DB si el estado cambió o si hay montos recibidos que guardar.
            if (MainId > 0 && (finalStatus != "Pendiente" || receivedAmount > 0))
            {
                string qryUpdatePayment = @"UPDATE tblMain SET
            status = @status, received = @received, change = @change
            WHERE MainID = @ID";

                SQLiteCommand cmdUpdate = new SQLiteCommand(qryUpdatePayment, MainClass.con);
                cmdUpdate.Parameters.AddWithValue("@ID", MainId);
                // NOTA: Si tu columna 'status' en tblMain es corta, considera guardar solo "Pagado" o "Pendiente".
                // Si es lo suficientemente grande (ej: 50 caracteres), guarda el estatus completo.
                cmdUpdate.Parameters.AddWithValue("@status", finalStatus);
                cmdUpdate.Parameters.AddWithValue("@received", receivedAmount);
                cmdUpdate.Parameters.AddWithValue("@change", changeAmount);

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                try
                {
                    cmdUpdate.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar datos de pago/estatus: " + ex.Message);
                }
                finally
                {
                    MainClass.con.Close();
                }
            }

            // 5. Procesar productos y detalles (Lógica para tblDetails, sin cambios)
            List<string> tempNewProductDetails = new List<string>();
            bool hayProductosParaEnviar = false;

            // ... (Tu bucle 'foreach' para guardar tblDetails va aquí, sin cambios) ...
            foreach (DataGridViewRow row in btnImprimirCuenta.Rows)
            {
                if (row.IsNewRow) continue;
                int prodID = Convert.ToInt32(row.Cells["dgvProID"].Value);
                int currentQtyInGrid = Convert.ToInt32(row.Cells["dgvQty"].Value);
                double price = Convert.ToDouble(row.Cells["dgvPrice"].Value);
                string observation = row.Cells["dgvObs"].Value?.ToString() ?? "";
                int qtyAlreadySent = Convert.ToInt32(row.Cells["dgvQtyEnviado"].Value ?? 0);
                int qtyToSend = currentQtyInGrid - qtyAlreadySent;
                if (qtyToSend <= 0) continue;

                hayProductosParaEnviar = true;
                string qryDetail = @"INSERT INTO tblDetails
            (MainID, prodID, qty, price, amount, observation, IsSentToKitchen)
            VALUES (@MainID, @ProdID, @qty, @price, @amount, @observation, 1);";

                SQLiteCommand cmdDetail = new SQLiteCommand(qryDetail, MainClass.con);
                cmdDetail.Parameters.AddWithValue("@MainID", MainId);
                cmdDetail.Parameters.AddWithValue("@ProdID", prodID);
                cmdDetail.Parameters.AddWithValue("@qty", qtyToSend);
                cmdDetail.Parameters.AddWithValue("@price", price);
                cmdDetail.Parameters.AddWithValue("@amount", qtyToSend * price);
                cmdDetail.Parameters.AddWithValue("@observation", observation);

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                try
                {
                    cmdDetail.ExecuteNonQuery();
                    tempNewProductDetails.Add($"{qtyToSend} x {row.Cells["dgvPName"].Value}");
                    if (!string.IsNullOrWhiteSpace(observation))
                        tempNewProductDetails.Add($"  -> {observation}");

                    row.Cells["dgvQtyEnviado"].Value = qtyAlreadySent + qtyToSend;
                    // Lógica de grilla (ReadOnly, Color, etc.)
                    if (Convert.ToInt32(row.Cells["dgvQtyEnviado"].Value) == currentQtyInGrid)
                    {
                        row.Cells["dgvIsSent"].Value = true;
                        row.ReadOnly = true;
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                        row.DefaultCellStyle.Font = new Font(btnImprimirCuenta.Font, FontStyle.Strikeout);
                    }
                    else
                    {
                        row.Cells["dgvIsSent"].Value = false;
                        row.ReadOnly = false;
                        row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                        row.DefaultCellStyle.Font = new Font(btnImprimirCuenta.Font, FontStyle.Bold);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar detalle del producto {row.Cells["dgvPName"].Value}: {ex.Message}");
                }
                finally
                {
                    MainClass.con.Close();
                }
            }
            // ... (Fin del bucle 'foreach') ...


            if (!hayProductosParaEnviar)
            {
                guna2MessageDialog1.Show("No hay productos nuevos ni modificados para enviar a cocina.");
                return;
            }

            guna2MessageDialog1.Show("Pedido enviado a cocina correctamente");

            // 6. Llama a la función para imprimir, pasando el estatus descriptivo.
            string textoComanda = GenerarTextoComandaForNewItems(
                MainId,
                tempNewProductDetails,
                totalAmount,
                receivedAmount,
                changeAmount,
                finalStatus // 👈 Se pasa el estatus completo (Ej: "Pagado con Transferencia")
            );

            ComandaPrinter.ImprimirTexto(textoComanda);
        }








        // --- NEW METHOD FOR GENERATING KOT FOR ONLY NEWLY SENT ITEMS ---
        // Ejemplo de cómo podría ser tu GenerarTextoComandaForNewItems
         public string GenerarTextoComandaForNewItems(
                 int mainId,
                 List<string> newItemsDetails,
                 // 🛑 NUEVOS PARÁMETROS DE PAGO AGREGADOS 🛑
                 double total,
                 double received,
                 double change,
                 string status
             )
         {
             StringBuilder comanda = new StringBuilder();
             string tableName = "N/A";
             string waiterName = "N/A";
             string orderType = "N/A";

             // --- Consulta para obtener detalles de encabezado y cliente ---
             string qryMain = @"SELECT m.aDate, m.aTime, m.TableName, m.WaiterName, m.orderType, c.Name AS CustomerName, 
                          c.Address, c.Phone, c.Reference
                       FROM tblMain m
                       LEFT JOIN tblCustomers c ON m.CustomerID = c.CustomerID
                       WHERE m.MainID = @MainId;";

             // CAMBIAR: Usar SQLiteCommand
             using (SQLiteCommand cmd = new SQLiteCommand(qryMain, MainClass.con))
             {
                 cmd.Parameters.AddWithValue("@MainId", mainId);
                 if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();

                 try
                 {
                     using (SQLiteDataReader dr = cmd.ExecuteReader())
                     {
                         if (dr.Read())
                         {
                             // Actualizar las variables con los datos de la DB
                             tableName = dr["TableName"]?.ToString() ?? "N/A";
                             waiterName = dr["WaiterName"]?.ToString() ?? "N/A";
                             orderType = dr["orderType"]?.ToString() ?? "N/A";

                             comanda.AppendLine("******** COMANDA Nº " + mainId + " ********");
                             comanda.AppendLine("Fecha: " + Convert.ToDateTime(dr["aDate"]).ToShortDateString());
                             comanda.AppendLine("Hora: " + dr["aTime"]);
                             comanda.AppendLine("Mesa/Orden: " + tableName);
                             comanda.AppendLine("Mozo: " + waiterName);
                             comanda.AppendLine("Tipo: " + orderType);
                             comanda.AppendLine("========================================="); // Separador más grande

                             // Lógica para AÑADIR DATOS DEL CLIENTE DE DELIVERY
                             string customerName = dr["CustomerName"]?.ToString();
                             string phone = dr["Phone"]?.ToString() ?? "N/A";
                             string address = dr["Address"]?.ToString() ?? "N/A";
                             string reference = dr["Reference"]?.ToString() ?? "N/A";
                            if (orderType.ToLower() == "delivery" || !string.IsNullOrEmpty(customerName))
                            {
                                comanda.AppendLine("--- DATOS DE ENTREGA ---");

                                // Estos campos no suelen ser muy largos y pueden ir en la misma línea que su etiqueta.
                                comanda.AppendLine($"Cliente: {customerName ?? "N/A"}");
                                comanda.AppendLine($"Telefono: {phone}");

                                // **DIRECCIÓN (Salto de línea garantizado)**
                                // Imprimimos la etiqueta, luego el valor en la siguiente línea.
                                comanda.AppendLine("Direccion:");
                                comanda.AppendLine($"{address}"); // El valor de 'address' ocupa toda la línea que necesite.

                                // **REFERENCIA (Salto de línea garantizado)**
                                // Imprimimos la etiqueta, luego el valor en la siguiente línea.
                                comanda.AppendLine("Referencia:");
                                comanda.AppendLine($"{reference}"); // El valor de 'reference' ocupa toda la línea que necesite.

                                comanda.AppendLine("-----------------------------------------");
                            }
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     comanda.AppendLine($"*** ERROR al leer encabezado ({ex.Message}) ***");
                 }
                 finally
                 {
                     MainClass.con.Close();
                 }
             }

             // Si la comanda está vacía (por error de conexión), al menos pone los datos de la UI
             if (comanda.Length == 0)
             {
                 comanda.AppendLine("b**** COMANDA Nº " + mainId + " ****");
                 comanda.AppendLine("Fecha: " + DateTime.Now.ToShortDateString());
                 comanda.AppendLine("Hora: " + DateTime.Now.ToShortTimeString());
                 comanda.AppendLine("Mesa/Orden: " + tableName);
                 comanda.AppendLine("Mozo: " + waiterName);
                 comanda.AppendLine("Tipo: " + orderType);
                 comanda.AppendLine("=========================================");
             }

             // 🛑 Lógica para añadir los productos nuevos - MÁS DETALLADA Y GRANDE 🛑
             comanda.AppendLine(">>> PRODUCTOS NUEVOS <<<");
             comanda.AppendLine("=========================================");

             foreach (string itemDetail in newItemsDetails)
             {
                 // Dividimos el string para resaltar la cantidad y el nombre
                 // Se asume el formato generado en btnKot_Click: "QTY x PROD_NAME" y " -> OBSERVATION"
                 if (itemDetail.Contains(" x "))
                 {
                     string[] parts = itemDetail.Split(new string[] { " x " }, StringSplitOptions.None);
                     if (parts.Length == 2)
                     {
                         // Resalta la cantidad con asteriscos o espacios para simular mayor tamaño
                         // La impresora térmica interpretará esto como un bloque más notable.
                         comanda.AppendLine($" {parts[0].Trim()} {parts[1].Trim().ToUpper()}");
                     }
                     else
                     {
                         comanda.AppendLine(itemDetail.ToUpper()); // Fallback
                     }
                 }
                 else if (itemDetail.StartsWith(" -> "))
                 {
                     // Resalta las observaciones con indentación y mayúsculas
                     comanda.AppendLine($"     OBSERVACIÓN: {itemDetail.Substring(4).Trim()}");
                     comanda.AppendLine("-----------------------------------------");
                 }
                 else
                 {
                     comanda.AppendLine(itemDetail.ToUpper());
                     comanda.AppendLine("-----------------------------------------");
                 }
             }

             comanda.AppendLine("=========================================");

             // 🛑 LÓGICA: DETALLE DE PAGO 🛑
             comanda.AppendLine("--- RESUMEN DE PAGO ---");
             comanda.AppendLine($"TOTAL: {total.ToString("C")}");
             comanda.AppendLine($"RECIBIDO: {received.ToString("C")}");
             comanda.AppendLine($"CAMBIO: {change.ToString("C")}");
             comanda.AppendLine($"ESTADO: {status.ToUpper()}");
             comanda.AppendLine("-----------------------------------------");

             comanda.AppendLine("¡Gracias!");
             return comanda.ToString();
         }

        // Se asume que WrapText existe y está disponible.

      /*  public string GenerarTextoComandaForNewItems(
            int mainId,
            List<string> newItemsDetails,
            double total,
            double received,
            double change,
            string status
        )
        {
            // ANCHO MÁXIMO DEFINIDO PARA 58MM
            const int MaxWidth = 30;
            const string Separator = "------------------------------";
            const string BoldSeparator = "==============================";

            StringBuilder comanda = new StringBuilder();
            string tableName = "N/A";
            string waiterName = "N/A";
            string orderType = "N/A";

            // Bandera para gestionar el separador en el bucle de productos
            bool lastItemWasObservation = false;

            // --- Consulta para obtener detalles de encabezado y cliente (Se mantiene igual) ---
            // ... (Se mantiene la sección de consulta a la DB para obtener datos) ...
            // Asegúrate de que tu función WrapText usa ' ' (espacio) en lugar de ' ' (non-breaking space) si lo copiaste del ejemplo anterior.

            string qryMain = @"SELECT m.aDate, m.aTime, m.TableName, m.WaiterName, m.orderType, c.Name AS CustomerName, 
                         c.Address, c.Phone, c.Reference
                       FROM tblMain m
                       LEFT JOIN tblCustomers c ON m.CustomerID = c.CustomerID
                       WHERE m.MainID = @MainId;";

            // Se asume que MainClass.con y SQLiteCommand están disponibles
            using (SQLiteCommand cmd = new SQLiteCommand(qryMain, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@MainId", mainId);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();

                try
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Actualizar las variables con los datos de la DB
                            tableName = dr["TableName"]?.ToString() ?? "N/A";
                            waiterName = dr["WaiterName"]?.ToString() ?? "N/A";
                            orderType = dr["orderType"]?.ToString() ?? "N/A";
                            string customerName = dr["CustomerName"]?.ToString();

                            // 1. ENCABEZADO Y DATOS BÁSICOS
                            comanda.AppendLine("**** COMANDA Nro. " + mainId + " ****");
                            comanda.AppendLine(Separator);
                            comanda.AppendLine("Fecha: " + Convert.ToDateTime(dr["aDate"]).ToShortDateString());
                            comanda.AppendLine("Hora: " + dr["aTime"]);
                            comanda.AppendLine("Mesa/Orden: " + tableName);
                            comanda.AppendLine("Mozo: " + waiterName);
                            comanda.AppendLine("TIPO DE ORDEN: " + orderType.ToUpper());
                            comanda.AppendLine(BoldSeparator);

                            // 2. DATOS DEL CLIENTE DE DELIVERY/TAKE AWAY
                            if (orderType.ToLower() == "delivery" || orderType.ToLower().Contains("take") || !string.IsNullOrEmpty(customerName))
                            {
                                comanda.AppendLine("--- DATOS DE ENTREGA ---");
                                comanda.AppendLine($"CLIENTE: {customerName ?? "N/A"}");

                                // Teléfono
                                string phone = dr["Phone"]?.ToString() ?? "N/A";
                                comanda.AppendLine($"TELEFONO: {phone}");

                                // Dirección con WrapText
                                string address = dr["Address"]?.ToString() ?? "N/A";
                                comanda.AppendLine("DIRECCION:");
                                // Usar solo espacios, no caracteres non-breaking: "  > "
                                comanda.Append(WrapText(address.ToUpper(), MaxWidth, "  > "));

                                // Referencia con WrapText
                                string reference = dr["Reference"]?.ToString() ?? "N/A";
                                comanda.AppendLine("REFERENCIA:");
                                // Usar solo espacios, no caracteres non-breaking: "  > "
                                comanda.Append(WrapText(reference.ToUpper(), MaxWidth, "  > "));

                                comanda.AppendLine(Separator);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    comanda.AppendLine($"** ERROR EN DB: {ex.Message.Substring(0, Math.Min(ex.Message.Length, MaxWidth - 10))}...");
                }
                finally
                {
                    MainClass.con.Close();
                }
            }

            // 3. DETALLE DE PRODUCTOS (CORREGIDO)
            comanda.AppendLine(">>> PRODUCTOS NUEVOS <<<");
            comanda.AppendLine(BoldSeparator);

            foreach (string itemDetail in newItemsDetails)
            {
                if (itemDetail.Contains(" x "))
                {
                    // Si el ítem anterior fue una observación, ya tiene un separador.
                    if (!lastItemWasObservation)
                    {
                        comanda.AppendLine(Separator); // Separador entre productos sin observaciones
                    }

                    string[] parts = itemDetail.Split(new string[] { " x " }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        string qty = parts[0].Trim();
                        string name = parts[1].Trim().ToUpper();

                        // Formato de producto (resaltado)
                        comanda.AppendLine();
                        // Usar solo espacios: "      "
                        comanda.AppendLine($"** {qty} ** {WrapText(name, MaxWidth, "      ").Trim()}");
                        comanda.AppendLine();
                    }
                    lastItemWasObservation = false; // Reset la bandera
                }
                else if (itemDetail.StartsWith(" -> ")) // 🛑 Verificación de prefijo con espacio normal
                {
                    // Lógica que se ejecuta si se encuentra la observación
                    string observation = itemDetail.Substring(4).Trim();

                    comanda.AppendLine("  > OBSERVACION:");

                    // 🛑 Asegúrate de que los prefijos de WrapText también usen espacio normal
                    comanda.Append(WrapText(observation.ToUpper(), MaxWidth, "    - "));

                    comanda.AppendLine(Separator);
                    lastItemWasObservation = true;
                }
                else if (itemDetail.StartsWith("->")) // Sin el espacio inicial
                {
                    // También funcionaría si eliminas el espacio en el Substring:
                    string observation = itemDetail.Substring(2).Trim();
                }
            }

            // Asegurarse de poner un separador después del último producto si no hubo observación
            if (!lastItemWasObservation)
            {
                comanda.AppendLine(Separator);
            }

            comanda.AppendLine(BoldSeparator);

            // 4. RESUMEN DE PAGO (Se mantiene igual, la alineación es correcta)
            comanda.AppendLine("--- RESUMEN DE PAGO ---");

            string totalFormatted = total.ToString("C");
            comanda.AppendLine($"TOTAL:{totalFormatted.PadLeft(MaxWidth - 6)}");

            string receivedFormatted = received.ToString("C");
            comanda.AppendLine($"RECIBIDO:{receivedFormatted.PadLeft(MaxWidth - 9)}");

            string changeFormatted = change.ToString("C");
            comanda.AppendLine($"CAMBIO:{changeFormatted.PadLeft(MaxWidth - 7)}");

            comanda.AppendLine("ESTADO:");
            comanda.Append(WrapText(status.ToUpper(), MaxWidth, "  "));

            comanda.AppendLine(Separator);

            comanda.AppendLine("      ¡GRACIAS!");
            return comanda.ToString();
        }
        private static string WrapText(string text, int maxWidth, string prefix = "")
        {
            // ... (Mantener la implementación de WrapText, se asume que existe) ...
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            StringBuilder wrappedText = new StringBuilder();
            string[] words = text.Split(' ');
            StringBuilder currentLine = new StringBuilder(prefix);

            int effectiveWidth = maxWidth - prefix.Length;
            if (effectiveWidth < 1) effectiveWidth = 1;

            foreach (string word in words)
            {
                // Revisa si la palabra sola es más larga que el ancho disponible sin prefijo
                if (word.Length > effectiveWidth && effectiveWidth > 0)
                {
                    // Si la palabra es muy larga, la corta con un guion si es necesario.
                    // Para impresoras termicas, simplemente la agregaremos y dejaremos que se rompa,
                    // o la dividimos manualmente. Aquí usaremos un enfoque simple:
                    // Si el buffer de la línea está vacío, la agrega, sabiendo que se desbordará.
                    if (currentLine.Length == prefix.Length)
                    {
                        currentLine.Append(word);
                    }
                    // Si no, salta de línea.
                    else
                    {
                        wrappedText.AppendLine(currentLine.ToString().TrimEnd());
                        currentLine.Clear();
                        currentLine.Append(prefix);
                        currentLine.Append(word + " ");
                    }
                }
                else if (currentLine.Length + 1 + word.Length > maxWidth && currentLine.Length > prefix.Length)
                {
                    wrappedText.AppendLine(currentLine.ToString().TrimEnd());
                    currentLine.Clear();
                    currentLine.Append(prefix);
                    currentLine.Append(word + " ");
                }
                else
                {
                    currentLine.Append(word + " ");
                }
            }

            if (currentLine.Length > prefix.Length)
            {
                wrappedText.AppendLine(currentLine.ToString().TrimEnd());
            }

            return wrappedText.ToString();
        }
        private void btnPreviewKot_Click(object sender, EventArgs e)
        {
            // 1. Verificación Inicial
            if (btnImprimirCuenta.Rows.Count == 0)
            {
                guna2MessageDialog1.Show("No hay productos en la cuenta para previsualizar.");
                return;
            }

            // --- SIMULACIÓN DE DATOS (Necesarios para llamar a GenerarTextoComandaForNewItems) ---
            int mainId = MainId == 0 ? 99999 : MainId;
            double totalAmount = Convert.ToDouble(lblTotal.Text);
            double receivedAmount = 0;
            double changeAmount = 0;
            string finalStatus = "Pendiente (SIMULACIÓN DE PAGO)";

            // 2. Simular la obtención de los ítems a enviar
            List<string> tempNewProductDetails = new List<string>();
            bool hayProductosParaEnviar = false;

            foreach (DataGridViewRow row in btnImprimirCuenta.Rows)
            {
                if (row.IsNewRow) continue;

                int currentQtyInGrid = Convert.ToInt32(row.Cells["dgvQty"].Value);
                string observation = row.Cells["dgvObs"].Value?.ToString() ?? "";
                int qtyAlreadySent = Convert.ToInt32(row.Cells["dgvQtyEnviado"].Value ?? 0);
                int qtyToSend = currentQtyInGrid - qtyAlreadySent;

                if (qtyToSend <= 0) continue;

                hayProductosParaEnviar = true;

                tempNewProductDetails.Add($"{qtyToSend} x {row.Cells["dgvPName"].Value}");
                if (!string.IsNullOrWhiteSpace(observation))
                    tempNewProductDetails.Add($" -> {observation}");
            }

            if (!hayProductosParaEnviar)
            {
                guna2MessageDialog1.Show("No hay productos nuevos para previsualizar.");
                return;
            }

            // 3. Generar el texto de la comanda con el formato de 58mm (30 caracteres)
            string textoComanda = GenerarTextoComandaForNewItems(
                mainId,
                tempNewProductDetails,
                totalAmount,
                receivedAmount,
                changeAmount,
                finalStatus
            );

            // 4. Mostrar la previsualización en una ventana simulando el tamaño real (ShowDialog)
            ShowComandaPreview(textoComanda);
        }


        // --- Método auxiliar para mostrar la vista previa simulada (SIMULACIÓN DE 58MM) ---
        private void ShowComandaPreview(string text)
        {
            using (Form previewForm = new Form())
            {
                previewForm.Text = "Previsualización Comanda (58mm - 30 Char. Ancho)";

                // Ajuste de ancho para simular el papel de 58mm (Aprox. 200 píxeles de ancho real para 30 caracteres)
                previewForm.Width = 220; // Ancho ajustado para que el TextBox quepa y mantenga el formato
                previewForm.Height = 500; // La altura será variable, pero 500px debería ser suficiente.
                previewForm.StartPosition = FormStartPosition.CenterScreen;
                previewForm.MinimizeBox = false;
                previewForm.MaximizeBox = false;
                previewForm.FormBorderStyle = FormBorderStyle.FixedDialog; // Bloquea el redimensionamiento

                TextBox txtPreview = new TextBox();
                txtPreview.Multiline = true;
                txtPreview.ReadOnly = true;
                txtPreview.ScrollBars = ScrollBars.Vertical;

                // Es esencial usar una fuente monoespaciada (como Consolas o Courier New)
                // para que la simulación de los 30 caracteres sea precisa.
                txtPreview.Font = new Font("Consolas", 10, FontStyle.Regular);
                txtPreview.Text = text;
                txtPreview.Dock = DockStyle.Fill;

                previewForm.Controls.Add(txtPreview);
                previewForm.ShowDialog(); // Muestra el diálogo modal
            }
        }*/
        /*public string GenerarTextoComandaForNewItems(int mainId, List<string> newItemsDetails)
        {
            StringBuilder comanda = new StringBuilder();
            // ... (obtener datos de mesa, mozo, etc. usando mainId si es necesario)
            comanda.AppendLine("******** COMANDA Nº " + mainId + " ********");
            comanda.AppendLine("Fecha: " + DateTime.Now.ToShortDateString());
            comanda.AppendLine("Hora: " + DateTime.Now.ToShortTimeString());
            comanda.AppendLine("Mesa: " + lblTable.Text); // O obtener de la DB si no está disponible aquí
            comanda.AppendLine("Mozo: " + lblWaiter.Text); // Idem
            comanda.AppendLine("Tipo: " + OrderType); // Idem
            comanda.AppendLine("-----------------------------------");
            comanda.AppendLine("Productos:");
            foreach (string itemDetail in newItemsDetails)
            {
                comanda.AppendLine(itemDetail);
            }
            comanda.AppendLine("-----------------------------------");
            comanda.AppendLine("¡Gracias!");
            return comanda.ToString();
        }*/

        private string GenerarTextoComanda(int MainId)
        {
            StringBuilder sb = new StringBuilder();

            string qryMain = @"SELECT m.aDate, m.aTime, m.TableName, m.WaiterName, m.orderType, c.Name AS CustomerName, 
                                        c.Address, c.Phone, c.Reference
                                    FROM tblMain m
                                    LEFT JOIN tblCustomers c ON m.CustomerID = c.CustomerID
                                    WHERE m.MainID = @MainId;
";

            //using (SqlCommand cmd = new SqlCommand(qryMain, MainClass.con))
            // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
            using (SQLiteCommand cmd = new SQLiteCommand(qryMain, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@MainId", MainId);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                //using (SqlDataReader dr = cmd.ExecuteReader())
                // CAMBIAR: Se reemplaza SqlDataReader por SQLiteDataReader
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {

                        sb.AppendLine("***** COMANDA *****");
                        sb.AppendLine($"Fecha: {Convert.ToDateTime(dr["aDate"]).ToShortDateString()}");
                        sb.AppendLine($"Hora: {dr["aTime"]}");
                        sb.AppendLine($"Mesa: {dr["TableName"]}");
                        sb.AppendLine($"Mozo: {dr["WaiterName"]}");
                        sb.AppendLine($"Tipo: {dr["orderType"]}");
                        sb.AppendLine();

                        if (!string.IsNullOrEmpty(dr["CustomerName"]?.ToString()))
                        {
                            sb.AppendLine($"Cliente: {dr["CustomerName"]}");
                            sb.AppendLine($"Telefono: {dr["Phone"]}");
                            sb.AppendLine($"Direccion: {dr["Address"]}");
                            sb.AppendLine($"Referencia: {dr["Reference"]}");
                            sb.AppendLine();
                        }
                    }
                }
                MainClass.con.Close();
            }

            sb.AppendLine("Productos:");
            sb.AppendLine("--------------------------------");

            string qryDetails = @"SELECT d.qty, p.pname, d.observation 
                                    FROM tblDetails d
                                    JOIN products p ON p.pid = d.prodID
                                    WHERE d.MainID = @mainID AND d.IsSentToKitchen = 0
                                    ";

            //using (SqlCommand cmd2 = new SqlCommand(qryDetails, MainClass.con))
            // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
            using (SQLiteCommand cmd2 = new SQLiteCommand(qryDetails, MainClass.con))
            {
                cmd2.Parameters.AddWithValue("@mainId", MainId);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                //using (SqlDataReader dr = cmd2.ExecuteReader())
                // CAMBIAR: Se reemplaza SqlDataReader por SQLiteDataReader
                using (SQLiteDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        sb.AppendLine($"{dr["qty"]} x {dr["pName"]}");
                        string obs = dr["Observation"].ToString();
                        if (!string.IsNullOrWhiteSpace(obs))
                            sb.AppendLine($"  -> {obs}");
                    }
                }
                MainClass.con.Close();
            }

            sb.AppendLine("--------------------------------");
            sb.AppendLine("  ¡Gracias!");
            MessageBox.Show(sb.ToString()); // SOLO PARA DEPURAR
            return sb.ToString();
        }



        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && btnImprimirCuenta.Columns[e.ColumnIndex].Name == "dgvObs")
            {

                string currentObservation = btnImprimirCuenta.Rows[e.RowIndex].Cells["dgvObs"].Value?.ToString() ?? string.Empty;


                using (frmObservation obsForm = new frmObservation(currentObservation))
                {
                    if (obsForm.ShowDialog() == DialogResult.OK)
                    {

                        btnImprimirCuenta.Rows[e.RowIndex].Cells["dgvObs"].Value = obsForm.Observation;
                    }
                }
            }
        }

        public int id = 0;
        private int newRowIndex;

        private void btnBill_Click(object sender, EventArgs e)
        {
            frmBillList frm = new frmBillList();
            frm.ShowDialog();

            if (frm.MainID > 0)
            {
                id = frm.MainID;
                MainId = frm.MainID;
                LoadEntries();
            }
        }

        private void LoadEntries()
        {
            // Esta consulta ahora agrupa por ProdID, nombre del producto, precio y observación
            // para obtener las cantidades totales y las cantidades ya enviadas de forma AGREGADA.
            string qry = @"
                            SELECT
                                d.ProdID,
                                p.pName,
                                SUM(d.qty) AS TotalQty, -- Suma la cantidad total del producto con esa observación
                                d.price,                -- Asumimos que el precio es consistente para un producto
                                d.observation,
                                SUM(CASE WHEN d.IsSentToKitchen = 1 THEN d.qty ELSE 0 END) AS TotalQtyEnviado, -- Suma las cantidades YA ENVIADAS
                                m.TableName,
                                m.WaiterName,
                                m.orderType
                            FROM tblMain m
                            INNER JOIN tblDetails d ON m.MainID = d.MainID
                            INNER JOIN products p ON p.pID = d.ProdID
                            WHERE m.MainID = @MainID
                            GROUP BY d.ProdID, p.pName, d.price, d.observation, m.TableName, m.WaiterName, m.orderType;
                        ";

            //SqlCommand cmd = new SqlCommand(qry, MainClass.con); // Sintaxis corregida
            // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
            SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con);
            cmd.Parameters.AddWithValue("@MainID", id);

            DataTable dt = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            // CAMBIAR: Se reemplaza SqlDataAdapter por SQLiteDataAdapter
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows.Count == 0) return;

            lblTable.Text = dt.Rows[0]["TableName"].ToString();
            lblWaiter.Text = dt.Rows[0]["WaiterName"].ToString();
            lblTable.Visible = true;
            lblWaiter.Visible = true;

            btnImprimirCuenta.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                int totalQty = Convert.ToInt32(row["TotalQty"]); // Cantidad total del producto en el pedido (en DB)
                int totalQtyEnviado = Convert.ToInt32(row["TotalQtyEnviado"]); // Cantidad total del producto ya enviada (en DB)

                object[] obj = {
            0, // S/N
            0, // dgvID (DetailID no aplica directamente a filas agregadas, lo ponemos en 0)
            row["ProdID"],
            row["pName"],
            totalQty, // Cantidad TOTAL para esta fila en la grilla (dgvQty)
            row["price"],
            Convert.ToDouble(row["price"]) * totalQty, // Cálculo del monto total
            row["observation"],
            (totalQty == totalQtyEnviado), // dgvIsSent: true si TODA la cantidad de esta fila ya fue enviada
            0, // QtyOriginal (no se usa con esta lógica agregada)
            totalQtyEnviado // dgvQtyEnviado contendrá la cantidad total que YA fue enviada para este ítem
        };

                int index = btnImprimirCuenta.Rows.Add(obj);
                var dgvRow = btnImprimirCuenta.Rows[index];

                // Establecer el estilo de la fila: si todo está enviado, gris y tachado; si no, verde y negrita.
                if (totalQty == totalQtyEnviado)
                {
                    dgvRow.DefaultCellStyle.ForeColor = Color.Gray;
                    dgvRow.DefaultCellStyle.Font = new Font(btnImprimirCuenta.Font, FontStyle.Strikeout);
                    dgvRow.ReadOnly = true; // Hacer la fila de solo lectura si ya está todo enviado
                }
                else
                {
                    dgvRow.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    dgvRow.DefaultCellStyle.Font = new Font(btnImprimirCuenta.Font, FontStyle.Bold);
                    dgvRow.ReadOnly = false; // Hacerla editable si hay algo pendiente
                }
            }

            // Numerar las filas (esto sigue igual)
            for (int i = 0; i < btnImprimirCuenta.Rows.Count; i++)
            {
                btnImprimirCuenta.Rows[i].Cells[0].Value = i + 1;
            }

            GetTotal(); // Actualiza el total del pedido
        }

        private void UpdateQtyEnviado(object detailID, int qtyEnviado)
        {
            string qry = @"
        UPDATE tblDetails 
        SET IsSentToKitchen = 1 
        WHERE DetailID = @DetailID";

            //SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
            SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con);
            cmd.Parameters.AddWithValue("@DetailID", detailID);

            if (MainClass.con.State == ConnectionState.Closed)
                MainClass.con.Open();

            cmd.ExecuteNonQuery();
            MainClass.con.Close();

            // Opcional: registrar qtyEnviado en otra tabla si llevás historial
        }




        private void btnCheckout_Click(object sender, EventArgs e)
        {
            frmCheout frm = new frmCheout();
            frm.MainID = id;
            frm.amt = Convert.ToDouble(lblTotal.Text);
            frm.TableName = lblTable.Text;
            frm.ShowDialog();

            guna2MessageDialog1.Show("Guardado Correctamente");

            // 🔁 Liberar la mesa automáticamente
            if (!string.IsNullOrWhiteSpace(lblTable.Text))
            {
                string updateStatusQry = "UPDATE tables SET status = 'Libre' WHERE tname = @tableName";
                //using (SqlCommand cmd = new SqlCommand(updateStatusQry, MainClass.con))
                // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
                using (SQLiteCommand cmd = new SQLiteCommand(updateStatusQry, MainClass.con))
                {
                    cmd.Parameters.AddWithValue("@tableName", lblTable.Text.Trim());

                    if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                    cmd.ExecuteNonQuery();
                    if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
                }
            }

            MainId = 0;
            btnImprimirCuenta.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "00";


        }

        /// <summary>
        /// Handles the click event for the Print Bill button.
        /// This method initiates the printing process for the current bill, optimized for thermal printers.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnPrintCuenta_Click(object sender, EventArgs e)
        {
            // Verificar si hay elementos para imprimir
            if (btnImprimirCuenta.Rows.Count == 0)
            {
                guna2MessageDialog1.Show("No hay productos en la cuenta para imprimir.");
                return;
            }

            // Asegúrate de que MainId esté disponible y sea > 0
            if (MainId == 0)
            {
                guna2MessageDialog1.Show("Error: La orden no ha sido guardada. Guarde la orden primero.");
                return;
            }

            // 1. Obtener todos los detalles (cliente, pago) de la base de datos
            // Esta función retornará un diccionario con todos los datos extra
            var dbDetails = GetOrderDetailsFromDB(MainId);

            // 2. Crear un diccionario con la información de la cabecera e inicializar variables
            string orderType = dbDetails.ContainsKey("OrderType") ? dbDetails["OrderType"] : "Mesa";

            var infoHeaders = new Dictionary<string, string>
    {
        { "Mesa", lblTable.Text },
        { "Mozo", lblWaiter.Text },
        { "TipoOrden", orderType } // Añadimos el tipo de orden
    };

            // Si el cliente está visible, añadir su nombre (siempre es bueno tenerlo)
            if (lblCustomer.Visible && !string.IsNullOrWhiteSpace(lblCustomer.Text))
            {
                infoHeaders["Cliente"] = lblCustomer.Text.Replace("CUSTOMER: ", "").Replace("PHONE: ", "");
            }

            // 3. 🛑 AÑADIR DETALLES DE DELIVERY/TAKE AWAY Y PAGO 🛑
            bool isDeliveryOrTakeAway = orderType.ToLower() == "delivery" || orderType.ToLower().Contains("take");

            if (isDeliveryOrTakeAway)
            {
                // Agregar datos de cliente/entrega si existen
                if (dbDetails.ContainsKey("CustomerName") && !string.IsNullOrWhiteSpace(dbDetails["CustomerName"]))
                {
                    infoHeaders["Cliente"] = dbDetails["CustomerName"];
                    infoHeaders["Telefono"] = dbDetails.ContainsKey("Phone") ? dbDetails["Phone"] : "N/A";
                    infoHeaders["Direccion"] = dbDetails.ContainsKey("Address") ? dbDetails["Address"] : "N/A";
                    infoHeaders["Referencia"] = dbDetails.ContainsKey("Reference") ? dbDetails["Reference"] : "N/A";
                }

                // Agregar detalles de pago
                infoHeaders["Total"] = dbDetails.ContainsKey("Total") ? dbDetails["Total"] : lblTotal.Text;
                infoHeaders["Recibido"] = dbDetails.ContainsKey("Received") ? dbDetails["Received"] : "0";
                infoHeaders["Cambio"] = dbDetails.ContainsKey("Change") ? dbDetails["Change"] : "0";
                infoHeaders["EstatusPago"] = dbDetails.ContainsKey("Status") ? dbDetails["Status"] : "Pendiente";
            }
            // Si no es delivery, al menos pasamos el total y el estatus para que la cuenta lo incluya
            else
            {
                infoHeaders["Total"] = lblTotal.Text; // Usar el total de la UI
                infoHeaders["EstatusPago"] = dbDetails.ContainsKey("Status") ? dbDetails["Status"] : "Pendiente";
            }

            // Llamar a la clase CuentaPrinter para imprimir el recibo (impresora de 80mm)
            CuentaPrinter.ImprimirCuenta(btnImprimirCuenta, lblTotal.Text, infoHeaders);
        }


        // =========================================================================================

        /// <summary>
        /// Obtiene el encabezado de la orden y los detalles del cliente de la base de datos.
        /// </summary>
        private Dictionary<string, string> GetOrderDetailsFromDB(int mainId)
        {
            var details = new Dictionary<string, string>();

            string qryMain = @"SELECT 
                         m.orderType, m.total, m.received, m.change, m.status, 
                         c.Name AS CustomerName, c.Address, c.Phone, c.Reference
                       FROM tblMain m
                       LEFT JOIN tblCustomers c ON m.CustomerID = c.CustomerID
                       WHERE m.MainID = @MainId;";

            using (SQLiteCommand cmd = new SQLiteCommand(qryMain, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@MainId", mainId);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();

                try
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Almacenar los resultados en el diccionario
                            details["OrderType"] = dr["orderType"]?.ToString();
                            details["Total"] = dr["total"]?.ToString();
                            details["Received"] = dr["received"]?.ToString();
                            details["Change"] = dr["change"]?.ToString();
                            details["Status"] = dr["status"]?.ToString();
                            details["CustomerName"] = dr["CustomerName"]?.ToString();
                            details["Address"] = dr["Address"]?.ToString();
                            details["Phone"] = dr["Phone"]?.ToString();
                            details["Reference"] = dr["Reference"]?.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener datos de la orden para imprimir: " + ex.Message);
                }
                finally
                {
                    MainClass.con.Close();
                }
            }

            return details;
        }

        /// <summary>
        /// This method is called by the PrintDocument when it needs to render a page for printing.
        /// It draws the bill details onto the provided Graphics object, optimized for thermal printers.
        /// </summary>
        /// <param name="sender">The source of the event (the PrintDocument).</param>
        /// <param name="e">The <see cref="PrintPageEventArgs"/> containing data about the page to be printed.</param>
        private void PrintBill(object sender, PrintPageEventArgs e)
        {
            // Define different fonts for various sections of the bill for better readability
            // Optimized for thermal printer (smaller fonts)
            Font fontTitle = new Font("Arial", 10, FontStyle.Bold);     // Font for the main title
            Font fontHeader = new Font("Arial", 8, FontStyle.Bold);    // Font for section headers (e.g., "Producto", "Cantidad")
            Font fontBody = new Font("Arial", 8);                      // Font for general text and product details
            Font fontTotal = new Font("Arial", 9, FontStyle.Bold);     // Font for the final total amount
            Font fontObservation = new Font("Arial", 7, FontStyle.Italic); // Font for observations

            // Get the Graphics object from the PrintPageEventArgs. This object allows us to draw
            // text, lines, and other graphics onto the printer page.
            Graphics g = e.Graphics;

            // Define the starting Y position for drawing, considering the top margin of the page.
            float yPos = e.MarginBounds.Top;
            // Define the left and right margins of the printable area.
            float leftMargin = e.MarginBounds.Left;
            float rightMargin = e.MarginBounds.Right;
            float lineWidth = rightMargin - leftMargin;

            // --- Draw the Bill Title ---
            string title = "--- DETALLE DE CUENTA ---";
            // Measure the size of the title string with the specified font to center it.
            SizeF titleSize = g.MeasureString(title, fontTitle);
            // Draw the title string, centered horizontally.
            g.DrawString(title, fontTitle, Brushes.Black, leftMargin + (lineWidth - titleSize.Width) / 2, yPos);
            // Move the Y position down for the next section, adding some padding.
            yPos += titleSize.Height + 10;

            // --- Draw General Information ---
            // Concatenate table, waiter, date, and time information.
            string billInfo = $"Mesa: {lblTable.Text}\nMozo: {lblWaiter.Text}\nFecha: {DateTime.Now.ToShortDateString()}\nHora: {DateTime.Now.ToShortTimeString()}";
            // Draw the general information.
            g.DrawString(billInfo, fontBody, Brushes.Black, leftMargin, yPos);
            // Move the Y position down based on the height of the drawn text.
            yPos += g.MeasureString(billInfo, fontBody).Height + 10;

            // --- Draw Product List Headers ---
            // Draw column headers for the product details.
            // Define X positions for columns
            float colQty = leftMargin;
            float colProduct = leftMargin + 50;
            float colUnitPrice = leftMargin + 180;
            float colTotal = rightMargin;

            g.DrawString("Cant.", fontHeader, Brushes.Black, colQty, yPos);
            g.DrawString("Producto", fontHeader, Brushes.Black, colProduct, yPos);
            g.DrawString("Precio U.", fontHeader, Brushes.Black, colUnitPrice, yPos);
            g.DrawString("Total", fontHeader, Brushes.Black, colTotal - g.MeasureString("Total", fontHeader).Width, yPos);
            // Move Y position down after headers.
            yPos += g.MeasureString("Producto", fontHeader).Height + 3; // Reduced spacing
            // Draw a horizontal line to separate headers from product items.
            g.DrawLine(Pens.Black, leftMargin, yPos, rightMargin, yPos);
            yPos += 3; // Small padding after the line.

            // --- Draw Individual Product Details ---
            // Loop through each row in the DataGridView (btnImprimirCuenta) to get product information.
            foreach (DataGridViewRow row in btnImprimirCuenta.Rows)
            {
                // Skip new rows that are not yet committed.
                if (row.IsNewRow) continue;

                // Extract product details from the DataGridView cells.
                string productName = row.Cells["dgvPName"].Value.ToString();
                string quantity = row.Cells["dgvQty"].Value.ToString();
                // Format price and total as currency (e.g., "$12.34").
                string price = double.Parse(row.Cells["dgvPrice"].Value.ToString()).ToString("N2"); // Use N2 for standard number format
                string total = double.Parse(row.Cells["dgvAmount"].Value.ToString()).ToString("N2"); // Use N2 for standard number format
                string observation = row.Cells["dgvObs"].Value?.ToString() ?? string.Empty;

                // Draw quantity, product name, unit price, and total on the current line.
                g.DrawString(quantity, fontBody, Brushes.Black, colQty, yPos);
                g.DrawString(productName, fontBody, Brushes.Black, colProduct, yPos);
                g.DrawString(price, fontBody, Brushes.Black, colUnitPrice, yPos);
                // Align total to the right.
                g.DrawString(total, fontBody, Brushes.Black, colTotal - g.MeasureString(total, fontBody).Width, yPos);
                // Move Y position down.
                yPos += g.MeasureString(productName, fontBody).Height;

                // If there's an observation, print it indented below the product.
                if (!string.IsNullOrEmpty(observation))
                {
                    g.DrawString($"  - Obs: {observation}", fontObservation, Brushes.Black, leftMargin + 10, yPos);
                    yPos += g.MeasureString("  - Obs:", fontObservation).Height;
                }

                yPos += 3; // Small padding between items.
            }

            // --- Draw Separator and Total ---
            // Draw a line to separate the product list from the total section.
            g.DrawLine(Pens.Black, leftMargin, yPos, rightMargin, yPos);
            yPos += 5; // Padding after the line.

            // Define the text for "TOTAL:" and get the actual total value from lblTotal.
            string totalText = "TOTAL:";
            string totalValue = double.Parse(lblTotal.Text).ToString("N2"); // Format total as standard number.

            // Measure sizes to align the total.
            SizeF totalTextSize = g.MeasureString(totalText, fontTotal);
            SizeF totalValueSize = g.MeasureString(totalValue, fontTotal);

            // Draw "TOTAL:" aligned to the left.
            g.DrawString(totalText, fontTotal, Brushes.Black, leftMargin, yPos);
            // Draw the total value aligned to the right.
            g.DrawString(totalValue, fontTotal, Brushes.Black, rightMargin - totalValueSize.Width, yPos);

            yPos += fontTotal.Height + 10;
            // Add a simple footer
            string footer = "--- ¡GRACIAS POR SU VISITA! ---";
            SizeF footerSize = g.MeasureString(footer, fontBody);
            g.DrawString(footer, fontBody, Brushes.Black, leftMargin + (lineWidth - footerSize.Width) / 2, yPos);
        }

        // Variables de nivel de clase (fuera de cualquier método)
        private int originalWidth;
        private int originalHeight;
        private bool isReduced = false; // Nuevo flag para rastrear el estado reducido (75%)

        // El método Load permanece igual, guarda el tamaño inicial.
       

        private void btnMaximixed_Click(object sender, EventArgs e)
        {
            // Lógica principal: Maximizar/Restaurar/Reducir

            if (this.WindowState == FormWindowState.Normal && !isReduced)
            {
                // 1. ESTADO ACTUAL: Normal (tamaño original).
                // ACCIÓN: Maximizar la ventana.
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                // 2. ESTADO ACTUAL: Maximizado.
                // ACCIÓN: Volver al estado Normal y luego reducir al 75%
                this.WindowState = FormWindowState.Normal;

                // Ejecutamos la reducción al 97%
                const double factor = 0.97;
                this.Width = (int)(originalWidth * factor);
                this.Height = (int)(originalHeight * factor);
                this.CenterToScreen();

                // Establecer la bandera para indicar que ahora está en el estado reducido.
                isReduced = true;
            }
            else if (this.WindowState == FormWindowState.Normal && isReduced)
            {
                // 3. ESTADO ACTUAL: Normal (pero reducido al 75%).
                // ACCIÓN: Volver al tamaño original completo.
                this.Width = originalWidth;
                this.Height = originalHeight;
                this.CenterToScreen();

                // Limpiar la bandera para indicar que ahora está en el estado Normal/Original.
                isReduced = false;
            }
        }

        private void btnMinimixed_Click(object sender, EventArgs e)
        {
            // Sets the form state to minimized, which hides it from the user 
            // and places it on the taskbar.
            this.WindowState = FormWindowState.Minimized;
        }
    }

}
