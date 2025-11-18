using System;
using System.ComponentModel;

namespace Biblioteca.DTO
{
    public class PresupuestoDTO : INotifyPropertyChanged
    {
        private int? _id;
        public int? ID
        {
            get => _id;
            set { if (_id != value) { _id = value; OnPropertyChanged(nameof(ID)); } }
        }

        private int? _cuentaID;
        public int? CuentaID
        {
            get => _cuentaID;
            set { if (_cuentaID != value) { _cuentaID = value; OnPropertyChanged(nameof(CuentaID)); } }
        }

        private int _usuarioID;
        public int UsuarioID
        {
            get => _usuarioID;
            set { if (_usuarioID != value) { _usuarioID = value; OnPropertyChanged(nameof(UsuarioID)); } }
        }

        private string _descrip;
        public string Descrip
        {
            get => _descrip;
            set { if (_descrip != value) { _descrip = value; OnPropertyChanged(nameof(Descrip)); } }
        }

        private decimal _prEjecTotal;
        public decimal PrEjecTotal
        {
            get => _prEjecTotal;
            set { if (_prEjecTotal != value) { _prEjecTotal = value; OnPropertyChanged(nameof(PrEjecTotal)); } }
        }

        private decimal _prEjecTotal1;
        public decimal PrEjecTotal1
        {
            get => _prEjecTotal1;
            set { if (_prEjecTotal1 != value) { _prEjecTotal1 = value; OnPropertyChanged(nameof(PrEjecTotal1)); } }
        }

        private char _ejecMoneda;
        public char EjecMoneda
        {
            get => _ejecMoneda;
            set { if (_ejecMoneda != value) { _ejecMoneda = value; OnPropertyChanged(nameof(EjecMoneda)); } }
        }

        private char _ejecMoneda1;
        public char EjecMoneda1
        {
            get => _ejecMoneda1;
            set { if (_ejecMoneda1 != value) { _ejecMoneda1 = value; OnPropertyChanged(nameof(EjecMoneda1)); } }
        }

        private char _ejecConsolidado;
        public char EjecConsolidado
            {
            get => _ejecConsolidado;
            set { if (_ejecConsolidado != value) { _ejecConsolidado = value; OnPropertyChanged(nameof(EjecConsolidado)); } }
            }

        private char _ejecConsolidado1;
        public char EjecConsolidado1
            {
            get => _ejecConsolidado1;
            set { if (_ejecConsolidado1 != value) { _ejecConsolidado1 = value; OnPropertyChanged(nameof(EjecConsolidado1)); } }
            }

        private decimal? _superficie;
        public decimal? Superficie
        {
            get => _superficie;
            set { if (_superficie != value) { _superficie = value; OnPropertyChanged(nameof(Superficie)); } }
        }

        private DateTime _mesBase;
        public DateTime MesBase
        {
            get => _mesBase;
            set { if (_mesBase != value) { _mesBase = value; OnPropertyChanged(nameof(MesBase)); } }
        }

        private DateTime _fechaC;
        public DateTime FechaC
        {
            get => _fechaC;
            set { if (_fechaC != value) { _fechaC = value; OnPropertyChanged(nameof(FechaC)); } }
        }

        private DateTime _fechaM;
        public DateTime FechaM
        {
            get => _fechaM;
            set { if (_fechaM != value) { _fechaM = value; OnPropertyChanged(nameof(FechaM)); } }
        }

        private bool _esModelo;
        public bool EsModelo
        {
            get => _esModelo;
            set { if (_esModelo != value) { _esModelo = value; OnPropertyChanged(nameof(EsModelo)); } }
        }

        private decimal _tipoCambioD;
        public decimal TipoCambioD
        {
            get => _tipoCambioD;
            set { if (_tipoCambioD != value) { _tipoCambioD = value; OnPropertyChanged(nameof(TipoCambioD)); } }
        }

        // Nuevos totales
        private decimal _egresosTotales;
        public decimal EgresosTotales
        {
            get => _egresosTotales;
            set { if (_egresosTotales != value) { _egresosTotales = value; OnPropertyChanged(nameof(EgresosTotales)); } }
        }

        private decimal _egresosTotales1;
        public decimal EgresosTotales1
        {
            get => _egresosTotales1;
            set { if (_egresosTotales1 != value) { _egresosTotales1 = value; OnPropertyChanged(nameof(EgresosTotales1)); } }
        }

        private decimal _ingresosTotales;
        public decimal IngresosTotales
        {
            get => _ingresosTotales;
            set { if (_ingresosTotales != value) { _ingresosTotales = value; OnPropertyChanged(nameof(IngresosTotales)); } }
        }

        private decimal _ingresosTotales1;
        public decimal IngresosTotales1
        {
            get => _ingresosTotales1;
            set { if (_ingresosTotales1 != value) { _ingresosTotales1 = value; OnPropertyChanged(nameof(IngresosTotales1)); } }
        }

        private decimal _valorM2;
        public decimal ValorM2
        {
            get => _valorM2;
            set { if (_valorM2 != value) { _valorM2 = value; OnPropertyChanged(nameof(ValorM2)); } }
        }

        private decimal _gastoPesos;
        public decimal GastoPesos
        {
            get => _gastoPesos;
            set { if (_gastoPesos != value) { _gastoPesos = value; OnPropertyChanged(nameof(GastoPesos)); } }
        }

        private decimal _gastoDolares;
        public decimal GastoDolares
        {
            get => _gastoDolares;
            set { if (_gastoDolares != value) { _gastoDolares = value; OnPropertyChanged(nameof(GastoDolares)); } }
        }

        private decimal _cobroPesos;
        public decimal CobroPesos
        {
            get => _cobroPesos;
            set { if (_cobroPesos != value) { _cobroPesos = value; OnPropertyChanged(nameof(CobroPesos)); } }
        }

        private decimal _cobroDolares;
        public decimal CobroDolares
        {
            get => _cobroDolares;
            set { if (_cobroDolares != value) { _cobroDolares = value; OnPropertyChanged(nameof(CobroDolares)); } }
        }

        public static PresupuestoDTO CopiarPresupuestoDTO(PresupuestoDTO original)
        {
            var copia = new PresupuestoDTO();
            copia.ID = original.ID;
            copia.CuentaID = original.CuentaID;
            copia.UsuarioID = original.UsuarioID;
            copia.Descrip = original.Descrip;
            copia.PrEjecTotal = original.PrEjecTotal;
            copia.PrEjecTotal1 = original.PrEjecTotal1;
            copia.EjecMoneda = original.EjecMoneda;
            copia.EjecMoneda1 = original.EjecMoneda1;
            copia.Superficie = original.Superficie;
            copia.MesBase = original.MesBase;
            copia.FechaC = original.FechaC;
            copia.FechaM = original.FechaM;
            copia.EsModelo = original.EsModelo;
            copia.TipoCambioD = original.TipoCambioD;
            copia.EgresosTotales = original.EgresosTotales;
            copia.EgresosTotales1 = original.EgresosTotales1;
            copia.IngresosTotales = original.IngresosTotales;
            copia.IngresosTotales1 = original.IngresosTotales1;
            copia.ValorM2 = original.ValorM2;
            copia.GastoPesos = original.GastoPesos;
            copia.GastoDolares = original.GastoDolares;
            copia.CobroPesos = original.CobroPesos;
            copia.CobroDolares = original.CobroDolares;
            return copia;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
