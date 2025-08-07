using popus_pizzeria.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace popus_pizzeria.View
{
    public partial class frmProducto : Form
    {
        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private Button btnAgregar;
        private DataGridView dgvProductos;

        public frmProducto()
        {
            InitializeComponent();
            InitializeCustomComponents();
            CargarProductos();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Gestión de Productos";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblNombre = new Label { Text = "Nombre:", Left = 20, Top = 20, AutoSize = true };
            txtNombre = new TextBox { Left = 120, Top = 18, Width = 200 };

            Label lblDescripcion = new Label { Text = "Descripción:", Left = 20, Top = 50, AutoSize = true };
            txtDescripcion = new TextBox { Left = 120, Top = 48, Width = 400 };

            btnAgregar = new Button { Text = "Agregar Producto", Left = 120, Top = 80, Width = 150 };
            btnAgregar.Click += BtnAgregar_Click;

            dgvProductos = new DataGridView
            {
                Left = 20,
                Top = 130,
                Width = 650,
                Height = 250,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false
            };

            this.Controls.Add(lblNombre);
            this.Controls.Add(txtNombre);
            this.Controls.Add(lblDescripcion);
            this.Controls.Add(txtDescripcion);
            this.Controls.Add(btnAgregar);
            this.Controls.Add(dgvProductos);
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese un nombre para el producto.");
                return;
            }

            Producto nuevo = new Producto(0, txtNombre.Text, txtDescripcion.Text);
            DataManager.AgregarProducto(nuevo);

            txtNombre.Clear();
            txtDescripcion.Clear();

            MessageBox.Show("Producto agregado correctamente.");
            CargarProductos();
        }

        private void CargarProductos()
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = DataManager.GetProductos();
        }
    }
}
