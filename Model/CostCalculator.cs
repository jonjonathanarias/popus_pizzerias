using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace popus_pizzeria.Model
{
    public static class CostCalculator
    {
        public static decimal CalcularCostoProducto(int productoId)
        {
            decimal costoTotal = 0m;

            // Obtener todas las entradas de receta para este producto
            var ingredientesReceta = DataManager.Recetas.Where(r => r.ProductoId == productoId).ToList();

            foreach (var recetaIngrediente in ingredientesReceta)
            {
                // Buscar la MateriaPrima correspondiente
                MateriaPrima mp = DataManager.MateriasPrimas.FirstOrDefault(m => m.Id == recetaIngrediente.MateriaPrimaId);

                if (mp != null)
                {
                    // Calcular el costo de esta materia prima para la receta
                    // Asegúrate de que las unidades sean consistentes (ej. si CostoPorUnidad es por kg, CantidadUsada debe ser en kg)
                    decimal costoIngrediente = recetaIngrediente.CantidadUsada * mp.CostoPorUnidad;
                    costoTotal += costoIngrediente;
                }
                else
                {
                    // Manejar el caso donde no se encuentra la MateriaPrima (ej. mostrar una advertencia)
                    Console.WriteLine($"Advertencia: MateriaPrima con ID {recetaIngrediente.MateriaPrimaId} no encontrada para el Producto {productoId}.");
                }
            }

            return costoTotal;
        }
    }
}