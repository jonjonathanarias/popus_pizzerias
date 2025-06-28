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
        private bool hayPedidoAbierto = false;
        private int mesaPedidoAbierta = -1;
        public int PedidoMainID { get; set; }




        private void frmTableSelect_Load(object sender, EventArgs e)
        {
            panelMesas.Controls.Clear(); // Asegurate que panelMesas sea el panel visual

            PlanoHelper.CargarPlanoVisual(
                panelMesas,
                alSeleccionarMesa: (btn) =>
                {
                    int tid = Convert.ToInt32(btn.Tag);
                    TableName = btn.Text;
                    mesaSeleccionadaId = tid;

                    // Buscar el MainID del pedido activo en esa mesa
                    string qry = "SELECT MainID FROM tblMain WHERE TableName = @TableName AND status = 'Pendiente'";
                    SqlCommand cmd = new SqlCommand(qry, MainClass.con);
                    cmd.Parameters.AddWithValue("@TableName", TableName);

                    if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
                    object result = cmd.ExecuteScalar();
                    if (MainClass.con.State == ConnectionState.Open) MainClass.con.Close();

                    if (result != null)
                        PedidoMainID = Convert.ToInt32(result);  // Lo usamos luego en frmPOS
                    else
                        PedidoMainID = 0;  // No hay pedido activo

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                },
                alDobleClickMesa: (btn) =>
                {
                    int tid = Convert.ToInt32(btn.Tag);

                    // Marcar como Libre
                    string qry = "UPDATE tables SET status = 'Libre' WHERE tid = @tid";
                    Hashtable ht = new Hashtable { { "@tid", tid } };
                    MainClass.SQl(qry, ht);

                    mesaSeleccionadaId = -1;
                    mesasPagadas.Remove(tid);
                    frmTableSelect_Load(null, null);
                }
            );

            

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
