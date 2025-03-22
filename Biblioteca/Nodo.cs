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

//Nodos con dos monedas
// Moneda 1
//public decimal PU1 { get => pu1; set { pu1 = value; OnPropertyChanged(nameof(PU1)); } }
//public decimal Importe1 { get => importe1; set { importe1 = value; OnPropertyChanged(nameof(Importe1)); } }
//public decimal Materiales1 { get => materiales1; set { materiales1 = value; OnPropertyChanged(nameof(Materiales1)); } }
//public decimal ManodeObra1 { get => manodeobra1; set { manodeobra1 = value; OnPropertyChanged(nameof(ManodeObra1)); } }
//public decimal Equipos1 { get => equipos1; set { equipos1 = value; OnPropertyChanged(nameof(Equipos1)); } }
//public decimal Subcontratos1 { get => subcontratos1; set { subcontratos1 = value; OnPropertyChanged(nameof(Subcontratos1)); } }
//public decimal Otros1 { get => otros1; set { otros1 = value; OnPropertyChanged(nameof(Otros1)); } }
//// Moneda 2 
//public decimal PU2 { get => pu2; set { pu2 = value; OnPropertyChanged(nameof(PU2)); } }
//public decimal Importe2 { get => importe2; set { importe2 = value; OnPropertyChanged(nameof(Importe2)); } }
//public decimal Materiales2 { get => materiales2; set { materiales2 = value; OnPropertyChanged(nameof(Materiales2)); } }
//public decimal ManodeObra2 { get => manodeobra2; set { manodeobra2 = value; OnPropertyChanged(nameof(ManodeObra2)); } }
//public decimal Equipos2 { get => equipos2; set { equipos2 = value; OnPropertyChanged(nameof(Equipos2)); } }
//public decimal Subcontratos2 { get => subcontratos2; set { subcontratos2 = value; OnPropertyChanged(nameof(Subcontratos2)); } }
//public decimal Otros2 { get => otros2; set { otros2 = value; OnPropertyChanged(nameof(Otros2)); } }
