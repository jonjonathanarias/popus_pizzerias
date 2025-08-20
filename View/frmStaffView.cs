using Microsoft.VisualBasic;
using popus_pizzeria.Model;
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

namespace popus_pizzeria.View
{
    public partial class frmStaffView : SampleView
    {
        public frmStaffView()
        {
            InitializeComponent();
        }

        private void frmStaff_Load(object sender, EventArgs e)
        {
            GetData();
        }

        public void GetData()
        {
            string qry = "Select * From staff where sName like '%" + txtBuscar.Text + "%' ";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvName);
            lb.Items.Add(dgvPhone);
            lb.Items.Add(dgvRole);

            MainClass.LoadData(qry, guna2DataGridView1, lb);

        }

        public override void btnAgregar_Click(object sender, EventArgs e)
        {
            frmStaffAdd frm = new frmStaffAdd();
            frm.ShowDialog();
            GetData();
        }

        public override void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvedit")
            {
                //alerta para confirmar la editar de la categoria
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                if (guna2MessageDialog1.Show("Estas seguro que desea editar la categoria?") == DialogResult.Yes)
                {
                    frmStaffAdd frm = new frmStaffAdd();
                    frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    frm.txtNombre.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvName"].Value);
                    frm.txtPhone.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvPhone"].Value);
                    frm.cbRole.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvRole"].Value);
                    frm.Show();
                    GetData();
                }



            }
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvdelete")
            {
                //alerta para confirmar la eliminacion de la categoria
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                if (guna2MessageDialog1.Show("Estas seguro que desea eliminar el empleado?") == DialogResult.Yes)
                {
                    // Pide la contraseña
                    string password = Interaction.InputBox("Por favor, ingrese la contraseña para eliminar el Empleado:", "Confirmar Eliminación", "");

                    //Verifica si la contraseña es correcta

                    if (password == "ivanleonelbrendapopulin") 
                    {
                        int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                        string qry = "Delete from staff where staffID= " + id + " ";
                        Hashtable ht = new Hashtable();
                        MainClass.SQl(qry, ht);

                        guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                        guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                        guna2MessageDialog1.Show("Empleado Eliminado Correctamente");
                        GetData();
                    }
                    else
                    {
                        // Muestra un mensaje de error si la contraseña es incorrecta
                        guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                        guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                        guna2MessageDialog1.Show("Contraseña incorrecta. No se pudo eliminar el empleado.");
                    }


                }


            }
        }
    }
}
