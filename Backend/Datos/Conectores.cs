using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.DTO;

namespace Backend.Datos
    {
    public class Conectores
        {
        /// <summary>
        /// Obtener todos los insumos de un usuario.
        /// </summary>
        public async Task<(bool Success, string Message, List<InsumoDTO> Insumos)> ObtenerInsumosPorUsuarioAsync(int usuarioID)
            {
            return await DatosWeb.ObtenerInsumosPorUsuarioAsync(usuarioID);
            }

        /// <summary>
        /// Procesar (insertar/actualizar) un insumo y sus artículos relacionados.
        /// </summary>
        public async Task<(bool Success, string Message, int? InsumoID)> ProcesarInsumoAsync(InsumoDTO insumo, List<ArticuloRelDTO> articulos)
            {
            return await DatosWeb.ProcesarInsumoAsync(insumo, articulos);
            }

        /// <summary>
        /// Eliminar un insumo y sus artículos relacionados.
        /// </summary>
        public async Task<(bool Success, string Message)> EliminarInsumoYArticulosRelAsync(int insumoID)
            {
            return await DatosWeb.EliminarInsumoYArticulosRelAsync(insumoID);
            }

        /// <summary>
        /// Procesar (insertar/actualizar) una lista de artículos y sus artículos.
        /// </summary>
        public async Task<(bool Success, string Message, int? ListaID)> ProcesarArticulosListaAsync(ArticulosListaDTO lista, List<ArticuloDTO> articulos)
            {
            return await DatosWeb.ProcesarArticulosListaAsync(lista, articulos);
            }

        /// <summary>
        /// Eliminar una lista de artículos y sus artículos relacionados.
        /// </summary>
        public async Task<(bool Success, string Message)> EliminarArticulosListaYArticulosAsync(int listaID)
            {
            return await DatosWeb.EliminarArticulosListaYArticulosAsync(listaID);
            }

        // =========================
        // ==== EJEMPLOS DE USO ====
        // =========================

        public async Task EjemploUsoAsync()
            {
            // 1. Obtener insumos por usuario
            int usuarioID = 1;
            var (success1, message1, insumos) = await ObtenerInsumosPorUsuarioAsync(usuarioID);
            if (success1)
                {
                foreach (var insumo in insumos)
                    Console.WriteLine($"{insumo.ID} - {insumo.Descrip}");
                }
            else
                {
                Console.WriteLine($"Error: {message1}");
                }

            // 2. Procesar (insertar/actualizar) un insumo y sus artículos relacionados
            var insumoDTO = new InsumoDTO
                {
                ID = 0, // 0 para insertar, o el ID existente para actualizar
                CuentaID = 1,
                UsuarioID = usuarioID,
                Editado = DateTime.Now,
                Tipo = "M",
                Descrip = "Insumo de prueba",
                Unidad = "UN",
                Moneda = "P",
                Precio = 123.45m,
                Codigo = "TEST001",
                ArticulosRel = true
                };
            var articulosRel = new List<ArticuloRelDTO>
            {
                new ArticuloRelDTO
                {
                    ArticuloID = 1001,
                    InsumoID = 0, // 0 o null para nuevo
                    CuentaID = 1,
                    FactorPrecio = 1.0m,
                    FactorUnidad = 1.0m,
                    Seleccionado = true,
                    Accion = 'A' // 'A' = Agregar, 'M' = Modificar, 'B' = Borrar
                }
            };
            var (success2, message2, insumoID) = await ProcesarInsumoAsync(insumoDTO, articulosRel);
            if (success2)
                Console.WriteLine($"Insumo procesado. Nuevo ID: {insumoID}");
            else
                Console.WriteLine($"Error: {message2}");

            // 3. Eliminar un insumo y sus artículos relacionados
            int insumoIdAEliminar = 1; // ID de insumo de prueba
            var (success3, message3) = await EliminarInsumoYArticulosRelAsync(insumoIdAEliminar);
            if (success3)
                Console.WriteLine("Insumo eliminado correctamente.");
            else
                Console.WriteLine($"Error: {message3}");

            // 4. Procesar (insertar/actualizar) una lista de artículos y sus artículos
            var listaDTO = new ArticulosListaDTO
                {
                ID = 0, // 0 para insertar, o el ID existente para actualizar
                CuentaID = 1,
                UsuarioID = usuarioID,
                TipoID = "M",
                EntidadID = null,
                Descrip = "Lista de prueba",
                Fecha = DateTime.Now,
                Moneda = "P",
                Active = true
                };
            var articulos = new List<ArticuloDTO>
            {
                new ArticuloDTO
                {
                    ID = 2001,
                    CuentaID = 1,
                    UsuarioID = usuarioID,
                    ListaID = 0, // 0 o null para nuevo
                    EntidadID = null,
                    TipoID = "M",
                    Descrip = "Artículo de prueba",
                    Unidad = "UN",
                    UnidadFactor = 1.0m,
                    Codigo = "ART001",
                    Fecha = DateTime.Now,
                    Precio = 99.99m,
                    Moneda = "P",
                    Accion = 'A'
                }
            };
            var (success4, message4, listaID) = await ProcesarArticulosListaAsync(listaDTO, articulos);
            if (success4)
                Console.WriteLine($"Lista procesada. Nuevo ID: {listaID}");
            else
                Console.WriteLine($"Error: {message4}");

            // 5. Eliminar una lista de artículos y sus artículos relacionados
            int listaIdAEliminar = 1; // ID de lista de prueba
            var (success5, message5) = await EliminarArticulosListaYArticulosAsync(listaIdAEliminar);
            if (success5)
                Console.WriteLine("Lista eliminada correctamente.");
            else
                Console.WriteLine($"Error: {message5}");
            }
        }
    }