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
        public int UsuarioID;

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

        public Maestro(List<ConceptoMDTO> conBase, List<RelacionMDTO> relBase, int usuarioID)
            {
            if (conBase == null)
                {
                listaConceptosLeer = new List<ConceptoMDTO>();
                listaRelacionesLeer = new List<RelacionMDTO>();
                Arbol = new ObservableCollection<Nodo>();

                }
            else
                {
                this.listaConceptosLeer = conBase;
                this.listaRelacionesLeer = relBase;
                generaPresupuesto();
                }

            this.listaConceptosGrabar = new List<ConceptoMDTO>();
            this.listaRelacionesGrabar = new List<RelacionMDTO>();
            UsuarioID = usuarioID;

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

        public void aplanar(IEnumerable<Nodo> items, Nodo parentItem)
            {
            foreach (var item in items)
                {
                bool existe = listaConceptosGrabar.Any(a => a.ConceptoID == item.ID);
                if (existe == false)
                    {
                    ConceptoMDTO registroC = new ConceptoMDTO();
                    registroC.UsuarioID = UsuarioID;
                    registroC.ConceptoID = item.ID;
                    registroC.Descrip = item.Descripcion;
                    registroC.Tipo = item.Tipo[0];
                    registroC.PrEjec = item.PU1;
                         registroC.Unidad = item.Unidad;
                    registroC.MesBase = DateTime.Now;
                    listaConceptosGrabar.Add(registroC);
                    }
                RelacionMDTO registroR = new RelacionMDTO();
                registroR.UsuarioID = UsuarioID;
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

        public Biblioteca.DTO.ProcesaTareaMaestroRequest EmpaquetarPresupuesto()
            {
            // 1. Limpiar y aplanar el árbol
            listaConceptosGrabar = new List<ConceptoMDTO>();
            listaRelacionesGrabar = new List<RelacionMDTO>();
            aplanar(Arbol, null);

            // 2. ACCIONES PARA CONCEPTOS
            foreach (var concepto in listaConceptosLeer)
                {
                bool yaExiste = listaConceptosGrabar.Any(c => c.ConceptoID == concepto.ConceptoID);
                if (!yaExiste)
                    {
                    var conceptoBaja = new ConceptoMDTO
                        {
                        UsuarioID = concepto.UsuarioID,
                        ConceptoID = concepto.ConceptoID,
                        Descrip = concepto.Descrip,
                        Tipo = concepto.Tipo,
                        Unidad = concepto.Unidad,
                        PrEjec = concepto.PrEjec,
                        EjecMoneda = concepto.EjecMoneda,
                        MesBase = DateTime.Today,
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
                    var relacionBaja = new RelacionMDTO
                        {
                        UsuarioID  = relacion.UsuarioID,
                        CodSup = relacion.CodSup,
                        CodInf = relacion.CodInf,
                        CanEjec = relacion.CanEjec,
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

            // Empaquetar el request
            var request = new Biblioteca.DTO.ProcesaTareaMaestroRequest
                {
                Conceptos = listaConceptosGrabar,
                Relaciones = listaRelacionesGrabar
                };


            return request;
            }

        public Nodo clonar(Nodo origen)
            {
            Nodo respuesta = new Nodo();
            respuesta.ID = origen.ID;
            respuesta.Descripcion = origen.Descripcion;
            respuesta.Unidad = origen.Unidad;
            respuesta.Cantidad = origen.Cantidad;
            respuesta.PU1 = origen.PU1;
            respuesta.Tipo = origen.Tipo;
            respuesta.Inferiores = GetClonesInferiores(origen);
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
                    respuesta.PU1 = item.PU1;
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

        }

    }