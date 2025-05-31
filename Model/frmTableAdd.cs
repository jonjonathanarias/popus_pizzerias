using System;
using System.Collections;
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
    public partial class frmTableAdd : SampleAdd
    {
        public frmTableAdd()
        {
            InitializeComponent();
        }

        public int id = 0;

        public override void btnGuardar_Click(object sender, EventArgs e)
        {
            string qry = "";

            if (id == 0)
            {
                qry = "Insert into tables Values(@Name)";
            }
            else
            {
                qry = "Update tables Set tname = @Name where tid = @id ";
            }

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtNombre.Text);

            if (MainClass.SQl(qry, ht) > 0)
            {
                MessageBox.Show("Mesa Guardada");
                id = 0;
                txtNombre.Text = "";
                txtNombre.Focus();
            }
        }

        public override void btnCerrar_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}
