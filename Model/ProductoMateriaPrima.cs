using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace popus_pizzeria.Model
{
    // Esta clase representa la relación entre un producto y los ingredientes de su receta
    public class ProductoMateriaPrima
    {
        public int ProductoId { get; set; } // Clave foránea al Producto
        public int MateriaPrimaId { get; set; } // Clave foránea a la MateriaPrima
        public decimal CantidadUsada { get; set; } // Cuánta MateriaPrima se usa para este Producto

        public ProductoMateriaPrima(int productoId, int materiaPrimaId, decimal cantidadUsada)
        {
            ProductoId = productoId;
            MateriaPrimaId = materiaPrimaId;
            CantidadUsada = cantidadUsada;
        }

        public ProductoMateriaPrima()
        {
            // Constructor vacío
        }
    }
}