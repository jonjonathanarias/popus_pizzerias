using System;
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

namespace popus_pizzeria.Model
{
    public partial class frmPOS : Form
    {
        public frmPOS()
        {
            InitializeComponent();
        }

        public int MainId = 0;
        public string OrderType;

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

        private void _Click(object sender, EventArgs e)
        {

            Guna.UI2.WinForms.Guna2Button b = (Guna.UI2.WinForms.Guna2Button)sender;
            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = pro.PCategory.ToLower().Contains(b.Text.Trim().ToLower());
            }
        }

        private void AddItems(string id, string name, string cat, string price, Image pimage)
        {
            bool found = false;
            var w = new ucProduct()
            {
                PName = name,
                PPrice = price,
                PCategory = cat,
                PImage = pimage,
                id = Convert.ToInt32(id),
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
                    if (Convert.ToInt32(item.Cells["dgvid"].Value) == wdg.id)
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
                    guna2DataGridView1.Rows.Add(new object[] { 0, wdg.id, wdg.PName, 1, wdg.PPrice, wdg.PPrice });
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

                AddItems(item["pID"].ToString(), item["pName"].ToString(), item["catName"].ToString(), 
                    item["pPrice"].ToString(), Image.FromStream(new MemoryStream(imagearry)));
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = pro.PName.ToLower().Contains(txtBuscar.Text.Trim().ToLower());
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
            
            guna2DataGridView1.Rows.Clear();
            MainId = 0;
            lblTotal.Text = "00";
        }

        private void btnDelivery_Click(object sender, EventArgs e)
        {
            OrderType = "Delivery";
        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            OrderType = "Mostrador";
        }

        private void btnDin_Click(object sender, EventArgs e)
        {
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

            MessageBox.Show("Mesa seleccionada: " + frm.TableName);
            MessageBox.Show("Mozo seleccionado: " + frm2.WaiterName);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

}
