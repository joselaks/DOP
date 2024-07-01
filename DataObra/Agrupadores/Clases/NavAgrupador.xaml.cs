using DataObra.Sistema.Clases;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataObra.Agrupadores.Clases
{
    public partial class NavAgrupador : UserControl
    {
        Servidor azure;
        public NavAgrupador()
        {
            InitializeComponent();

            azure = new Servidor();
            this.GrillaAgrupadores.ItemsSource = azure.Agrupadores;
        }

        // mm



    }
}
