using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Bibioteca.Clases
{
    public class Nodo : ObjetoNotificable
    {
        private bool sup;
        private string id;
        private string item;
        private decimal incidencia;
        private int ordenInt;
        private string descripcion;
        private string unidad;
        private string tipo;
        private decimal cantidad;
        private decimal factor;
        // Moneda 1
        private decimal pu1;
        private decimal importe1;
        private decimal materiales1;
        private decimal manodeobra1;
        private decimal equipos1;
        private decimal subcontratos1;
        private decimal otros1;
        // Moneda 2
        private decimal pu2;
        private decimal importe2;
        private decimal materiales2;
        private decimal manodeobra2;
        private decimal equipos2;
        private decimal subcontratos2;
        private decimal otros2;
        // Moneda 3
        private decimal pu3;
        private decimal importe3;
        private decimal materiales3;
        private decimal manodeobra3;
        private decimal equipos3;
        private decimal subcontratos3;
        private decimal otros3;





        private ObservableCollection<Nodo> inferiores;

        public string Item { get => item; set { item = value; OnPropertyChanged(nameof(Item)); } }
        public decimal Incidencia { get => incidencia; set { incidencia = value; OnPropertyChanged(nameof(Incidencia)); } }
        public bool Sup { get => sup; set { sup = value; OnPropertyChanged(nameof(Sup)); } }
        public string ID { get => id; set { id = value; OnPropertyChanged(nameof(ID)); } }
        public int OrdenInt { get => ordenInt; set { ordenInt = value; OnPropertyChanged(nameof(OrdenInt)); } }
        public string Descripcion { get => descripcion; set { descripcion = value; OnPropertyChanged(nameof(Descripcion)); } }
        public string Unidad { get => unidad; set { unidad = value; OnPropertyChanged(nameof(Unidad)); } }
        public string Tipo { get => tipo; set { tipo = value; OnPropertyChanged(nameof(Tipo)); } }
        public decimal Cantidad { get => cantidad; set { cantidad = value; OnPropertyChanged(nameof(Cantidad)); } }
        public decimal Factor { get => factor; set { factor = value; OnPropertyChanged(nameof(Factor)); } }
        //Nodos con dos monedas
        // Moneda 1
        public decimal PU1 { get => pu1; set { pu1 = value; OnPropertyChanged(nameof(PU1)); } }
        public decimal Importe1 { get => importe1; set { importe1 = value; OnPropertyChanged(nameof(Importe1)); } }
        public decimal Materiales1 { get => materiales1; set { materiales1 = value; OnPropertyChanged(nameof(Materiales1)); } }
        public decimal ManodeObra1 { get => manodeobra1; set { manodeobra1 = value; OnPropertyChanged(nameof(ManodeObra1)); } }
        public decimal Equipos1 { get => equipos1; set { equipos1 = value; OnPropertyChanged(nameof(Equipos1)); } }
        public decimal Subcontratos1 { get => subcontratos1; set { subcontratos1 = value; OnPropertyChanged(nameof(Subcontratos1)); } }
        public decimal Otros1 { get => otros1; set { otros1 = value; OnPropertyChanged(nameof(Otros1)); } }
        // Moneda 2 
        public decimal PU2 { get => pu2; set { pu2 = value; OnPropertyChanged(nameof(PU2)); } }
        public decimal Importe2 { get => importe2; set { importe2 = value; OnPropertyChanged(nameof(Importe2)); } }
        public decimal Materiales2 { get => materiales2; set { materiales2 = value; OnPropertyChanged(nameof(Materiales2)); } }
        public decimal ManodeObra2 { get => manodeobra2; set { manodeobra2 = value; OnPropertyChanged(nameof(ManodeObra2)); } }
        public decimal Equipos2 { get => equipos2; set { equipos2 = value; OnPropertyChanged(nameof(Equipos2)); } }
        public decimal Subcontratos2 { get => subcontratos2; set { subcontratos2 = value; OnPropertyChanged(nameof(Subcontratos2)); } }
        public decimal Otros2 { get => otros2; set { otros2 = value; OnPropertyChanged(nameof(Otros2)); } }
        // Moneda 3 
        public decimal PU3 { get => pu3; set { pu3 = value; OnPropertyChanged(nameof(PU3)); } }
        public decimal Importe3 { get => importe3; set { importe3 = value; OnPropertyChanged(nameof(Importe3)); } }
        public decimal Materiales3 { get => materiales3; set { materiales3 = value; OnPropertyChanged(nameof(Materiales3)); } }
        public decimal ManodeObra3 { get => manodeobra3; set { manodeobra3 = value; OnPropertyChanged(nameof(ManodeObra3)); } }
        public decimal Equipos3 { get => equipos3; set { equipos3 = value; OnPropertyChanged(nameof(Equipos3)); } }
        public decimal Subcontratos3 { get => subcontratos3; set { subcontratos3 = value; OnPropertyChanged(nameof(Subcontratos3)); } }
        public decimal Otros3 { get => otros3; set { otros3 = value; OnPropertyChanged(nameof(Otros3)); } }


        public bool HasItems => Inferiores != null && Inferiores.Count > 0;

        // Nuevo: true si algún inferior tiene PU1 distinto de 0
        public bool HasItem1 => Inferiores != null && Inferiores.Any(i => i != null && i.PU1 != 0m);


        public Nodo Copia() => (Nodo)this.MemberwiseClone();

        public ObservableCollection<Nodo> Inferiores
            {
            get => inferiores;
            set
                {
                if (inferiores != null)
                    inferiores.CollectionChanged -= Inferiores_CollectionChanged;

                inferiores = value;

                if (inferiores != null)
                    inferiores.CollectionChanged += Inferiores_CollectionChanged;

                OnPropertyChanged(nameof(Inferiores));
                OnPropertyChanged(nameof(HasItems));
                }
            }

        private void Inferiores_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
            OnPropertyChanged(nameof(HasItems));
            }
        }
}

