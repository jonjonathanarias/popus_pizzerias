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
    public partial class frmCheout : SampleAdd
    {
        public frmCheout()
        {
            InitializeComponent();
        }

        public double amt;
        public int MainID = 0;
        public string TableName = "";

        private void txtRecived_TextChanged(object sender, EventArgs e)
        {
            double amt = 0;
            double receipt = 0;
            double change = 0;
            

            double.TryParse(txtBillAmount.Text, out amt);
            double.TryParse(txtRecived.Text, out receipt);
            
            change = Math.Abs( amt - receipt); //Convierte en positivo

            txtChange.Text = change.ToString();
        }

        public override void btnGuardar_Click(object sender, EventArgs e)
        {
            string qry = @"UPDATE tblMain 
               SET total = @total, 
                   received = @rec, 
                   change = @change, 
                   status = 'Pagado', 
                   paymentMethod = @paymentMethod 
               WHERE MainID = @id";


            string updateTableQry = "UPDATE tables SET status = 'Pagada' WHERE tname = @tableName";


            Hashtable ht = new Hashtable();
            ht.Add("@id", MainID);
            ht.Add("@total", txtBillAmount.Text);
            ht.Add("@rec", txtRecived.Text);
            ht.Add("@change", txtChange.Text);
            ht.Add("@paymentMethod", cmbPaymentMethod.Text);


            if (!string.IsNullOrEmpty(TableName)) 
            {
                ht.Add("@tableName", TableName);
            }

            if (string.IsNullOrWhiteSpace(cmbPaymentMethod.Text))
            {
                MessageBox.Show("Seleccione un medio de pago");
                return;
            }



            if (MainClass.SQl(qry, ht) > 0)
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Show("Cobrado correctamente");
                guna2MessageDialog1.Parent = this;

                
                if (!string.IsNullOrEmpty(TableName))
                {
                    
                    Hashtable htTableUpdate = new Hashtable();
                    htTableUpdate.Add("@tableName", TableName);
                    MainClass.SQl(updateTableQry, htTableUpdate);
                }
                

                this.Close(); 
            }
            
        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMethod.Text == "Efectivo")
            {
                txtRecived.Enabled = true;
                txtRecived.Text = "";
            }
            else
            {
                txtRecived.Enabled = false;
                txtRecived.Text = txtBillAmount.Text;
                txtChange.Text = "0";
            }
        }


        private void frmCheout_Load(object sender, EventArgs e)
        {
            txtBillAmount.Text = amt.ToString();
        }
    }
}
