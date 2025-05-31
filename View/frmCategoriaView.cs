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
    public partial class frmCategoriaView : SampleView
    {
        public frmCategoriaView()
        {
            InitializeComponent();
        }

        public void GetData()
        {
            string qry = "Select * From category where catName like '%"+txtBuscar.Text+"%' ";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvName);

            MainClass.LoadData(qry, guna2DataGridView1, lb);

        }

        private void frmCategoriaView_Load(object sender, EventArgs e)
        {
            GetData();
        }

        public override void btnAgregar_Click(object sender, EventArgs e)
        {
            frmCategoriaAdd frm = new frmCategoriaAdd();
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
                if(guna2MessageDialog1.Show("Estas seguro que desea editar la categoria?")==DialogResult.Yes)
                {
                    frmCategoriaAdd frm = new frmCategoriaAdd();
                    frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    frm.txtNombre.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvName"].Value);
                    frm.Show();
                    GetData();
                }


                
            }
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvdelete")
            {
                //alerta para confirmar la eliminacion de la categoria
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                if (guna2MessageDialog1.Show("Estas seguro que desea eliminar la categoria?") == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    string qry = "Delete from category where catID= " + id + " ";
                    Hashtable ht = new Hashtable();
                    MainClass.SQl(qry, ht);

                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Show("Categoria Eliminada Correctamente");
                    GetData();
                }

                
            }
        }
    }
}
