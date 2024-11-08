using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataObra.Presupuestos
{
    public class ViewModel
    {
        public ViewModel()
        {
            this.ComboBoxItems = new ObservableCollection<ComboBoxItem>()
            {
                new ComboBoxItem() { DisplayName = "Rubro", IconPath = "https://th.bing.com/th/id/OIP.B8u7c37kE5ZdM5ZCRm-4hwHaFF?w=234&h=180&c=7&r=0&o=5&dpr=1.3&pid=1.7", Value = "R" },
                new ComboBoxItem() { DisplayName = "Tarea", IconPath = "https://th.bing.com/th/id/OIP.HxV79tFMPfBAIo0BBF-sOgHaEy?rs=1&pid=ImgDetMain", Value = "T" }
            };

        }
        ObservableCollection<ComboBoxItem> items;
        public ObservableCollection<ComboBoxItem> ComboBoxItems
        {
            get { return items; }
            set { items = value; }
        }
    }
    public class ComboBoxItem
    {
        public string DisplayName { get; set; } // Name to display in the combo
        public string IconPath { get; set; }  // Icon path
        public string Value { get; set; } // Alphanumeric value containing the column
    }


}
