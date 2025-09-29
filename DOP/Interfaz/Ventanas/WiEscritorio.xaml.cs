using DataObra.Interfaz.Componentes;
using DOP.Interfaz.Ventanas;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Tools.Controls;


using System.Windows;
using Biblioteca.DTO;
using System.ComponentModel;
using DOP.Datos;
using DOP;

namespace DataObra.Interfaz.Ventanas
{
    public partial class WiEscritorio : Window
    {
        //Presupuesto
        private nPresupuestos normalPresupuesto;
        private mPresupuestos miniPresupuesto;
        private xPresupuestos expanPresupuesto;

        //Gastos
        private nGastos normalGastos;
        private mGastos miniGastos;
        private xGastos expanGastos;


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



        //Presupuestos y modelos
        public ObservableCollection<PresupuestoDTO> _presupuestos = new();
        public ObservableCollection<PresupuestoDTO> _modelos = new();
        public ObservableCollection<PresupuestoDTO> _modelosPropios = new();
        public ObservableCollection<ListaArticuloItem> InfoCombo { get; set; } = new();
        public ObservableCollection<ArticuloDTO> ArticulosLista { get; set; } = new();
        public ObservableCollection<ArticuloBusquedaDTO> ArticulosBusqueda { get; set; } = new();
        public event PropertyChangedEventHandler PropertyChanged;
        public List<ArticuloExceDTO> articulosImportados = new();
        public bool HayArticulosImportados => articulosImportados != null && articulosImportados.Count > 0;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WiEscritorio()
        {
            InitializeComponent();
            this.Loaded += WiEscritorio_Loaded;
        }


        private async void WiEscritorio_Loaded(object sender, RoutedEventArgs e)
        {

            // 1. Presupuestos
            var (success, message, lista) = await DatosWeb.ObtenerPresupuestosUsuarioAsync();
            if (success)
            {
                // Calcular ValorM2 para cada presupuesto
                foreach (var p in lista)
                {
                    if (p.Superficie.HasValue && p.Superficie.Value > 0)
                        p.ValorM2 = p.PrEjecTotal / p.Superficie.Value;
                    else
                        p.ValorM2 = 0;
                }

                _presupuestos = new ObservableCollection<PresupuestoDTO>(lista.Where(p => p.UsuarioID == App.IdUsuario));
                _modelos = new ObservableCollection<PresupuestoDTO>(lista.Where(p => p.EsModelo && p.UsuarioID == 4));
                _modelosPropios = new ObservableCollection<PresupuestoDTO>(lista.Where(p => p.EsModelo && p.UsuarioID == App.IdUsuario));
                InstanciaComponentes();



            }
            else
            {
                MessageBox.Show($"No se pudieron cargar los presupuestos.\n{message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // 2. Listas de artículos
            InfoCombo.Clear();
            int usuarioID = 4;
            var (okListas, msgListas, listas) = await DOP.Datos.DatosWeb.ObtenerListasArticulosPorUsuarioAsync(usuarioID);

            if (!okListas)
            {
                MessageBox.Show($"No se pudo obtener las listas: {msgListas}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var listaItem in listas)
            {
                InfoCombo.Add(new ListaArticuloItem
                {
                    ID = listaItem.ID,
                    Descrip = listaItem.Descrip,
                    TipoID = listaItem.TipoID
                });
            }



        }

        private void InstanciaComponentes()
        {
            // Instanciación de los campos

            // Presupuesto
            normalPresupuesto = new nPresupuestos(this);
            miniPresupuesto = new mPresupuestos(this);
            expanPresupuesto = new xPresupuestos(this);

            // Gastos
            normalGastos = new nGastos(this);
            miniGastos = new mGastos(this);
            expanGastos = new xGastos(this);


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

            AgregarTileOperativo( expanPresupuesto, normalPresupuesto, miniPresupuesto);
            AgregarTileOperativo(normalGastos, expanGastos, miniGastos);

            AgregarTileMercado(normalModelo, expanModelo, miniModelo);
            AgregarTileMercado(normalMaestro, expanMaestro, miniMaestro);
            AgregarTileMercado(normalPrecios, expanPrecios, miniPrecios);

        }

        private void AgregarTileMercado(object contenidoNormal, object contenidoMax, object contenidoMin)
        {
            var tile = new TileViewItem
            {
                Content = contenidoNormal,
                MaximizedItemContent = contenidoMax,
                MinimizedItemContent = contenidoMin
            };
            tileEscritorioMercado.Items.Add(tile);
        }

        private void AgregarTileOperativo(object contenidoNormal, object contenidoMax, object contenidoMin)
            {
            var tile = new TileViewItem
                {
                Content = contenidoNormal,
                MaximizedItemContent = contenidoMax,
                MinimizedItemContent = contenidoMin
                };
            tileEscritorioOperativo.Items.Add(tile);
            }

        public void CambioEstado(string nombreTile, string estado, string contenedor)
            {
            TileViewControl tileView = null;
            if (contenedor == "M")
                tileView = tileEscritorioMercado;
            else if (contenedor == "O")
                tileView = tileEscritorioOperativo;
            else
                return;

            foreach (var item in tileView.Items)
                {
                // Obtener el nombre del tipo del contenido
                var contenido = (item as TileViewItem)?.Content ?? item;
                string nombreContenido = contenido.GetType().Name;
                if (estado == "Normal")
                    {
                    foreach (var it in tileView.Items)
                        {
                        var cont = (it as TileViewItem)?.Content ?? it;
                        string nomCont = cont.GetType().Name;
                        if (nomCont != nombreTile)
                            {
                            var container = tileView.ItemContainerGenerator.ContainerFromItem(it) as TileViewItem;
                            if (container != null)
                                container.TileViewItemState = Syncfusion.Windows.Shared.TileViewItemState.Normal;
                            }
                        }
                    return;
                    }

                if (nombreContenido.Equals(nombreTile, StringComparison.OrdinalIgnoreCase))
                    {
                    var container = tileView.ItemContainerGenerator.ContainerFromItem(item) as TileViewItem;
                    if (container != null)
                        {
                        switch (estado)
                            {
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
