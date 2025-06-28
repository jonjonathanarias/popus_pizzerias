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
    public partial class frmProductsView : SampleView
    {
        public frmProductsView()
        {
            InitializeComponent();
        }

        private void frmProductsView_Load(object sender, EventArgs e)
        {
            GetData();
        }

        public void GetData()
        {
            string qry = "SELECT pID, pName, pPrice, categoryID, c.catName, pCode FROM products p INNER JOIN category c ON c.catID = p.categoryID WHERE pName LIKE '%" + txtBuscar.Text + "%' OR pCode LIKE '%" + txtBuscar.Text + "%'";
            ListBox lb = new ListBox();

            lb.Items.Add(dgvid);
            lb.Items.Add(dgvName);
            lb.Items.Add(dgvPrice);
            lb.Items.Add(dgvcID);
            lb.Items.Add(dgvCat);
            lb.Items.Add(dgvCode);


            MainClass.LoadData(qry, guna2DataGridView1, lb);

        }

        public override void btnAgregar_Click(object sender, EventArgs e)
        {
            frmProductsAdd frm = new frmProductsAdd();
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
                if (guna2MessageDialog1.Show("Estas seguro que desea editar el Producto?") == DialogResult.Yes)
                {
                    frmProductsAdd frm = new frmProductsAdd();
                    frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    frm.txtNombre.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvName"].Value);
                    frm.txtCodigo.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvCode"].Value);
                    frm.txtPrice.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvPrice"].Value);
                    frm.cbCat.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvCat"].Value);
                    
                    frm.Show();
                    GetData();
                }



            }
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvdelete")
            {
                //alerta para confirmar la eliminacion de la categoria
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                if (guna2MessageDialog1.Show("Estas seguro que desea eliminar el Producto?") == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    string qry = "Delete from products where pID= " + id + " ";
                    Hashtable ht = new Hashtable();
                    MainClass.SQl(qry, ht);

                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Show("Producto Eliminado Correctamente");
                    GetData();
                }


            }
            
        }

        
    }
}
