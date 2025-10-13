using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.Model
{
    public partial class frmTableAdd : SampleAdd
    {
        public frmTableAdd()
        {
            InitializeComponent();
        }

        public int id = 0;

        public override void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombreNuevo = txtNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreNuevo))
            {
                MessageBox.Show("Por favor ingresá el nombre de la mesa.");
                return;
            }

            // Verificar duplicado
            string consulta = "SELECT COUNT(*) FROM tables WHERE tname = @Name AND tid <> @id";

            // CAMBIAR: SqlCommand -> SQLiteCommand. Usamos 'using' para asegurar la liberación.
            using (SQLiteCommand checkCmd = new SQLiteCommand(consulta, MainClass.con))
            {
                checkCmd.Parameters.AddWithValue("@Name", nombreNuevo);
                checkCmd.Parameters.AddWithValue("@id", id);

                if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();

                // NOTA: SQLite ExecuteScalar devuelve long. Convertimos a int.
                int cantidad = Convert.ToInt32(checkCmd.ExecuteScalar());
                MainClass.con.Close();

                if (cantidad > 0)
                {
                    MessageBox.Show("Ya existe una mesa con ese nombre.");
                    return;
                }
            }

            // Guardar
            // Se asume que MainClass.SQl ha sido refactorizado para usar SQLiteCommand
            string qry = (id == 0)
                ? "INSERT INTO tables (tname) VALUES (@Name)"
                : "UPDATE tables SET tname = @Name WHERE tid = @id";

            Hashtable ht = new Hashtable
            {
                { "@id", id },
                { "@Name", nombreNuevo }
            };

            if (MainClass.SQl(qry, ht) > 0)
            {
                MessageBox.Show("Mesa guardada correctamente.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /* public override void btnGuardar_Click(object sender, EventArgs e)
         {
             string nombreNuevo = txtNombre.Text.Trim();

             if (string.IsNullOrWhiteSpace(nombreNuevo))
             {
                 MessageBox.Show("Por favor ingresá el nombre de la mesa.");
                 return;
             }

             // Verificar duplicado
             string consulta = "SELECT COUNT(*) FROM tables WHERE tname = @Name AND tid <> @id";
             SqlCommand checkCmd = new SqlCommand(consulta, MainClass.con);
             checkCmd.Parameters.AddWithValue("@Name", nombreNuevo);
             checkCmd.Parameters.AddWithValue("@id", id);

             if (MainClass.con.State == ConnectionState.Closed) MainClass.con.Open();
             int cantidad = Convert.ToInt32(checkCmd.ExecuteScalar());
             MainClass.con.Close();

             if (cantidad > 0)
             {
                 MessageBox.Show("Ya existe una mesa con ese nombre.");
                 return;
             }

             // Guardar
             string qry = (id == 0)
                 ? "INSERT INTO tables (tname) VALUES (@Name)"
                 : "UPDATE tables SET tname = @Name WHERE tid = @id";

             Hashtable ht = new Hashtable
             {
                 { "@id", id },
                 { "@Name", nombreNuevo }
             };

             if (MainClass.SQl(qry, ht) > 0)
             {
                 MessageBox.Show("Mesa guardada correctamente.");
                 this.DialogResult = DialogResult.OK;
                 this.Close();
             }
         }*/



        public override void btnCerrar_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}
