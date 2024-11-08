using Bibioteca.Clases;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Presupuestos
{
    
        public class IconTemplateSelector : DataTemplateSelector
        {
            public DataTemplate RubroTemplate { get; set; }
            public DataTemplate TareaTemplate { get; set; }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                var nodo = item as Nodo;
                if (nodo != null)
                {
                    switch (nodo.Tipo)
                    {
                        case "R":
                            return RubroTemplate;
                        case "T":
                            return TareaTemplate;
                    }
                }
                return base.SelectTemplate(item, container);
            }
        }
  

}
