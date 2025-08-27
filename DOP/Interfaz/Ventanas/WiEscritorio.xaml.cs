using DataObra.Interfaz.Componentes;
using DOP.Interfaz.Ventanas;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Tools.Controls;


using System.Windows;

namespace DataObra.Interfaz.Ventanas
{
    public partial class WiEscritorio : Window
    {
        private nMaestro normalMaestro;
        private mMaestro miniMaestro;
        private xMaestro expanMaestro;


        public WiEscritorio()
        {
            InitializeComponent();

            this.Loaded += WiEscritorio_Loaded;
        }


        private void WiEscritorio_Loaded(object sender, RoutedEventArgs e)
        {
            tile1.Content = new nMaestro(this);                // Normal
            tile1.MaximizedItemContent = new xMaestro(this);   // Maximizado
            tile1.MinimizedItemContent = new mMaestro(this);   // Minimizado

        }




    }




}
