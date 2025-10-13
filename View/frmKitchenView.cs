using System;
using System.Collections;
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


namespace popus_pizzeria.View
{
    public partial class frmKitchenView : Form
    {
        public frmKitchenView()
        {
            InitializeComponent();
        }

        private void frmKitchenView_Load(object sender, EventArgs e)
        {
            GetOrders();
        }

        private void GetOrders()
        {
            flowLayoutPanel1.Controls.Clear();
            string qryMain = @"SELECT * FROM tblMain WHERE status = 'Pendiente'";

            try
            {
                //using (SqlCommand cmdMain = new SqlCommand(qryMain, MainClass.con))
                // using (SqlDataAdapter daMain = new SqlDataAdapter(cmdMain))
               // CAMBIO: SqlCommand->SQLiteCommand, SqlDataAdapter->SQLiteDataAdapter
                using (SQLiteCommand cmdMain = new SQLiteCommand(qryMain, MainClass.con))
                using (SQLiteDataAdapter daMain = new SQLiteDataAdapter(cmdMain))
                {
                    DataTable dtMain = new DataTable();
                    daMain.Fill(dtMain);

                    foreach (DataRow row in dtMain.Rows)
                    {
                        int mainId = Convert.ToInt32(row["MainID"]);
                        FlowLayoutPanel orderPanel = CrearPanelPedido();
                        FlowLayoutPanel headerPanel = CrearPanelEncabezado(
                            row["TableName"].ToString(),
                            row["WaiterName"].ToString(),
                            row["aTime"].ToString(),
                            row["orderType"].ToString()
                        );

                        // Agregar encabezado del pedido
                        orderPanel.Controls.Add(headerPanel);

                        // Agregar info del cliente
                        AgregarDatosCliente(orderPanel, mainId);

                        // Agregar productos
                        AgregarProductosPedido(orderPanel, mainId);

                        // Botón de completado
                        var btnCompletar = new Guna.UI2.WinForms.Guna2Button
                        {
                            AutoRoundedCorners = true,
                            Size = new Size(100, 35),
                            FillColor = Color.FromArgb(241, 85, 126),
                            Margin = new Padding(30, 5, 3, 10),
                            Text = "Completado",
                            Tag = mainId.ToString()
                        };
                        btnCompletar.Click += new EventHandler(b_click);
                        orderPanel.Controls.Add(btnCompletar);

                        // Agregar todo al flow principal
                        flowLayoutPanel1.Controls.Add(orderPanel);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener pedidos: " + ex.Message);
            }
        }

        private FlowLayoutPanel CrearPanelPedido()
        {
            return new FlowLayoutPanel
            {
                AutoSize = true,
                Width = 230,
                Height = 350,
                FlowDirection = FlowDirection.TopDown,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10, 10, 10, 10)
            };
        }

            private FlowLayoutPanel CrearPanelEncabezado(string table, string waiter, string time, string orderType)
            {
                var panel = new FlowLayoutPanel
                {
                    BackColor = Color.FromArgb(50, 55, 89),
        
                    Width = 230,
                    Height = 125,
                    FlowDirection = FlowDirection.TopDown,
                    Margin = new Padding(0, 0, 0, 0)
                };

                panel.Controls.Add(CrearLabel("Mesa: " + table, Color.White, new Padding(10, 10, 3, 0)));
                panel.Controls.Add(CrearLabel("Mozo: " + waiter, Color.White, new Padding(10, 10, 3, 0)));
                panel.Controls.Add(CrearLabel("Hora: " + time, Color.White, new Padding(10, 10, 3, 0)));
                panel.Controls.Add(CrearLabel("Tipo: " + orderType, Color.White, new Padding(10, 5, 3, 10)));

                return panel;
            }

            private Label CrearLabel(string texto, Color color, Padding margen)
            {
                return new Label
                {
                    Text = texto,
                    ForeColor = color,
                    Margin = margen,
                    AutoSize = true
                };
            }

            private void AgregarDatosCliente(FlowLayoutPanel panel, int mainId)
            {
                string qry = @"SELECT c.Name, c.Phone, c.Address, c.Reference 
                               FROM tblMain m
                               INNER JOIN tblCustomers c ON m.CustomerID = c.CustomerID 
                               WHERE m.MainID = @mid";

            //using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            // CAMBIO: SqlCommand -> SQLiteCommand, SqlDataAdapter -> SQLiteDataAdapter
            using (SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con))
            {
                    cmd.Parameters.AddWithValue("@mid", mainId);
                    DataTable dt = new DataTable();
                    //using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        DataRow r = dt.Rows[0];
                        panel.Controls.Add(CrearLabel("Cliente: " + r["Name"], Color.Black, new Padding(10, 5, 3, 0)));
                        panel.Controls.Add(CrearLabel("Teléfono: " + r["Phone"], Color.Black, new Padding(10, 2, 3, 0)));
                        panel.Controls.Add(CrearLabel("Dirección: " + r["Address"], Color.Black, new Padding(10, 2, 3, 0)));
                        panel.Controls.Add(CrearLabel("Referencia: " + r["Reference"], Color.Black, new Padding(10, 2, 3, 10)));
                    }
                }
            }

            private void AgregarProductosPedido(FlowLayoutPanel panel, int mainId)
            {
                string qry = @"SELECT p.pName, d.qty, d.Observation
                               FROM tblMain m
                               INNER JOIN tblDetails d ON m.MainID = d.MainID
                               INNER JOIN products p ON p.pID = d.ProdID
                               WHERE m.MainID = @mid";

            //using (SqlCommand cmd = new SqlCommand(qry, MainClass.con))
            // CAMBIO: SqlCommand -> SQLiteCommand, SqlDataAdapter -> SQLiteDataAdapter
            using (SQLiteCommand cmd = new SQLiteCommand(qry, MainClass.con))
            {
                    cmd.Parameters.AddWithValue("@mid", mainId);
                    DataTable dt = new DataTable();
                    //using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string nombre = dt.Rows[i]["pName"].ToString();
                        string cantidad = dt.Rows[i]["qty"].ToString();
                        string detalle = dt.Rows[i]["Observation"].ToString();
                        panel.Controls.Add(CrearLabel($"{cantidad}. {nombre}  {detalle}", Color.Black, new Padding(10, 10, 3, 0)));
                    }
                }
            }

            private void b_click(object sender, EventArgs e)
            {
                int id = Convert.ToInt32((sender as Guna.UI2.WinForms.Guna2Button).Tag.ToString());
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                if (guna2MessageDialog1.Show("Estas seguro que desea completar el pedido?") == DialogResult.Yes)
                {
                    string qry = @"update tblMain set status = 'Completo' where MainID = @ID";
                    Hashtable ht = new Hashtable();
                    ht.Add("@ID", id);

                    if (MainClass.SQl(qry, ht)>0)
                    {
                        guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                        guna2MessageDialog1.Show("Pedido completado correctamente.\nCobrar y cerrar en lista de pedidos");
                    }
                    GetOrders();
                }
            }
    }
}
