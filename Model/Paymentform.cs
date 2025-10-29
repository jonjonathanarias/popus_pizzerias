using System;
using System.Drawing;
using System.Windows.Forms;

namespace popus_pizzeria.Model
{
    public partial class Paymentform : SampleAdd
    {
        // ... (Tus propiedades existentes) ...
        public double Total { get; private set; }
        public double Recibido { get; private set; }
        public double Cambio { get; private set; }
        public bool PagoRealizado { get; private set; } = false;

        // 🛑 NUEVA PROPIEDAD: Para devolver el método de pago seleccionado
        public string MetodoPago { get; private set; } = "Efectivo";

        public double MontoAPagar
        {
            get { return Total; }
            set
            {
                Total = value;
                txtTotal.Text = Total.ToString("N2");
            }
        }

        public Paymentform(double total)
        {
            InitializeComponent();
            this.MontoAPagar = total;
            // 🛑 Llenar el ComboBox al inicializar (ejemplo)
            cmbPaymentMethod.Items.AddRange(new object[] { "Efectivo", "Transferencia", "QR", "Tarjeta de Crédito", "Tarjeta de Débito" });
            cmbPaymentMethod.SelectedIndex = 0; // Efectivo por defecto
        }

        private void txtRecibido_TextChanged(object sender, EventArgs e)
        {
            // Solo calcular cambio si el método es Efectivo
            if (this.MetodoPago == "Efectivo" && double.TryParse(txtRecibido.Text, out double recibido))
            {
                this.Recibido = recibido;
                this.Cambio = recibido - this.Total;
                txtCambio.Text = Cambio.ToString("N2");
            }
            else if (this.MetodoPago != "Efectivo")
            {
                // Para pagos electrónicos, se asume que Recibido = Total
                this.Recibido = this.Total;
                this.Cambio = 0;
                txtCambio.Text = "0.00";
            }
            else
            {
                this.Recibido = 0;
                this.Cambio = 0;
                txtCambio.Text = "0.00";
            }
        }

        // 🛑 NUEVA LÓGICA DE CONFIRMACIÓN DE PAGO 🛑
        public override void btnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Si es pago Electrónico (Transferencia, QR, Tarjeta), asumimos pago exitoso.
            if (this.MetodoPago != "Efectivo")
            {
                this.Recibido = this.Total;
                this.Cambio = 0;
                this.PagoRealizado = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            // 2. Si es Efectivo, se requiere que la cantidad recibida sea suficiente.
            if (this.Recibido < this.Total)
            {
                MessageBox.Show("El monto recibido en efectivo es insuficiente.", "Error de Pago", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.PagoRealizado = true;
            this.DialogResult = DialogResult.OK; // Indica que se confirmó el pago
            this.Close();
        }

        private void Paymentform_Load(object sender, EventArgs e)
        {
            txtRecibido.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 🛑 Actualizar la propiedad y forzar el recálculo
            this.MetodoPago = cmbPaymentMethod.SelectedItem.ToString();

            // Bloquear/desbloquear txtRecibido y forzar el cálculo para reflejar el estado
            if (this.MetodoPago != "Efectivo")
            {
                txtRecibido.Enabled = false;
                txtRecibido.Text = this.Total.ToString("N2"); // Mostrar el total
            }
            else
            {
                txtRecibido.Enabled = true;
                txtRecibido.Text = string.Empty; // Limpiar para que el usuario escriba
            }

            // Forzar la ejecución de la lógica de cambio/recibido
            txtRecibido_TextChanged(txtRecibido, EventArgs.Empty);
        }

        public override void btnCerrar_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}