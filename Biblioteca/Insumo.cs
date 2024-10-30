using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bibioteca.Clases
{
    public class Insumo : ObjetoNotificable
    {

        string id { get; set; }
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        string descripcion { get; set; }
        public string Descripcion
        {
            get
            {
                return descripcion;
            }
            set
            {
                descripcion = value;
                OnPropertyChanged("Descripcion");
            }
        }

        string unidad { get; set; }
        public string Unidad
        {
            get
            {
                return unidad;
            }
            set
            {
                unidad = value;
                OnPropertyChanged("Unidad");
            }
        }

        public string Tipo { get; set; }

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

        decimal factor { get; set; }
        public decimal Factor
        {
            get
            {
                return factor;
            }
            set
            {
                factor = value;
                OnPropertyChanged("Factor");
            }
        }


        decimal pu { get; set; }
        public decimal PU
        {
            get
            {
                return pu;
            }
            set
            {
                pu = value;
                OnPropertyChanged("PU");
            }
        }

        decimal importe;
        public decimal Importe
        {
            get
            {
                return importe;
            }
            set
            {
                importe = value;
                OnPropertyChanged("Importe");
            }
        }

    }
}