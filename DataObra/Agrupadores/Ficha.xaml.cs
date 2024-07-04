using DataObra.Agrupadores.Clases;
using DataObra.Documentos.Clases;
using DataObra.Sistema.Clases;
using Syncfusion.UI.Xaml.Kanban;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Agrupadores
{
    public partial class Ficha : Window
    {
        Agrupador oActivo;
        Servidor azure = new Servidor();
        public event EventHandler<Agrupador> AgrupadorModified;

        public Ficha(Agrupador pAgrupador, int pTipoAgrupa)
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