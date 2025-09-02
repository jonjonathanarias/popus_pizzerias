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
            string qry = @"
        SELECT 
            m.MainID, 
            m.TableName, 
            m.WaiterName, 
            m.orderType, 
            m.status, 
            m.total,
            CASE 
                WHEN m.orderType IN ('Delivery', 'Take Away') THEN c.Name 
                ELSE '' 
            END AS CustomerName,
            CASE 
                WHEN m.orderType IN ('Delivery', 'Take Away') THEN c.Phone 
                ELSE '' 
            END AS CustomerPhone
        FROM tblMain m
        LEFT JOIN tblCustomers c ON m.CustomerID = c.CustomerID
        WHERE m.status = 'Completo'";

            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);             // MainID
            lb.Items.Add(dgvTable);          // TableName
            lb.Items.Add(dgvWaiter);         // WaiterName
            lb.Items.Add(dgvType);           // orderType
            lb.Items.Add(dgvStatus);         // status
            lb.Items.Add(dgvTotal);          // total
            lb.Items.Add(dgvCustomerName);   // CustomerName
            lb.Items.Add(dgvCustomerPhone);  // CustomerPhone

            MainClass.LoadData(qry, guna2DataGridView1, lb);

            if (guna2DataGridView1.Columns.Contains("dgvedit"))
            {
                guna2DataGridView1.Columns["dgvedit"].DisplayIndex = guna2DataGridView1.Columns.Count - 1;
            }
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
