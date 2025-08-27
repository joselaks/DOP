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

        private nModelos normalModelo;
        private mModelos miniModelo;
        private xModelos expanModelo;


        public WiEscritorio()
        {
            InitializeComponent();

            // Instanciación de los campos
            normalMaestro = new nMaestro(this);
            miniMaestro = new mMaestro(this);
            expanMaestro = new xMaestro(this);

            normalModelo = new nModelos();
            miniModelo = new mModelos();
            expanModelo = new xModelos();

            this.Loaded += WiEscritorio_Loaded;
        }


        private void WiEscritorio_Loaded(object sender, RoutedEventArgs e)
        {
            // Crear el primer TileViewItem y guardar la referencia
            var tMaestro = new TileViewItem()
            {
                Content = normalMaestro,
            };

            // Asignar los contenidos para los estados maximizado y minimizado
            tMaestro.MaximizedItemContent = expanMaestro;   // Maximizado
            tMaestro.MinimizedItemContent = miniMaestro;   // Minimizado

            // Agregar el item al control
            tileViewControl.Items.Add(tMaestro);

            // Crear el primer TileViewItem y guardar la referencia
            var tModelo = new TileViewItem()
                {
                Content = normalModelo,
                };

            // Asignar los contenidos para los estados maximizado y minimizado
            tModelo.MaximizedItemContent = expanModelo;   // Maximizado
            tModelo.MinimizedItemContent = miniModelo;   // Minimizado

            // Agregar el item al control
            tileViewControl.Items.Add(tModelo);

            }

        private void tileViewControl1_MaximizedItemChanged(object sender, Syncfusion.Windows.Shared.TileViewEventArgs args)
            {
            TileViewControl? tileView = sender as TileViewControl;





           






        }




}
