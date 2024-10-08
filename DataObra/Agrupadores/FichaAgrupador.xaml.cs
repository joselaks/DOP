using DataObra.Sistema;
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

namespace DataObra.Agrupadores
{
    public partial class FichaAgrupador : Window
    {
        Agrupador oActivo;
        Servidor azure = new Servidor();
        public event EventHandler<Agrupador> AgrupadorModified;

        public FichaAgrupador(Agrupador pAgrupador, char pTipoAgrupa)
        {
            InitializeComponent();
            if (pAgrupador == null)
            {
                oActivo = new Agrupador()
                {
                    UsuarioID = azure.UsuarioID,
                    Editado = System.DateTime.Today,
                    Active = false,
                    Descrip = "La descripcion",
                    Numero = 1,
                    TipoID = pTipoAgrupa
                };
            }
            else
            {
                oActivo = pAgrupador;
            }

            this.DataContext = oActivo;
        }

        private void BotonGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Procedimiento guardar en servidor

            if (oActivo.ID == 0)
            {
                // Provisorio hasta obtener ID de la base
                oActivo.ID = GenerateRandomInt(332, int.MaxValue);
            }

            oActivo.UsuarioID = azure.UsuarioID;
            oActivo.Editado = DateTime.Now;

            AgrupadorModified?.Invoke(this, oActivo);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int GenerateRandomInt(int minValue, int maxValue)
        {
            Random random = new Random();
            return random.Next(minValue, maxValue);
        }
    }
}