using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria.Model
{
    public partial class frmCostosExcel : Form
    {
        public frmCostosExcel()
        {
            InitializeComponent();
            InicializarComponentesManual();
        }

        private DataGridView dataGridView1;
        private Button btnCargarExcel;
        private Button btnGuardar;
        private Button btnCalcular;


        private void InicializarComponentesManual()
        {
            this.Text = "Importar Costos desde Excel";
            this.Size = new Size(800, 600);

            dataGridView1 = new DataGridView { Location = new Point(20, 60), Size = new Size(740, 400) };
            btnCargarExcel = new Button { Text = "Cargar Excel", Location = new Point(20, 20) };
            btnGuardar = new Button { Text = "Guardar en BD", Location = new Point(150, 20) };
            btnCalcular = new Button { Text = "Calcular Total", Location = new Point(280, 20) };

            btnCargarExcel.Click += BtnCargarExcel_Click;
            btnGuardar.Click += BtnGuardar_Click;
            btnCalcular.Click += BtnCalcular_Click;

            Controls.Add(dataGridView1);
            Controls.Add(btnCargarExcel);
            Controls.Add(btnGuardar);
            Controls.Add(btnCalcular);

        }

        private void BtnCargarExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos Excel|*.xlsx";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CargarExcelEnGrid(ofd.FileName);
            }
        }

        private void CargarExcelEnGrid(string rutaArchivo)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(rutaArchivo)))
            {
                ExcelWorksheet hoja = package.Workbook.Worksheets[0];
                DataTable dt = new DataTable();

                if (hoja.Dimension == null)
                {
                    MessageBox.Show("La hoja está vacía.");
                    return;
                }

                bool tieneCabeceras = true;
                int filaInicio = tieneCabeceras ? 2 : 1;

                // Definir columnas
                for (int col = 1; col <= hoja.Dimension.End.Column; col++)
                {
                    string nombreCol = tieneCabeceras ? hoja.Cells[1, col].Text : $"Columna {col}";
                    if (string.IsNullOrWhiteSpace(nombreCol)) nombreCol = $"Columna {col}";
                    dt.Columns.Add(nombreCol);
                }

                // Llenar filas
                for (int fila = filaInicio; fila <= hoja.Dimension.End.Row; fila++)
                {
                    DataRow row = dt.NewRow();
                    for (int col = 1; col <= hoja.Dimension.End.Column; col++)
                    {
                        row[col - 1] = hoja.Cells[fila, col].Text;
                    }
                    dt.Rows.Add(row);
                }

                dataGridView1.DataSource = dt;
            }
        }



        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            GuardarDatosEnBaseDeDatos();
        }

        private void GuardarDatosEnBaseDeDatos()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow && row.Cells.Count >= 2 && row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    string producto = row.Cells[0].Value.ToString();
                    string costoTexto = row.Cells[1].Value.ToString();

                    if (!double.TryParse(costoTexto, out double costo))
                    {
                        MessageBox.Show($"El valor de costo '{costoTexto}' no es válido para el producto '{producto}'.");
                        continue;
                    }

                    string query = "INSERT INTO costos (producto, costo) VALUES (@producto, @costo)";
                    using (SqlConnection con = new SqlConnection("Server=DESKTOP-FFIHDBP\\SQLEXPRESS;Database=popus_pizzeria;Trusted_Connection=True; TrustServerCertificate=True;" ))
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@producto", producto);
                        cmd.Parameters.AddWithValue("@costo", costo); // ahora es double
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }

            MessageBox.Show("Datos guardados correctamente.");
        }


        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            double total = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow && double.TryParse(row.Cells[1].Value.ToString(), out double valor))
                {
                    total += valor;
                }
            }

            MessageBox.Show("Total: $" + total);
        }
    }
}
