using System;
using System.ComponentModel;

namespace Biblioteca.DTO
{
    public class GastoDetalleDTO : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public int? GastoID { get; set; }
        public int? CobroID { get; set; }
        public int UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public char TipoID { get; set; } = '0';
        public int? PresupuestoID { get; set; }
        public string? Presupuesto { get; set; }
        public string? RubroID { get; set; }
        public string? Rubro { get; set; }
        public string? TareaID { get; set; }
        public string? Tarea { get; set; }
        public string? AuxiliarID { get; set; }
        public string? Auxiliar { get; set; }
        public string? InsumoID { get; set; }
        public string? Insumo { get; set; }
        public string Descrip { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;

        private decimal _cantidad;
        public decimal Cantidad
        {
            get => _cantidad;
            set
            {
                if (_cantidad != value)
                {
                    _cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                    RecalcularImporte();
                }
            }
        }

        public decimal FactorCantidad { get; set; } = 1.0000m;

        private decimal _precioUnitario;
        public decimal PrecioUnitario
        {
            get => _precioUnitario;
            set
            {
                if (_precioUnitario != value)
                {
                    _precioUnitario = value;
                    OnPropertyChanged(nameof(PrecioUnitario));
                    RecalcularImporte();
                }
            }
        }

        private decimal _importe;
        public decimal Importe
        {
            get => _importe;
            set
            {
                if (_importe != value)
                {
                    _importe = value;
                    OnPropertyChanged(nameof(Importe));
                }
            }
        }

        public int? ArticuloID { get; set; }
        public string? Articulo { get; set; }
        public string? ListaDePrecios { get; set; }
        public int? MaestroID { get; set; }
        public string? Maestro { get; set; }
        public string? ConceptoMaestroID { get; set; }
        public string? ConceptoMaestro { get; set; }
        public char Moneda { get; set; } = 'P';
        public decimal TipoCambioD { get; set; } = 1.0000000000m;
        public DateTime? Fecha { get; set; }
        public char Accion { get; set; } = 'A';

        private void RecalcularImporte()
        {
            Importe = Cantidad * PrecioUnitario;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
