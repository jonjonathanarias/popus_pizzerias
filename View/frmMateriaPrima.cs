using popus_pizzeria.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace popus_pizzeria.View
{
    public partial class frmMateriaPrima : Form
    {
        private DataGridView dgvMateriasPrimas;
        private TextBox txtNombre;
        private TextBox txtUnidadMedida;
        private TextBox txtCostoPorUnidad;
        private Button btnAgregar;

        public frmMateriaPrima()
        {
            InitializeComponent();
            InitializeCustomComponents();
            CargarMateriasPrimas();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Gestión de Materia Prima";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 20, AutoSize = true };
            txtNombre = new TextBox { Left = 120, Top = 18, Width = 200 };

            Label lblUnidadMedida = new Label { Text = "Unidad Medida:", Left = 20, Top = 50, AutoSize = true };
            txtUnidadMedida = new TextBox { Left = 120, Top = 48, Width = 100 };

            Label lblCostoPorUnidad = new Label { Text = "Costo/Unidad:", Left = 20, Top = 80, AutoSize = true };
            txtCostoPorUnidad = new TextBox { Left = 120, Top = 78, Width = 100 };

            btnAgregar = new Button { Text = "Agregar Materia Prima", Left = 20, Top = 110, Width = 180 };
            btnAgregar.Click += BtnAgregar_Click;

            dgvMateriasPrimas = new DataGridView
            {
                Left = 20,
                Top = 150,
                Width = 650,
                Height = 250,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            this.Controls.Add(lblNombre);
            this.Controls.Add(txtNombre);
            this.Controls.Add(lblUnidadMedida);
            this.Controls.Add(txtUnidadMedida);
            this.Controls.Add(lblCostoPorUnidad);
            this.Controls.Add(txtCostoPorUnidad);
            this.Controls.Add(btnAgregar);
            this.Controls.Add(dgvMateriasPrimas);
        }

        private void CargarMateriasPrimas()
        {
            dgvMateriasPrimas.DataSource = null;
            dgvMateriasPrimas.DataSource = DataManager.GetMateriasPrimas();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtUnidadMedida.Text) || string.IsNullOrWhiteSpace(txtCostoPorUnidad.Text))
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            if (!decimal.TryParse(txtCostoPorUnidad.Text, out decimal costo))
            {
                MessageBox.Show("Costo inválido.");
                return;
            }

            MateriaPrima nueva = new MateriaPrima(0, txtNombre.Text, txtUnidadMedida.Text, costo);
            DataManager.AgregarMateriaPrima(nueva);

            CargarMateriasPrimas();

            txtNombre.Clear();
            txtUnidadMedida.Clear();
            txtCostoPorUnidad.Clear();

            MessageBox.Show("Materia prima guardada.");
        }
    }
}
