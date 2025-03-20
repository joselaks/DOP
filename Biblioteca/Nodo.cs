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
        private int ordenInt;
        private string descripcion;
        private string unidad;
        private string tipo;
        private decimal cantidad;
        private decimal factor;
        // Moneda 1
        private decimal pu;
        private decimal importe;
        private decimal materiales;
        private decimal manodeobra;
        private decimal equipos;
        private decimal subcontratos;
        private decimal otros;
        // Moneda 2
        private decimal pu2;
        private decimal importe2;
        private decimal materiales2;
        private decimal manodeobra2;
        private decimal equipos2;
        private decimal subcontratos2;
        private decimal otros2;
        private ObservableCollection<Nodo> inferiores;

        public bool Sup { get => sup; set { sup = value; OnPropertyChanged(nameof(Sup)); } }
        public string ID { get => id; set { id = value; OnPropertyChanged(nameof(ID)); } }
        public int OrdenInt { get => ordenInt; set { ordenInt = value; OnPropertyChanged(nameof(OrdenInt)); } }
        public string Descripcion { get => descripcion; set { descripcion = value; OnPropertyChanged(nameof(Descripcion)); } }
        public string Unidad { get => unidad; set { unidad = value; OnPropertyChanged(nameof(Unidad)); } }
        public string Tipo { get => tipo; set { tipo = value; OnPropertyChanged(nameof(Tipo)); } }
        public decimal Cantidad { get => cantidad; set { cantidad = value; OnPropertyChanged(nameof(Cantidad)); } }
        public decimal Factor { get => factor; set { factor = value; OnPropertyChanged(nameof(Factor)); } }
        // Moneda 1
        public decimal PU { get => pu; set { pu = value; OnPropertyChanged(nameof(PU)); } }
        public decimal Importe { get => importe; set { importe = value; OnPropertyChanged(nameof(Importe)); } }
        public decimal Materiales { get => materiales; set { materiales = value; OnPropertyChanged(nameof(Materiales)); } }
        public decimal ManodeObra { get => manodeobra; set { manodeobra = value; OnPropertyChanged(nameof(ManodeObra)); } }
        public decimal Equipos { get => equipos; set { equipos = value; OnPropertyChanged(nameof(Equipos)); } }
        public decimal Subcontratos { get => subcontratos; set { subcontratos = value; OnPropertyChanged(nameof(Subcontratos)); } }
        public decimal Otros { get => otros; set { otros = value; OnPropertyChanged(nameof(Otros)); } }
        // Moneda 2 





        public bool HasItems => Inferiores != null && Inferiores.Count > 0;

        public Nodo Copia() => (Nodo)this.MemberwiseClone();

        public ObservableCollection<Nodo> Inferiores { get => inferiores; set => inferiores = value; }
    }
}
