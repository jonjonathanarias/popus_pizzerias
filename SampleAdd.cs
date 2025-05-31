using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria
{
    public partial class SampleAdd : Form
    {
        public SampleAdd()
        {
            InitializeComponent();
        }

        public virtual void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        public virtual void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
