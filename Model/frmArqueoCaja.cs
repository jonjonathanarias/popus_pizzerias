using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.Model
{
    public partial class frmArqueoCaja : frmHome
    {
        private DateTime _fecha;
        private ArqueoResultado _resultado;



        public frmArqueoCaja(DateTime fecha)
        {
            InitializeComponent();
            _fecha = fecha;
            CargarArqueo();
            label1.Visible = false;
        }

        private void CargarArqueo()
        {
            var arqueo = new ArqueoCaja(_fecha);

            if (arqueo.YaFueCerrado())
            {
                btnCerrarArqueo.Enabled = false;
                MessageBox.Show("El arqueo de esta fecha ya fue cerrado.", "Arqueo Cerrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            _resultado = arqueo.RealizarArqueo();

            lblFecha.Text = $"Fecha: {_fecha.ToShortDateString()}";
            lblDinIn.Text = $"Mesas: {_resultado.TotalMesas:C2}";
            lblTakeAway.Text = $"Take Away: {_resultado.TotalTakeAway:C2}";
            lblDelivery.Text = $"Delivery: {_resultado.TotalDelivery:C2}";
            lblTotalGeneral.Text = $"TOTAL: {_resultado.TotalGeneral:C2}";
        }

       
        private void label3_Click(object sender, EventArgs e)
        {

        }

        

        private void btnCerrarArqueo_Click(object sender, EventArgs e)
        {
            var arqueo = new ArqueoCaja(_fecha);

            if (arqueo.CerrarArqueo(_resultado))
            {
                MessageBox.Show("Arqueo cerrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 🔽 Limpiar Labels
                lblDinIn.Text = "Mesas: $0.00";
                lblTakeAway.Text = "Take Away: $0.00";
                lblDelivery.Text = "Delivery: $0.00";
                lblTotalGeneral.Text = "TOTAL: $0.00";

                // 🔽 También podés desactivar el botón si querés
                btnCerrarArqueo.Enabled = false;
            }
            else
            {
                MessageBox.Show("Error al cerrar el arqueo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCierreX_Click(object sender, EventArgs e)
        {
            var arqueo = new ArqueoCaja(_fecha);
            var resultado = arqueo.ObtenerArqueoParcial();
            txtResultado.Text = "--- Cierre X (Parcial) ---\r\n" + resultado.ToString();
        }

        private void btnCierreY_Click(object sender, EventArgs e)
        {
            var arqueo = new ArqueoCaja(_fecha);
            var resultado = arqueo.ObtenerArqueoAcumulado();
            txtResultado.Text = "--- Cierre Y (Acumulado) ---\r\n" + resultado.ToString();
        }
    }
}

