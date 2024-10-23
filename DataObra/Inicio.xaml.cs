using DataObra.Agrupadores;
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

namespace DataObra
{
    public partial class Inicio : Window
    {
        // Listados a descargar al iniciar DO de la Cuenta

        ObservableCollection<SurUsuario> Usuarios = new ObservableCollection<SurUsuario>();
        public string Solapa;

        NavAgrupador navAgrupador;
        NavDocumento navDocumentos;
        NavInsumo navInsumos;
        DatosWeb datosWeb;
        ////NavTareas navTareas;

        public Inicio()
        {
            InitializeComponent();
            datosWeb = new DatosWeb();
            Solapa = "Inicio";

            GrupoAgrupadores();

            GrupoDocumentos();
            GrupoInsumos();

            TileChico tc = new TileChico("Este es el titulo", "La descripción mas detallada del tile");
            Tile4.Content = tc;

            DiagramaDocs diagPrincipal = new DiagramaDocs();

            this.GrillaDiagramaDocs.Children.Add(diagPrincipal);

            DiagramaInsumos diagInsumos = new DiagramaInsumos();
            this.GrillaDiagramaInsumos.Children.Add(diagInsumos);

            this.TabPrincipal.SelectedIndex = 0;
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

        #region Pruebas
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Prueba.ColumnCount = 6;
            Prueba.RowCount = 6;

        }
        #endregion

        private void TabAgrupadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PantallaBase pantallaBase = new PantallaBase();
            pantallaBase.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Login pantallaLogin = new Login();
            pantallaLogin.Show();
        }

        


            private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            VenDocumento ventanaDoc = new VenDocumento("Facturas");
            ventanaDoc.Show();
        }

        // Botón de verificacion del usuario
        private async void conectaUsuario_Click(object sender, RoutedEventArgs e)
        {
            string email = "jose@dataobra.com"; // Email a validar
            string pass = "contra"; // Contraseña

            var (success, message, usuario) = await datosWeb.ValidarUsuarioAsync(email, pass);

            if (success)
            {
                MessageBox.Show(message, "Éxito");
                // Aquí puedes mostrar los detalles del usuario en la interfaz de usuario
                // Por ejemplo:
                // textBoxNombre.Text = usuario.Nombre;
            }
            else
            {
                MessageBox.Show(message, "Error");
            }

        }

        // Boton de buscar los documentos de la cuenta
        private async void BuscaDocumento(object sender, RoutedEventArgs e)
        {
            short cuentaID = 5; // ID de la cuenta a consultar
            var (success, message, documentos) = await datosWeb.GetDocumentosPorCuentaIDAsync(cuentaID);

            if (success)
            {
                MessageBox.Show(message, "Éxito");
                // Aquí se puede obtener la lista de documentos
                // Por ejemplo:
                // foreach (var doc in documentos)
                // {
                //     listBoxDocumentos.Items.Add(doc.Descrip);
                // }
            }
            else
            {
                MessageBox.Show(message, "Error");


            }
        }

        // Boton de crear un documento
        private async void crearDoc_Click(object sender, RoutedEventArgs e)
        {
            var documento = new Biblioteca.Documento
            {
                // Define las propiedades del documento
                CuentaID = 1,
                TipoID = 2,
                UsuarioID = 3,
                CreadoFecha = DateTime.Now,
                EditadoID = 4,
                EditadoFecha = DateTime.Now,
                RevisadoID = 5,
                RevisadoFecha = DateTime.Now,
                AdminID = 3,
                ObraID = 5,
                PresupuestoID = 6,
                RubroID = 6,
                EntidadID = 7,
                DepositoID = 5,
                Descrip = "a",
                Concepto1 = "b",
                Fecha1 = DateTime.Now,
                Fecha2 = DateTime.Now,

                Fecha3 = DateTime.Now,
                Numero1 = 0,
                Numero2 = 0,
                Numero3 = 0,
                Notas = "bb",
                Active = false,
                Pesos = 0,
                Dolares = 0,
                Impuestos = 0,
                ImpuestosD = 0,
                Materiales = 0,
                ManodeObra = 0,
                Subcontratos = 0,
                Equipos = 0,
                Otros = 0,
                MaterialesD = 0,
                ManodeObraD = 0,
                SubcontratosD = 0,
                EquiposD = 0,
                OtrosD = 0,
                RelDoc = false,
                RelArt = false,
                RelMov = false,
                RelImp = false,
                RelRub = false,
                RelTar = false,
                RelIns = false
                // Añadir más propiedades según sea necesario...
            };

            var (success, message) = await datosWeb.PostDocumentoAsync(documento);

            MessageBox.Show(message, success ? "Éxito" : "Error");

        }

        // Botón de actualizar un documento
        private async void actualizarDoc_Click(object sender, RoutedEventArgs e)
        {
            var documento = new Biblioteca.Documento
            {
                // Define las propiedades del documento
                ID = 10,
                CuentaID = 55,
                TipoID = 5,
                UsuarioID = 1,
                CreadoFecha = DateTime.Now,
                EditadoID = 4,
                EditadoFecha = DateTime.Now,
                RevisadoID = 5,
                RevisadoFecha = DateTime.Now,
                AdminID = 3,
                ObraID = 5,
                PresupuestoID = 6,
                RubroID = 6,
                EntidadID = 7,
                DepositoID = 5,
                Descrip = "a",
                Concepto1 = "b",
                Fecha1 = DateTime.Now,
                Fecha2 = DateTime.Now,

                Fecha3 = DateTime.Now,
                Numero1 = 0,
                Numero2 = 0,
                Numero3 = 0,
                Notas = "bb",
                Active = false,
                Pesos = 0,
                Dolares = 0,
                Impuestos = 0,
                ImpuestosD = 0,
                Materiales = 0,
                ManodeObra = 0,
                Subcontratos = 0,
                Equipos = 0,
                Otros = 0,
                MaterialesD = 0,
                ManodeObraD = 0,
                SubcontratosD = 0,
                EquiposD = 0,
                OtrosD = 0,
                RelDoc = false,
                RelArt = false,
                RelMov = false,
                RelImp = false,
                RelRub = false,
                RelTar = false,
                RelIns = false
                // Añadir más propiedades según sea necesario...
            };
            var (success, message) = await datosWeb.PutDocumentoAsync(documento);
            MessageBox.Show(message, success ? "Éxito" : "Error");

        }

        // Botón de borrar un documento
        private async void borrarDoc_Click(object sender, RoutedEventArgs e)
        {
            int id = 10; // ID del documento a eliminar
            var (success, message) = await datosWeb.DeleteDocumentoAsync(id);
            MessageBox.Show(message, success ? "Éxito" : "Error");

        }

        // Botón de obtener un documento por ID
        private async void obtenerPorID_Click(object sender, RoutedEventArgs e)
        {
            int id = 3; // ID del documento a obtener
            var (success, message, documento) = await datosWeb.ObtenerDocumentoPorIDAsync(id);

            if (success)
            {
                MessageBox.Show(message, "Éxito");
                // Aquí se puede mostrar los detalles del documento en la interfaz de usuario
                // Por ejemplo:
                // textBoxDescripcion.Text = documento.Descrip;
            }
            else
            {
                MessageBox.Show(message, "Error");
            }
        }

        private void vConect_Click(object sender, RoutedEventArgs e)
        {
            Datos.Conectores v1 = new Conectores();
            v1.Show();
        }
    }
}
