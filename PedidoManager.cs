using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace popus_pizzeria
{
    public class PedidoManager
    {
        private readonly List<RowTracker> _rows = new List<RowTracker>();

        public IReadOnlyList<RowTracker> Items => _rows.AsReadOnly();

        public void Limpiar()
        {
            _rows.Clear();
        }

        public void ActualizarCantidad(int productId, int nuevaCantidad)
        {
            var tracker = _rows.LastOrDefault(r => r.ProductID == productId && !r.IsSent);
            if (tracker != null)
            {
                tracker.Cantidad = nuevaCantidad;
                tracker.IsModified = true;
            }
        }

        public void ActualizarObservacion(int productId, string nuevaObs)
        {
            var tracker = _rows.LastOrDefault(r => r.ProductID == productId && !r.IsSent);
            if (tracker != null)
            {
                tracker.Observacion = nuevaObs;
                tracker.IsModified = true;
            }
        }

        public void EliminarProducto(int productId)
        {
            var tracker = _rows.LastOrDefault(r => r.ProductID == productId && !r.IsSent);
            if (tracker != null)
            {
                _rows.Remove(tracker);
            }
        }


        public void AgregarProducto(int prodID, string nombre, string categoria, double precio, string observacion = "")
        {
            _rows.Add(new RowTracker
            {
                DetailID = 0,
                ProductID = prodID,
                ProductName = nombre,
                Cantidad = 1,
                Precio = precio,
                Observacion = observacion,
                IsNewlyAdded = true,
                IsSent = false
            });
        }

        public IEnumerable<RowTracker> ObtenerParaEnviarAKitchen()
        {
            return _rows.Where(r => r.DebeEnviarAKitchen());
        }

        public void MarcarComoEnviado(RowTracker tracker, int nuevoDetailID)
        {
            tracker.DetailID = nuevoDetailID;
            tracker.IsSent = true;
            tracker.IsNewlyAdded = false;
        }

        public void CargarDesdeDataTable(DataTable dt)
        {
            _rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                _rows.Add(new RowTracker
                {
                    DetailID = Convert.ToInt32(row["DetailID"]),
                    ProductID = Convert.ToInt32(row["ProdID"]),
                    ProductName = row["pName"].ToString(),
                    Cantidad = Convert.ToInt32(row["qty"]),
                    Precio = Convert.ToDouble(row["price"]),
                    Observacion = row["observation"]?.ToString() ?? "",
                    IsSent = Convert.ToBoolean(row["IsSentToKitchen"]),
                    IsNewlyAdded = false
                });
            }
        }
    }

}
