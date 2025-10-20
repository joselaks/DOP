using Bibioteca.Clases;
using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca
    {
    public class Gasto : ObjetoNotificable
        {
        #region Estructura

        public GastoDTO encabezado;
        public List<GastoDetalleDTO> detalleLeer;
        public List<GastoDetalleDTO> detalleGrabar;
        #endregion

        public Gasto(GastoDTO _encabezado, List<GastoDetalleDTO> _detalle)
            {
            if (_encabezado == null)
                {
                encabezado = new GastoDTO
                    {
                    ID = 0,
                    };
                }
            else
                { 
                encabezado = CloneEncabezado(_encabezado);
                }

            if (_detalle == null)
                {
                detalleLeer = new List<GastoDetalleDTO>();
                detalleGrabar = new List<GastoDetalleDTO>();
                }
            else
                {
                detalleLeer = _detalle;
                detalleGrabar = CloneDetallesDeep(_detalle);
                }   

            }

        private static GastoDTO CloneEncabezado(GastoDTO source)
        {
            if (source == null) return new GastoDTO();
            return new GastoDTO
            {
                ID = source.ID,
                CuentaID = source.CuentaID,
                UsuarioID = source.UsuarioID,
                FechaDoc = source.FechaDoc,
                FechaCreado = source.FechaCreado,
                FechaEditado = source.FechaEditado,
                Entidad = source.Entidad,
                Documento = source.Documento,
                Descrip = source.Descrip,
                Notas = source.Notas,
                Importe = source.Importe,
                Moneda = source.Moneda
            };
        }

        private static List<GastoDetalleDTO> CloneDetallesDeep(IEnumerable<GastoDetalleDTO>? source)
        {
            if (source == null) return new List<GastoDetalleDTO>();
            return source.Select(d => new GastoDetalleDTO
            {
                ID = d.ID,
                GastoID = d.GastoID,
                UsuarioID = d.UsuarioID,
                CuentaID = d.CuentaID,
                TipoID = d.TipoID,
                PresupuestoID = d.PresupuestoID,
                Presupuesto = d.Presupuesto,
                RubroID = d.RubroID,
                Rubro = d.Rubro,
                TareaID = d.TareaID,
                Tarea = d.Tarea,
                AuxiliarID = d.AuxiliarID,
                Auxiliar = d.Auxiliar,
                InsumoID = d.InsumoID,
                Insumo = d.Insumo,
                Descrip = d.Descrip,
                Unidad = d.Unidad,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                ArticuloID = d.ArticuloID,
                Articulo = d.Articulo,
                ListaDePrecios = d.ListaDePrecios,
                MaestroID = d.MaestroID,
                Maestro = d.Maestro,
                ConceptoMaestroID = d.ConceptoMaestroID,
                ConceptoMaestro = d.ConceptoMaestro,
                Moneda = d.Moneda,
                Importe = d.Importe,
                Accion = d.Accion
            }).ToList();
        }

        }
    }
