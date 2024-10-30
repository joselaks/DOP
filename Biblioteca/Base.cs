using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibioteca.Clases
{
    public class Concepto
    {
        private string codigo;
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string descrip;
        public string Descrip
        {
            get { return descrip; }
            set { descrip = value; }
        }

        private decimal precio;
        public decimal Precio
        {
            get { return precio; }
            set { precio = value; }
        }

        private string tipo;
        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private string unidad;
        public string Unidad
        {
            get { return unidad; }
            set { unidad = value; }
        }

        private DateTime fecha;
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }

    }

    public class Relacion
    {
        private int ordenInt;
        public int OrdenInt
        {
            get { return ordenInt; }
            set { ordenInt = value; }
        }

        private string superior;
        public string Superior
        {
            get { return superior; }
            set { superior = value; }
        }

        private string inferior;
        public string Inferior
        {
            get { return inferior; }
            set { inferior = value; }
        }

        private string descrip;
        public string Descrip
        {
            get { return descrip; }
            set { descrip = value; }
        }

        private decimal cantidad;
        public decimal Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }

    }
}
