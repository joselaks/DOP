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
    public partial class Inicio : Window
    {
        public List<string> Listado;

        public Inicio()
        {
            InitializeComponent();
            Listado = new List<string>();

            string texto = "uno";
            Listado.Add(texto);
            texto = "dos";
            Listado.Add(texto);

            foreach (var item in Listado)
            {
                MessageBox.Show(item);
            }
        }
    }
}
