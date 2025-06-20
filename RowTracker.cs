using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace popus_pizzeria
{
    public class RowTracker
    {
        public int DetailID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public string Observacion { get; set; }
        public bool IsSent { get; set; }
        public bool IsNewlyAdded { get; set; }
        public bool IsModified { get; set; }

        public RowTracker() { }
        public RowTracker(DataGridViewRow row)
        {
            DetailID = Convert.ToInt32(row.Cells["dgvid"].Value);
            ProductID = Convert.ToInt32(row.Cells["dgvProID"].Value);
            ProductName = row.Cells["dgvPName"].Value?.ToString();
            Cantidad = Convert.ToInt32(row.Cells["dgvQty"].Value);
            Precio = Convert.ToDouble(row.Cells["dgvPrice"].Value);
            Observacion = row.Cells["dgvObs"].Value?.ToString() ?? "";
            IsSent = Convert.ToBoolean(row.Cells["dgvIsSent"].Value ?? false);
            IsNewlyAdded = Convert.ToBoolean(row.Cells["dgvIsNewlyAdded"].Value ?? false);
            IsModified = false; // Podés modificar este valor dinámicamente según necesidad
        }

        public bool DebeEnviarAKitchen()
        {
            return DetailID == 0 && !IsSent && IsNewlyAdded;
        }
    }

}
