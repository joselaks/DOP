using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

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
        private decimal pu1Consolidado;
        private decimal importe1;
        private decimal importe1Consolidado;
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
        // Moneda 4
        private decimal pu4;
        private decimal importe4;
        private decimal materiales4;
        private decimal manodeobra4;
        private decimal equipos4;
        private decimal subcontratos4;
        private decimal otros4;

        private ObservableCollection<Nodo> inferiores;

        // Seguimiento de adjuntos profundos
        private readonly HashSet<Nodo> _deepAttachedNodes = new();
        private readonly Dictionary<Nodo, ObservableCollection<Nodo>?> _deepAttachedCollections = new();

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
        // Moneda 1
        public decimal PU1 { get => pu1; set { pu1 = value; OnPropertyChanged(nameof(PU1)); } }
        public decimal PU1Consolidado { get => pu1Consolidado; set { pu1Consolidado = value; OnPropertyChanged(nameof(PU1Consolidado)); } }
        public decimal Importe1 { get => importe1; set { importe1 = value; OnPropertyChanged(nameof(Importe1)); } }
        public decimal Importe1Consolidado { get => importe1Consolidado; set { importe1Consolidado = value; OnPropertyChanged(nameof(Importe1Consolidado)); } }
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
        // Moneda 4 
        public decimal PU4 { get => pu4; set { pu4 = value; OnPropertyChanged(nameof(PU4)); } }
        public decimal Importe4 { get => importe4; set { importe4 = value; OnPropertyChanged(nameof(Importe4)); } }
        public decimal Materiales4 { get => materiales4; set { materiales4 = value; OnPropertyChanged(nameof(Materiales4)); } }
        public decimal ManodeObra4 { get => manodeobra4; set { manodeobra4 = value; OnPropertyChanged(nameof(ManodeObra4)); } } // fix
        public decimal Equipos4 { get => equipos4; set { equipos4 = value; OnPropertyChanged(nameof(Equipos4)); } }
        public decimal Subcontratos4 { get => subcontratos4; set { subcontratos4 = value; OnPropertyChanged(nameof(Subcontratos4)); } } // fix
        public decimal Otros4 { get => otros4; set { otros4 = value; OnPropertyChanged(nameof(Otros4)); } }

        public bool HasItems => Inferiores != null && Inferiores.Count > 0;

        // Propagación profunda: true si algún descendiente (no solo hijo directo) tiene PU1/PU2 != 0
        public bool HasItem1 => Inferiores != null && Inferiores.Any(i => i != null && (i.PU1 != 0m || i.HasItem1));
        public bool HasItem2 => Inferiores != null && Inferiores.Any(i => i != null && (i.PU2 != 0m || i.HasItem2));
        //public bool HasItem3 => Inferiores != null && Inferiores.Any(i => i != null && (i.PU3 != 0m || i.HasItem3));

        public Nodo Copia() => (Nodo)this.MemberwiseClone();

        public ObservableCollection<Nodo> Inferiores
            {
            get => inferiores;
            set
                {
                if (inferiores != null)
                    {
                    inferiores.CollectionChanged -= Inferiores_CollectionChanged;
                    DetachHandlersFromInferiores(inferiores);
                    }

                inferiores = value;

                if (inferiores != null)
                    {
                    inferiores.CollectionChanged += Inferiores_CollectionChanged;
                    AttachHandlersToInferiores(inferiores);
                    }

                OnPropertyChanged(nameof(Inferiores));
                OnPropertyChanged(nameof(HasItems));
                OnPropertyChanged(nameof(HasItem1));
                OnPropertyChanged(nameof(HasItem2));
                //OnPropertyChanged(nameof(HasItem3));
                }
            }

        private void Inferiores_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
            // Adjuntar/detachar handlers para items añadidos o eliminados (nivel inmediato)
            if (e.NewItems != null)
                {
                foreach (var obj in e.NewItems)
                    {
                    if (obj is Nodo n)
                        AttachHandlersDeep(n);
                    }
                }

            if (e.OldItems != null)
                {
                foreach (var obj in e.OldItems)
                    {
                    if (obj is Nodo n)
                        DetachHandlersDeep(n);
                    }
                }

            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(HasItem1));
            OnPropertyChanged(nameof(HasItem2));
            //OnPropertyChanged(nameof(HasItem3));
            }

        // Propagación profunda: adjuntar recursivamente
        private void AttachHandlersToInferiores(ObservableCollection<Nodo> list)
            {
            if (list == null) return;
            foreach (var n in list)
                AttachHandlersDeep(n);
            }

        // Propagación profunda: desadjuntar recursivamente
        private void DetachHandlersFromInferiores(ObservableCollection<Nodo> list)
            {
            if (list == null) return;
            foreach (var n in list)
                DetachHandlersDeep(n);
            }

        private void AttachHandlersDeep(Nodo node)
            {
            if (node == null) return;

            // Suscribirse al PropertyChanged del nodo si no lo está
            if (_deepAttachedNodes.Add(node))
                {
                node.PropertyChanged += Inferior_PropertyChanged;
                }

            // Vigilar cambios en su colección Inferiores
            var coll = node.Inferiores;
            if (!_deepAttachedCollections.TryGetValue(node, out var current) || !ReferenceEquals(current, coll))
                {
                if (current != null)
                    current.CollectionChanged -= ChildInferiores_CollectionChanged;

                if (coll != null)
                    coll.CollectionChanged += ChildInferiores_CollectionChanged;

                _deepAttachedCollections[node] = coll;
                }

            // Recursión
            if (coll != null)
                {
                foreach (var child in coll)
                    AttachHandlersDeep(child);
                }
            }

        private void DetachHandlersDeep(Nodo node)
            {
            if (node == null) return;

            // Desuscribirse del PropertyChanged del nodo
            if (_deepAttachedNodes.Remove(node))
                {
                node.PropertyChanged -= Inferior_PropertyChanged;
                }

            // Desuscribirse de cambios de su colección y procesar recursivamente
            if (_deepAttachedCollections.TryGetValue(node, out var coll))
                {
                if (coll != null)
                    {
                    coll.CollectionChanged -= ChildInferiores_CollectionChanged;
                    foreach (var child in coll)
                        DetachHandlersDeep(child);
                    }
                _deepAttachedCollections.Remove(node);
                }
            }

        // Handler para cambios en colecciones Inferiores de cualquier descendiente
        private void ChildInferiores_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
            if (e.NewItems != null)
                {
                foreach (var obj in e.NewItems)
                    if (obj is Nodo n) AttachHandlersDeep(n);
                }
            if (e.OldItems != null)
                {
                foreach (var obj in e.OldItems)
                    if (obj is Nodo n) DetachHandlersDeep(n);
                }

            // Cualquier cambio estructural puede afectar a HasItemX
            OnPropertyChanged(nameof(HasItem1));
            OnPropertyChanged(nameof(HasItem2));
            //OnPropertyChanged(nameof(HasItem3));
            }

        private void Inferior_PropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
            if (string.IsNullOrEmpty(e.PropertyName))
                return;

            // Cualquier cambio relevante en un descendiente dispara actualización
            if (e.PropertyName == nameof(PU1) || e.PropertyName == nameof(HasItem1) || e.PropertyName == nameof(Inferiores))
                OnPropertyChanged(nameof(HasItem1));

            if (e.PropertyName == nameof(PU2) || e.PropertyName == nameof(HasItem2) || e.PropertyName == nameof(Inferiores))
                OnPropertyChanged(nameof(HasItem2));

            //if (e.PropertyName == nameof(PU3) || e.PropertyName == nameof(HasItem3) || e.PropertyName == nameof(Inferiores))
            //    OnPropertyChanged(nameof(HasItem3));
            }
        }
    }