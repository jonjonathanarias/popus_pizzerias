using Guna.UI2.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.Model
{
    public partial class frmTableSelect : Form
    {
        public frmTableSelect()
        {
            InitializeComponent();
        }

        public string TableName { get; set; }
        private Dictionary<int, DateTime> mesasPagadas = new Dictionary<int, DateTime>();
        private int mesaSeleccionadaId = -1;
        private Timer timerLiberacion;
        private int minutosParaLiberar = 3;



        private void frmTableSelect_Load(object sender, EventArgs e)
        {
            string qry = "Select * from tables";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            panelMesas.Controls.Clear();
            string zonasQry = "SELECT * FROM zonas";
            SqlDataAdapter daZ = new SqlDataAdapter(zonasQry, MainClass.con);
            DataTable zonas = new DataTable();
            daZ.Fill(zonas);

            foreach (DataRow row in zonas.Rows)
            {
                var zona = new Panel
                {
                    Location = new Point(Convert.ToInt32(row["x"]), Convert.ToInt32(row["y"])),
                    Width = Convert.ToInt32(row["ancho"]),
                    Height = Convert.ToInt32(row["alto"]),
                    BackColor = row["tipo"].ToString().Contains("Mostrador") ? Color.DarkRed :
                                row["tipo"].ToString().Contains("Interno") ? Color.LightGreen : Color.LightBlue,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = row["tipo"].ToString()
                };

                zona.Controls.Add(new Label
                {
                    Text = row["nombre"].ToString(),
                    Location = new Point(5, 5),
                    BackColor = Color.Transparent
                });

                panelMesas.Controls.Add(zona);
            }


            foreach (DataRow row in dt.Rows)
            {
                Guna2Button btn = new Guna2Button();
                btn.Text = row["tname"].ToString();
                btn.Width = 60;
                btn.Height = 60;
                btn.Location = new Point(Convert.ToInt32(row["xpos"]), Convert.ToInt32(row["ypos"]));

                string estado = row["status"].ToString();
                switch (estado)
                {
                    case "Libre":
                        btn.FillColor = Color.Green;
                        break;
                    case "Ocupada":
                        btn.FillColor = Color.Red;
                        break;
                    case "Pagada":
                        btn.FillColor = Color.Blue;
                        break;
                }

                btn.Tag = row["tid"];
                btn.Click += b_Click;
                btn.DoubleClick += Mesa_DoubleClick;


                panelMesas.Controls.Add(btn);
            }

            IniciarTimerLiberacion();
            InicializarBotonCheckout();
        }
        private void InicializarBotonCheckout()
        {
            if (!this.Controls.ContainsKey("btnCheckout"))
            {
                Guna2Button btnCheckout = new Guna2Button();
                btnCheckout.Name = "btnCheckout";
                btnCheckout.Text = "Pagar y liberar";
                btnCheckout.Width = 150;
                btnCheckout.Height = 40;
                btnCheckout.Location = new Point(10, panelMesas.Bottom);
                btnCheckout.FillColor = Color.DarkOrange;
                btnCheckout.Click += btnCheckout_Click;
                this.Controls.Add(btnCheckout);
            }
        }
        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (mesaSeleccionadaId == -1)
            {
                MessageBox.Show("Seleccioná una mesa primero.");
                return;
            }

            DialogResult dr = MessageBox.Show("¿Confirmás el pago de esta mesa?", "Confirmar", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                string qry = "UPDATE tables SET status = 'Pagada' WHERE tid = @tid";
                Hashtable ht = new Hashtable();
                ht.Add("@tid", mesaSeleccionadaId);
                MainClass.SQl(qry, ht);

                mesasPagadas[mesaSeleccionadaId] = DateTime.Now;
                mesaSeleccionadaId = -1;

                frmTableSelect_Load(null, null);
            }
        }
        private void Mesa_DoubleClick(object sender, EventArgs e)
        {
            Guna2Button btn = sender as Guna2Button;
            int tid = Convert.ToInt32(btn.Tag);

            // Cambiar a Libre manualmente
            string qry = "UPDATE tables SET status = 'Libre' WHERE tid = @tid";
            Hashtable ht = new Hashtable();
            ht.Add("@tid", tid);
            MainClass.SQl(qry, ht);

            mesaSeleccionadaId = -1;
            mesasPagadas.Remove(tid);
            frmTableSelect_Load(null, null);
        }
        public void IniciarTimerLiberacion()
        {
            if (timerLiberacion == null)
            {
                timerLiberacion = new Timer();
                timerLiberacion.Interval = 10000; // cada 10 segundos
                timerLiberacion.Tick += TimerLiberacion_Tick;
                timerLiberacion.Start();
            }
        }
        private void TimerLiberacion_Tick(object sender, EventArgs e)
        {
            List<int> mesasLiberadas = new List<int>();

            foreach (var kvp in mesasPagadas)
            {
                int mesaId = kvp.Key;
                DateTime tiempoPago = kvp.Value;

                if ((DateTime.Now - tiempoPago).TotalMinutes >= minutosParaLiberar)
                {
                    string qry = "UPDATE tables SET status = 'Libre' WHERE tid = @tid";
                    Hashtable ht = new Hashtable();
                    ht.Add("@tid", mesaId);
                    MainClass.SQl(qry, ht);
                    mesasLiberadas.Add(mesaId);
                }
            }

            foreach (int id in mesasLiberadas)
                mesasPagadas.Remove(id);

            if (mesasLiberadas.Count > 0)
                frmTableSelect_Load(null, null);
        }
       

        private void b_Click(object sender, EventArgs e)
        {

            Guna2Button btn = sender as Guna2Button;
            int tid = Convert.ToInt32(btn.Tag);

            // Cambiar el estado a "Ocupada"
            string qry = "UPDATE tables SET status = 'Ocupada' WHERE tid = @tid";
            Hashtable ht = new Hashtable();
            ht.Add("@tid", tid);
            MainClass.SQl(qry, ht);

            TableName = btn.Text;
            mesaSeleccionadaId = tid;
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
