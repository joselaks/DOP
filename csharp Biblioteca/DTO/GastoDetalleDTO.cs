using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Biblioteca.DTO
{
    public class GastoDetalleDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propName);
            return true;
        }

        private int _id;
        public int ID { get => _id; set => SetProperty(ref _id, value); }

        private int _gastoId;
        public int GastoID { get => _gastoId; set => SetProperty(ref _gastoId, value); }

        private int _usuarioId;
        public int UsuarioID { get => _usuarioId; set => SetProperty(ref _usuarioId, value); }

        private char _tipoId;
        public char TipoID { get => _tipoId; set => SetProperty(ref _tipoId, value); }

        private int? _presupuestoId;
        public int? PresupuestoID { get => _presupuestoId; set => SetProperty(ref _presupuestoId, value); }

        private string? _rubroId;
        public string? RubroID { get => _rubroId; set => SetProperty(ref _rubroId, value); }

        private string? _tareaId;
        public string? TareaID { get => _tareaId; set => SetProperty(ref _tareaId, value); }

        private string? _auxiliarId;
        public string? AuxiliarID { get => _auxiliarId; set => SetProperty(ref _auxiliarId, value); }

        private string? _insumoId;
        public string? InsumoID { get => _insumoId; set => SetProperty(ref _insumoId, value); }

        private string? _descrip;
        public string? Descrip { get => _descrip; set => SetProperty(ref _descrip, value); }

        private string? _unidad;
        public string? Unidad { get => _unidad; set => SetProperty(ref _unidad, value); }

        private decimal _cantidad;
        public decimal Cantidad { get => _cantidad; set
            {
                if (SetProperty(ref _cantidad, value))
                {
                    // recalcular Importe automáticamente si se desea
                    Importe = _cantidad * _precioUnitario;
                }
            }
        }

        private decimal _precioUnitario;
        public decimal PrecioUnitario { get => _precioUnitario; set
            {
                if (SetProperty(ref _precioUnitario, value))
                {
                    Importe = _cantidad * _precioUnitario;
                }
            }
        }

        private char _moneda;
        public char Moneda { get => _moneda; set => SetProperty(ref _moneda, value); }

        private decimal _importe = 0m;
        public decimal Importe { get => _importe; set => SetProperty(ref _importe, value); }

        private char _accion;
        public char Accion { get => _accion; set => SetProperty(ref _accion, value); }
    }
}