using Bibioteca.Clases;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Presupuestos
{

    public class IconTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RubTemplate { get; set; }
        public DataTemplate TarTemplate { get; set; }
        public DataTemplate MatTemplate { get; set; }
        public DataTemplate MdoTemplate { get; set; }
        public DataTemplate SubTemplate { get; set; }
        public DataTemplate EquTemplate { get; set; }
        public DataTemplate OtrTemplate { get; set; }
        public DataTemplate AuxTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var nodo = item as Nodo;
            if (nodo != null)
            {
                switch (nodo.Tipo)
                {
                    case "R":
                        return RubTemplate;
                    case "T":
                        return TarTemplate;
                    case "M":
                        return MatTemplate;
                    case "D":
                        return MdoTemplate;
                    case "E":
                        return EquTemplate;
                    case "S":
                        return SubTemplate;
                    case "O":
                        return OtrTemplate;
                    case "A":
                        return AuxTemplate;
                }
            }

            return base.SelectTemplate(item, container);
           
        }


    }

}