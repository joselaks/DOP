using Bibioteca.Clases;
using DOP.Presupuestos.Controles;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DataObra.Presupuestos.Controles.SubControles
    {
    /// <summary>
    /// Lógica de interacción para VentanaGraficoNodos.xaml
    /// </summary>

    public partial class VentanaGraficoNodos : Window
        {
        public Nodo Superior { get; }
        public ObservableCollection<Nodo> Inferiores { get; }
        private UcPlanilla ucPlanilla;

        public VentanaGraficoNodos(Nodo superior, ObservableCollection<Nodo> inferiores, UcPlanilla _planilla)
            {
            this.Superior = superior;
            this.Inferiores = inferiores;
            ucPlanilla = _planilla;

            Title = "Inferiores";
            Width = 600;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.CanResize;

            var canvas = new Canvas();

            // Calcula el alto necesario para el contenido
            double supX = 30;
            double supY = 30;
            double btnWidth = 200;
            double btnHeight = 30;
            double infX = supX + 120;
            double startY = 100;
            double spacing = 10;
            double totalHeight = startY + (btnHeight + spacing) * Inferiores.Count;

            // Superior alineado a la izquierda
            var supBtn = CrearNodoVisual(superior, supX, supY, isSuperior: true);
            canvas.Children.Add(supBtn);

            // Línea vertical
            double lineX = supX + btnWidth / 2;
            double lineY1 = supY + btnHeight;
            double lineY2 = startY + (btnHeight + spacing) * Inferiores.Count - spacing;
            var verticalLine = new Line
                {
                X1 = lineX,
                Y1 = lineY1,
                X2 = lineX,
                Y2 = lineY2,
                Stroke = Brushes.Black,
                StrokeThickness = 2
                };
            canvas.Children.Add(verticalLine);

            for (int i = 0; i < Inferiores.Count; i++)
                {
                double y = startY + i * (btnHeight + spacing);

                // Línea horizontal
                var line = new Line
                    {
                    X1 = lineX,
                    Y1 = y + btnHeight / 2,
                    X2 = infX,
                    Y2 = y + btnHeight / 2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                    };
                canvas.Children.Add(line);

                // Botón inferior
                var infBtn = CrearNodoVisual(Inferiores[i], infX, y, isSuperior: false);
                canvas.Children.Add(infBtn);
                }

            // Ajusta el tamaño del canvas para el scroll
            canvas.Width = 400;
            canvas.Height = Math.Max(400, totalHeight + 50);

            var scroll = new ScrollViewer
                {
                Content = canvas,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                };
            Content = scroll;
            }

        private Button CrearNodoVisual(Nodo nodo, double x, double y, bool isSuperior)
            {
            string texto = $"{nodo.Descripcion} ({nodo.ID})";

            var typeface = new Typeface("Segoe UI");
            var formattedText = new FormattedText(
                texto,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                12, // Tamaño de fuente por defecto
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            double anchoTexto = formattedText.Width + 24; // 24px de margen para padding y bordes

            var btn = new Button
                {
                Content = texto,
                Width = anchoTexto,
                Height = 30,
                Background = isSuperior ? Brushes.LightBlue : Brushes.LightGreen,
                Tag = nodo
                };
            Canvas.SetLeft(btn, x);
            Canvas.SetTop(btn, y);

            if (!isSuperior && ucPlanilla != null)
                {
                btn.MouseDoubleClick += (s, e) =>
                {
                    ucPlanilla.SeleccionarNodoEnArbol(nodo);
                };
                }
            return btn;
            }



        }





    }
