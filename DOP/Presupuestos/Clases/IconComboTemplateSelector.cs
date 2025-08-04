using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DOP.Presupuestos.Clases
{
    public class IconComboTemplateSelector : DataTemplateSelector
        {
        public DataTemplate IconComboEditTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
            return IconComboEditTemplate;
            }
        }
    }
