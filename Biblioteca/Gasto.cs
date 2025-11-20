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
                    FechaDoc = DateTime.Today,
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
                Accion = 'R'
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
                if (det.ID == 0)
                    {
                    det.Accion = 'A';
                    }
                }

            // Marcar modificaciones: los que no estén marcados como 'A' o 'B' => 'M'
            foreach (var det in detallesParaGrabar)
                {
                if (det.Accion != 'A' && det.Accion != 'B')
                    det.Accion = 'M';
                }

            // Normalizar campos de detalles para evitar errores al crear el TVP / al SP
            foreach (var det in detallesParaGrabar)
                {
                // TipoID: normalizar a '0' si no viene
                if (det.TipoID == '\0')
                    det.TipoID = '0';

                // Moneda: tomar la moneda del encabezado si no está especificada
                if (det.Moneda == '\0' && encabezado != null && encabezado.Moneda != '\0')
                    det.Moneda = encabezado.Moneda;

                // FactorCantidad por defecto 1
                if (det.FactorCantidad == 0)
                    det.FactorCantidad = 1.0000m;

                // Evitar nulos en campos NOT NULL en la BD y truncar cadenas a la longitud máxima
                det.Descrip = det.Descrip ?? string.Empty;
                if (det.Descrip.Length > 65)
                    det.Descrip = det.Descrip.Substring(0, 65);

                det.Unidad = det.Unidad ?? string.Empty;
                if (det.Unidad.Length > 6)
                    det.Unidad = det.Unidad.Substring(0, 6);

                det.RubroID = string.IsNullOrWhiteSpace(det.RubroID) ? null : det.RubroID;
                if (det.RubroID != null && det.RubroID.Length > 13)
                    det.RubroID = det.RubroID.Substring(0, 13);

                det.TareaID = string.IsNullOrWhiteSpace(det.TareaID) ? null : det.TareaID;
                if (det.TareaID != null && det.TareaID.Length > 13)
                    det.TareaID = det.TareaID.Substring(0, 13);

                det.AuxiliarID = string.IsNullOrWhiteSpace(det.AuxiliarID) ? null : det.AuxiliarID;
                if (det.AuxiliarID != null && det.AuxiliarID.Length > 13)
                    det.AuxiliarID = det.AuxiliarID.Substring(0, 13);

                det.InsumoID = string.IsNullOrWhiteSpace(det.InsumoID) ? null : det.InsumoID;
                if (det.InsumoID != null && det.InsumoID.Length > 13)
                    det.InsumoID = det.InsumoID.Substring(0, 13);

                // PrecioUnitario y Cantidad: dejar tal cual (0 es válido)
                // Fecha: mantener null si no hay valor (TVP acepta NULL)
                }

            // Construir lista de PresupuestosAfectados a partir de PresupuestoID en detallesParaGrabar y detalleLeer
            var presupuestosSet = new HashSet<int>();
            foreach (var d in detallesParaGrabar)
                {
                if (d.PresupuestoID.HasValue)
                    presupuestosSet.Add(d.PresupuestoID.Value);
                }
            foreach (var d in detalleLeer ?? Enumerable.Empty<GastoDetalleDTO>())
                {
                if (d.PresupuestoID.HasValue)
                    presupuestosSet.Add(d.PresupuestoID.Value);
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

            // Asegurar moneda en encabezado
            if (encabezadoEmpaquetado.Moneda == '\0')
                encabezadoEmpaquetado.Moneda = 'P';

            // Empaquetar request incluyendo PresupuestosAfectados
            var request = new Biblioteca.DTO.ProcesarGastoRequest
                {
                Gasto = encabezadoEmpaquetado,
                Detalles = detallesParaGrabar,
                PresupuestosAfectados = presupuestosSet.ToList()
                };

            return request;
            }

        }
    }
