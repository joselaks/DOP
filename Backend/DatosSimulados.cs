using System.Collections.Generic;

namespace Backend
{
    public static class DatosSimulados
    {
        //public static List<Insumo> ObtenerInsumos() => new()
        //{
        //    new Insumo { Descripcion = "Cemento", Tipo = "Materiales", PrecioUnitario = 1200, Unidad = "KG", MetodoCalculo = "Directo", Codigo = "MAT001" },
        //    new Insumo { Descripcion = "Pintura", Tipo = "Materiales", PrecioUnitario = 850, Unidad = "LT", MetodoCalculo = "Promedio", Codigo = "MAT002" },
        //    new Insumo { Descripcion = "Albañil", Tipo = "Mano de Obra", PrecioUnitario = 3500, Unidad = "HS", MetodoCalculo = "Estimado", Codigo = "MO001" },
        //    new Insumo { Descripcion = "Excavadora", Tipo = "Equipos", PrecioUnitario = 8000, Unidad = "HS", MetodoCalculo = "Tarifa", Codigo = "EQ001" },
        //    new Insumo { Descripcion = "Flete", Tipo = "Otros", PrecioUnitario = 3000, Unidad = "VIAJE", MetodoCalculo = "Tarifa", Codigo = "OT001" },
        //};

        //public static List<Articulo> ObtenerArticulosProveedor() => new()
        //{
        //    new Articulo { Descripcion = "Bolsa Cemento Loma Negra", Unidad = "KG", Factor = 50, Moneda = "Corralon Medrano", Precio = 1200, Codigo = "ART001" },
        //    new Articulo { Descripcion = "Pintura Blanca 10L", Unidad = "LT", Factor = 10, Moneda = "Corralon Medrano", Precio = 8500, Codigo = "ART002" },
        //    new Articulo { Descripcion = "Pala de Punta", Unidad = "UN", Factor = 1, Moneda = "Corralon Medrano", Precio = 1500, Codigo = "ART003" },
        //    new Articulo { Descripcion = "Camión Mixer", Unidad = "HS", Factor = 1, Moneda = "Corralon Medrano", Precio = 10000, Codigo = "ART004" },
        //};

        //        public static List<Tarea> ObtenerTareas() => new()
        //        {
        //            new Tarea { Descripcion = "Revoque Fino Interior", Unidad = "m2", Precio = 800, Rubro = "Obra Gruesa", SubRubro = "Revoques" },
        //            new Tarea { Descripcion = "Revoque Grueso Exterior", Unidad = "m2", Precio = 900, Rubro = "Obra Gruesa", SubRubro = "Revoques" },
        //            new Tarea { Descripcion = "Colocación Cerámicos Piso", Unidad = "m2", Precio = 1500, Rubro = "Terminaciones", SubRubro = "Pisos y Revestimientos" },
        //            new Tarea { Descripcion = "Colocación Cerámicos Pared", Unidad = "m2", Precio = 1450, Rubro = "Terminaciones", SubRubro = "Pisos y Revestimientos" },
        //            new Tarea { Descripcion = "Pintura Interior Látex", Unidad = "m2", Precio = 1200, Rubro = "Terminaciones", SubRubro = "Pintura" },
        //            new Tarea { Descripcion = "Pintura Exterior", Unidad = "m2", Precio = 1300, Rubro = "Terminaciones", SubRubro = "Pintura" },
        //            new Tarea { Descripcion = "Excavación Zanjas Fundación", Unidad = "ml", Precio = 2500, Rubro = "Movimiento de Suelo", SubRubro = "Excavación" },
        //            new Tarea { Descripcion = "Relleno Compactado", Unidad = "m3", Precio = 1800, Rubro = "Movimiento de Suelo", SubRubro = "Relleno" },
        //        };

        //        public static List<Rubro> ObtenerRubrosConSubrubros()
        //        {
        //            return new List<Rubro>
        //    {
        //        new Rubro
        //        {
        //            Nombre = "Obra Gruesa",
        //            SubRubros = new List<SubRubro>
        //            {
        //                new SubRubro
        //                {
        //                    Nombre = "Revoques",
        //                    Tareas = new List<Tarea>
        //                    {
        //                        new() { Descripcion = "Revoque Fino Interior", Unidad = "m2", Precio = 800 },
        //                        new() { Descripcion = "Revoque Grueso Exterior", Unidad = "m2", Precio = 900 }
        //                    }
        //                }
        //            }
        //        },
        //        new Rubro
        //        {
        //            Nombre = "Terminaciones",
        //            SubRubros = new List<SubRubro>
        //            {
        //                new SubRubro
        //                {
        //                    Nombre = "Pisos y Revestimientos",
        //                    Tareas = new List<Tarea>
        //                    {
        //                        new() { Descripcion = "Colocación Cerámicos Piso", Unidad = "m2", Precio = 1500 },
        //                        new() { Descripcion = "Colocación Cerámicos Pared", Unidad = "m2", Precio = 1450 }
        //                    }
        //                },
        //                new SubRubro
        //                {
        //                    Nombre = "Pintura",
        //                    Tareas = new List<Tarea>
        //                    {
        //                        new() { Descripcion = "Pintura Interior Látex", Unidad = "m2", Precio = 1200 },
        //                        new() { Descripcion = "Pintura Exterior", Unidad = "m2", Precio = 1300 }
        //                    }
        //                }
        //            }
        //        },
        //        new Rubro
        //        {
        //            Nombre = "Movimiento de Suelo",
        //            SubRubros = new List<SubRubro>
        //            {
        //                new SubRubro
        //                {
        //                    Nombre = "Excavación",
        //                    Tareas = new List<Tarea>
        //                    {
        //                        new() { Descripcion = "Excavación Zanjas Fundación", Unidad = "ml", Precio = 2500 }
        //                    }
        //                },
        //                new SubRubro
        //                {
        //                    Nombre = "Relleno",
        //                    Tareas = new List<Tarea>
        //                    {
        //                        new() { Descripcion = "Relleno Compactado", Unidad = "m3", Precio = 1800 }
        //                    }
        //                }
        //            }
        //        }
        //    };
        //        }


        //public static Dictionary<string, List<string>> ObtenerInsumoArticuloMap() => new()
        //{
        //    { "MAT001", new List<string> { "ART001" } },
        //    { "MAT002", new List<string> { "ART002" } },
        //    { "MO001", new List<string> { "ART003" } },
        //    { "EQ001", new List<string> { "ART004" } },
        //    { "OT001", new List<string> { "ART003" } },
        //};

        //public static Dictionary<string, List<string>> ObtenerTareaInsumoMap() => new()
        //{
        //    { "Revoque Fino Interior", new List<string> { "MAT001", "MO001", "OT001", "EQ001" } },
        //    { "Revoque Grueso Exterior", new List<string> { "MAT001", "MO001", "OT001", "EQ001" } },
        //    { "Colocación Cerámicos Piso", new List<string> { "MAT001", "MAT002", "MO001", "OT001" } },
        //    { "Colocación Cerámicos Pared", new List<string> { "MAT001", "MAT002", "MO001", "OT001" } },
        //    { "Pintura Interior Látex", new List<string> { "MAT002", "MO001", "OT001" } },
        //    { "Pintura Exterior", new List<string> { "MAT002", "MO001", "OT001", "EQ001" } },
        //    { "Excavación Zanjas Fundación", new List<string> { "EQ001", "MO001", "OT001" } },
        //    { "Relleno Compactado", new List<string> { "EQ001", "MAT001", "MO001", "OT001" } },
        //};
    }
}
