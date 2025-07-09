using Bibioteca.Clases;
using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca
    {
    public class Maestro : ObjetoNotificable
        {

        #region Estructura

        public List<ConceptoMDTO> listaConceptosLeer;
        public List<RelacionMDTO> listaRelacionesLeer;
        public List<ConceptoMDTO> listaConceptosGrabar;
        public List<RelacionMDTO> listaRelacionesGrabar;

        ObservableCollection<Nodo> arbol;
        bool existe;

        public bool Existe
            {
            get
                {
                return this.existe;
                }
            set
                {
                this.existe = value;
                }
            }

        public ObservableCollection<Nodo> Arbol
            {
            get
                {
                if (arbol == null)
                    {
                    arbol = new ObservableCollection<Nodo>();
                    }
                return arbol;
                }
            set
                {
                arbol = value;
                OnPropertyChanged("Arbol");
                }
            }

        public int ordenint = 1;

        int numeraRubro = 1;

        public int NumeraRubro
            {
            get
                {
                return this.numeraRubro;
                }
            set
                {
                this.numeraRubro = value;
                }
            }


        #endregion

        public Maestro(List<ConceptoMDTO> conBase, List<RelacionMDTO> relBase)
            {
            this.listaConceptosLeer = conBase;
            this.listaRelacionesLeer = relBase;
            this.listaConceptosGrabar = new List<ConceptoMDTO>();
            this.listaRelacionesGrabar = new List<RelacionMDTO>();
            Arbol = new ObservableCollection<Nodo>();

            }


        // Procedimiento que en base a una lista de conceptos y relaciones, genera un presupuesto arbol.
        public void generaPresupuesto()
            {

            var relaciones = listaRelacionesLeer.Where(a => a.CodSup == "0").OrderBy(b => b.OrdenInt).ToList(); //Nodos Raiz
            // Recorro los rubros
            foreach (var item in relaciones)
                {
                //Obtengo el concepto del rubro
                ConceptoMDTO rubro = listaConceptosLeer.FirstOrDefault(a => a.ConceptoID == item.CodInf); //Concepto de los rubros raiz
                //Genero el nodo Rubro
                Nodo registro = new Nodo();
                registro.ID = rubro.ConceptoID;
                registro.Descripcion = rubro.Descrip;
                registro.Tipo = (rubro.Tipo == '0') ? "R" : rubro.Tipo.ToString();
                registro.Unidad = rubro.Unidad;
                registro.Cantidad = 1;
                registro.Sup = true;
                this.Arbol.Add(registro);
                //Obtengo los elementos bajo el rubro
                var bajoRubro = GetElementosHijos(registro, listaConceptosLeer, listaRelacionesLeer, 1); //Rama del rubro
                // Si hay inferiores, los agrego al rubro.
                if (bajoRubro != null)
                    {
                    registro.Inferiores = new ObservableCollection<Nodo>();
                    foreach (var itemBajoRubro in bajoRubro)
                        {
                        registro.Inferiores.Add(itemBajoRubro);
                        }
                    }
                }
            //NumeraItems(Arbol, "");
            }

        private ObservableCollection<Nodo> GetElementosHijos(Nodo elemento, List<ConceptoMDTO> listaConceptos, List<RelacionMDTO> listaRelaciones, int _nivel)
            {
            //Si por cualquier razón el elemento es nulo, regresamos inmediatamente
            //la ejecución
            if (elemento == null)
                return null;

            ObservableCollection<Nodo> elementosHijos = null;
            var qryRelacion = listaRelaciones.Where(a => a.CodSup == elemento.ID).OrderBy(b => b.OrdenInt).ToList();
            if (qryRelacion != null && qryRelacion.Count > 0)
                {
                foreach (var item in qryRelacion)
                    {
                    ConceptoMDTO concepto = listaConceptos.FirstOrDefault(a => a.ConceptoID == item.CodInf);
                    if (concepto != null)
                        {
                        Nodo newItem = new Nodo();
                        newItem.ID = concepto.ConceptoID;
                        newItem.Descripcion = concepto.Descrip;
                        newItem.PU1 = (decimal)concepto.PrEjec;
                        newItem.Cantidad = item.CanEjec;
                        newItem.Sup = false;
                        newItem.Unidad = concepto.Unidad;
                        //newItem.Tipo = (concepto.Tipo == "0") ? _nivel.ToString() : concepto.Tipo;
                        newItem.Tipo = concepto.Tipo.ToString();
                        newItem.Inferiores = new ObservableCollection<Nodo>();
                        // Recurrencia para obtener los elementos hijos de este elemento.
                        var subelementosHijos = GetElementosHijos(newItem, listaConceptos, listaRelaciones, _nivel + 1);
                        if (subelementosHijos != null && subelementosHijos.Count > 0)
                            {
                            newItem.Inferiores = new ObservableCollection<Nodo>();
                            foreach (var elementoHijo in subelementosHijos)
                                {
                                if (newItem.Tipo == "0")//viene de fiebdc
                                    {
                                    if (elementoHijo.Tipo != "0")
                                        {
                                        newItem.Tipo = "T";
                                        }

                                    }
                                newItem.Inferiores.Add(elementoHijo);
                                }
                            }
                        if (elementosHijos == null)
                            elementosHijos = new ObservableCollection<Nodo>();
                        elementosHijos.Add(newItem);
                        }
                    else
                        {

                        }
                    }
                }
            else
                {
                if (elementosHijos == null)
                    elementosHijos = new ObservableCollection<Nodo>();
                }
            return elementosHijos;

            }

        }

        }