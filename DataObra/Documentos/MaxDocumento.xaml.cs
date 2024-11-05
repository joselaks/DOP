using Biblioteca;
using DataObra.Agrupadores;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataObra.Documentos
{
    public partial class MaxDocumento : UserControl
    {
        Servidor azure = new Servidor();
        Documento oActivo;
        public event EventHandler<Documento> DocumentoModified;

        public MaxDocumento(Biblioteca.Documento pDoc) 
        {
            InitializeComponent();
            this.ComboObras.ItemsSource = azure.Agrupadores.Where(a => a.TipoID == 1).OrderBy(a => a.Descrip);
            this.ComboAdmin.ItemsSource = azure.Agrupadores.Where(a => a.TipoID == 2).OrderBy(a => a.Descrip);

            if (pDoc == null)
            {
                oActivo = new Documento()
                {
                    Fecha1 = System.DateTime.Today,
                    CreadoFecha = System.DateTime.Today,
                    Usuario = azure.Usuario,
                    EditadoFecha = System.DateTime.Today,
                    EntidadID = 1
                };
                this.ComboObras.SelectedItem = azure.GetFirstAgrupadorByTipoIDOrdered(1);
                this.ComboAdmin.SelectedItem = azure.GetFirstAgrupadorByTipoIDOrdered(2);
            }
            else
            {
                // Recibe el Documento a editar bajado del servidor y lo convierte
                oActivo = Documento.Convertir(pDoc);

                this.ComboObras.SelectedItem = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.ObraID);
                this.ComboAdmin.SelectedItem = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.AdminID);
                var entidad = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.EntidadID);

                if (entidad != null)
                {
                    switch (entidad.TipoID)
                    {
                        case 'C':
                            this.EsCliente.IsChecked = true;
                            break;
                        case 'P':
                            this.EsProveedor.IsChecked = true;
                            break;
                        case 'S':
                            this.EsContratista.IsChecked = true;
                            break;
                        case 'O':
                            this.EsPersonal.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                    this.ComboEntidad.SelectedItem = entidad;
                }

                if (oActivo.RevisadoID != 0)
                {
                    //this.CelRevisadoFecha.Visibility = Visibility.Visible;
                }
                else
                {
                    //this.CelRevisadoFecha.Visibility = Visibility.Collapsed;
                }
            }

            this.DataContext = pDoc;
        }

        #region COMBOS

        private void ComboPresupuestos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboPresupuestos.SelectedItem as Agrupador;

            if (sele != null)
            {
                oActivo.PresupuestoID = sele.ID;
                oActivo.Presupuesto = sele.Descrip;
            }
        }

        private void ComboObras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboObras.SelectedItem as Agrupador;

            if (sele != null)
            {
                oActivo.ObraID = sele.ID;
                oActivo.Obra = sele.Descrip;
            }
        }
        private void ComboAdmin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboAdmin.SelectedItem as Agrupador;

            if (sele != null)
            {
                oActivo.AdminID = sele.ID;
                oActivo.Admin = sele.Descrip;
            }
        }
        private void ComboEntidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sele = ComboEntidad.SelectedItem as Agrupador;

            if (sele != null)
            {
                oActivo.EntidadID = sele.ID;
                oActivo.Entidad = sele.Descrip;
                oActivo.EntidadTipo = EntidadTipoHelper.GetEntidadTipo(sele.ID);
            }
        }

        #endregion


        #region PANTALLA

        private void EntityType_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                var tipoDiccionario = new Dictionary<string, int>
                {
                    { "EsCliente", 10 },
                    { "EsProveedor", 20 },
                    { "EsContratista", 30 },
                    { "EsPersonal", 40 }
                };

                if (tipoDiccionario.TryGetValue(radioButton.Name, out int tipoID))
                {
                    this.ComboEntidad.ItemsSource = azure.Agrupadores.Where(a => a.TipoID == tipoID).OrderBy(a => a.Descrip);
                    this.ComboEntidad.SelectedItem = azure.GetFirstAgrupadorByTipoIDOrdered(tipoID);
                }
            }
        }

        private void BotonVerifica_Click(object sender, RoutedEventArgs e)
        {
            if (oActivo.RevisadoID == 0)
            {
                oActivo.RevisadoID = azure.UsuarioID;
                oActivo.Revisado = azure.Usuario;
                oActivo.RevisadoFecha = System.DateTime.Today;
            }
            else
            {
                oActivo.RevisadoID = 0;
                oActivo.Revisado = "";
            }
        }

        private void BotonGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Procedimiento guardar en servidor

            if (oActivo.ID == 0)
            {
            }

            oActivo.EditadoID = azure.UsuarioID;
            oActivo.Editado = azure.Usuario;
            oActivo.EditadoFecha = DateTime.Now;

            DocumentoModified?.Invoke(this, oActivo);
            //this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
        }

        #endregion Pantalla
    }
}
