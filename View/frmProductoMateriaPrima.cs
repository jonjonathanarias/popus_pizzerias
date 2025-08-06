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
    public partial class frmProductoMateriaPrima: Form
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

            // Controles para seleccionar Producto
            Label lblProducto = new Label { Text = "Producto:", Left = 20, Top = 20, AutoSize = true };
            cmbProductos = new ComboBox { Left = 120, Top = 18, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbProductos.SelectedIndexChanged += CmbProductos_SelectedIndexChanged;

            // Controles para seleccionar Materia Prima y Cantidad
            Label lblMateriaPrima = new Label { Text = "Materia Prima:", Left = 20, Top = 60, AutoSize = true };
            cmbMateriasPrimas = new ComboBox { Left = 120, Top = 58, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblCantidad = new Label { Text = "Cantidad Usada:", Left = 20, Top = 100, AutoSize = true };
            txtCantidadUsada = new TextBox { Left = 120, Top = 98, Width = 100 };

            btnAgregarIngrediente = new Button { Text = "Agregar Ingrediente a Receta", Left = 20, Top = 140, Width = 200 };
            btnAgregarIngrediente.Click += BtnAgregarIngrediente_Click;

            // DataGridView para mostrar la receta del producto seleccionado
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
        }

        private void CargarDatosIniciales()
        {
            // Cargar Productos en el ComboBox
            cmbProductos.DataSource = DataManager.Productos;
            cmbProductos.DisplayMember = "Nombre"; // Mostrar el nombre del producto
            cmbProductos.ValueMember = "Id";     // Usar el Id como valor

            // Cargar Materias Primas en el ComboBox
            cmbMateriasPrimas.DataSource = DataManager.MateriasPrimas;
            cmbMateriasPrimas.DisplayMember = "Nombre"; // Mostrar el nombre de la materia prima
            cmbMateriasPrimas.ValueMember = "Id";     // Usar el Id como valor
        }

        private void CmbProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarRecetaProductoSeleccionado();
        }

        private void CargarRecetaProductoSeleccionado()
        {
            // Verificación agregada para evitar el error cuando no hay un producto seleccionado
            if (cmbProductos.SelectedValue != null)
            {
                int productoId = (int)cmbProductos.SelectedValue;

                // Obtener las entradas de receta para el producto seleccionado
                var recetaActual = DataManager.Recetas
                                    .Where(r => r.ProductoId == productoId)
                                    .Join(DataManager.MateriasPrimas, // Unir con MateriasPrimas para obtener nombres
                                          receta => receta.MateriaPrimaId,
                                          mp => mp.Id,
                                          (receta, mp) => new
                                          {
                                              MateriaPrima = mp.Nombre,
                                              Cantidad = receta.CantidadUsada,
                                              Unidad = mp.UnidadMedida,
                                              MateriaPrimaId = mp.Id // Mantener el ID para posibles eliminaciones
                                          })
                                    .ToList();

                dgvReceta.DataSource = null;
                dgvReceta.DataSource = recetaActual;
                dgvReceta.Columns["MateriaPrimaId"].Visible = false; // Ocultar esta columna
            }
            else
            {
                dgvReceta.DataSource = null; // Limpiar si no hay producto seleccionado
            }
        }

        private void BtnAgregarIngrediente_Click(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedValue == null)
            {
                MessageBox.Show("Por favor, seleccione un producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbMateriasPrimas.SelectedValue == null)
            {
                MessageBox.Show("Por favor, seleccione una materia prima.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(txtCantidadUsada.Text, out decimal cantidad))
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor que cero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productoId = (int)cmbProductos.SelectedValue;
            int materiaPrimaId = (int)cmbMateriasPrimas.SelectedValue;

            // Verificar si el ingrediente ya existe en la receta del producto
            var existingEntry = DataManager.Recetas.FirstOrDefault(r => r.ProductoId == productoId && r.MateriaPrimaId == materiaPrimaId);

            if (existingEntry != null)
            {
                existingEntry.CantidadUsada = cantidad;
                MessageBox.Show("Cantidad de ingrediente actualizada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ProductoMateriaPrima nuevaEntrada = new ProductoMateriaPrima(productoId, materiaPrimaId, cantidad);
                DataManager.Recetas.Add(nuevaEntrada);
                MessageBox.Show("Ingrediente agregado a la receta.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            CargarRecetaProductoSeleccionado();

            txtCantidadUsada.Clear();
        }
    }
}
