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
using System.Collections.ObjectModel;
using Biblioteca.DTO;

namespace DataObra.Presupuestos.Ventanas
{
    /// <summary>
    /// Lógica de interacción para WiInicioControlPres.xaml
    /// </summary>
    public partial class WiInicioControlPres : Window
    {
        // Exponer la colección para binding o uso interno
        public ObservableCollection<PresupuestoDTO> Presupuestos { get; private set; }

        public WiInicioControlPres()
        {
            InitializeComponent();
            Presupuestos = new ObservableCollection<PresupuestoDTO>();
            DataContext = this;
        }

        // Constructor que recibe la colección del escritorio (se mantiene la misma instancia para reflejar cambios)
        public WiInicioControlPres(ObservableCollection<PresupuestoDTO> presupuestos) : this()
        {
            // Reemplaza la colección local por la referencia recibida (si es null, se deja la colección vacía creada arriba)
            Presupuestos = presupuestos ?? Presupuestos;
                DataContext = this;
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAbrir_Click(object sender, RoutedEventArgs e)
        {
            AbrirPresupuestoSeleccionado();
        }

        private void GrillaPresupuestos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AbrirPresupuestoSeleccionado();
        }

        private void AbrirPresupuestoSeleccionado()
        {
            if (GrillaPresupuestos.SelectedItem is PresupuestoDTO seleccionado && seleccionado.ID.HasValue)
            {
                // Guardar referencia del owner antes de cerrar
                var parentOwner = this.Owner;

                // Cerrar esta ventana de selección
                this.Close();

                // Abrir ventana de control con el presupuesto seleccionado
                var winControl = new WiControlPres(seleccionado)
                {
                    Owner = parentOwner ?? Application.Current.MainWindow
                };
                winControl.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione un presupuesto para abrir.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
