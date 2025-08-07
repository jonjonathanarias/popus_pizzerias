using popus_pizzeria.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            Label lblProducto = new Label { Text = "Seleccione Producto:", Left = 20, Top = 20, AutoSize = true };
            cmbProductos = new ComboBox { Left = 160, Top = 18, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbProductos.SelectedIndexChanged += CmbProductos_SelectedIndexChanged;

            Label lblTituloCosto = new Label { Text = "Costo Total del Producto:", Left = 20, Top = 60, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            lblCostoCalculado = new Label { Text = "$ 0.00", Left = 200, Top = 60, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };

            Label lblDetalle = new Label { Text = "Detalle de Costos por Ingrediente:", Left = 20, Top = 100, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            dgvDetalleCosto = new DataGridView
            {
                Left = 20,
                Top = 130,
                Width = 650,
                Height = 300,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
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
            cmbProductos.DataSource = DataManager.GetProductos();
            cmbProductos.DisplayMember = "Nombre";
            cmbProductos.ValueMember = "Id";
        }

        private void CmbProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedItem is Producto prod)
            {
                decimal total = DataManager.CalcularCostoProducto(prod.Id);
                lblCostoCalculado.Text = $"$ {total:N2}";

                var detalles = DataManager.GetRecetasPorProducto(prod.Id)
                    .Select(r =>
                    {
                        var mp = DataManager.GetMateriasPrimas().FirstOrDefault(m => m.Id == r.MateriaPrimaId);
                        decimal costo = r.CantidadUsada * (mp?.CostoPorUnidad ?? 0);
                        return new
                        {
                            Ingrediente = mp?.Nombre,
                            Cantidad = $"{r.CantidadUsada:N3} {mp?.UnidadMedida}",
                            CostoUnitario = $"{mp?.CostoPorUnidad:N2}",
                            CostoPorIngrediente = $"{costo:N2}"
                        };
                    }).ToList();

                dgvDetalleCosto.DataSource = detalles;
            }
        }
    }
}
