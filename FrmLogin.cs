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
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
           if (MainClass.IsValidUser(txtUser.Text, txtPass.Text) == false)
            {
                guna2MessageDialog1.Show("Usuario y Contraseña Incorrecta");
                return;
            }
           else
            {
                this.Hide();
                frmMain frm = new frmMain();
                frm.Show();
            }
        }
    }
}
