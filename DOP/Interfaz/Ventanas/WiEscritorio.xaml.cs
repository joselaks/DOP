using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Windows;

namespace DataObra.Interfaz.Ventanas
{
    public partial class WiEscritorio : Window
    {
        public WiEscritorio()
        {
            InitializeComponent();
        }

        

        private void tileView_MaximizedItemChanged(object sender, TileViewEventArgs args)
        {
            TileViewControl? tileView = sender as TileViewControl;
            TileViewItem maximizedItem = (TileViewItem)args.Source;

            foreach (var item in tSelector.Items)
                {
                var container = tSelector.ItemContainerGenerator.ContainerFromItem(item) as TileViewItem;
                if (container != null)
                    {
                    container.HeaderVisibility = (item == maximizedItem)
                        ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
    }
}
