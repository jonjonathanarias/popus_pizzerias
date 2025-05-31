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
    public partial class frmStaffAdd : SampleAdd
    {
        public frmStaffAdd()
        {
            InitializeComponent();
        }

        public int id = 0;

        public override void btnGuardar_Click(object sender, EventArgs e)
        {
            string qry = "";

            if (id == 0)
            {
                qry = "Insert into staff Values(@Name, @Phone, @Role)";
            }
            else
            {
                qry = "Update staff Set sName = @Name, sPhone = @Phone, sRole = @Role where staffID = @id ";
            }

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtNombre.Text);
            ht.Add("@Phone", txtPhone.Text);
            ht.Add("@Role", cbRole.Text);

            if (MainClass.SQl(qry, ht) > 0)
            {
                MessageBox.Show("Empleado Registrado");
                id = 0;
                txtNombre.Text = "";
                txtPhone.Text = "";
                cbRole.SelectedIndex =  -1;
                txtNombre.Focus();
            }
        }

        public override void btnCerrar_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
    
}
