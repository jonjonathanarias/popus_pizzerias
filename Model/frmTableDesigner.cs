using Microsoft.VisualBasic;
using Guna.UI2.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace popus_pizzeria.Model
{
    public partial class frmTableDesigner : Form
    {
        private Control selectedControl;
        private Point mouseOffset;

        public frmTableDesigner()
        {
            InitializeComponent();
        }

        

        private void CargarMesas()
        {
            panelPlano.Controls.Clear();
            string qry = "SELECT * FROM tables";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            DataTable dt = new DataTable();
            new SqlDataAdapter(cmd).Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                Guna2Button btn = new Guna2Button();
                btn.Text = row["tname"].ToString();
                btn.Width = 60;
                btn.Height = 60;
                btn.Location = new Point(Convert.ToInt32(row["xpos"]), Convert.ToInt32(row["ypos"]));
                btn.Tag = row["tid"];
                btn.FillColor = Color.Gray;
                btn.MouseDown += Btn_MouseDown;
                btn.MouseMove += Btn_MouseMove;
                btn.MouseUp += Btn_MouseUp;
                btn.MouseEnter += (s, ev) =>
                {
                    if (selectedControl != btn)
                    {
                        btn.BorderColor = Color.SkyBlue;
                        btn.BorderThickness = 2;
                    }
                };

                btn.MouseLeave += (s, ev) =>
                {
                    if (selectedControl != btn)
                    {
                        btn.BorderThickness = 0;
                    }
                };


                btn.Click += (s, ev) =>
                {
                    selectedControl = btn;
                    btn.BorderColor = Color.Red;
                    btn.BorderThickness = 2;

                    // Limpia bordes de otras mesas
                    foreach (Control ctrl in panelPlano.Controls)
                    {
                        if (ctrl is Guna2Button mesa && mesa != btn)
                        {
                            mesa.BorderThickness = 0;
                        }
                    }
                };

                btn.DoubleClick += (s, ev) =>
                {
                    frmTableAdd frm = new frmTableAdd();
                    frm.id = Convert.ToInt32(btn.Tag);   // Pasás el ID real de la mesa
                    frm.txtNombre.Text = btn.Text;       // Mostrás el nombre actual

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        CargarMesas();  // Actualizás el diseño visual
                    }
                };





                panelPlano.Controls.Add(btn);
            }
        }

        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            selectedControl = sender as Control;
            mouseOffset = e.Location;
        }

        private void Btn_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedControl != null && e.Button == MouseButtons.Left)
            {
                selectedControl.Left = e.X + selectedControl.Left - mouseOffset.X;
                selectedControl.Top = e.Y + selectedControl.Top - mouseOffset.Y;
            }
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            
        }





        private void btnAgregarMesa_Click_1(object sender, EventArgs e)
        {
            string qryExistentes = "SELECT tname FROM tables";
            SqlCommand cmd = new SqlCommand(qryExistentes, MainClass.con);
            DataTable dt = new DataTable();
            new SqlDataAdapter(cmd).Fill(dt);

            int n = 1;
            while (dt.Rows.Cast<DataRow>().Any(r => r["tname"].ToString() == $"Mesa {n}"))
                n++;

            string mesaNombre = $"Mesa {n}";
            string qryInsert = "INSERT INTO tables (tname, xpos, ypos, status) VALUES (@name, 10, 10, 'Libre')";
            Hashtable ht = new Hashtable();
            ht.Add("@name", mesaNombre);
            MainClass.SQl(qryInsert, ht);

            CargarMesas();
        }


        private void btnGuardarPosiciones_Click_1(object sender, EventArgs e)
        {
            // Guardar zonas especiales
            foreach (Control ctrl in panelPlano.Controls)
            {
                if (ctrl is Panel zona && zona.Tag != null)
                {
                    string tipoZona = zona.Tag.ToString();
                    int x = zona.Left;
                    int y = zona.Top;
                    int w = zona.Width;
                    int h = zona.Height;

                    string qryUpsert = @"
                IF EXISTS (SELECT 1 FROM zonas WHERE tipo = @tipo)
                    UPDATE zonas SET x = @x, y = @y, ancho = @w, alto = @h WHERE tipo = @tipo
                ELSE
                    INSERT INTO zonas (nombre, tipo, x, y, ancho, alto) VALUES (@nombre, @tipo, @x, @y, @w, @h)";

                    Hashtable ht = new Hashtable
                    {
                        { "@nombre", tipoZona },
                        { "@tipo", tipoZona },
                        { "@x", x },
                        { "@y", y },
                        { "@w", w },
                        { "@h", h }
                    };

                    MainClass.SQl(qryUpsert, ht);
                }
            }

            // Guardar posiciones de las mesas
            foreach (Control ctrl in panelPlano.Controls)
            {
                if (ctrl is Guna2Button btnMesa && btnMesa.Tag != null)
                {
                    int id = Convert.ToInt32(btnMesa.Tag);
                    int x = btnMesa.Left;
                    int y = btnMesa.Top;

                    string qryUpdate = "UPDATE tables SET xpos = @x, ypos = @y WHERE tid = @id";
                    Hashtable ht = new Hashtable
                    {
                        { "@x", x },
                        { "@y", y },
                        { "@id", id }
                    };

                    MainClass.SQl(qryUpdate, ht);
                    

                }
            }

            guna2MessageDialog1.Show("Posiciones guardadas correctamente");
        }


        private void btnEliminarMesa_Click(object sender, EventArgs e)
        {
            if (selectedControl is Guna2Button btn)
            {
                int mesaId = Convert.ToInt32(btn.Tag);
                string qry = "DELETE FROM tables WHERE tid = @id";
                Hashtable ht = new Hashtable { { "@id", mesaId } };
                MainClass.SQl(qry, ht);

                panelPlano.Controls.Remove(btn);
                selectedControl = null;
            }
            else
            {
                MessageBox.Show("Seleccioná una mesa para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CargarZonasEspeciales()
        {
            Panel mostrador = new Panel
            {
                BackColor = Color.DarkRed,
                Width = 200,
                Height = 80,
                Location = new Point(10, 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            Label lblMostrador = new Label { Text = "MOSTRADOR", ForeColor = Color.White, BackColor = Color.Transparent };
            lblMostrador.Location = new Point(10, 10);
            mostrador.Controls.Add(lblMostrador);
            HacerZonaInteractiva(mostrador);
            panelPlano.Controls.Add(mostrador);

            // Patio Interno
            Panel patioInterno = new Panel
            {
                BackColor = Color.LightGreen,
                Width = 400,
                Height = 300,
                Location = new Point(250, 10),
                BorderStyle = BorderStyle.Fixed3D
            };
            patioInterno.Tag = "PatioInterno";
            HacerZonaInteractiva(patioInterno);
            panelPlano.Controls.Add(patioInterno);

            // Patio Externo
            Panel patioExterno = new Panel
            {
                BackColor = Color.LightBlue,
                Width = 400,
                Height = 300,
                Location = new Point(250, 320),
                BorderStyle = BorderStyle.Fixed3D
            };
            patioExterno.Tag = "PatioExterno";
            
            string qry = "SELECT * FROM zonas";
            SqlDataAdapter da = new SqlDataAdapter(qry, MainClass.con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                var panelZona = new Panel
                {
                    Name = "zona_" + row["tipo"].ToString(),
                    Tag = row["tipo"].ToString(),
                    BackColor = row["tipo"].ToString().Contains("Mostrador") ? Color.DarkRed :
                                row["tipo"].ToString().Contains("Interno") ? Color.LightGreen : Color.LightBlue,
                                
                    Width = Convert.ToInt32(row["ancho"]),
                    Height = Convert.ToInt32(row["alto"]),
                    Location = new Point(Convert.ToInt32(row["x"]), Convert.ToInt32(row["y"])),
                    BorderStyle = BorderStyle.FixedSingle
                };

                panelZona.Controls.Add(new Label
                {
                    Text = row["nombre"].ToString(),
                    ForeColor = Color.Black,
                    BackColor = Color.Transparent,
                    Location = new Point(5, 5)
                });

                HacerZonaInteractiva(panelZona);
                panelPlano.Controls.Add(panelZona);
            }

        }

        private void frmTableDesigner_Load_1(object sender, EventArgs e)
        {
            CargarMesas();
            
            panelPlano.MouseDown += (s, f) =>
            {
                if (f.Button == MouseButtons.Left)
                {
                    // Solo si se hizo clic directo en el panel, no sobre un control hijo (como una mesa)
                    Control clicked = panelPlano.GetChildAtPoint(f.Location);
                    if (clicked == null)
                    {
                        foreach (Control ctrl in panelPlano.Controls)
                        {
                            if (ctrl is Guna2Button mesa)
                                mesa.BorderThickness = 0;
                        }

                        selectedControl = null;
                    }
                }
            };
        }

        private void HacerZonaInteractiva(Control zona)
        {
            Point mouseOffset = Point.Empty;
            bool dragging = false;
            bool resizing = false;
            const int grip = 10;

            zona.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.X >= zona.Width - grip && e.Y >= zona.Height - grip)
                        resizing = true;
                    else
                        dragging = true;

                    mouseOffset = e.Location;
                    zona.BringToFront();
                }
            };

            zona.MouseMove += (s, e) =>
            {
                if (dragging)
                {
                    zona.Left += e.X - mouseOffset.X;
                    zona.Top += e.Y - mouseOffset.Y;
                }
                else if (resizing)
                {
                    zona.Width = Math.Max(50, zona.Width + (e.X - mouseOffset.X));
                    zona.Height = Math.Max(50, zona.Height + (e.Y - mouseOffset.Y));
                }
            };

            zona.MouseUp += (s, e) =>
            {
                dragging = false;
                resizing = false;
            };

            zona.Cursor = Cursors.SizeAll;
        }

    }

}
