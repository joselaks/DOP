using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Bibioteca.Clases
{
    public class CambioAuxiliar : ObjetoNotificable
    {
        string idSuperior { get; set; }
        public string IdSuperior
        {
            get
            {
                return idSuperior;
            }
            set
            {
                idSuperior = value;
                OnPropertyChanged("IdSuperior");
            }
        }

        string idInferior { get; set; }
        public string IdInferior
        {
            get
            {
                return idInferior;
            }
            set
            {
                idInferior = value;
                OnPropertyChanged("IdInferior");
            }
        }

        decimal cantidad { get; set; }
        public decimal Cantidad
        {
            get
            {
                return cantidad;
            }
            set
            {
                cantidad = value;
                OnPropertyChanged("Cantidad");
            }
        }
    }
}

