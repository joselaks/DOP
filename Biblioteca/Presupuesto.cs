using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Biblioteca;
using Biblioteca.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bibioteca.Clases
    {
    public class Presupuesto : ObjetoNotificable
        {
        public Nodo respuesta;

        #region Estructura

        public PresupuestoDTO encabezado;
        public List<ConceptoDTO> listaConceptosLeer;
        public List<RelacionDTO> listaRelacionesLeer;
        public List<ConceptoDTO> listaConceptosGrabar;
        public List<RelacionDTO> listaRelacionesGrabar;


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

        ObservableCollection<Nodo> insumos;

        public ObservableCollection<Nodo> Insumos
            {
            get
                {
                if (insumos == null)
                    {
                    insumos = new ObservableCollection<Nodo>();
                    }
                return insumos;
                }
            set
                {
                insumos = value;
                OnPropertyChanged("Insumos");
                }
            }

        ObservableCollection<Nodo> rubros;

        public ObservableCollection<Nodo> Rubros
            {
            get
                {
                if (rubros == null)
                    {
                    rubros = new ObservableCollection<Nodo>();
                    }
                return rubros;
                }
            set
                {
                rubros = value;
                OnPropertyChanged("Rubros");
                }
            }

        ObservableCollection<Nodo> tareas;

        public ObservableCollection<Nodo> Tareas
            {
            get
                {
                if (tareas == null)
                    {
                    tareas = new ObservableCollection<Nodo>();
                    }
                return tareas;
                }
            set
                {
                tareas = value;
                OnPropertyChanged("Tareas");
                }
            }

        ObservableCollection<Nodo> auxiliares;

        public ObservableCollection<Nodo> Auxiliares
            {
            get
                {
                if (auxiliares == null)
                    {
                    auxiliares = new ObservableCollection<Nodo>();
                    }
                return auxiliares;
                }
            set
                {
                auxiliares = value;
                OnPropertyChanged("Auxiliares");
                }
            }


        #endregion

        #region Numeradores

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

        int numeraTarea = 1;

        public int NumeraTarea
            {
            get
                {
                return this.numeraTarea;
                }
            set
                {
                this.numeraTarea = value;
                }
            }

        int numeraMaterial = 1;

        public int NumeraMaterial
            {
            get
                {
                return this.numeraMaterial;
                }
            set
                {
                this.numeraMaterial = value;
                }
            }

        int numeraManodeObra = 1;

        public int NumeraManodeObra
            {
            get
                {
                return this.numeraManodeObra;
                }
            set
                {
                this.numeraManodeObra = value;
                }
            }

        int numeraEquipo = 1;

        public int NumeraEquipo
            {
            get
                {
                return this.numeraEquipo;
                }
            set
                {
                this.numeraEquipo = value;
                }
            }

        int numeraSubcontrato = 1;

        public int NumeraSubcontrato
            {
            get
                {
                return this.numeraSubcontrato;
                }
            set
                {
                this.numeraSubcontrato = value;
                }
            }

        int numeraOtro = 1;

        public int NumeraOtro
            {
            get
                {
                return this.numeraOtro;
                }
            set
                {
                this.numeraOtro = value;
                }
            }


        int numeraAux = 1;
        public int NumeraAux
            {
            get
                {
                return this.numeraAux;
                }
            set
                {
                this.numeraAux = value;
                }
            }


        #endregion

        public Presupuesto(PresupuestoDTO? _encabezado, List<ConceptoDTO> conceptos, List<RelacionDTO> relaciones)
            {
            if (_encabezado == null)
                {
                encabezado = new PresupuestoDTO();
                encabezado.Descrip = "Nuevo Presupuesto";
                listaConceptosLeer = new List<ConceptoDTO>();
                listaRelacionesLeer = new List<RelacionDTO>();
                }
            else
                {
                encabezado = _encabezado;
                listaConceptosLeer = conceptos;
                listaRelacionesLeer = relaciones;
                //Tomar los datos del encabezado para los campos editables.
                //Obtener el detalle del presupuesto.
                generaPresupuesto(null, listaConceptosLeer, listaRelacionesLeer);

                }
            }

        // Procedimiento que en base a una lista de conceptos y relaciones, genera un presupuesto arbol.
        public void generaPresupuesto(string origen, List<ConceptoDTO> listaConceptos, List<RelacionDTO> listaRelaciones)
            {
            // No es necesario generar un nuevo objeto, se puede reutilizar el existente.
            Arbol.Clear();
            //instanciar los lostados de origen
            listaConceptosLeer = listaConceptos;
            listaRelacionesLeer = listaRelaciones;
            //Obtengo lista de relaciones con superior 0, que son los rubros.
            string? raiz;
            if (origen == "fie")
                {
                raiz = null;
                }
            else
                {
                raiz = "0";

                }
            var relaciones = listaRelaciones.Where(a => a.CodSup == raiz).OrderBy(b => b.OrdenInt).ToList(); //Nodos Raiz
            // Recorro los rubros
            foreach (var item in relaciones)
                {
                //Obtengo el concepto del rubro
                ConceptoDTO rubro = listaConceptos.FirstOrDefault(a => a.ConceptoID == item.CodInf); //Concepto de los rubros raiz
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
                var bajoRubro = GetElementosHijos(registro, listaConceptos, listaRelaciones, 1); //Rama del rubro
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
            if (origen == "fie")
                {
                DetectaAuxilarRecursivo(this.Arbol);
                }

            NumeraItems(Arbol, "");
            }

        private ObservableCollection<Nodo> GetElementosHijos(Nodo elemento, List<ConceptoDTO> listaConceptos, List<RelacionDTO> listaRelaciones, int _nivel)
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
                    ConceptoDTO concepto = listaConceptos.FirstOrDefault(a => a.ConceptoID == item.CodInf);
                    if (concepto != null)
                        {
                        Nodo newItem = new Nodo();
                        newItem.ID = concepto.ConceptoID;
                        newItem.Descripcion = concepto.Descrip;
                        newItem.PU1 = (decimal)concepto.PrEjec;
                        newItem.PU2 = (decimal)concepto.PrVent;
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


        public void mismoCodigo(IEnumerable<Nodo> items, Nodo editado)
            {
            foreach (var item in items)
                {
                if (item.ID == editado.ID)
                    {
                    item.Descripcion = editado.Descripcion;
                    item.Unidad = editado.Unidad;
                    item.PU1 = editado.PU1;
                    item.PU2 = editado.PU2;
                    item.Tipo = editado.Tipo;
                    }
                if (item.HasItems)
                    {
                    mismoCodigo(item.Inferiores, editado);
                    }
                }
            }

        private void DetectaAuxilarRecursivo(ObservableCollection<Nodo> nodos)
            {
            foreach (var nodo in nodos)
                {
                if (nodo.Tipo == "T" || nodo.Tipo == "A")
                    {
                    foreach (var inferior in nodo.Inferiores)
                        {
                        if (inferior.Tipo == "T")
                            {
                            inferior.Tipo = "A";
                            }
                        DetectaAuxilarRecursivo(inferior.Inferiores);
                        }
                    }
                else
                    {
                    DetectaAuxilarRecursivo(nodo.Inferiores);
                    }
                }
            }

        public void cambiaCodigo(IEnumerable<Nodo> items, string nuevo, string viejo)
            {
            foreach (var item in items)
                {
                if (item.ID == viejo)
                    {
                    item.ID = nuevo;
                    }
                if (item.HasItems)
                    {
                    cambiaCodigo(item.Inferiores, nuevo, viejo);
                    }
                }
            }

        public bool verificaCodigo(IEnumerable<Nodo> items, string codigo) //verifica si existe el código, pero no para cambios de codigos porque ahi ya existiría el original.
            {
            bool respuesta = false;
            foreach (var item in items)
                {
                if (item.ID == codigo)
                    {
                    respuesta = true;
                    return respuesta;
                    }
                if (item.HasItems && respuesta != true)
                    {
                    bool parcial = verificaCodigo(item.Inferiores, codigo);
                    if (parcial == true)
                        {
                        respuesta = true;
                        }
                    }
                }
            return respuesta;
            }

        public void cambioDesdeInsumo(IEnumerable<Nodo> items, string id, decimal nuevoPU1, decimal nuevoPU2)
            {
            foreach (var item in items)
                {
                if (item.ID == id)
                    {
                    item.PU1 = nuevoPU1;
                    item.PU2 = nuevoPU2;
                    }
                if (item.HasItems)
                    {
                    cambioDesdeInsumo(item.Inferiores, id, nuevoPU1, nuevoPU2);
                    }
                }
            }

        public void cambioCantidadAuxiliar(IEnumerable<Nodo> items, CambioAuxiliar dato)
            {
            foreach (var item in items)
                {
                if (item.ID == dato.IdInferior && FindParentNode(Arbol, item, null).ID == dato.IdSuperior)
                    {
                    item.Cantidad = dato.Cantidad;
                    }
                if (item.HasItems)
                    {
                    cambioCantidadAuxiliar(item.Inferiores, dato);
                    }
                }
            }


        public void operarTipos(IEnumerable<Nodo> items, string tipo, decimal coef)
            {
            foreach (var item in items)
                {
                if (tipo == "todo")
                    {
                    item.PU1 = item.PU1 * coef;
                    item.PU2 = item.PU2 * coef;
                    }
                else
                    {
                    if (item.Tipo == tipo)
                        {
                        item.PU1 = item.PU1 * coef;
                        item.PU2 = item.PU2 * coef;
                        }

                    }
                if (item.HasItems)
                    {
                    operarTipos(item.Inferiores, tipo, coef);
                    }
                }

            }


        public void recalculo(IEnumerable<Nodo> items, bool inicio, decimal FactorSup, bool cInsumos)
            {
            if (inicio == true) //comienza el procedimiento
                {
                if (Insumos == null)
                    {
                    Insumos = new ObservableCollection<Nodo>();
                    }
                else
                    {
                    foreach (var item in Insumos)
                        {
                        item.Cantidad = 0;
                        item.PU1 = 0;
                        item.PU2 = 0;
                        item.Unidad = null;
                        item.Importe1 = 0;
                        item.Importe2 = 0;
                        }
                    }
                ordenint = 1;
                }
            foreach (var item in items)
                {
                //if (item.Tipo == "R" || item.Tipo == "T")
                //    {
                item.OrdenInt = ordenint;
                ordenint = ordenint + 1;

                //}
                if (item.HasItems) //tiene inferiores
                    {
                    if (item.Sup == true) //es el rubro o nodo superior
                        {
                        FactorSup = item.Cantidad;

                        }

                    recalculo(item.Inferiores, false, FactorSup * item.Cantidad, true); //Primero sigo hasta abajo

                    #region Calculo de PU y suma de importes por tipo

                    item.Factor = FactorSup;

                    item.PU1 = (from c in item.Inferiores
                                select c.Importe1).Sum();

                    item.Materiales1 = (from c in item.Inferiores
                                        select c.Materiales1).Sum() * item.Cantidad;
                    item.ManodeObra1 = (from c in item.Inferiores
                                        select c.ManodeObra1).Sum() * item.Cantidad;
                    item.Equipos1 = (from c in item.Inferiores
                                     select c.Equipos1).Sum() * item.Cantidad;
                    item.Subcontratos1 = (from c in item.Inferiores
                                          select c.Subcontratos1).Sum() * item.Cantidad;
                    item.Otros1 = (from c in item.Inferiores
                                   select c.Otros1).Sum() * item.Cantidad;



                    item.PU2 = (from c in item.Inferiores
                                select c.Importe2).Sum();

                    item.Materiales2 = (from c in item.Inferiores
                                        select c.Materiales2).Sum() * item.Cantidad;
                    item.ManodeObra2 = (from c in item.Inferiores
                                        select c.ManodeObra2).Sum() * item.Cantidad;
                    item.Equipos2 = (from c in item.Inferiores
                                     select c.Equipos2).Sum() * item.Cantidad;
                    item.Subcontratos2 = (from c in item.Inferiores
                                          select c.Subcontratos2).Sum() * item.Cantidad;
                    item.Otros2 = (from c in item.Inferiores
                                   select c.Otros2).Sum() * item.Cantidad;

                    #endregion
                    }
                else //es el ultimo del arbol
                    {
                    #region calculo totales por tipo

                    switch (item.Tipo)
                        {
                        case "M":
                            item.Materiales1 = item.Cantidad * item.PU1;
                            item.ManodeObra1 = 0;
                            item.Equipos1 = 0;
                            item.Subcontratos1 = 0;
                            item.Otros1 = 0;
                            item.Materiales2 = item.Cantidad * item.PU2;
                            item.ManodeObra2 = 0;
                            item.Equipos2 = 0;
                            item.Subcontratos2 = 0;
                            item.Otros2 = 0;
                            break;
                        case "D":
                            item.ManodeObra1 = item.Cantidad * item.PU1;
                            item.Materiales1 = 0;
                            item.Equipos1 = 0;
                            item.Subcontratos1 = 0;
                            item.Otros1 = 0;
                            item.ManodeObra2 = item.Cantidad * item.PU2;
                            item.Materiales2 = 0;
                            item.Equipos2 = 0;
                            item.Subcontratos2 = 0;
                            item.Otros2 = 0;
                            break;
                        case "E":
                            item.Equipos1 = item.Cantidad * item.PU1;
                            item.ManodeObra1 = 0;
                            item.Materiales1 = 0;
                            item.Subcontratos1 = 0;
                            item.Otros1 = 0;
                            item.Equipos2 = item.Cantidad * item.PU2;
                            item.ManodeObra2 = 0;
                            item.Materiales2 = 0;
                            item.Subcontratos2 = 0;
                            item.Otros2 = 0;

                            break;
                        case "S":
                            item.Subcontratos1 = item.Cantidad * item.PU1;
                            item.Materiales1 = 0;
                            item.ManodeObra1 = 0;
                            item.Equipos1 = 0;
                            item.Otros1 = 0;
                            item.Subcontratos2 = item.Cantidad * item.PU2;
                            item.Materiales2 = 0;
                            item.ManodeObra2 = 0;
                            item.Equipos2 = 0;
                            item.Otros2 = 0;

                            break;
                        case "O":
                            item.Otros1 = item.Cantidad * item.PU1;
                            item.ManodeObra1 = 0;
                            item.Equipos1 = 0;
                            item.Subcontratos1 = 0;
                            item.Equipos1 = 0;
                            item.Otros2 = item.Cantidad * item.PU2;
                            item.ManodeObra2 = 0;
                            item.Equipos2 = 0;
                            item.Subcontratos2 = 0;
                            item.Equipos2 = 0;

                            break;
                        default:
                            break;
                        }

                    #endregion

                    Nodo sele = Insumos.FirstOrDefault(a => a.ID == item.ID);
                    if (sele == null)
                        {
                        Nodo registro = new Nodo();
                        registro.ID = item.ID;
                        registro.Descripcion = item.Descripcion;
                        registro.Cantidad = item.Cantidad * FactorSup;
                        registro.Unidad = item.Unidad;
                        registro.PU1 = item.PU1;
                        registro.PU2 = item.PU2;
                        registro.Importe1 = registro.Cantidad * registro.PU1;
                        registro.Importe2 = registro.Cantidad * registro.PU2;
                        registro.Tipo = item.Tipo;
                        Insumos.Add(registro);
                        }
                    else
                        {
                        sele.Descripcion = item.Descripcion;
                        sele.Unidad = item.Unidad;
                        sele.PU1 = item.PU1;
                        sele.PU2 = item.PU2;
                        sele.Cantidad = sele.Cantidad + (item.Cantidad * FactorSup);
                        sele.Tipo = item.Tipo;
                        sele.Importe1 = sele.PU1 * sele.Cantidad;
                        sele.Importe2 = sele.PU2 * sele.Cantidad;
                        }
                    }

                item.Importe1 = item.Cantidad * item.PU1;
                item.Importe2 = item.Cantidad * item.PU2;
                item.Factor = FactorSup;
                }
            }

        public void listaRubrosTareasAuxiliares(IEnumerable<Nodo> items)
            {
            foreach (Nodo item in items)
                {
                if (item.Tipo == "R" || item.Tipo == "T" || item.Tipo == "A")
                    {
                    Nodo registro = new Nodo();
                    registro.ID = item.ID;
                    registro.Descripcion = item.Descripcion;
                    registro.Cantidad = item.Cantidad;
                    registro.Unidad = item.Unidad;
                    registro.PU1 = item.PU1;
                    registro.PU2 = item.PU2;
                    registro.Importe1 = item.Importe1;
                    registro.Importe2 = item.Importe2;
                    registro.Tipo = item.Tipo;

                    if (item.Tipo == "R")
                        {
                        Rubros.Add(registro);
                        }
                    else if (item.Tipo == "T")
                        {
                        Tareas.Add(registro);
                        }
                    else if (item.Tipo == "A")
                        {
                        Auxiliares.Add(registro);
                        }
                    }
                if (item.HasItems)
                    {
                    listaRubrosTareasAuxiliares(item.Inferiores);
                    }
                }

            }


        public void NumeraItems(IEnumerable<Nodo> nodos, string prefijo)
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

        public Nodo FindParentNode(IEnumerable<Nodo> items, Nodo inferior, Nodo superior)
            {
            foreach (var item in items)
                {
                if (item.Equals(inferior))
                    {
                    respuesta = superior;

                    }
                if (item.HasItems)
                    {
                    FindParentNode(item.Inferiores, inferior, item);

                    }
                }

            return respuesta;
            }

        public void sinCero()
            {
            foreach (var item in Insumos.ToList())
                {
                if (item.Cantidad == 0)
                    {
                    Insumos.Remove(item);
                    }
                }
            }

        public (Nodo, string) agregaNodo(string tipo, Nodo superior)
            {
            Nodo nuevoNodo = null;
            string mensaje = string.Empty;

            switch (tipo)
                {
                case "R":
                    nuevoNodo = new Nodo
                        {
                        Tipo = "R",
                        Cantidad = 1,
                        Unidad = "Gl",
                        Sup = true,
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    Existe = false;
                    while (verificaCodigo(Arbol, "R" + NumeraRubro.ToString()))
                        {
                        NumeraRubro++;
                        }
                    nuevoNodo.ID = "R" + NumeraRubro.ToString();
                    nuevoNodo.Descripcion = "Rubro " + NumeraRubro.ToString();
                    Arbol.Add(nuevoNodo);
                    mensaje = "Rubro agregado exitosamente.";
                    break;

                case "T":
                    nuevoNodo = new Nodo
                        {
                        Tipo = "T",
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    Existe = false;
                    while (verificaCodigo(Arbol, "T" + NumeraTarea.ToString()))
                        {
                        NumeraTarea++;
                        }
                    nuevoNodo.ID = "T" + NumeraTarea.ToString();
                    nuevoNodo.Descripcion = "Tarea " + NumeraTarea.ToString();
                    superior.Inferiores.Add(nuevoNodo);
                    mensaje = "Tarea agregada exitosamente.";
                    break;

                case "M":
                    nuevoNodo = new Nodo
                        {
                        Tipo = "M",
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    Existe = false;
                    while (verificaCodigo(Arbol, "M" + NumeraMaterial.ToString()))
                        {
                        NumeraMaterial++;
                        }
                    nuevoNodo.ID = "M" + NumeraMaterial.ToString();
                    nuevoNodo.Descripcion = "Material " + NumeraMaterial.ToString();
                    superior.Inferiores.Add(nuevoNodo);
                    mensaje = "Material agregado exitosamente.";
                    break;

                case "D":
                    nuevoNodo = new Nodo
                        {
                        Tipo = "D",
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    Existe = false;
                    while (verificaCodigo(Arbol, "D" + NumeraManodeObra.ToString()))
                        {
                        NumeraManodeObra++;
                        }
                    nuevoNodo.ID = "D" + NumeraManodeObra.ToString();
                    nuevoNodo.Descripcion = "Mano de obra " + NumeraManodeObra.ToString();
                    superior.Inferiores.Add(nuevoNodo);
                    mensaje = "Mano de obra agregada exitosamente.";
                    break;

                case "E":
                    nuevoNodo = new Nodo
                        {
                        Tipo = "E",
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    Existe = false;
                    while (verificaCodigo(Arbol, "E" + NumeraEquipo.ToString()))
                        {
                        NumeraEquipo++;
                        }
                    nuevoNodo.ID = "E" + NumeraEquipo.ToString();
                    nuevoNodo.Descripcion = "Equipo " + NumeraEquipo.ToString();
                    superior.Inferiores.Add(nuevoNodo);
                    mensaje = "Equipo agregado exitosamente.";
                    break;

                case "S":
                    nuevoNodo = new Nodo
                        {
                        Tipo = "S",
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    Existe = false;
                    while (verificaCodigo(Arbol, "S" + NumeraSubcontrato.ToString()))
                        {
                        NumeraSubcontrato++;
                        }
                    nuevoNodo.ID = "S" + NumeraSubcontrato.ToString();
                    nuevoNodo.Descripcion = "Subcontrato " + NumeraSubcontrato.ToString();
                    superior.Inferiores.Add(nuevoNodo);
                    mensaje = "Subcontrato agregado exitosamente.";
                    break;

                case "O":
                    nuevoNodo = new Nodo
                        {
                        Tipo = "O",
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    Existe = false;
                    while (verificaCodigo(Arbol, "O" + NumeraOtro.ToString()))
                        {
                        NumeraOtro++;
                        }
                    nuevoNodo.ID = "O" + NumeraOtro.ToString();
                    nuevoNodo.Descripcion = "Insumo " + NumeraOtro.ToString();
                    superior.Inferiores.Add(nuevoNodo);
                    mensaje = "Insumo agregado exitosamente.";
                    break;

                case "A":
                    nuevoNodo = new Nodo
                        {
                        Tipo = "A",
                        Inferiores = new ObservableCollection<Nodo>()
                        };
                    Existe = false;
                    while (verificaCodigo(Arbol, "A" + NumeraAux.ToString()))
                        {
                        NumeraAux++;
                        }
                    nuevoNodo.ID = "A" + NumeraAux.ToString();
                    nuevoNodo.Descripcion = "Auxiliar " + NumeraAux.ToString();
                    superior.Inferiores.Add(nuevoNodo);
                    mensaje = "Auxiliar agregado exitosamente.";
                    break;

                default:
                    mensaje = "Tipo de nodo no reconocido.";
                    break;
                }

            return (nuevoNodo, mensaje);
            }


        public void aplanar(IEnumerable<Nodo> items, Nodo parentItem)
            {
            foreach (var item in items)
                {
                bool existe = listaConceptosGrabar.Any(a => a.ConceptoID == item.ID);
                if (existe == false)
                    {
                    ConceptoDTO registroC = new ConceptoDTO();
                    registroC.PresupuestoID = encabezado?.ID ?? 0;
                    registroC.ConceptoID = item.ID;
                    registroC.Descrip = item.Descripcion;
                    registroC.Tipo = item.Tipo[0];
                    registroC.PrEjec = item.PU1;
                    registroC.PrVent = item.PU2;
                    registroC.Unidad = item.Unidad;
                    registroC.MesBase = DateTime.Now;
                    listaConceptosGrabar.Add(registroC);
                    }
                RelacionDTO registroR = new RelacionDTO();
                registroR.PresupuestoID = encabezado?.ID ?? 0;
                registroR.CodSup = parentItem == null ? "0" : parentItem.ID;
                registroR.CodInf = item.ID;
                registroR.CanEjec = item.Cantidad;
                registroR.OrdenInt = (short)item.OrdenInt;
                listaRelacionesGrabar.Add(registroR);
                if (item.HasItems == true)
                    {
                    //antes de seguir bajando, verificar si ya existen relaciones anteriores para no agregar.
                    bool anterior = listaRelacionesGrabar.Any(a => a.CodSup == item.ID);
                    if (anterior == false)
                        {
                        aplanar(item.Inferiores, item);
                        }
                    }
                }

            // Mostrar mensaje con las cantidades de registros
            }


        public Nodo clonar(Nodo origen, bool profundidad)
            {
            Nodo respuesta = new Nodo();
            respuesta.ID = origen.ID;
            respuesta.Descripcion = origen.Descripcion;
            respuesta.Unidad = origen.Unidad;
            respuesta.Cantidad = origen.Cantidad;
            respuesta.OrdenInt = origen.OrdenInt;
            Nodo existe = this.Insumos.FirstOrDefault(a => a.ID == origen.ID);
            if (existe != null)
                {
                respuesta.PU1 = existe.PU1;
                respuesta.PU2 = existe.PU2;
                }
            else
                {
                respuesta.PU1 = origen.PU1;
                respuesta.PU2 = origen.PU2;
                }
            respuesta.Tipo = origen.Tipo;
            if (profundidad)
                {
                respuesta.Inferiores = GetClonesInferiores(origen);

                }
            return respuesta;

            }

        public ObservableCollection<Nodo> GetClonesInferiores(Nodo elemento)
            {
            if (elemento == null)
                return null;

            if (!elemento.HasItems)
                {
                ObservableCollection<Nodo> inferioresVacios = new ObservableCollection<Nodo>();
                return inferioresVacios;

                }
            else
                {
                ObservableCollection<Nodo> inferioresLlenos = new ObservableCollection<Nodo>();
                foreach (var item in elemento.Inferiores)
                    {
                    Nodo respuesta = new Nodo();
                    respuesta.ID = item.ID;
                    respuesta.Descripcion = item.Descripcion;
                    respuesta.Unidad = item.Unidad;
                    respuesta.Cantidad = item.Cantidad;
                    Nodo existe = this.Insumos.FirstOrDefault(a => a.ID == item.ID);
                    if (existe != null)
                        {
                        respuesta.PU1 = existe.PU1;
                        respuesta.PU2 = existe.PU2;
                        }
                    else
                        {

                        respuesta.PU1 = item.PU1;
                        respuesta.PU2 = item.PU2;
                        }
                    respuesta.Tipo = item.Tipo;
                    respuesta.Inferiores = GetClonesInferiores(item);
                    inferioresLlenos.Add(respuesta);
                    if (item.HasItems)
                        {
                        respuesta.Inferiores = GetClonesInferiores(item);
                        }

                    }
                return inferioresLlenos;

                }

            }

        public void RestaurarNodo(Nodo nodo, Nodo nodoPadre, int posicion)
            {
            if (nodoPadre == null)
                {
                Arbol.Insert(posicion, nodo);
                }
            else
                {
                nodoPadre.Inferiores.Insert(posicion, nodo);
                }
            }

        public void borraNodo(ObservableCollection<Nodo> collection, Nodo nodoABorrar)
            {
            if (collection.Contains(nodoABorrar))
                {
                collection.Remove(nodoABorrar);
                return;
                }

            foreach (var item in collection)
                {
                if (item.HasItems)
                    {
                    borraNodo(item.Inferiores, nodoABorrar);
                    }
                }
            }

        public Biblioteca.DTO.ProcesaPresupuestoRequest EmpaquetarPresupuesto()
            {
            // 1. Limpiar y aplanar el árbol
            listaConceptosGrabar = new List<ConceptoDTO>();
            listaRelacionesGrabar = new List<RelacionDTO>();
            aplanar(Arbol, null);

            // 2. ACCIONES PARA CONCEPTOS
            foreach (var concepto in listaConceptosLeer)
                {
                bool yaExiste = listaConceptosGrabar.Any(c => c.ConceptoID == concepto.ConceptoID);
                if (!yaExiste)
                    {
                    var conceptoBaja = new ConceptoDTO
                        {
                        PresupuestoID = encabezado?.ID ?? 0,
                        ConceptoID = concepto.ConceptoID,
                        Descrip = concepto.Descrip,
                        Tipo = concepto.Tipo,
                        Unidad = concepto.Unidad,
                        PrEjec = concepto.PrEjec,
                        PrVent = concepto.PrVent,
                        EjecMoneda = concepto.EjecMoneda,
                        VentMoneda = concepto.VentMoneda,
                        MesBase = DateTime.Today,
                        CanTotalEjec = concepto.CanTotalEjec,
                        InsumoID = concepto.InsumoID,
                        Accion = 'B'
                        };
                    listaConceptosGrabar.Add(conceptoBaja);
                    }
                }
            foreach (var concepto in listaConceptosGrabar)
                {
                bool existiaAntes = listaConceptosLeer.Any(c => c.ConceptoID == concepto.ConceptoID);
                if (!existiaAntes)
                    {
                    concepto.Accion = 'A';
                    }
                }
            foreach (var concepto in listaConceptosGrabar)
                {
                if (concepto.Accion != 'B' && concepto.Accion != 'A')
                    {
                    concepto.Accion = 'M';
                    }
                }

            // 3. ACCIONES PARA RELACIONES
            foreach (var relacion in listaRelacionesLeer)
                {
                bool yaExiste = listaRelacionesGrabar.Any(r =>
                    r.CodSup == relacion.CodSup &&
                    r.CodInf == relacion.CodInf);
                if (!yaExiste)
                    {
                    var relacionBaja = new RelacionDTO
                        {
                        PresupuestoID = encabezado?.ID ?? 0,
                        CodSup = relacion.CodSup,
                        CodInf = relacion.CodInf,
                        CanEjec = relacion.CanEjec,
                        CanVenta = relacion.CanVenta,
                        OrdenInt = relacion.OrdenInt,
                        Accion = 'B'
                        };
                    listaRelacionesGrabar.Add(relacionBaja);
                    }
                }
            foreach (var relacion in listaRelacionesGrabar)
                {
                bool existiaAntes = listaRelacionesLeer.Any(r =>
                    r.CodSup == relacion.CodSup &&
                    r.CodInf == relacion.CodInf);
                if (!existiaAntes)
                    {
                    relacion.Accion = 'A';
                    }
                }
            foreach (var relacion in listaRelacionesGrabar)
                {
                if (relacion.Accion != 'B' && relacion.Accion != 'A')
                    {
                    relacion.Accion = 'M';
                    }
                }

            // 4. Clonar el encabezado (PresupuestoDTO) y asignar fecha actual a todas las fechas
            var fechaActual = DateTime.Today;
            var encabezadoEmpaquetado = new PresupuestoDTO
                {
                ID = encabezado.ID ?? 0,
                CuentaID = encabezado?.CuentaID,
                UsuarioID = encabezado?.UsuarioID ?? 0,
                Descrip = encabezado?.Descrip,
                PrEjecTotal = Arbol.Sum(i => i.Importe1),
                PrEjecDirecto = encabezado?.PrEjecDirecto ?? 0,
                EjecMoneda = encabezado?.EjecMoneda ?? 'P',
                PrVentaTotal = encabezado?.PrVentaTotal ?? 0,
                PrVentaDirecto = encabezado?.PrVentaDirecto ?? 0,
                VentaMoneda = encabezado?.VentaMoneda ?? 'P',
                Superficie = encabezado?.Superficie,
                MesBase = fechaActual,
                FechaC = fechaActual,
                FechaM = fechaActual,
                EsModelo = encabezado?.EsModelo ?? false,
                TipoCambioD = encabezado?.TipoCambioD ?? 0
                };
            if (encabezado == null)
                encabezadoEmpaquetado.ID = null;

            // 5. Asignar fecha actual a todos los conceptos
            foreach (var concepto in listaConceptosGrabar)
                {
                concepto.MesBase = fechaActual;
                }

            // 6. Empaquetar el request
            var request = new Biblioteca.DTO.ProcesaPresupuestoRequest
                {
                Presupuesto = encabezadoEmpaquetado,
                Conceptos = listaConceptosGrabar,
                Relaciones = listaRelacionesGrabar
                };


            return request;
            }








        }

    }