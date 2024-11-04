using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Bibioteca.Clases
{
    public class Nodo : ObjetoNotificable
    {
        bool sup;

        public bool Sup
        {
            get
            {
                return sup;
            }
            set
            {
                sup = value;
                OnPropertyChanged("Sup");
            }
        }

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

        int ordenInt { get; set; }

        public int OrdenInt
        {
            get
            {
                return ordenInt;
            }
            set
            {
                ordenInt = value;
                OnPropertyChanged("OrdenInt");
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

        string tipo { get; set; }

        public string Tipo
        {
            get
            {
                return tipo;
            }
            set
            {
                tipo = value;
                OnPropertyChanged("Tipo");
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

        decimal factor;

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
                if (this.HasItems)
                {
                    pu = (from c in this.Inferiores
                          select c.Importe).Sum();

                    return pu;
                }
                else
                {
                    return pu;
                }
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

        decimal materiales;

        public decimal Materiales
        {
            get
            {
                return materiales;
            }
            set
            {
                materiales = value;
                OnPropertyChanged("Materiales");
            }
        }

        decimal manodeobra;

        public decimal ManodeObra
        {
            get
            {
                return manodeobra;
            }
            set
            {
                manodeobra = value;
                OnPropertyChanged("ManodeObra");
            }
        }

        decimal equipos;

        public decimal Equipos
        {
            get
            {
                return equipos;
            }
            set
            {
                equipos = value;
                OnPropertyChanged("Equipos");
            }
        }

        decimal subcontratos;

        public decimal Subcontratos
        {
            get
            {
                return subcontratos;
            }
            set
            {
                subcontratos = value;
                OnPropertyChanged("Subcontratos");
            }
        }

        decimal otros;

        public decimal Otros
        {
            get
            {
                return otros;
            }
            set
            {
                otros = value;
                OnPropertyChanged("Otros");
            }
        }

        public bool HasItems
        {
            get
            {
                return (this.Inferiores != null && this.Inferiores.Count > 0);
            }
        }

        public Nodo Copia()
        {
            Nodo other = (Nodo)this.MemberwiseClone();
            return other;
        }

        public ObservableCollection<Nodo> inferiores;
        public ObservableCollection<Nodo> Inferiores
        {
            get { return inferiores; }
            set { inferiores = value; }
        }




    }
}