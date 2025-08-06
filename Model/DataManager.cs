using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace popus_pizzeria.Model
{
    public static class DataManager
    {
        // Listas en memoria para almacenar los datos
        public static List<MateriaPrima> MateriasPrimas { get; private set; }
        public static List<Producto> Productos { get; private set; }
        public static List<ProductoMateriaPrima> Recetas { get; private set; }

        static DataManager()
        {
            // Inicializar las listas al cargar la aplicación
            MateriasPrimas = new List<MateriaPrima>();
            Productos = new List<Producto>();
            Recetas = new List<ProductoMateriaPrima>();

            // *** Datos de ejemplo para probar ***
            CargarDatosDeEjemplo();
        }

        private static void CargarDatosDeEjemplo()
        {
            // Materias Primas de ejemplo
            MateriasPrimas.Add(new MateriaPrima(1, "Harina de Trigo", "kg", 1.20m));
            MateriasPrimas.Add(new MateriaPrima(2, "Tomate Perita", "kg", 0.80m));
            MateriasPrimas.Add(new MateriaPrima(3, "Queso Mozzarella", "kg", 8.50m));
            MateriasPrimas.Add(new MateriaPrima(4, "Aceite de Oliva", "litro", 5.00m));
            MateriasPrimas.Add(new MateriaPrima(5, "Sal", "kg", 0.50m));
            MateriasPrimas.Add(new MateriaPrima(6, "Levadura Fresca", "gramos", 0.01m)); // 10 gramos = 0.10

            // Productos de ejemplo
            Productos.Add(new Producto(101, "Pizza Margarita Grande", "Pizza clásica con tomate y mozzarella"));
            Productos.Add(new Producto(102, "Pizza Napolitana Mediana", "Pizza con tomate, mozzarella y orégano"));

            // Recetas de ejemplo (Asignar Insumos a Producto)
            // Receta para Pizza Margarita Grande
            Recetas.Add(new ProductoMateriaPrima(101, 1, 0.300m)); // 300 gramos de harina
            Recetas.Add(new ProductoMateriaPrima(101, 2, 0.200m)); // 200 gramos de tomate
            Recetas.Add(new ProductoMateriaPrima(101, 3, 0.250m)); // 250 gramos de queso
            Recetas.Add(new ProductoMateriaPrima(101, 4, 0.020m)); // 20 ml de aceite
            Recetas.Add(new ProductoMateriaPrima(101, 5, 0.005m)); // 5 gramos de sal
            Recetas.Add(new ProductoMateriaPrima(101, 6, 20m));    // 20 gramos de levadura

            // Receta para Pizza Napolitana Mediana (ejemplo simplificado)
            Recetas.Add(new ProductoMateriaPrima(102, 1, 0.200m)); // 200 gramos de harina
            Recetas.Add(new ProductoMateriaPrima(102, 2, 0.150m)); // 150 gramos de tomate
            Recetas.Add(new ProductoMateriaPrima(102, 3, 0.180m)); // 180 gramos de queso
            Recetas.Add(new ProductoMateriaPrima(102, 5, 0.003m)); // 3 gramos de sal
        }

        // Métodos de ayuda para la gestión de IDs
        public static int GetNextMateriaPrimaId()
        {
            return MateriasPrimas.Any() ? MateriasPrimas.Max(mp => mp.Id) + 1 : 1;
        }

        public static int GetNextProductoId()
        {
            return Productos.Any() ? Productos.Max(p => p.Id) + 1 : 1;
        }
    }
}