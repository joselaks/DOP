using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Agrupadores.Clases
{
    public static class EntidadTipoHelper
    {
        private static readonly Dictionary<int, string> entidadTipos = new Dictionary<int, string>
    {
        { 10, "Cliente" },
        { 20, "Proveedor" },
        { 30, "Contratista" },
        { 40, "Obrero" }
    };

        public static string GetEntidadTipo(int id)
        {
            return entidadTipos.TryGetValue(id, out var entidadTipo) ? entidadTipo : "Otro";
        }
    }
}
