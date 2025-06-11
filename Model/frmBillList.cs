using Guna.UI2.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.Model
{
    public partial class frmBillList : SampleAdd
    {
        public frmBillList()
        {
            InitializeComponent();
            btnGuardar.Visible = false;
            
        }

        public int MainID = 0;

        private void frmBillList_Load(object sender, EventArgs e)
        {
            LoadDate();
        }

        private void LoadDate()
        {
            string qry = @"Select MainID, TableName, WaiterName, orderType, status, total from tblMain
                                where status <> 'Pendiente' ";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvTable);
            lb.Items.Add(dgvWaiter);
            lb.Items.Add(dgvType);
            lb.Items.Add(dgvStatus);
            lb.Items.Add(dgvTotal);


            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // por serial
            int count = 0;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {

                count++;
                row.Cells[0].Value = count;
            }
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvedit")
            {

                MainID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                this.Close();



            }



        }

        
    }
}
