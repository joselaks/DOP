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
        public char TipoID { get; set; }
        public int? PresupuestoID { get; set; }

        public string? InsumoID { get; set; }
        public string? TareaID { get; set; }
        public bool? UnicoUso { get; set; }

        [Obsolete("El nuevo esquema no usa RubroID.")]
        public string? RubroID { get; set; }
        [Obsolete("El nuevo esquema no usa AuxiliarID.")]
        public string? AuxiliarID { get; set; }

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
                    RecalcularImporte();
                    OnPropertyChanged(nameof(Cantidad));
                }
            }
        }

        public decimal FactorConcepto { get; set; }

        private decimal _precioUnitario;
        public decimal PrecioUnitario
        {
            get => _precioUnitario;
            set
            {
                if (_precioUnitario != value)
                {
                    _precioUnitario = value;
                    RecalcularImporte();
                    OnPropertyChanged(nameof(PrecioUnitario));
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
        public char Moneda { get; set; }
        public decimal TipoCambioD { get; set; }
        public DateTime? Fecha { get; set; }

        // 'A','M','B' o ' ' (lectura)
        public char Accion { get; set; } = ' ';

        private void RecalcularImporte()
        {
            Importe = Math.Round(Cantidad * PrecioUnitario, 2, MidpointRounding.AwayFromZero);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
