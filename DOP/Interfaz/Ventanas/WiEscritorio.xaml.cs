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
        //Presupuesto
        private nPresupuestos normalPresupuesto;
        private mPresupuestos miniPresupuesto;
        private xPresupuestos expanPresupuesto;

        //Precios
        private nPrecios normalPrecios;
        private mPrecios miniPrecios;
        private xPrecios expanPrecios;

        //Maestro
        private nMaestro normalMaestro;
        private mMaestro miniMaestro;
        private xMaestro expanMaestro;

        //Modelos
        private nModelos normalModelo;
        private mModelos miniModelo;
        private xModelos expanModelo;


        public WiEscritorio()
        {
            InitializeComponent();

            // Instanciación de los campos

            // Presupuesto
            normalPresupuesto = new nPresupuestos(this);
            miniPresupuesto = new mPresupuestos(this);
            expanPresupuesto = new xPresupuestos(this);

            // Precios
            normalPrecios = new nPrecios(this);
            miniPrecios = new mPrecios(this);
            expanPrecios = new xPrecios(this);

            // Maestro
            normalMaestro = new nMaestro(this);
            miniMaestro = new mMaestro(this);
            expanMaestro = new xMaestro(this);

            // Modelos
            normalModelo = new nModelos(this);
            miniModelo = new mModelos(this);
            expanModelo = new xModelos(this);

            this.Loaded += WiEscritorio_Loaded;
        }


        private void WiEscritorio_Loaded(object sender, RoutedEventArgs e)
        {
            AgregarTile(normalMaestro, expanMaestro, miniMaestro);
            AgregarTile(normalModelo, expanModelo, miniModelo);
            AgregarTile(normalPresupuesto, expanPresupuesto, miniPresupuesto);
            AgregarTile(normalPrecios, expanPrecios, miniPrecios);



        }

        private void AgregarTile(object contenidoNormal, object contenidoMax, object contenidoMin)
        {
            var tile = new TileViewItem
            {
                Content = contenidoNormal,
                MaximizedItemContent = contenidoMax,
                MinimizedItemContent = contenidoMin
            };
            tileEscritorio.Items.Add(tile);
        }

        public void CambioEstado(string nombreTile, string estado)
        {
            foreach (var item in tileEscritorio.Items)
            {
                // Obtener el nombre del tipo del contenido
                var contenido = (item as TileViewItem)?.Content ?? item;
                string nombreContenido = contenido.GetType().Name;

                if (nombreContenido.Equals(nombreTile, StringComparison.OrdinalIgnoreCase))
                {
                    var container = tileEscritorio.ItemContainerGenerator.ContainerFromItem(item) as TileViewItem;
                    if (container != null)
                    {
                        switch (estado)
                        {
                            case "Normal":
                                container.TileViewItemState = Syncfusion.Windows.Shared.TileViewItemState.Normal;
                                break;
                            case "Maximizado":
                                container.TileViewItemState = Syncfusion.Windows.Shared.TileViewItemState.Maximized;
                                break;
                            case "Minimizado":
                                container.TileViewItemState = Syncfusion.Windows.Shared.TileViewItemState.Minimized;
                                break;
                        }
                    }
                    break; // Salir tras encontrar el tile correcto
                }
            }
        }



    }




}
