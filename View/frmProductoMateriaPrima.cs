using popus_pizzeria.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace popus_pizzeria.View
{
    public partial class frmProductoMateriaPrima : Form
    {
        private ComboBox cmbProductos;
        private ComboBox cmbMateriasPrimas;
        private TextBox txtCantidadUsada;
        private Button btnAgregarIngrediente;
        private DataGridView dgvReceta;

        public frmProductoMateriaPrima()
        {
            InitializeComponent();
            InitializeCustomComponents();
            CargarDatosIniciales();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Asignar Insumos a Producto";
            this.Size = new Size(800, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblProducto = new Label { Text = "Producto:", Left = 20, Top = 20, AutoSize = true };
            cmbProductos = new ComboBox { Left = 120, Top = 18, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbProductos.SelectedIndexChanged += (s, e) => CargarRecetaProductoSeleccionado();

            Label lblMateriaPrima = new Label { Text = "Materia Prima:", Left = 20, Top = 60, AutoSize = true };
            cmbMateriasPrimas = new ComboBox { Left = 120, Top = 58, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblCantidad = new Label { Text = "Cantidad Usada:", Left = 20, Top = 100, AutoSize = true };
            txtCantidadUsada = new TextBox { Left = 120, Top = 98, Width = 100 };

            btnAgregarIngrediente = new Button { Text = "Agregar Ingrediente a Receta", Left = 20, Top = 140, Width = 200 };
            btnAgregarIngrediente.Click += BtnAgregarIngrediente_Click;

            dgvReceta = new DataGridView
            {
                Left = 20,
                Top = 180,
                Width = 750,
                Height = 300,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            this.Controls.Add(lblProducto);
            this.Controls.Add(cmbProductos);
            this.Controls.Add(lblMateriaPrima);
            this.Controls.Add(cmbMateriasPrimas);
            this.Controls.Add(lblCantidad);
            this.Controls.Add(txtCantidadUsada);
            this.Controls.Add(btnAgregarIngrediente);
            this.Controls.Add(dgvReceta);

            // Agregar columna de botón "Eliminar"
            var btnEliminar = new DataGridViewButtonColumn
            {
                Name = "Eliminar",
                HeaderText = "Acción",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true
            };
            dgvReceta.Columns.Add(btnEliminar);
            dgvReceta.CellClick += DgvReceta_CellClick;

        }

        private void CargarDatosIniciales()
        {
            cmbProductos.DataSource = DataManager.GetProductos();
            cmbProductos.DisplayMember = "Nombre";
            cmbProductos.ValueMember = "Id";

            cmbMateriasPrimas.DataSource = DataManager.GetMateriasPrimas();
            cmbMateriasPrimas.DisplayMember = "Nombre";
            cmbMateriasPrimas.ValueMember = "Id";
        }

        private void CargarRecetaProductoSeleccionado()
        {
            if (cmbProductos.SelectedItem is Producto prod)
            {
                var receta = DataManager.GetRecetasPorProducto(prod.Id)
                    .Select(r =>
                    {
                        var mp = DataManager.GetMateriasPrimas().FirstOrDefault(m => m.Id == r.MateriaPrimaId);
                        return new
                        {
                            Ingrediente = mp?.Nombre,
                            Unidad = mp?.UnidadMedida,
                            Cantidad = r.CantidadUsada
                        };
                    }).ToList();

                dgvReceta.DataSource = receta;
            }
        }

        private void BtnAgregarIngrediente_Click(object sender, EventArgs e)
        {
            if (!(cmbProductos.SelectedItem is Producto producto) || !(cmbMateriasPrimas.SelectedItem is MateriaPrima mp))
            {
                MessageBox.Show("Seleccione producto y materia prima.");
                return;
            }

            if (!decimal.TryParse(txtCantidadUsada.Text, out decimal cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }

            var receta = new ProductoMateriaPrima(producto.Id, mp.Id, cantidad);
            DataManager.AgregarReceta(receta);

            MessageBox.Show("Ingrediente asignado.");
            txtCantidadUsada.Clear();
            CargarRecetaProductoSeleccionado();
        }

        private void DgvReceta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvReceta.Columns[e.ColumnIndex].Name == "Eliminar")
            {
                if (!(cmbProductos.SelectedItem is Producto producto)) return;

                // Buscar el nombre del ingrediente que se quiere eliminar
                string nombreIngrediente = dgvReceta.Rows[e.RowIndex].Cells["Ingrediente"].Value?.ToString();
                var materia = DataManager.GetMateriasPrimas().FirstOrDefault(m => m.Nombre == nombreIngrediente);

                if (materia != null)
                {
                    var confirm = MessageBox.Show($"¿Seguro que querés eliminar '{materia.Nombre}' de la receta?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        DataManager.EliminarIngredienteDeReceta(producto.Id, materia.Id);
                        CargarRecetaProductoSeleccionado();
                    }
                }
            }
        }

    }
}
