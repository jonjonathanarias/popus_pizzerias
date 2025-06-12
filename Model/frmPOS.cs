using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPOS_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.BorderStyle = BorderStyle.FixedSingle;
            AddCategory();

            ProductPanel.Controls.Clear();
            LoadProduct();
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
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    b = new Guna.UI2.WinForms.Guna2Button();
                    b.Location = new Point(b.Location.X, diff);
                    b.FillColor = Color.FromArgb(50, 55, 89);
                    b.Size = new Size(244, 58);
                    b.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
                    b.Text = row["catName"].ToString();

                    //evento para el click que ejecuta el metodo
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

            foreach (Control item in ProductPanel.Controls)
            {
                if (item is ucProduct pro)
                {
                    pro.Visible = pro.PCategory.ToLower().Contains(b.Text.Trim().ToLower());
                }
            }

            ReorganizeProducts(); // reorganiza los visibles
        }


        private void AddItems(string id,string proID, string name, string cat, string price, Image pimage)
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
                    if (Convert.ToInt32(item.Cells["dgvProID"].Value) == wdg.id)
                    {
                        found = true;
                        item.Cells["dgvQty"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) + 1;
                        item.Cells["dgvAmount"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) *
                                                        double.Parse(item.Cells["dgvPrice"].Value.ToString());
                        break;
                    }

                }
                if (!found)
                {
                    guna2DataGridView1.Rows.Add(new object[] { 0,  0, wdg.id, wdg.PName, 1, wdg.PPrice, wdg.PPrice });
                }

                GetTotal();
            };

        }

        //traer productos de la base de datos

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

                AddItems("0",item["pID"].ToString(), item["pName"].ToString(), item["catName"].ToString(), 
                    item["pPrice"].ToString(), Image.FromStream(new MemoryStream(imagearry)));
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            foreach (Control item in ProductPanel.Controls)
            {
                if (item is ucProduct pro)
                {
                    pro.Visible = pro.PName.ToLower().Contains(txtBuscar.Text.Trim().ToLower());
                }
            }

            ReorganizeProducts(); // reorganiza los visibles
        }
        

        //Metodo para agregar y quitar productos del dataGreid
        private void AddDeleteButtonColumn()
        {
            DataGridViewButtonColumn dgvDeleteButton = new DataGridViewButtonColumn();
            dgvDeleteButton.HeaderText = "Action";
            dgvDeleteButton.Name = "dgvDelete"; 
            dgvDeleteButton.Text = "-"; 
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
        //*****************************
        
        
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
            OrderType = "Din In";
            // Mostrar formulario de selección de mesa
            frmTableSelect frm = new frmTableSelect();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                lblTable.Text = frm.TableName;
                lblTable.Visible = true;
            }
            else
            {
                lblTable.Text = "";
                lblTable.Visible = false;
            }

            // Mostrar formulario de selección de mozo
            frmWaiterSelect frm2 = new frmWaiterSelect();
            if (frm2.ShowDialog() == DialogResult.OK)
            {
                lblWaiter.Text = frm2.WaiterName;
                lblWaiter.Visible = true;
            }
            else
            {
                lblWaiter.Text = "";
                lblWaiter.Visible = false;
            }

            guna2MessageDialog1.Show("Mesa seleccionada: " + frm.TableName);
            guna2MessageDialog1.Show("Mozo seleccionado: " + frm2.WaiterName);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnKot_Click(object sender, EventArgs e)
        {
            //guardar los datos en la base de datos
            //crear tablas en base

            string qry1 = ""; //Main table
            string qry2 = ""; //Detail table

            int detailID = 0;

            if (MainId == 0) // insert
            {
                qry1 = @"insert into tblMain values (@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @CustomerID);
                                             Select SCOPE_IDENTITY()";
            }
            else //update
            {
                qry1 = @"update tblMain set status = @status, total = @total, 
                                                     received = @received, change = @change, CustomerID = @CustomerID
                                                     where MainID = @ID ";
            }

            
            

            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainId);
            cmd.Parameters.AddWithValue("@aDate", Convert.ToDateTime(DateTime.Now.Date));
            cmd.Parameters.AddWithValue("@aTime",   DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Pendiente");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text));
            cmd.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@change", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

            if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
            if (MainId == 0) {MainId = Convert.ToInt32 (cmd.ExecuteScalar()); } else { cmd.ExecuteNonQuery(); }
            if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // evita fila vacía

                detailID = Convert.ToInt32(row.Cells["dgvid"].Value);

                if (detailID == 0) //insert
                {
                    qry2 = @"insert into tblDetails values (@MainID, @ProdID, @qty, @price, @amount, @observation)";
                }
                else //update
                {
                    qry2 = @"update tblDetails set prodID = @ProdID, qty = @qty, price = @price, amount = @amount, observation = @observation
                                        where DetailID  = @ID";
                }

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainId);
                cmd2.Parameters.AddWithValue("@ProdID", Convert.ToInt32(row.Cells["dgvProID"].Value)); // ✅ aquí está correcto
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));
                cmd2.Parameters.AddWithValue("@observation", row.Cells["dgvObs"].Value?.ToString() ?? "");

                if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
                cmd2.ExecuteNonQuery();
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }
            }



            // ✅ Mensaje de éxito y limpieza — solo una vez
            guna2MessageDialog1.Show("Guardado Correctamente");
            MainId = 0;
            detailID = 0;
            guna2DataGridView1.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "00";

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
            string qry = @"select * from tblMain m 
                                        inner join tblDetails d on m.MainID = d.MainID
                                        inner join products p on p.pID = d.ProdID
                                        where m.MainID = "+id+"";

            SqlCommand cmd2 = new SqlCommand(qry, MainClass.con);
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            da2.Fill(dt2);

            if (dt2.Rows[0]["orderType"].ToString() == "Delivery")
            {
                btnDelivery.Checked = true;
                lblWaiter.Visible = false ;
                lblTable.Visible = false;
            }
            else if(dt2.Rows[0]["orderType"].ToString() == "Take away")
            {
                btnTake.Checked = true;
                lblWaiter.Visible = false;
                lblTable.Visible = false;
            }
            else
            {
                btnDin.Checked = true;
                lblWaiter.Visible = true;
                lblTable.Visible = true;
            }

            guna2DataGridView1.Rows.Clear();
            
            foreach (DataRow item in dt2.Rows)
            {
                lblTable.Text = item["TableName"].ToString();
                lblWaiter.Text = item["WaiterName"].ToString();

                string detailid = item["DetailID"].ToString();
                string proName = item["pName"].ToString();
                string proid = item["ProdID"].ToString();
                string qty = item["qty"].ToString();
                string price = item["price"].ToString();
                string amount = item["amount"].ToString();

                object[] obj = {0, detailid, proid, proName, qty, price, amount};
                guna2DataGridView1.Rows.Add(obj);
            }
            GetTotal();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            frmCheout frm = new frmCheout();
            frm.MainID = id;
            frm.amt = Convert.ToDouble( lblTotal.Text);
            frm.ShowDialog();

            guna2MessageDialog1.Show("Guardado Correctamente");
            guna2MessageDialog1.Parent = this;
            MainId = 0;
            
            guna2DataGridView1.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "00";


        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            string qry1 = ""; //Main table
            string qry2 = ""; //Detail table

            int detailID = 0;

            if (OrderType == "")
            {
                guna2MessageDialog1.Show( "Porfavor elija un tipo de orden de pedido antes de enviar el pedido a la cocina");
                return;
            }

            if (MainId == 0) // insert
            {
                qry1 = @"insert into tblMain values (@aDate, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change);
                                             Select SCOPE_IDENTITY()";
            }
            else //update
            {
                qry1 = @"update tblMain set status = @status, total = @total, 
                                                     received = @received, change = @change where MainID = @ID ";
            }




            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainId);
            cmd.Parameters.AddWithValue("@aDate", Convert.ToDateTime(DateTime.Now.Date));
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Hold");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text));
            cmd.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@change", Convert.ToDouble(0));

            if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
            if (MainId == 0) { MainId = Convert.ToInt32(cmd.ExecuteScalar()); } else { cmd.ExecuteNonQuery(); }
            if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // evita fila vacía

                detailID = Convert.ToInt32(row.Cells["dgvid"].Value);

                if (detailID == 0) //insert
                {
                    qry2 = @"insert into tblDetails values (@MainID, @ProdID, @qty, @price, @amount, @observation)";
                }
                else //update
                {
                    qry2 = @"update tblDetails set prodID = @ProdID, qty = @qty, price = @price, amount = @amount, observation = @observation
                                        where DetailID  = @ID";
                }

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainId);
                cmd2.Parameters.AddWithValue("@ProdID", Convert.ToInt32(row.Cells["dgvProID"].Value)); // ✅ aquí está correcto
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));
                cmd2.Parameters.AddWithValue("@observation", row.Cells["dgvObs"].Value?.ToString() ?? "");

                if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
                cmd2.ExecuteNonQuery();
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }
            }



            // ✅ Mensaje de éxito y limpieza — solo una vez
            guna2MessageDialog1.Show("Guardado Correctamente");
            MainId = 0;
            detailID = 0;
            guna2DataGridView1.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "00";
        }
    }

}
