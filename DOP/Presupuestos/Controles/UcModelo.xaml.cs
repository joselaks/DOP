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

namespace DOP.Presupuestos.Controles
{
    /// <summary>
    /// Lógica de interacción para UcModelo.xaml
    /// </summary>
    public partial class UcModelo : UserControl
    {
        public UcModelo()
        {
            InitializeComponent();
        }

        public string TituloTexto
        {
            get => Titulo.Text;
            set => Titulo.Text = value;
        }

        public string DescripcionTexto
        {
            get => Descripcion.Text;
            set => Descripcion.Text = value;
        }
    }
}
