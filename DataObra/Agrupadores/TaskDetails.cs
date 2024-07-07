using Syncfusion.UI.Xaml.Kanban;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObra.Agrupadores
{
    public class TaskDetails
    {
        public TaskDetails()
        {
            Tasks = new ObservableCollection<KanbanModel>();

            KanbanModel task = new KanbanModel();
            task.Title = "Obra 33";
            task.ID = "6593";
            task.Description = "Detalle Obra 33";
            task.Category = "Postponed";
            task.ColorKey = "High";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle1.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Dato 32";
            task.ID = "6593";
            task.Description = "Detalle dato 34";
            task.Category = "Postponed";
            task.ColorKey = "Low";
            task.Tags = new string[] { "Varios" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle2.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Dato35";
            task.ID = "6516";
            task.Description = "Detalle Dato 35";
            task.Category = "Postponed";
            task.ColorKey = "Normal";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle3.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Obra 33";
            task.ID = "651";
            task.Description = "Detalle dato 4332 ccc";
            task.Category = "Open";
            task.ColorKey = "High";
            task.Tags = new string[] { "Area Técnica" };
            //task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle4.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Obra 4318";
            task.ID = "646";
            task.Description = "Datos varios Obra 4318";
            task.Category = "Open";
            task.ColorKey = "Low";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle5.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Agrupador";
            task.ID = "23822";
            task.Description = "Detalle Agrupador.";
            task.Category = "Open";
            task.ColorKey = "High";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle6.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Dato ";
            task.ID = "25678";
            task.Description = "Contenido dato";
            task.Category = "InProgress";
            task.ColorKey = "Low";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle7.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Dato 1";
            task.ID = "1254";
            task.Description = "Dato 1 de ejemplo";
            task.Category = "InProgress";
            task.ColorKey = "High";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle8.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Corralon de la esquina";
            task.ID = "28066";
            task.Description = "Datos del Corralon de Ejemplo";
            task.Category = "Review";
            task.ColorKey = "Normal";
            task.Tags = new string[] { "Urgente" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle9.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Dato 3";
            task.ID = "29477";
            task.Description = "Dato 3 de Ejemplo";
            task.Category = "Review";
            task.ColorKey = "Normal";
            task.Tags = new string[] { "Urgente" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle10.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Obra Ramon";
            task.ID = "29574";
            task.Description = "Dato 4 de Ejemplo";
            task.Category = "Review";
            task.ColorKey = "High";
            task.Tags = new string[] { "Urgente" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle11.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Dato 5 de Ejemplo";
            task.ID = "29574";
            task.Description = "Contenido Dato 5 de Ejemplo";
            task.Category = "Review";
            task.ColorKey = "High";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle12.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Agrupador 6";
            task.ID = "29574";
            task.Description = "Contenido Dato 6";
            task.Category = "Closed";
            task.ColorKey = "Normal";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle13.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);

            task = new KanbanModel();
            task.Title = "Agrupador 7";
            task.ID = "29574";
            task.Description = "Contenido 7";
            task.Category = "Open";
            task.ColorKey = "Low";
            task.Tags = new string[] { "Area Técnica" };
            task.ImageURL = new Uri(@"syncfusion.demoscommon.wpf;component/Assets/People/People_Circle14.png", UriKind.RelativeOrAbsolute);
            Tasks.Add(task);
        }
        public ObservableCollection<KanbanModel> Tasks { get; set; }
    }
}
