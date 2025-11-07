using Syncfusion.UI.Xaml.TreeGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DOP
    {
    public class SelectorClass : StyleSelector
        {
        public override Style SelectStyle(object item, DependencyObject container)
            {
            var cell = container as TreeGridCell;
            var columnBase = cell.ColumnBase;
            // Use reflection to get the DataRow property from ColumnBase
            var dataRowProperty = columnBase.GetType().GetProperty("DataRow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var dataRow = dataRowProperty?.GetValue(columnBase);

            var iseSelectedRow = (dataRow as TreeDataRow).IsSelectedRow;
            var hasEditing = columnBase.TreeGridColumn.AllowEditing;
            if (iseSelectedRow)
                {
                return App.Current.Resources["Selected"] as Style;
                }

            if (!hasEditing)
                return App.Current.Resources["NoEditable"] as Style;

            return base.SelectStyle(item, container);
            }
        }
    }

