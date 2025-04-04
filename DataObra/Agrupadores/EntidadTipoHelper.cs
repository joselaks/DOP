using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Agrupadores
{
    public static class EntidadTipoHelper
    {
        private static readonly Dictionary<int, string> entidadTipos = new Dictionary<int, string>
        {
            { 'O', "Obra" },
            { 'A', "Administración" },
            { 'C', "Cliente" },
            { 'P', "Proveedor" },
            { 'E', "Empleado" },
            { 'S', "SubContratista" },
            { 'U', "Cuenta" },
            { 'D', "Deposito" },
            { 'I', "Impuesto" },
            { 'T', "Tema" }
        };

        public static string GetEntidadTipo(int id)
        {
            return entidadTipos.TryGetValue(id, out var entidadTipo) ? entidadTipo : "Otro";
        }

    }
}
