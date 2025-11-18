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

        public Gasto(GastoDTO _encabezado, List<GastoDetalleDTO> _detalle, bool tipoGasto)
            {
            if (_encabezado == null)
                {
                encabezado = new GastoDTO
                    {
                    ID = 0,
                    TipoID = tipoGasto ? (byte)10 : (byte)20
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
                TipoID = source.TipoID,
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
                Moneda = source.Moneda,
                TipoCambioD = source.TipoCambioD
            };
        }

        private static List<GastoDetalleDTO> CloneDetallesDeep(IEnumerable<GastoDetalleDTO>? source)
        {
            if (source == null) return new List<GastoDetalleDTO>();
            return source.Select(d => new GastoDetalleDTO
            {
                ID = d.ID,
                GastoID = d.GastoID,
                CobroID = d.CobroID,
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
                FactorCantidad = d.FactorCantidad,
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

        public Biblioteca.DTO.ProcesarGastoRequest EmpaquetarGasto()
            {
            // Preparar lista de detalles a enviar: partir de detalleGrabar (clonada)
            var detallesParaGrabar = CloneDetallesDeep(this.detalleGrabar);

            // Si en detalleLeer hay registros que ya no están en detalleGrabar => marcarlos como 'B' (baja)
            foreach (var dLeer in detalleLeer ?? Enumerable.Empty<GastoDetalleDTO>())
                {
                bool yaExiste = detallesParaGrabar.Any(d => d.ID == dLeer.ID);
                if (!yaExiste)
                    {
                    // Añadir una copia marcada como baja
                    var detBaja = new GastoDetalleDTO
                        {
                        ID = dLeer.ID,
                        GastoID = dLeer.GastoID,
                        CobroID = dLeer.CobroID,
                        UsuarioID = dLeer.UsuarioID,
                        CuentaID = dLeer.CuentaID,
                        TipoID = dLeer.TipoID,
                        PresupuestoID = dLeer.PresupuestoID,
                        Presupuesto = dLeer.Presupuesto,
                        RubroID = dLeer.RubroID,
                        Rubro = dLeer.Rubro,
                        TareaID = dLeer.TareaID,
                        Tarea = dLeer.Tarea,
                        AuxiliarID = dLeer.AuxiliarID,
                        Auxiliar = dLeer.Auxiliar,
                        InsumoID = dLeer.InsumoID,
                        Insumo = dLeer.Insumo,
                        Descrip = dLeer.Descrip,
                        Unidad = dLeer.Unidad,
                        Cantidad = dLeer.Cantidad,
                        FactorCantidad = dLeer.FactorCantidad,
                        PrecioUnitario = dLeer.PrecioUnitario,
                        ArticuloID = dLeer.ArticuloID,
                        Articulo = dLeer.Articulo,
                        ListaDePrecios = dLeer.ListaDePrecios,
                        MaestroID = dLeer.MaestroID,
                        Maestro = dLeer.Maestro,
                        ConceptoMaestroID = dLeer.ConceptoMaestroID,
                        ConceptoMaestro = dLeer.ConceptoMaestro,
                        Moneda = dLeer.Moneda,
                        Importe = dLeer.Importe,
                        Accion = 'B'
                        };
                    detallesParaGrabar.Add(detBaja);
                    }
                }

            // Marcar altas: los que están en detallesParaGrabar pero no existían en detalleLeer => 'A'
            foreach (var det in detallesParaGrabar)
                {
                bool existiaAntes = (detalleLeer ?? Enumerable.Empty<GastoDetalleDTO>()).Any(d => d.ID == det.ID);
                if (!existiaAntes)
                    det.Accion = 'A';
                }

            // Marcar modificaciones: los que no estén marcados como 'A' o 'B' => 'M'
            foreach (var det in detallesParaGrabar)
                {
                if (det.Accion != 'A' && det.Accion != 'B')
                    det.Accion = 'M';
                }

            // Clonar encabezado para el request y normalizar fechas mínimas
            var fechaActual = DateTime.Today;
            DateTime fechaMinSql = new DateTime(1753, 1, 1);

            var encabezadoEmpaquetado = new GastoDTO
                {
                ID = encabezado.ID,
                TipoID = encabezado.TipoID,
                CuentaID = encabezado.CuentaID,
                UsuarioID = encabezado.UsuarioID,
                FechaDoc = (encabezado.FechaDoc >= fechaMinSql) ? encabezado.FechaDoc : fechaActual,
                FechaCreado = (encabezado.FechaCreado >= fechaMinSql) ? encabezado.FechaCreado : fechaActual,
                FechaEditado = (encabezado.FechaEditado >= fechaMinSql) ? encabezado.FechaEditado : fechaActual,
                Entidad = encabezado.Entidad,
                Documento = encabezado.Documento,
                Descrip = encabezado.Descrip,
                Notas = encabezado.Notas,
                Importe = encabezado.Importe,
                Moneda = encabezado.Moneda,
                TipoCambioD = encabezado.TipoCambioD
                };

            // Si el encabezado original representaba uno nuevo (ID == 0) puedes dejar ID=0 o marcar según tu API.
            // Empaquetar request
            var request = new Biblioteca.DTO.ProcesarGastoRequest
                {
                Gasto = encabezadoEmpaquetado,
                Detalles = detallesParaGrabar
                };

            return request;
            }

        }
    }
