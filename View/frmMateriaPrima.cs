using popus_pizzeria.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            // Controles para agregar nueva Materia Prima
            Label lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 20, AutoSize = true };
            txtNombre = new TextBox { Left = 120, Top = 18, Width = 200 };

            Label lblUnidadMedida = new Label { Text = "Unidad Medida:", Left = 20, Top = 50, AutoSize = true };
            txtUnidadMedida = new TextBox { Left = 120, Top = 48, Width = 100 };

            Label lblCostoPorUnidad = new Label { Text = "Costo/Unidad:", Left = 20, Top = 80, AutoSize = true };
            txtCostoPorUnidad = new TextBox { Left = 120, Top = 78, Width = 100 };

            btnAgregar = new Button { Text = "Agregar Materia Prima", Left = 20, Top = 110, Width = 180 };
            btnAgregar.Click += BtnAgregar_Click;

            // DataGridView para mostrar Materias Primas
            dgvMateriasPrimas = new DataGridView
            {
                Left = 20,
                Top = 150,
                Width = 650,
                Height = 250,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false, // No permitir agregar filas directamente en el dgv
                ReadOnly = true // Hacer el DGV de solo lectura por defecto
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
            dgvMateriasPrimas.DataSource = null; // Limpiar DataSource
            dgvMateriasPrimas.DataSource = DataManager.MateriasPrimas;
            dgvMateriasPrimas.Columns["Id"].Visible = false; // Ocultar la columna Id si no quieres que se vea
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtUnidadMedida.Text) ||
                string.IsNullOrWhiteSpace(txtCostoPorUnidad.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtCostoPorUnidad.Text, out decimal costo))
            {
                MessageBox.Show("El costo por unidad debe ser un número válido.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Crear nueva Materia Prima
            int newId = DataManager.GetNextMateriaPrimaId();
            MateriaPrima nuevaMateriaPrima = new MateriaPrima(newId, txtNombre.Text, txtUnidadMedida.Text, costo);

            // Agregar a la lista en DataManager
            DataManager.MateriasPrimas.Add(nuevaMateriaPrima);

            // Actualizar el DataGridView
            CargarMateriasPrimas();

            // Limpiar los campos de entrada
            txtNombre.Clear();
            txtUnidadMedida.Clear();
            txtCostoPorUnidad.Clear();

            MessageBox.Show("Materia Prima agregada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
