using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO; // Necesario para Path.Combine
using System.Reflection; // Necesario para obtener la ruta base

namespace popus_pizzeria
{
    internal class MainClass
    {
        // ====================================================================
        // MODIFICACIONES CLAVE PARA LA RUTA DE INSTALACIÓN
        // ====================================================================

        // Método privado para construir la cadena de conexión con la ruta correcta.
        // Se asegura de que apunte a la carpeta del .exe una vez instalado.
        private static string GetDynamicConnectionString()
        {
            // La ruta base es la carpeta donde se ejecuta el archivo .exe (carpeta de instalación)
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Combina la ruta base con el nombre del archivo de la base de datos
            string dbPath = Path.Combine(baseDirectory, "popus_pizzeria.db");

            // Construye la cadena de conexión final
            return $"Data Source={dbPath};Version=3;";
        }

        // Ahora, la cadena de conexión se inicializa usando el método dinámico.
        public static readonly string con_string = GetDynamicConnectionString();

        // El objeto de conexión usa la cadena de conexión dinámica.
        public static SQLiteConnection con = new SQLiteConnection(con_string);

        // ====================================================================
        // FIN DE MODIFICACIONES CLAVE
        // ====================================================================


        // metodo para validacion de usuario
        public static bool IsValidUser(string user, string pass)
        {
            bool isValid = false;
            string qry = @"SELECT * FROM users WHERE username = @username AND upass = @upass";

            SQLiteCommand cmd = new SQLiteCommand(qry, con);

            cmd.Parameters.AddWithValue("@username", user);
            cmd.Parameters.AddWithValue("@upass", pass);

            DataTable dt = new DataTable();

            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);

            // Nota: SQLiteDataAdapter abre y cierra la conexión automáticamente.
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

        //Metodo para operaciones crud (INSERT, UPDATE, DELETE)
        public static int SQl(string qry, Hashtable ht)
        {
            int res = 0;
            // *RECOMENDACIÓN: Usar un bloque 'using' para la conexión, en lugar de un campo estático
            // Esto garantiza que la conexión se cierre y libere recursos, previniendo errores de concurrencia.
            // Para mantener el código similar, solo he añadido el chequeo de cierre al final.
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(qry, con);
                cmd.CommandType = CommandType.Text;

                foreach (DictionaryEntry item in ht)
                {
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
                SQLiteCommand cmd = new SQLiteCommand(qry, con);
                cmd.CommandType = CommandType.Text;

                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt); // El DataAdapter gestiona la apertura y cierre

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
                // No es necesario cerrar aquí si el DataAdapter cerró, pero por seguridad...
                // con.Close(); 
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
            SQLiteCommand cmd = new SQLiteCommand(qry, con);
            cmd.CommandType = CommandType.Text;

            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt); // El DataAdapter gestiona la apertura y cierre

            cb.DisplayMember = "name";
            cb.ValueMember = "id";
            cb.DataSource = dt;
            cb.SelectedIndex = -1;
        }
    }
}