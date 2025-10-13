using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
// using System.Data.SqlClient; // COMENTAR: Ya no se usa para la conexión
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Microsoft.Data.Sqlite; // Si usaste el paquete Microsoft.Data.Sqlite, usa este
using System.Data.SQLite; // USAR: Este es el proveedor que usaremos (System.Data.SQLite)

namespace popus_pizzeria
{
    internal class MainClass
    {
        // Se mantiene la cadena de conexión del App.config
        public static readonly string con_string = ConfigurationManager.ConnectionStrings["con_string_bd"].ConnectionString;

        // CAMBIAR: Se reemplaza SqlConnection por SQLiteConnection
        public static SQLiteConnection con = new SQLiteConnection(con_string);

        // metodo para validacion de usuario
        public static bool IsValidUser(string user, string pass)
        {
            bool isValid = false;

            // La sintaxis de la consulta es compatible
            string qry = @"SELECT * FROM users WHERE username = @username AND upass = @upass";

            // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
            SQLiteCommand cmd = new SQLiteCommand(qry, con);

            // Los parámetros son compatibles, solo cambia el objeto
            cmd.Parameters.AddWithValue("@username", user);
            cmd.Parameters.AddWithValue("@upass", pass);

            DataTable dt = new DataTable();

            // CAMBIAR: Se reemplaza SqlDataAdapter por SQLiteDataAdapter
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);

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
                // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
                SQLiteCommand cmd = new SQLiteCommand(qry, con);
                cmd.CommandType = CommandType.Text;

                foreach (DictionaryEntry item in ht)
                {
                    // Los parámetros son compatibles
                    cmd.Parameters.AddWithValue(item.Key.ToString(), item.Value);
                }

                if (con.State == ConnectionState.Closed)
                {
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
                // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
                SQLiteCommand cmd = new SQLiteCommand(qry, con);
                cmd.CommandType = CommandType.Text;

                // CAMBIAR: Se reemplaza SqlDataAdapter por SQLiteDataAdapter
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

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

            foreach (DataGridViewRow row in gv.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        //para cb fill categoria
        public static void CBFill(string qry, ComboBox cb)
        {
            // CAMBIAR: Se reemplaza SqlCommand por SQLiteCommand
            SQLiteCommand cmd = new SQLiteCommand(qry, con);
            cmd.CommandType = CommandType.Text;

            // CAMBIAR: Se reemplaza SqlDataAdapter por SQLiteDataAdapter
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cb.DisplayMember = "name";
            cb.ValueMember = "id";
            cb.DataSource = dt;
            cb.SelectedIndex = -1;
        }
    }
}