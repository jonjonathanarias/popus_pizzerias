using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.Model
{
    public partial class frmProductsAdd : SampleAdd
    {
        public frmProductsAdd()
        {
            InitializeComponent();
        }

        public int id = 0;
        public int cID = 0;

        private void frmProductsAdd_Load(object sender, EventArgs e)
        {
            //para cb fill categoria
            string qry = "Select catID 'id' , catName 'name' from category";
            MainClass.CBFill(qry, cbCat);

            if (cID > 0)//para actualizar
            {
                cbCat.SelectedValue = cID;
            }

            if(id > 0)
            {
                ForUpdateLoadData();
            }
        }


        Byte[] imageByteArray;
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    string filePath = ofd.FileName;


                    txtImage.Image = new Bitmap(filePath);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public override void btnGuardar_Click(object sender, EventArgs e)
        {
            string qry = "";

            if (id == 0)//para insertar
            {
                qry = "Insert into products Values(@Name, @Price, @Cat, @Img)";
            }
            else//para actuaslizar
            {
                qry = "Update products Set pName = @Name, pPrice = @Price, categoryID = @Cat, pImage = @Img where pID = @id ";
            }

            //para la imagen

            Image temp = new Bitmap(txtImage.Image);
            MemoryStream ms = new MemoryStream();
            temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            imageByteArray = ms.ToArray();

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@Name", txtNombre.Text);
            ht.Add("@Price", txtPrice.Text);
            ht.Add("@Cat", Convert.ToInt32(cbCat.SelectedValue));
            ht.Add("@Img", imageByteArray);


            if (MainClass.SQl(qry, ht) > 0)
            {
                MessageBox.Show("Empleado Registrado");
                id = 0;
                cID = 0;
                txtNombre.Text = "";
                txtPrice.Text = "";
                cbCat.SelectedIndex = 0;
                cbCat.SelectedIndex = -1;
                txtImage.Image = popus_pizzeria.Properties.Resources.pizza_171859921;
                txtNombre.Focus();
            }
        }

        public override void btnCerrar_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void ForUpdateLoadData() 
        {
            string qry = @"Select * From products where pid= "+id+"";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                txtNombre.Text = dt.Rows[0]["pName"].ToString();
                txtPrice.Text = dt.Rows[0]["pPrice"].ToString();

                Byte[] imageArray = (byte[])(dt.Rows[0]["pImage"]);
                byte[] imageByteArray = imageArray;
                txtImage.Image = Image.FromStream(new  MemoryStream(imageByteArray));
            }
        }
    }
}
