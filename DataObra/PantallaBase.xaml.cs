using DataObra.Agrupadores;
using DataObra.Agrupadores.Clases;
using DataObra.Documentos;
using DataObra.Insumos;
using DataObra.Sistema;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataObra
{
    /// <summary>
    /// Lógica de interacción para Base.xaml
    /// </summary>
    public partial class PantallaBase : Window
    {
        public PantallaBase()
        {
            InitializeComponent();
            GrupoCuentas();
            GrupoRoles();
            GrupoDocumentos();
            
        }

        private void GrupoCuentas()
        {
            Tiles nueva = new Tiles();
            nueva.ListaString = new string[] { "Propia", "Empresa1", "Colega1", "InvitarColega", "AceptarInvitación" };

            foreach (var item in nueva.ListaString)
            {
                TilesCuentas.Items.Add(CrearTile(item, "Novedades sobre " + item + ".", "Violeta"));
            }
        }

        private void GrupoRoles()
        {
            Tiles nueva = new Tiles();
            nueva.ListaString = new string[] { "Titular", "Presupuestos", "Compras", "DirecciónObras", "Administración", "Depósitos", "Otros" };

            foreach (var item in nueva.ListaString)
            {
                TilesRoles.Items.Add(CrearTile(item, "" + item + ".", "Naranja"));
            }
        }

        private void GrupoDocumentos()
        {
            Tiles nueva = new Tiles();
            nueva.ListaString = new string[] { "Precostos", "Presupuestos", "Pedidos", "Planes", "Certificados", "Partes", "Facturas", "Compras", "Remitos", "Pagos", "Cobros", "Anticipos", "Contratos", "Sueldos", "Gastos", "Acopios" };

            foreach (var item in nueva.ListaString)
            {
                TilesDocs.Items.Add(CrearTile(item, "" + item + ".", "Turco"));
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

        private void ItemTile_Click(object sender, MouseButtonEventArgs e)
        {
            TileViewItem tileSele = sender as TileViewItem;
            bool abierto = false;

            if (tileSele != null)
            {
              
            }
        }

    }
}
