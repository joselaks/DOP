using DataObra.Sistema.Clases;
using Syncfusion.ProjIO;
using Syncfusion.UI.Xaml.Kanban;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataObra.Agrupadores.Clases
{
    public partial class NavAgrupador : UserControl
    {
        Servidor azure;
        public ObservableCollection<KanbanModel> Tasks { get; set; }
        private KanbanModel selectedItem;
        // Comentario

        public NavAgrupador(string pTipo)
        {
            InitializeComponent();

            int TipoAgrupa;

            switch (pTipo)
            {
                case "Obras":
                    TipoAgrupa = 1;
                    break;
                case "Admin":
                    TipoAgrupa = 2;
                    break;
                case "Clientes":
                    TipoAgrupa = 3;
                    break;
                default:
                    TipoAgrupa = 100;
                    break;
            }

            azure = new Servidor();

            Tasks = new ObservableCollection<KanbanModel>();
            KanbanModel task;

            foreach (var item in azure.Agrupadores.Where(a => a.TipoID == TipoAgrupa))
            {
                task = new KanbanModel();

                task.ID = item.ID.ToString();
                task.Title = item.Descrip;
                task.Description = item.Numero.ToString();

                if (item.Active)
                {
                    task.Category = "Activos";
                }
                else
                {
                    task.Category = "Pendientes";
                }

                Tasks.Add(task);
            }

            this.GrillaAgrupadores.ItemsSource = Tasks;
        }

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            KanbanModel task = new KanbanModel();

            task.Title = "Nuevo Agrupador";
            task.Description = "Nuevo";
            task.Category = "Activos";

            Tasks.Add(task);
        }

        private void Modificar_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GrillaAgrupadores_CardTapped(object sender, KanbanTappedEventArgs e)
        {
            KanbanModel kanbanModel = e.SelectedCard.Content as KanbanModel;
            MessageBox.Show(kanbanModel.Title.ToString());
        }

        private void GrillaAgrupadores_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}

