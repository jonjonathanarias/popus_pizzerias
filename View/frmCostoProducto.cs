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
    public partial class frmCostoProducto : Form
    {

        private ComboBox cmbProductos;
        private Label lblCostoCalculado;
        private DataGridView dgvDetalleCosto;
        public frmCostoProducto()
        {
            InitializeComponent();
            InitializeCustomComponents();
            CargarProductos();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Calcular Costo de Producto";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Controles para seleccionar Producto
            Label lblProducto = new Label { Text = "Seleccione Producto:", Left = 20, Top = 20, AutoSize = true };
            cmbProductos = new ComboBox { Left = 160, Top = 18, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbProductos.SelectedIndexChanged += CmbProductos_SelectedIndexChanged;

            // Label para mostrar el costo total
            Label lblTituloCosto = new Label { Text = "Costo Total del Producto:", Left = 20, Top = 60, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblCostoCalculado = new Label { Text = "$ 0.00", Left = 200, Top = 60, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };

            // DataGridView para mostrar el detalle del costo (ingrediente por ingrediente)
            Label lblDetalle = new Label { Text = "Detalle de Costos por Ingrediente:", Left = 20, Top = 100, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            dgvDetalleCosto = new DataGridView
            {
                Left = 20,
                Top = 130,
                Width = 650,
                Height = 300,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            this.Controls.Add(lblProducto);
            this.Controls.Add(cmbProductos);
            this.Controls.Add(lblTituloCosto);
            this.Controls.Add(lblCostoCalculado);
            this.Controls.Add(lblDetalle);
            this.Controls.Add(dgvDetalleCosto);
        }

        private void CargarProductos()
        {
            cmbProductos.DataSource = DataManager.Productos;
            cmbProductos.DisplayMember = "Nombre";
            cmbProductos.ValueMember = "Id";
        }

        private void CmbProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedValue != null)
            {
                int productoId = (int)cmbProductos.SelectedValue;

                // Calcular el costo total
                decimal costoTotal = CostCalculator.CalcularCostoProducto(productoId);
                lblCostoCalculado.Text = $"$ {costoTotal:N2}"; // Formatear a 2 decimales

                // Mostrar el detalle por ingrediente en el DataGridView
                CargarDetalleCosto(productoId);
            }
            else
            {
                lblCostoCalculado.Text = "$ 0.00";
                dgvDetalleCosto.DataSource = null;
            }
        }

        private void CargarDetalleCosto(int productoId)
        {
            // Crear una lista anónima para mostrar en el DataGridView
            var detalle = new List<object>();

            // Obtener todas las entradas de receta para este producto
            var ingredientesReceta = DataManager.Recetas.Where(r => r.ProductoId == productoId).ToList();

            foreach (var recetaIngrediente in ingredientesReceta)
            {
                MateriaPrima mp = DataManager.MateriasPrimas.FirstOrDefault(m => m.Id == recetaIngrediente.MateriaPrimaId);

                if (mp != null)
                {
                    decimal costoIndividual = recetaIngrediente.CantidadUsada * mp.CostoPorUnidad;
                    detalle.Add(new
                    {
                        Ingrediente = mp.Nombre,
                        Cantidad = $"{recetaIngrediente.CantidadUsada:N3} {mp.UnidadMedida}", // Mostrar cantidad con unidad
                        CostoUnitario = $"{mp.CostoPorUnidad:N2}",
                        CostoPorIngrediente = $"{costoIndividual:N2}"
                    });
                }
            }

            dgvDetalleCosto.DataSource = null;
            dgvDetalleCosto.DataSource = detalle;
        }
    }
}
