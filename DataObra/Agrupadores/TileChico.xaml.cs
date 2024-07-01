using System;
using System.Windows;
using System.Windows.Controls;

namespace DataObra.Agrupadores
{
    public partial class TileChico : UserControl
    {
        public string Titulo;

        public TileChico(string pTitulo, string pDescrip)
        {
            InitializeComponent();
            TxtTitulo.Text = pTitulo;
            TxtDescrip.Text = pDescrip;

            //Random rand = new Random();
        }

        private void EditaFicha_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Edita Ficha");
        }
    }
}
