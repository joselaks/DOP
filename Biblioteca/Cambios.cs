using Bibioteca.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca
    {
    public class Cambios
        {
        public string TipoCambio { get; set; }
        public Nodo antesCambio { get; set; }
        public Nodo despuesCambio { get; set; }
        public string PropiedadCambiada { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public Nodo NodoPadre { get; set; }
        public Nodo NodoMovido { get; set; }
        public Nodo NodoPadreNuevo { get; set; }
        public Nodo NodoPadreAnterior { get; set; }
        public int Posicion { get; set; }
        }
    }
