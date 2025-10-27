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

        private decimal _prEjecDirecto;
        public decimal PrEjecDirecto
            {
            get => _prEjecDirecto;
            set { if (_prEjecDirecto != value) { _prEjecDirecto = value; OnPropertyChanged(nameof(PrEjecDirecto)); } }
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

        private char _ejecMoneda2;
        public char EjecMoneda2
            {
            get => _ejecMoneda2;
            set { if (_ejecMoneda2 != value) { _ejecMoneda2 = value; OnPropertyChanged(nameof(EjecMoneda2)); } }
            }

        private decimal _prVentaTotal;
        public decimal PrVentaTotal
            {
            get => _prVentaTotal;
            set { if (_prVentaTotal != value) { _prVentaTotal = value; OnPropertyChanged(nameof(PrVentaTotal)); } }
            }

        private decimal _prVentaDirecto;
        public decimal PrVentaDirecto
            {
            get => _prVentaDirecto;
            set { if (_prVentaDirecto != value) { _prVentaDirecto = value; OnPropertyChanged(nameof(PrVentaDirecto)); } }
            }

        private char _ventaMoneda;
        public char VentaMoneda
            {
            get => _ventaMoneda;
            set { if (_ventaMoneda != value) { _ventaMoneda = value; OnPropertyChanged(nameof(VentaMoneda)); } }
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

        private decimal _tipoCambio1;
        public decimal TipoCambio1
            {
            get => _tipoCambio1;
            set { if (_tipoCambio1 != value) { _tipoCambio1 = value; OnPropertyChanged(nameof(TipoCambio1)); } }
            }

        private decimal _tipoCambio2;
        public decimal TipoCambio2
            {
            get => _tipoCambioD;
            set { if (_tipoCambio2 != value) { _tipoCambio2 = value; OnPropertyChanged(nameof(TipoCambio2)); } }
            }

        private decimal _valorM2;
        public decimal ValorM2
            {
            get => _valorM2;
            set { if (_valorM2 != value) { _valorM2 = value; OnPropertyChanged(nameof(ValorM2)); } }
            }

        public static PresupuestoDTO CopiarPresupuestoDTO(PresupuestoDTO original)
            {
            var copia = new PresupuestoDTO();
            copia.ID = original.ID;
            copia.CuentaID = original.CuentaID;
            copia.UsuarioID = original.UsuarioID;
            copia.Descrip = original.Descrip;
            copia.PrEjecTotal = original.PrEjecTotal;
            copia.PrEjecDirecto = original.PrEjecDirecto;
            copia.EjecMoneda = original.EjecMoneda;
            copia.EjecMoneda1 = original.EjecMoneda1;
            copia.EjecMoneda2 = original.EjecMoneda2;
            copia.PrVentaTotal = original.PrVentaTotal;
            copia.PrVentaDirecto = original.PrVentaDirecto;
            copia.VentaMoneda = original.VentaMoneda;
            copia.Superficie = original.Superficie;
            copia.MesBase = original.MesBase;
            copia.FechaC = original.FechaC;
            copia.FechaM = original.FechaM;
            copia.EsModelo = original.EsModelo;
            copia.TipoCambioD = original.TipoCambioD;
            copia.ValorM2 = original.ValorM2;
            return copia;
            }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
