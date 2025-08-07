using System;
using System.Linq;

namespace popus_pizzeria.Model
{
    public static class CostCalculator
    {
        public static decimal CalcularCostoProducto(int productoId)
        {
            decimal costoTotal = 0m;

            // Obtener recetas desde base de datos
            var ingredientesReceta = DataManager.GetRecetasPorProducto(productoId);
            var materiasPrimas = DataManager.GetMateriasPrimas();

            foreach (var recetaIngrediente in ingredientesReceta)
            {
                var mp = materiasPrimas.FirstOrDefault(m => m.Id == recetaIngrediente.MateriaPrimaId);

                if (mp != null)
                {
                    decimal costoIngrediente = recetaIngrediente.CantidadUsada * mp.CostoPorUnidad;
                    costoTotal += costoIngrediente;
                }
                else
                {
                    Console.WriteLine($"Advertencia: MateriaPrima con ID {recetaIngrediente.MateriaPrimaId} no encontrada.");
                }
            }

            return costoTotal;
        }
    }
}
