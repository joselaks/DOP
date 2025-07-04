﻿using DataObra.Agrupadores;
using DataObra.Agrupadores.Clases;
using DataObra.Base.Controles;
using DataObra.Documentos;
using DataObra.Insumos;
using DataObra.Sistema;
using DataObra.Sur;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biblioteca;
using DataObra.Datos;
using DataObra.Presupuestos;
using System.Windows.Automation;
using Syncfusion.Windows.Tools;

namespace DataObra
{
    public partial class Inicio : Window
    {
        // Listados a descargar al iniciar DO de la Cuenta.

        ObservableCollection<SurUsuario> Usuarios = new ObservableCollection<SurUsuario>();
        public string Solapa;

        NavAgrupador navAgrupador;
        NavDocumento navDocumentos;
        NavInsumo navInsumos;


        ////NavTareas navTareas;

        public Inicio()
        {
            InitializeComponent();

            Solapa = "Inicio";

            GrupoAgrupadores();
            GrupoDocumentos();
            GrupoInsumos();

            DiagramaDocs diagPrincipal = new DiagramaDocs();

            this.GrillaDiagramaDocs.Children.Add(diagPrincipal);

            DiagramaInsumos diagInsumos = new DiagramaInsumos();
            this.GrillaDiagramaInsumos.Children.Add(diagInsumos);

            this.TabPrincipal.SelectedIndex = 0;

            VerificaUsuario_Click(null, null);
        }

        private async void VerificaUsuario_Click(object sender, RoutedEventArgs e)
        {
            #region Datos para testeo
            string email = "jose@dataobra.com"; // Email a validar
            string pass = "contra"; // Contraseña
            #endregion

            //// Codigo a utilizar
            //var respuesta = await ConsultasAPI.ValidarUsuarioAsync(email, pass);

            //// Respuestas
            //Usuario datosusuario = respuesta.Usuario;
            //bool conexionExitosa = respuesta.Success;
            //string mensaje = respuesta.Message;

            //// Mensaje para testeo
            //if (respuesta.Usuario != null)
            //{
            //    MessageBox.Show(respuesta.Usuario.Nombre + " " + respuesta.Usuario.Apellido);
            //}
        }

        #region Tiles
        private void GrupoAgrupadores()
        {
            Tiles nueva = new Tiles();
            nueva.ListaString = new string[] { "Obras", "Admin", "Clientes", "Proveedores", "Contratistas", "Obreros", "Cuentas", "Impuestos", "Depositos", "Temas" };

            foreach (string item in nueva.ListaString)
            {
                TilesAgrupadores.Items.Add(CrearTile(item, "Novedades sobre " + item + ".", "Naranja"));
            }
        }

        private void GrupoDocumentos()
        {
            Tiles nueva = new Tiles();
            nueva.ListaString = new string[] { "Precostos", "Presupuestos", "Pedidos", "Planes", "Certificados", "Partes", "Facturas", "Compras", "Remitos", "Pagos", "Cobros", "Anticipos", "Contratos", "Sueldos", "Gastos", "Acopios" };

            foreach (var item in nueva.ListaString)
            {
                TilesDocumentos.Items.Add(CrearTile(item, "Novedades sobre " + item + ".", "Turco"));
            }
        }

        private void GrupoInsumos()
        {
            Tiles nueva = new Tiles();
            nueva.ListaString = new string[] { "Materiales", "ManoDeObra", "Equipos", "SubContratos", "Otros", "Auxiliares", "Tareas", "Rubros", "Indices" };

            foreach (var item in nueva.ListaString)
            {
                TilesInsumos.Items.Add(CrearTile(item, "Novedades sobre " + item + ".", "Verde"));
            }
        }

        private TileViewItem CrearTile(string pNombre, string pDescrip, string pColor)
        {
            TileViewItem nuevo = new TileViewItem();

            TileChico contenido = new TileChico(pNombre, pDescrip);
            nuevo.Content = contenido;

            nuevo.Name = pNombre;
            nuevo.MinMaxButtonVisibility = Visibility.Collapsed;
            nuevo.HeaderVisibility = Visibility.Collapsed;
            nuevo.MouseLeftButtonUp += ItemTile_Click;

            switch (pColor)
            {
                case "Naranja":
                    nuevo.Background = new SolidColorBrush(Colors.DarkOrange);
                    break;
                case "Violeta":
                    nuevo.Background = new SolidColorBrush(Colors.Violet);
                    break;
                case "Verde":
                    nuevo.Background = new SolidColorBrush(Colors.Green);
                    break;
                case "Azul":
                    nuevo.Background = new SolidColorBrush(Colors.Blue);
                    break;
                case "Turqueza":
                default:
                    nuevo.Background = new SolidColorBrush(Colors.Turquoise);
                    break;
            }

            return nuevo;
        }

        #endregion

        #region Clicks
        private void ItemTile_Click(object sender, MouseButtonEventArgs e)
        {
            TileViewItem tileSele = sender as TileViewItem;
            bool abierto = false;

            if (tileSele != null)
            {
                switch (Solapa)
                {
                    case "Agrupadores":
                        foreach (var item in TabAgrupadores.Items)
                        {
                            var seleTab = item as TabItemExt;

                            if (seleTab.Header != null && seleTab.Header.ToString() == tileSele.Name)
                            {
                                abierto = true;
                                this.TabAgrupadores.SelectedItem = seleTab;
                            }
                        }
                        break;
                    case "Documentos":
                        foreach (var item in TabDocumentos.Items)
                        {
                            var seleTab = item as TabItemExt;

                            if (seleTab.Header != null && seleTab.Header.ToString() == tileSele.Name)
                            {
                                abierto = true;
                                this.TabDocumentos.SelectedItem = seleTab;
                            }
                        }
                        break;
                    case "Insumos":
                        foreach (var item in TabInsumos.Items)
                        {
                            var seleTab = item as TabItemExt;

                            if (seleTab.Header != null && seleTab.Header.ToString() == tileSele.Name)
                            {
                                abierto = true;
                                this.TabInsumos.SelectedItem = seleTab;
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (!abierto)
                {
                    Grid nuevaGrilla = new Grid();
                    TabItemExt nuevoTab = new TabItemExt();
                    nuevoTab.Header = tileSele.Name;
                    nuevoTab.AllowPin = true;
                    nuevoTab.ShowPin = true;

                    switch (Solapa)
                    {
                        case "Agrupadores":
                            navAgrupador = new NavAgrupador(tileSele.Name, nuevoTab, TabAgrupadores);
                            nuevaGrilla.Children.Add(navAgrupador);
                            break;
                        case "Documentos":
                            navDocumentos = new NavDocumento("", tileSele.Name);
                            nuevaGrilla.Children.Add(navDocumentos);
                            break;
                        case "Insumos":
                            navInsumos = new NavInsumo(tileSele.Name);
                            nuevaGrilla.Children.Add(navInsumos);
                            break;
                        default:
                            break;
                    }


                    nuevoTab.Content = nuevaGrilla;

                    switch (Solapa)
                    {
                        case "Agrupadores":
                            TabAgrupadores.Items.Add(nuevoTab);
                            var cant1 = TabAgrupadores.Items.Count;
                            this.TabAgrupadores.SelectedIndex = cant1 - 1;
                            break;
                        case "Documentos":
                            TabDocumentos.Items.Add(nuevoTab);
                            var cant2 = TabDocumentos.Items.Count;
                            this.TabDocumentos.SelectedIndex = cant2 - 1;
                            break;
                        case "Insumos":
                            TabInsumos.Items.Add(nuevoTab);
                            var cant3 = TabInsumos.Items.Count;
                            this.TabInsumos.SelectedIndex = cant3 - 1;
                            break;
                    }
                }
            }
        }

        private void TabAgrupadorNuevo_Click(object sender, EventArgs e)
        {
            foreach (var item in TabAgrupadores.Items)
            {
                var seleTab = item as TabItemExt;

                if (seleTab.Header.ToString() == ".")
                {
                    this.TabAgrupadores.SelectedItem = seleTab;
                    break;
                }
            }
        }

        private void TabDocumentoNuevo_Click(object sender, EventArgs e)
        {
            foreach (var item in TabDocumentos.Items)
            {
                var seleTab = item as TabItemExt;

                if (seleTab.Header.ToString() == ".")
                {
                    this.TabDocumentos.SelectedItem = seleTab;
                    break;
                }
            }
        }

        private void TabInsumoNuevo_Click(object sender, EventArgs e)
        {
            foreach (var item in TabInsumos.Items)
            {
                var seleTab = item as TabItemExt;

                if (seleTab.Header.ToString() == ".")
                {
                    this.TabInsumos.SelectedItem = seleTab;
                    break;
                }
            }
        }

        private void TabPrincipal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItemExt sele = TabPrincipal.SelectedItem as TabItemExt;

            if (sele != null)
                Solapa = sele.Header.ToString();
        }

        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DataObra.Sistema.Controles.Roles pantallaRoles = new Sistema.Controles.Roles();
            pantallaRoles.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Login pantallaLogin = new Login();
            pantallaLogin.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            int seleID = 19;

            VenDocumento ventanaDoc = new VenDocumento("Facturas", seleID);
            ventanaDoc.Show();
        }

        private void vConect_Click(object sender, RoutedEventArgs e)
        {
            Datos.Conectores v1 = new Conectores();
            v1.Show();
        }

        private void vPres_Click(object sender, RoutedEventArgs e)
        {
        //    VenPresupuesto ventanaPres = new VenPresupuesto();
        //    ventanaPres.Owner = this;
        //    ventanaPres.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //    ventanaPres.Show();
        }

        private void limpiaLogin_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_Principal(object sender, RoutedEventArgs e)
        {
            DataObra.Sistema.Principal ventana = new Principal();

            ventana.Show();
        }

        private void Button_Click_Roles(object sender, RoutedEventArgs e)
        {
            DataObra.Sistema.SeleccionRol ventana = new SeleccionRol();

            ventana.Show();
        }

        //private void BarraEstado_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button botonPresionado)
        //    {
        //        string contenido = botonPresionado.Content.ToString();

        //        if (contenido != null)
        //        {
        //            switch (contenido)
        //            {
        //                case "Btn":
        //                    SeleccionRol seleccionRolWindow = new SeleccionRol
        //                    {
        //                        //ParentWindow = this
        //                    };
        //                    seleccionRolWindow.ShowDialog();
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //}

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            SeleccionRol seleccionRolWindow = new SeleccionRol
            {
                //ParentWindow = this
            };
            seleccionRolWindow.ShowDialog();
        }
        private void BtnConexiones_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) { popupConexiones.IsOpen = true; }
        private void BtnConexiones_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) { if (!popupConexiones.IsMouseOver) { popupConexiones.IsOpen = false; } }
        private void popupConexiones_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) { if (!BtnConexiones.IsMouseOver) { popupConexiones.IsOpen = false; } }
    }
}
