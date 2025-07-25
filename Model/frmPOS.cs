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
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "QtyOriginal",
                Name = "dgvQtyOriginal",
                Visible = false
            });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
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

                    guna2DataGridView1.Rows.RemoveAt(i); // Eliminar fila duplicada
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

                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
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
                            item.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Bold);
                        }
                        break;
                    }
                }

                // Si no existe fila previa para ese producto, agregar nueva
                if (!found)
                {
                    int n = guna2DataGridView1.Rows.Add();

                    guna2DataGridView1.Rows[n].Cells["dgvID"].Value = 0;
                    guna2DataGridView1.Rows[n].Cells["dgvProID"].Value = wdg.id;
                    guna2DataGridView1.Rows[n].Cells["dgvPName"].Value = wdg.PName;
                    guna2DataGridView1.Rows[n].Cells["dgvQty"].Value = 1;
                    guna2DataGridView1.Rows[n].Cells["dgvPrice"].Value = wdg.PPrice;
                    guna2DataGridView1.Rows[n].Cells["dgvAmount"].Value = wdg.PPrice;
                    guna2DataGridView1.Rows[n].Cells["dgvObs"].Value = "";
                    guna2DataGridView1.Rows[n].Cells["dgvDelete"].Value = "Eliminar";
                    guna2DataGridView1.Rows[n].Cells["dgvIsSent"].Value = false; // Nuevo ítem, no está enviado
                    guna2DataGridView1.Rows[n].Cells["dgvQtyEnviado"].Value = 0; // Cantidad enviada es 0

                    guna2DataGridView1.Rows[n].DefaultCellStyle.ForeColor = Color.DarkGreen;
                    guna2DataGridView1.Rows[n].DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Bold);
                }
                MergePendingProductRows(); // Esto debería seguir funcionando
                GetTotal(); // Actualiza el total
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
            // 1. Guardar encabezado (La lógica para tblMain sigue siendo la misma)
            string qryMain = MainId == 0
                ? @"INSERT INTO tblMain 
        (aDate, aTime, TableName, WaiterName, status, orderType, total, received, change, CustomerID, discount)
        VALUES (@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @CustomerID, 0);
        SELECT SCOPE_IDENTITY();"
                : @"UPDATE tblMain SET 
        status = @status, total = @total, received = @received, change = @change, CustomerID = @CustomerID
        WHERE MainID = @ID";

            SqlCommand cmdMain = new SqlCommand(qryMain, MainClass.con);
            cmdMain.Parameters.AddWithValue("@ID", MainId);
            cmdMain.Parameters.AddWithValue("@aDate", DateTime.Now.Date);
            cmdMain.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmdMain.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmdMain.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmdMain.Parameters.AddWithValue("@status", "Pendiente");
            cmdMain.Parameters.AddWithValue("@orderType", OrderType);
            cmdMain.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text));
            cmdMain.Parameters.AddWithValue("@received", 0);
            cmdMain.Parameters.AddWithValue("@change", 0);
            cmdMain.Parameters.AddWithValue("@CustomerID", CustomerID);

            if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
            try
            {
                if (MainId == 0)
                    MainId = Convert.ToInt32(cmdMain.ExecuteScalar());
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

            // 2. Procesar productos (Lógica modificada para enviar solo lo pendiente)
            List<string> tempNewProductDetails = new List<string>();
            bool hayProductosParaEnviar = false;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                int prodID = Convert.ToInt32(row.Cells["dgvProID"].Value);
                int currentQtyInGrid = Convert.ToInt32(row.Cells["dgvQty"].Value); // Cantidad total actual de este ítem en la grilla
                double price = Convert.ToDouble(row.Cells["dgvPrice"].Value);
                string observation = row.Cells["dgvObs"].Value?.ToString() ?? "";

                int qtyAlreadySent = Convert.ToInt32(row.Cells["dgvQtyEnviado"].Value ?? 0); // Cantidad de este ítem ya enviada (desde BD o KOTs anteriores)

                int qtyToSend = currentQtyInGrid - qtyAlreadySent; // Esta es la cantidad NUEVA a enviar

                if (qtyToSend <= 0) continue; // Si no hay cantidad nueva (o es negativa), no enviamos nada

                hayProductosParaEnviar = true;

                // Insertar un nuevo registro en tblDetails para la *cantidad adicional* a enviar
                string qryDetail = @"INSERT INTO tblDetails
              (MainID, prodID, qty, price, amount, observation, IsSentToKitchen)
              VALUES
              (@MainID, @ProdID, @qty, @price, @amount, @observation, 1);"; // Marcar como enviado inmediatamente al insertar

                SqlCommand cmdDetail = new SqlCommand(qryDetail, MainClass.con);
                cmdDetail.Parameters.AddWithValue("@MainID", MainId);
                cmdDetail.Parameters.AddWithValue("@ProdID", prodID);
                cmdDetail.Parameters.AddWithValue("@qty", qtyToSend); // Enviamos solo la diferencia
                cmdDetail.Parameters.AddWithValue("@price", price);
                cmdDetail.Parameters.AddWithValue("@amount", qtyToSend * price);
                cmdDetail.Parameters.AddWithValue("@observation", observation);

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                try
                {
                    cmdDetail.ExecuteNonQuery();

                    // Añadir los detalles para la impresión de la comanda (solo los ítems recién enviados)
                    tempNewProductDetails.Add($"{qtyToSend} x {row.Cells["dgvPName"].Value}");
                    if (!string.IsNullOrWhiteSpace(observation))
                        tempNewProductDetails.Add($"  -> {observation}");

                    // Actualizar la fila del DataGridView para reflejar la cantidad recién enviada
                    row.Cells["dgvQtyEnviado"].Value = qtyAlreadySent + qtyToSend; // Suma al total ya enviado

                    // Actualizar el estilo de la fila si toda la cantidad ahora está enviada
                    if (Convert.ToInt32(row.Cells["dgvQtyEnviado"].Value) == currentQtyInGrid)
                    {
                        row.Cells["dgvIsSent"].Value = true; // Marca como completamente enviado
                        row.ReadOnly = true; // Hacer la fila de solo lectura
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                        row.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Strikeout);
                    }
                    else
                    {
                        // Si aún quedan cantidades sin enviar, asegurar que no esté tachado ni gris
                        row.Cells["dgvIsSent"].Value = false; // Marca como no completamente enviado
                        row.ReadOnly = false; // Asegurar que la fila sea editable
                        row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                        row.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Bold);
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

            if (!hayProductosParaEnviar)
            {
                guna2MessageDialog1.Show("No hay productos nuevos ni modificados para enviar a cocina.");
                return;
            }

            guna2MessageDialog1.Show("Pedido enviado a cocina correctamente");

            // Llama a la función para imprimir solo los ítems nuevos
            string textoComanda = GenerarTextoComandaForNewItems(MainId, tempNewProductDetails);
            ComandaPrinter.ImprimirTexto(textoComanda);

            // IMPORTANTE: No llamar a ClearForm() aquí si el pedido sigue abierto y se esperan más cambios/pagos.
            // Esto es lo que estaba borrando el formulario después de cada envío.
            // MainId = 0; 
            // ClearForm(); 
        }








        // --- NEW METHOD FOR GENERATING KOT FOR ONLY NEWLY SENT ITEMS ---
        // Ejemplo de cómo podría ser tu GenerarTextoComandaForNewItems
        public string GenerarTextoComandaForNewItems(int mainId, List<string> newItemsDetails)
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

            SqlCommand cmd = new SqlCommand(qry, MainClass.con); // Sintaxis corregida
            cmd.Parameters.AddWithValue("@MainID", id);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows.Count == 0) return;

            lblTable.Text = dt.Rows[0]["TableName"].ToString();
            lblWaiter.Text = dt.Rows[0]["WaiterName"].ToString();
            lblTable.Visible = true;
            lblWaiter.Visible = true;

            guna2DataGridView1.Rows.Clear();

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

                int index = guna2DataGridView1.Rows.Add(obj);
                var dgvRow = guna2DataGridView1.Rows[index];

                // Establecer el estilo de la fila: si todo está enviado, gris y tachado; si no, verde y negrita.
                if (totalQty == totalQtyEnviado)
                {
                    dgvRow.DefaultCellStyle.ForeColor = Color.Gray;
                    dgvRow.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Strikeout);
                    dgvRow.ReadOnly = true; // Hacer la fila de solo lectura si ya está todo enviado
                }
                else
                {
                    dgvRow.DefaultCellStyle.ForeColor = Color.DarkGreen;
                    dgvRow.DefaultCellStyle.Font = new Font(guna2DataGridView1.Font, FontStyle.Bold);
                    dgvRow.ReadOnly = false; // Hacerla editable si hay algo pendiente
                }
            }

            // Numerar las filas (esto sigue igual)
            for (int i = 0; i < guna2DataGridView1.Rows.Count; i++)
            {
                guna2DataGridView1.Rows[i].Cells[0].Value = i + 1;
            }

            GetTotal(); // Actualiza el total del pedido
        }

        private void UpdateQtyEnviado(object detailID, int qtyEnviado)
        {
            string qry = @"
        UPDATE tblDetails 
        SET IsSentToKitchen = 1 
        WHERE DetailID = @DetailID";

            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
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
