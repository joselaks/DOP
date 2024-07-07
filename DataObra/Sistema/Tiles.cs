using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Sistema
{
    public class Tiles
    {
        private string[] listaAgrupadores;
        public string[] ListaAgrupadores
        {
            get
            {
                return listaAgrupadores;
            }
            set
            {
                listaAgrupadores = value;
                //NotifyPropertyChanged("Tags");
            }
        }


        private string[] listaString;

        public string[] ListaString
        {
            get
            {
                return listaString;
            }
            set
            {
                listaString = value;
                //NotifyPropertyChanged("Tags");
            }
        }

        private string[,] lista2String;

        public string[,] Lista2String
        {
            get
            {
                return lista2String;
            }
            set
            {
                lista2String = value;
                //NotifyPropertyChanged("Tags");
            }
        }
    }
}
