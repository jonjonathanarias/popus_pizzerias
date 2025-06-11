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
    public partial class frmObservation : SampleAdd
    {
        public string Observation { get; set; }

        public frmObservation(string textoActual = "")
        {
            InitializeComponent();
            txtObservation.Text = textoActual;
        }

        public override void btnGuardar_Click(object sender, EventArgs e)
        {
            Observation = txtObservation.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public override void btnCerrar_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}

