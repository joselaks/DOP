using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Sistema.Controles
{
    public partial class Roles : Window
    {
        public Roles()
        {
            InitializeComponent();
        }

        private void RolesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RolesListBox.SelectedItem != null)
            {
                string selectedRole = (RolesListBox.SelectedItem as ListBoxItem).Content.ToString();
                ActualizaSeleccion(selectedRole);
            }
        }

        private void ActualizaSeleccion(string role)
        {
            // Limpiar las selecciones
            BorrarSeleccion();

            // Configurar selecciones basadas en el rol
            switch (role)
            {
                case "Administración":
                    CheckDocumentos("Facturas", "Pagos", "Cobros");
                    CheckAgrupadores("Administraciones", "Clientes");
                    CheckInsumos("Materiales", "Mano de Obra");
                    break;
                case "Compras":
                    CheckDocumentos("Compras", "Remitos");
                    CheckAgrupadores("Proveedores");
                    CheckInsumos("Artículos", "Equipos");
                    break;
                case "Gestión Depósitos":
                    CheckDocumentos("Partes", "Acopios");
                    CheckAgrupadores("Depósitos", "Cuentas");
                    CheckInsumos("Materiales", "Tareas");
                    break;
                case "Presupuestos":
                    CheckDocumentos("Presupuestos", "Planes");
                    CheckAgrupadores("Obras", "Clientes");
                    CheckInsumos("Rubros", "SubContratos");
                    break;
                case "Socio/Titular":
                    CheckDocumentos("Contratos", "Certificados");
                    CheckAgrupadores("Contratistas", "Obreros");
                    CheckInsumos("Equipos", "Otros");
                    break;
                case "Otro":
                    CheckDocumentos("Gastos", "Anticipos");
                    CheckAgrupadores("Impuestos", "Administraciones");
                    CheckInsumos("Tareas", "Artículos");
                    break;
                default:
                    break;
            }
        }

        private void BorrarSeleccion()
        {
            // Desmarcar todos los CheckBox de Documentos
            foreach (var child in DocumentosPanel.Children)
            {
                if (child is CheckBox checkbox)
                    checkbox.IsChecked = false;
            }

            // Desmarcar todos los CheckBox de Agrupadores
            foreach (var child in AgrupadoresPanel.Children)
            {
                if (child is CheckBox checkbox)
                    checkbox.IsChecked = false;
            }

            // Desmarcar todos los CheckBox de Insumos
            foreach (var child in InsumosPanel.Children)
            {
                if (child is CheckBox checkbox)
                    checkbox.IsChecked = false;
            }
        }

        private void CheckDocumentos(params string[] documentos)
        {
            foreach (var child in ((StackPanel)((ScrollViewer)DocumentosPanel.Children[1]).Content).Children)
            {
                if (child is CheckBox checkbox && documentos.Contains(checkbox.Content.ToString()))
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void CheckAgrupadores(params string[] agrupadores)
        {
            foreach (var child in ((StackPanel)((ScrollViewer)AgrupadoresPanel.Children[1]).Content).Children)
            {
                if (child is CheckBox checkbox && agrupadores.Contains(checkbox.Content.ToString()))
                {
                    checkbox.IsChecked = true;
                }
            }
        }

        private void CheckInsumos(params string[] insumos)
        {
            foreach (var child in ((StackPanel)((ScrollViewer)InsumosPanel.Children[1]).Content).Children)
            {
                if (child is CheckBox checkbox && insumos.Contains(checkbox.Content.ToString()))
                {
                    checkbox.IsChecked = true;
                }
            }
        }
    }
}
