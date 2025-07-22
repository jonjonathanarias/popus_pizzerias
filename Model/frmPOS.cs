using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            guna2DataGridView1.Rows.Clear(); 
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
            guna2DataGridView1.BorderStyle = BorderStyle.FixedSingle;
            SetupDataGridViewColumns(); // Call this method to set up columns
            AddCategory();

            ProductPanel.Controls.Clear();
            LoadProduct();
        }


        private void SetupDataGridViewColumns()
        {

            guna2DataGridView1.Columns.Clear();
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "S/N", Name = "dgvSerial", Width = 50 });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "DetailID", Name = "dgvid", Visible = false });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ProdID", Name = "dgvProID", Visible = false });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nombre", Name = "dgvPName", Width = 150 });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cantidad", Name = "dgvQty", Width = 70 });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Precio", Name = "dgvPrice", Width = 80 });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Total", Name = "dgvAmount", Width = 100 });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Observacion", Name = "dgvObs", Width = 150 });
            guna2DataGridView1.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "Enviado", Name = "dgvIsSent", Visible = false });
            AddDeleteButtonColumn();
        }


        private void AddCategory()
        {
            string qry = "Select * from category";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
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

            for (int i = guna2DataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                var row = guna2DataGridView1.Rows[i];
                if (row.IsNewRow) continue;

                bool isSent = Convert.ToBoolean(row.Cells["dgvIsSent"]?.Value ?? false);
                if (isSent) continue; // Solo combinamos filas nuevas

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

                    // Eliminar fila actual porque se fusionó
                    guna2DataGridView1.Rows.RemoveAt(i);
                }
                else
                {
                    groupedRows[key] = row;
                }
            }
        }



        private void AddItems(string code, string proID, string name, string cat, string price, Image pimage)
        {
            bool found = false;
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
            w.Tag = id; // reemplazar por el código real
            w.Tag = code; // Aquí sí va el código inventado del producto



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


                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                {
                    int prodId = Convert.ToInt32(item.Cells["dgvProID"].Value);
                    bool enviado = Convert.ToBoolean(item.Cells["dgvIsSent"]?.Value ?? false);

                    if (prodId == wdg.id && !enviado)
                    {
                        found = true;
                        item.Cells["dgvQty"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) + 1;
                        item.Cells["dgvAmount"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) *
                                        double.Parse(item.Cells["dgvPrice"].Value.ToString());
                        break;
                    }
                }

                // 👇 Si ya existe pero está marcado como enviado, agregar como nuevo
                if (!found)
                {
                    int newRowIndex = guna2DataGridView1.Rows.Add(new object[] {
                        0,
                        0, // DetailID = 0 => nuevo
                            wdg.id,
                        wdg.PName,
                        1,
                        wdg.PPrice,
                        wdg.PPrice,
                        "",
                        false // No enviado
                    });

                    // Estilo visual para nuevo producto
                    var newRow = guna2DataGridView1.Rows[newRowIndex];
                    newRow.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    newRow.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Bold);
                }


                MergePendingProductRows();


                GetTotal();
            };

        }

        private void LoadProduct()
        {
            string qry = "Select * From products inner join category on catID = categoryID";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
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
            dgvDeleteButton.Text = "*";
            dgvDeleteButton.UseColumnTextForButtonValue = true;
            dgvDeleteButton.Width = 50;
            guna2DataGridView1.Columns.Add(dgvDeleteButton);

        }
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.ColumnIndex == guna2DataGridView1.Columns["dgvDelete"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = guna2DataGridView1.Rows[e.RowIndex];
                int currentQty = Convert.ToInt32(row.Cells["dgvQty"].Value);

                if (currentQty > 1)
                {
                    
                    row.Cells["dgvQty"].Value = currentQty - 1;
                    
                    row.Cells["dgvAmount"].Value = (currentQty - 1) * Convert.ToDouble(row.Cells["dgvPrice"].Value);
                }
                else
                {
                    
                    guna2DataGridView1.Rows.RemoveAt(e.RowIndex);
                }

                
                GetTotal();
            }

            
            if (e.RowIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "dgvObs")
            {
                
                string currentObservation = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvObs"].Value?.ToString() ?? string.Empty;

                using (frmObservation obsForm = new frmObservation(currentObservation))
                {
                    if (obsForm.ShowDialog() == DialogResult.OK)
                    {
                        guna2DataGridView1.Rows[e.RowIndex].Cells["dgvObs"].Value = obsForm.Observation;
                    }
                }
            }
        }
        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // por serial
            int count = 0;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {

                count++;
                row.Cells[0].Value = count;
            }
        }

        private void GetTotal()
        {
            double total = 0;
            lblTotal.Text = "";

            foreach (DataGridViewRow item in guna2DataGridView1.Rows)
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
            guna2DataGridView1.Rows.Clear();
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
                using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
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
            string qryMain = "";
            string qryDetail = "";
            int detailID = 0; 
            
            if (MainId == 0) 
            {
                qryMain = @"INSERT INTO tblMain 
                    (aDate, aTime, TableName, WaiterName, status, orderType, total, received, change, CustomerID, discount)
                    VALUES (@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @CustomerID, 0);
                    SELECT SCOPE_IDENTITY()"; 
            }
            else 
            {
                
                qryMain = @"UPDATE tblMain SET 
                        status = @status, 
                        total = @total, 
                        received = @received, 
                        change = @change, 
                        CustomerID = @CustomerID
                    WHERE MainID = @ID";
            }

            SqlCommand cmdMain = new SqlCommand(qryMain, MainClass.con);
            cmdMain.Parameters.AddWithValue("@ID", MainId);
            cmdMain.Parameters.AddWithValue("@aDate", DateTime.Now.Date);
            cmdMain.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmdMain.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmdMain.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmdMain.Parameters.AddWithValue("@status", "Pendiente"); // Status is 'Pendiente' when KOT is sent
            cmdMain.Parameters.AddWithValue("@orderType", OrderType);
            cmdMain.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text));
            cmdMain.Parameters.AddWithValue("@received", 0); // Not received yet for KOT
            cmdMain.Parameters.AddWithValue("@change", 0);   // No change yet
            cmdMain.Parameters.AddWithValue("@CustomerID", CustomerID);

            if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
            try
            {
                if (MainId == 0)
                {
                    MainId = Convert.ToInt32(cmdMain.ExecuteScalar()); // Get the new MainID
                }
                else
                {
                    cmdMain.ExecuteNonQuery(); // Update existing Main record
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el encabezado del pedido: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
                return; // Exit if main saving fails
            }
            finally
            {
                if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
            }


      
            List<int> productsToKot = new List<int>();
            List<string> tempNewProductDetails = new List<string>(); // To build KOT text

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.IsNewRow) continue; 
                
                detailID = Convert.ToInt32(row.Cells["dgvid"].Value); // DetailID from DGV
                int prodID = Convert.ToInt32(row.Cells["dgvProID"].Value);
                int qty = Convert.ToInt32(row.Cells["dgvQty"].Value);
                double price = Convert.ToDouble(row.Cells["dgvPrice"].Value);
                double amount = Convert.ToDouble(row.Cells["dgvAmount"].Value);
                string observation = row.Cells["dgvObs"].Value?.ToString() ?? "";
                bool isSent = Convert.ToBoolean(row.Cells["dgvIsSent"]?.Value ?? false);


                // IMPORTANT: Only insert/update items that haven't been sent yet
                if (detailID == 0 && !isSent) // This is a brand new item in the DGV for this order
                {
                    // Insert new detail
                    qryDetail = @"INSERT INTO tblDetails 
                                  (MainID, prodID, qty, price, amount, observation, IsSentToKitchen) 
                                  VALUES 
                                  (@MainID, @ProdID, @qty, @price, @amount, @observation, 0);
                                  SELECT SCOPE_IDENTITY()"; // Get the new DetailID

                    SqlCommand cmdDetail = new SqlCommand(qryDetail, MainClass.con);
                    cmdDetail.Parameters.AddWithValue("@MainID", MainId);
                    cmdDetail.Parameters.AddWithValue("@ProdID", prodID);
                    cmdDetail.Parameters.AddWithValue("@qty", qty);
                    cmdDetail.Parameters.AddWithValue("@price", price);
                    cmdDetail.Parameters.AddWithValue("@amount", amount);
                    cmdDetail.Parameters.AddWithValue("@observation", observation);

                    if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                    try
                    {
                        // Update the DGV row's DetailID with the newly inserted ID from the DB
                        row.Cells["dgvid"].Value = Convert.ToInt32(cmdDetail.ExecuteScalar());
                        // Mark it as sent in the DGV immediately after saving
                        row.Cells["dgvIsSent"].Value = true;

                        // Add to list for KOT generation
                        productsToKot.Add(prodID);
                        tempNewProductDetails.Add($"{qty} x {row.Cells["dgvPName"].Value}");
                        if (!string.IsNullOrWhiteSpace(observation))
                            tempNewProductDetails.Add($"  -> {observation}");

                        // Visually update the row in the DGV
                        row.ReadOnly = true;
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                        row.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Strikeout);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al guardar detalle del producto {row.Cells["dgvPName"].Value}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
                    }
                }
                
                
                
            }

            guna2MessageDialog1.Show("Guardado correctamente");

            // --- Step 3: Check if there are any products to send to kitchen from the current operation ---
            if (productsToKot.Count == 0)
            {
                guna2MessageDialog1.Show("No hay productos nuevos para enviar a cocina.");
                // No need to clear the form here, the existing items stay.
                return;
            }

            // --- Step 4: Generate and Display/Print KOT for ONLY the newly sent items ---
            string textoComanda = GenerarTextoComandaForNewItems(MainId, tempNewProductDetails); // Pass the new items
            ComandaPrinter.ImprimirTexto(textoComanda);


            MainId = 0; 
            ClearForm();
        }




        // --- NEW METHOD FOR GENERATING KOT FOR ONLY NEWLY SENT ITEMS ---
        private string GenerarTextoComandaForNewItems(int MainId, List<string> newProductDetails)
        {
            StringBuilder sb = new StringBuilder();

            // Fetch main order details
            string qryMain = @"SELECT m.aDate, m.aTime, m.TableName, m.WaiterName, m.orderType, c.Name AS CustomerName, 
                                        c.Address, c.Phone, c.Reference
                                    FROM tblMain m
                                    LEFT JOIN tblCustomers c ON m.CustomerID = c.CustomerID
                                    WHERE m.MainID = @MainId;";

            using (SqlCommand cmd = new SqlCommand(qryMain, MainClass.con))
            {
                int nroComanda = ComandaManager.ObtenerNumeroCorrelativo();
                cmd.Parameters.AddWithValue("@MainId", MainId);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    
                    if (dr.Read())
                    {
                        sb.AppendLine($"**** COMANDA N° {nroComanda} ****");
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

            // Add the new product details passed from btnKot_Click
            foreach (string detailLine in newProductDetails)
            {
                sb.AppendLine(detailLine);
            }

            sb.AppendLine("--------------------------------");
            sb.AppendLine("  ¡Gracias!");
            MessageBox.Show(sb.ToString()); // SOLO PARA DEPURAR
            return sb.ToString();
        }
   
        private string GenerarTextoComanda(int MainId)
        {
            StringBuilder sb = new StringBuilder();

            string qryMain = @"SELECT m.aDate, m.aTime, m.TableName, m.WaiterName, m.orderType, c.Name AS CustomerName, 
                                        c.Address, c.Phone, c.Reference
                                    FROM tblMain m
                                    LEFT JOIN tblCustomers c ON m.CustomerID = c.CustomerID
                                    WHERE m.MainID = @MainId;
";

            using (SqlCommand cmd = new SqlCommand(qryMain, MainClass.con))
            {
                cmd.Parameters.AddWithValue("@MainId", MainId);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
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

            using (SqlCommand cmd2 = new SqlCommand(qryDetails, MainClass.con))
            {
                cmd2.Parameters.AddWithValue("@mainId", MainId);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                using (SqlDataReader dr = cmd2.ExecuteReader())
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

            string qryDetailupdate = @"UPDATE tblDetails SET IsSentToKitchen = 1 WHERE MainID = @mainID AND IsSentToKitchen = 0";

            using (SqlCommand cmd2 = new SqlCommand(qryDetailupdate, MainClass.con))
            {
                cmd2.Parameters.AddWithValue("@mainId", MainId);
                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                
                MainClass.con.Close();
            }
        }

        

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "dgvObs")
            {
                
                string currentObservation = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvObs"].Value?.ToString() ?? string.Empty;

                
                using (frmObservation obsForm = new frmObservation(currentObservation))
                {
                    if (obsForm.ShowDialog() == DialogResult.OK)
                    {
                     
                        guna2DataGridView1.Rows[e.RowIndex].Cells["dgvObs"].Value = obsForm.Observation;
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
            string qry = @"SELECT d.DetailID, d.ProdID, p.pName, d.qty, d.price, d.amount, d.observation, d.IsSentToKitchen,
                          m.TableName, m.WaiterName, m.orderType
                   FROM tblMain m 
                   INNER JOIN tblDetails d ON m.MainID = d.MainID
                   INNER JOIN products p ON p.pID = d.ProdID
                   WHERE m.MainID = " + id;

            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows.Count == 0) return;

            string orderType = dt.Rows[0]["orderType"].ToString();
            if (orderType == "Delivery") btnDelivery.Checked = true;
            else if (orderType == "Take away") btnTake.Checked = true;
            else btnDin.Checked = true;

            lblTable.Text = dt.Rows[0]["TableName"].ToString();
            lblWaiter.Text = dt.Rows[0]["WaiterName"].ToString();
            lblTable.Visible = true;
            lblWaiter.Visible = true;

            guna2DataGridView1.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                    object[] obj = {
                0, // Index opcional
                row["DetailID"],
                row["ProdID"],
                row["pName"],
                row["qty"],
                row["price"],
                row["amount"],
                row["observation"],
                Convert.ToBoolean(row["IsSentToKitchen"])
            };
                int index = guna2DataGridView1.Rows.Add(obj);

                // Si ya fue enviado, marcar visualmente
                if (Convert.ToBoolean(row["IsSentToKitchen"]))
                {
                    var dgvRow = guna2DataGridView1.Rows[index];
                    dgvRow.ReadOnly = true;
                    dgvRow.DefaultCellStyle.ForeColor = Color.Gray;
                    dgvRow.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Strikeout);
                }
                else
                {
                    var dgvRow = guna2DataGridView1.Rows[index];
                    dgvRow.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    dgvRow.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Bold);
                }

            }

            GetTotal();
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
                using (SqlCommand cmd = new SqlCommand(updateStatusQry, MainClass.con))
                {
                    cmd.Parameters.AddWithValue("@tableName", lblTable.Text.Trim());

                    if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                    cmd.ExecuteNonQuery();
                    if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();
                }
            }

            MainId = 0;
            guna2DataGridView1.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "00";


        }


    }

}
