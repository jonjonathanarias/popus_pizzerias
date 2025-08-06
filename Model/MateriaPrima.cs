using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace popus_pizzeria.Model
{
    public class MateriaPrima
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; } // Ejemplo: "kg", "gramos", "litros", "unidad"
        public decimal CostoPorUnidad { get; set; }

        // Constructor para facilitar la creación de objetos
        public MateriaPrima(int id, string nombre, string unidadMedida, decimal costoPorUnidad)
        {
            Id = id;
            Nombre = nombre;
            UnidadMedida = unidadMedida;
            CostoPorUnidad = costoPorUnidad;
        }

        public MateriaPrima()
        {
            // Constructor vacío para serialización o inicialización por defecto
        }
    }
}