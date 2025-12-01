using Bibioteca.Clases;
using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Biblioteca
{
    // Genera un árbol de presupuesto para control, agregando el Rubro "Insumos no imputados"
    public class ControlPresupuesto : ObjetoNotificable
    {
        public ObservableCollection<Nodo> Arbol { get; private set; } = new();

        // Construye el árbol base y agrega el Rubro "Insumos no imputados"
        public void Construir(List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones, List<GastoDetalleDTO> detalles)
        {
            if (conceptos == null) conceptos = new List<ConceptoDTO>();
            if (relaciones == null) relaciones = new List<RelacionDTO>();
            if (detalles == null) detalles = new List<GastoDetalleDTO>();

            // 1) Armar árbol como en Presupuesto.generaPresupuesto (raíz = "0")
            Arbol.Clear();

            var relacionesRaiz = relaciones
                .Where(r => r.CodSup == "0")
                .OrderBy(r => r.OrdenInt)
                .ToList();

            foreach (var rel in relacionesRaiz)
            {
                var rubro = conceptos.FirstOrDefault(c => c.ConceptoID == rel.CodInf);
                if (rubro == null) continue;

                var nodoRubro = new Nodo
                {
                    ID = rubro.ConceptoID,
                    Descripcion = rubro.Descrip,
                    Tipo = (rubro.Tipo == '0') ? "R" : rubro.Tipo.ToString(),
                    Unidad = rubro.Unidad,
                    Cantidad = 1,
                    Sup = true,
                    Inferiores = new ObservableCollection<Nodo>()
                };

                // Hijos
                var hijos = GetElementosHijos(nodoRubro, conceptos, relaciones, 1);
                if (hijos != null && hijos.Count > 0)
                {
                    nodoRubro.Inferiores = new ObservableCollection<Nodo>(hijos);
                }

                Arbol.Add(nodoRubro);
            }

            // 2) Agregar rubro "Insumos no imputados" con un T por cada naturaleza en detalles.TipoID
            AgregarInsumosNoImputados(detalles);

            // 3) Numerar
            NumeraItems(Arbol, "");
            OnPropertyChanged(nameof(Arbol));
        }

        private ObservableCollection<Nodo> GetElementosHijos(Nodo elemento, List<ConceptoDTO> listaConceptos, List<RelacionDTO> listaRelaciones, int nivel)
        {
            if (elemento == null) return null;

            var hijos = new ObservableCollection<Nodo>();

            var rels = listaRelaciones
                .Where(r => r.CodSup == elemento.ID)
                .OrderBy(r => r.OrdenInt)
                .ToList();

            foreach (var r in rels)
            {
                var c = listaConceptos.FirstOrDefault(x => x.ConceptoID == r.CodInf);
                if (c == null) continue;

                var n = new Nodo
                {
                    ID = c.ConceptoID,
                    Descripcion = c.Descrip,
                    PU1 = c.PrEjec,
                    PU2 = c.PrEjec1,
                    Cantidad = r.CanEjec,
                    Sup = false,
                    Unidad = c.Unidad,
                    Tipo = c.Tipo.ToString(),
                    Inferiores = new ObservableCollection<Nodo>()
                };

                // Si padre es T y el hijo también es T => convierte a auxiliar "A"
                if (elemento.Tipo == "T" && c.Tipo.ToString() == "T")
                    n.Tipo = "A";
                else
                {
                    // Último nivel con tipo no reconocido como insumo => mantener tipo real
                    bool esUltimoNivel = !listaRelaciones.Any(x => x.CodSup == c.ConceptoID);
                    if (esUltimoNivel && !"MDES".Contains(c.Tipo))
                        n.Tipo = c.Tipo.ToString(); // podría ser "O" u otro
                }

                var sub = GetElementosHijos(n, listaConceptos, listaRelaciones, nivel + 1);
                if (sub != null && sub.Count > 0)
                    n.Inferiores = new ObservableCollection<Nodo>(sub);

                hijos.Add(n);
            }

            return hijos;
        }

        private void AgregarInsumosNoImputados(List<GastoDetalleDTO> detalles)
        {
            // Agrupar por naturaleza (TipoID) de los detalles
            var gruposNaturaleza = detalles
                .Where(d => d != null)
                .GroupBy(d => d.TipoID)
                .OrderBy(g => g.Key)
                .ToList();

            if (gruposNaturaleza.Count == 0)
                return;

            // Rubro contenedor
            var rubroNoImputados = new Nodo
            {
                ID = "R-INP",
                Descripcion = "Insumos no imputados",
                Tipo = "R",
                Unidad = "Gl",
                Cantidad = 1,
                Sup = true,
                Inferiores = new ObservableCollection<Nodo>()
            };

            foreach (var g in gruposNaturaleza)
            {
                char nat = g.Key == '\0' ? 'O' : g.Key; // por defecto "Otros"
                string id = $"T-INP-{nat}";
                string desc = $"{NaturalezaDescripcion(nat)} no imputados";

                var nodoT = new Nodo
                {
                    ID = id,
                    Descripcion = desc,
                    Tipo = "T",
                    Unidad = "Gl",
                    Cantidad = 1,
                    Sup = false,
                    Inferiores = new ObservableCollection<Nodo>()
                };

                // Opcional: si deseases agregar ítems hoja por cada detalle, podrías iterar g aquí
                // y crear nodos de tipo 'M','D','E','S','O' con Cantidad/PU1/PU2 desde el detalle.

                rubroNoImputados.Inferiores.Add(nodoT);
            }

            Arbol.Add(rubroNoImputados);
        }

        private static string NaturalezaDescripcion(char tipo)
        {
            return tipo switch
            {
                'M' => "Materiales",
                'D' => "Mano de obra",
                'E' => "Equipos",
                'S' => "Subcontratos",
                'O' => "Otros",
                'A' => "Auxiliares",
                _   => $"Naturaleza {tipo}"
            };
        }

        private void NumeraItems(IEnumerable<Nodo> nodos, string prefijo)
        {
            int contador = 1;
            foreach (var nodo in nodos)
            {
                string itemActual = string.IsNullOrEmpty(prefijo) ? $"{contador}" : $"{prefijo}.{contador}";
                nodo.Item = itemActual;

                if (nodo.Inferiores != null && nodo.Inferiores.Count > 0)
                {
                    NumeraItems(nodo.Inferiores, itemActual);
                }
                contador++;
            }
        }
    }
}
