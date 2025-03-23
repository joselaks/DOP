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


        decimal pu1 { get; set; }
        public decimal PU1
        {
            get
            {
                return pu1;
            }
            set
            {
                pu1 = value;
                OnPropertyChanged("PU1");
            }
        }
        

        decimal pu2 { get; set; }
        public decimal PU2
        {
            get
            {
                return pu2;
            }
            set
            {
                pu2 = value;
                OnPropertyChanged("PU2");
            }
        }
        

        decimal importe1;
        public decimal Importe1
        {
            get
            {
                return importe1;
            }
            set
            {
                importe1 = value;
                OnPropertyChanged("Importe1");
            }
        }

        decimal importe2;
        public decimal Importe2
        {
            get
            {
                return importe2;
            }
            set
            {
                importe2 = value;
                OnPropertyChanged("Importe2");
            }
        }


    }
}