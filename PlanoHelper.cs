﻿using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria
{
    public static class PlanoHelper
    {
        public static void CargarPlanoVisual(
            Panel contenedor,
            Action<Guna2Button> alSeleccionarMesa = null,
            Action<Guna2Button> alDobleClickMesa = null)
        {
            contenedor.Controls.Clear();

            // Cargar zonas
            string qryZonas = "SELECT * FROM zonas";
            SqlDataAdapter daZonas = new SqlDataAdapter(qryZonas, MainClass.con);
            DataTable dtZonas = new DataTable();
            daZonas.Fill(dtZonas);

            foreach (DataRow row in dtZonas.Rows)
            {
                var zona = new Panel
                {
                    Location = new Point(Convert.ToInt32(row["x"]), Convert.ToInt32(row["y"])),
                    Width = Convert.ToInt32(row["ancho"]),
                    Height = Convert.ToInt32(row["alto"]),
                    BackColor = row["tipo"].ToString().Contains("Mostrador") ? Color.DarkRed :
                                row["tipo"].ToString().Contains("Interno") ? Color.LightGreen : Color.LightBlue,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = row["tipo"].ToString()
                };

                zona.Controls.Add(new Label
                {
                    Text = row["nombre"].ToString(),
                    Location = new Point(5, 5),
                    BackColor = Color.Transparent
                });

                contenedor.Controls.Add(zona);
            }

            // Cargar mesas
            string qryMesas = "SELECT * FROM tables";
            SqlDataAdapter daMesas = new SqlDataAdapter(qryMesas, MainClass.con);
            DataTable dtMesas = new DataTable();
            daMesas.Fill(dtMesas);

            foreach (DataRow row in dtMesas.Rows)
            {
                var btn = new Guna2Button
                {
                    Text = row["tname"].ToString(),
                    Width = 60,
                    Height = 60,
                    Location = new Point(Convert.ToInt32(row["xpos"]), Convert.ToInt32(row["ypos"])),
                    Tag = row["tid"]
                };

                string estado = row["status"].ToString();
                btn.FillColor = estado == "Libre" ? Color.Green :
                                estado == "Ocupada" ? Color.Red :
                                estado == "Pagada" ? Color.Blue :
                                Color.Gray;

                if (alSeleccionarMesa != null)
                    btn.Click += (s, e) => alSeleccionarMesa(btn);

                if (alDobleClickMesa != null)
                    btn.DoubleClick += (s, e) => alDobleClickMesa(btn);

                contenedor.Controls.Add(btn);
            }

        }
    }

}
