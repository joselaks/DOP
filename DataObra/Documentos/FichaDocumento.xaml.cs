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
using System.Windows.Shapes;

namespace DataObra.Documentos
{
    public partial class Ficha : UserControl
    {
        Servidor azure = new Servidor();
        public event EventHandler<Documento> DocumentoModified;

        private Documento oActivo;
        public Ficha(Documento pDoc)
        {
            InitializeComponent();
            this.ComboObras.ItemsSource = azure.Agrupadores.Where(a => a.TipoID == 1).OrderBy(a => a.Descrip);
            this.ComboAdmin.ItemsSource = azure.Agrupadores.Where(a => a.TipoID == 2).OrderBy(a => a.Descrip);

            // xx

            if (pDoc == null)
            {
                oActivo = new Documento
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
                oActivo = pDoc;

                this.ComboObras.SelectedItem = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.ObraID);
                this.ComboAdmin.SelectedItem = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.AdminID);
                var entidad = azure.Agrupadores.FirstOrDefault(a => a.ID == oActivo.EntidadID);
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

                if (oActivo.RevisadoID != 0)
                {
                    //this.CelRevisadoFecha.Visibility = Visibility.Visible;
                }
                else
                {
                    //this.CelRevisadoFecha.Visibility = Visibility.Collapsed;
                }
            }

            this.DataContext = oActivo;
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
                // Provisorio hasta obtener ID de la base
                oActivo.ID = GenerateRandomInt(332, int.MaxValue);
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

        #region Ver
        private int GenerateRandomInt(int minValue, int maxValue)
        {
            Random random = new Random();
            return random.Next(minValue, maxValue);
        }

        private void Boton1_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Porcentuales()
        {
            //if (oActivo.Total != 0)
            //{
            //    this.TxtMateriales.Text = "Materiales " + decimal.Round(oActivo.Materiales / oActivo.Total * 100, 1).ToString() + "%";
            //    this.TxtManodeobra.Text = "Mano de Obra " + decimal.Round(oActivo.Manodeobra / oActivo.Total * 100, 1).ToString() + "%";
            //    this.TxtSubcontratos.Text = "SubContratos " + decimal.Round(oActivo.Subcontratos / oActivo.Total * 100, 1).ToString() + "%";
            //    this.TxtEquipos.Text = "Equipos " + decimal.Round(oActivo.Equipos / oActivo.Total * 100, 1).ToString() + "%";
            //    this.TxtOtros.Text = "Otros " + decimal.Round(oActivo.Otros / oActivo.Total * 100, 1).ToString() + "%";
            //    this.TxtImpuestos.Text = "Impuestos " + decimal.Round(oActivo.Impuestos / oActivo.Total * 100, 1).ToString() + "%";
            //}
        }

        private void RadVacio_Checked(object sender, RoutedEventArgs e)
        {
            BloqueoTotales(false);

            //oActivo.Equipos = 0;
            //oActivo.Manodeobra = 0;
            //oActivo.Materiales = 0;
            //oActivo.Otros = 0;
            //oActivo.Subcontratos = 0;
            //oActivo.Total = 0;
        }

        private void BloqueoTotales(bool pBloqueo)
        {
            this.CelMateriales.IsReadOnly = pBloqueo;
            this.CelManodeObra.IsReadOnly = pBloqueo;
            this.CelEquipos.IsReadOnly = pBloqueo;
            this.CelSubcontratos.IsReadOnly = pBloqueo;
            this.CelOtros.IsReadOnly = pBloqueo;
            this.CelImpuestos.IsReadOnly = pBloqueo;
            this.CelMaterialesD.IsReadOnly = pBloqueo;
            this.CelManodeObraD.IsReadOnly = pBloqueo;
            this.CelEquiposD.IsReadOnly = pBloqueo;
            this.CelSubcontratosD.IsReadOnly = pBloqueo;
            this.CelOtrosD.IsReadOnly = pBloqueo;
            this.CelImpuestosD.IsReadOnly = pBloqueo;
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            //oActivo.Total = oActivo.Equipos + oActivo.Manodeobra + oActivo.Materiales + oActivo.Otros + oActivo.Subcontratos + oActivo.Impuestos;

            //Porcentuales();
        }


        #endregion Accesorias


    }
}