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
        public NavAgrupador()
        {
            InitializeComponent();

            azure = new Servidor();

            Tasks = new ObservableCollection<KanbanModel>();
            KanbanModel task;

            foreach (var item in azure.Agrupadores)
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
    }
}
