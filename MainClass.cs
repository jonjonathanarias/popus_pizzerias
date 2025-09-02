using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Microsoft.Data.SqlClient;


namespace popus_pizzeria
{
    internal class MainClass
    {

        // public static readonly string con_string = "Data Source=DESKTOP-FFIHDBP\\SQLEXPRESS; Initial Catalog=popus_pizzeria; Persist Security Info=True; Trusted_Connection=True;";

        public static readonly string con_string = ConfigurationManager.ConnectionStrings["con_string_bd"].ConnectionString;

        public static SqlConnection con = new SqlConnection(con_string);

        // metodo para validacion de usuario

        public static bool IsValidUser(string user, string pass)
        {
            bool isValid = false;

            
            string qry = @"SELECT * FROM users WHERE username = @username AND upass = @upass";
            SqlCommand cmd = new SqlCommand(qry, con);

            
            cmd.Parameters.AddWithValue("@username", user);
            cmd.Parameters.AddWithValue("@upass", pass);

            DataTable dt = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            dataAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                isValid = true;
                USER = dt.Rows[0]["uName"].ToString();
            }

            return isValid;
        }

        //Propiedades del usuario

        public static string user;

        public static string USER
        {
            get { return user; }
            private set { user = value; }

        }

        //Metodo para operaciones crud

        public static int SQl(string qry, Hashtable ht)
        {
            int res = 0;

            try
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;

                foreach (DictionaryEntry item in  ht) {

                    cmd.Parameters.AddWithValue(item.Key.ToString(), item.Value);
                }
                if(con.State == ConnectionState.Closed) { 
                    con.Open();
                }
                res = cmd.ExecuteNonQuery();
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
                con.Close();
            }
            return res;
        }

        //Para la carga de datos en la base de datos

        public static void LoadData(string qry, DataGridView gv, ListBox lb)
        {

            // sin serial el la gridview
            gv.CellFormatting += new DataGridViewCellFormattingEventHandler(gv_CellFormatting);
            //

            try
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < lb.Items.Count; i++)
                {
                    string name = ((DataGridViewColumn)lb.Items[i]).Name;
                    string colNam1 = name;
                    gv.Columns[colNam1].DataPropertyName = dt.Columns[i].ToString();
                }

                gv.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                con.Close();
            }
        }

        private static void gv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Guna.UI2.WinForms.Guna2DataGridView gv = (Guna.UI2.WinForms.Guna2DataGridView)sender;
            int count = 0;

            foreach (DataGridViewRow row in gv.Rows) {

                count++;
                row.Cells[0].Value = count;
            }
        }

        //para cb fill categoria

        public static void CBFill(string qry, ComboBox cb) 
        {
            SqlCommand cmd = new SqlCommand(qry, con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            cb.DisplayMember = "name";
            cb.ValueMember = "id";
            cb.DataSource = dt;
            cb.SelectedIndex = -1;
        }

    }


}
