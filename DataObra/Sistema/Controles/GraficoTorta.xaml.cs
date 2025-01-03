using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace DataObra.Sistema.Controles
{
    public partial class GraficoTorta : UserControl
    {
        public GraficoTorta()
        {
            InitializeComponent();
            Loaded += GraficoTorta_Loaded;

            // Definir los datos de ejemplo
            var datos = new ObservableCollection<DatoGrafico>
            {
                new DatoGrafico { Categoria = "Materiales", Importe = 50000 },
                new DatoGrafico { Categoria = "Mano de obra", Importe = 20000 },
                new DatoGrafico { Categoria = "Equipos", Importe = 15000 },
                new DatoGrafico { Categoria = "Subcontratos", Importe = 25000 },
                new DatoGrafico { Categoria = "Otros", Importe = 10000 }
            };

            // Calcular el porcentaje para cada categoría
            double total = datos.Sum(d => d.Importe);
            foreach (var dato in datos)
            {
                dato.Porcentaje = dato.Importe / total;
            }

            Datos = datos;
        }

        // Propiedad para enlazar al gráfico
        public ObservableCollection<DatoGrafico> Datos { get; set; }

        private void GraficoTorta_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DrawPieChart();
        }

        private void DrawPieChart()
        {
            double total = Datos.Sum(d => d.Importe);
            double angle = 0;
            double centerX = canvas.Width / 2;
            double centerY = canvas.Height / 2;
            double radius = Math.Min(centerX, centerY) - 20;
            Brush[] brushes = { Brushes.Green, Brushes.Yellow, Brushes.Blue, Brushes.Orange, Brushes.Purple };

            for (int i = 0; i < Datos.Count; i++)
            {
                var dato = Datos[i];
                double sliceAngle = dato.Importe / total * 360;

                var pathFigure = new PathFigure { StartPoint = new System.Windows.Point(centerX, centerY) };

                var arcSegment = new ArcSegment
                {
                    Point = new System.Windows.Point(
                        centerX + radius * Math.Cos((angle + sliceAngle) * Math.PI / 180),
                        centerY + radius * Math.Sin((angle + sliceAngle) * Math.PI / 180)),
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = sliceAngle > 180
                };

                pathFigure.Segments.Add(new LineSegment
                {
                    Point = new System.Windows.Point(
                        centerX + radius * Math.Cos(angle * Math.PI / 180),
                        centerY + radius * Math.Sin(angle * Math.PI / 180))
                });

                pathFigure.Segments.Add(arcSegment);
                pathFigure.Segments.Add(new LineSegment { Point = new System.Windows.Point(centerX, centerY) });

                var pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(pathFigure);

                var path = new Path
                {
                    Fill = brushes[i % brushes.Length],
                    Data = pathGeometry
                };

                canvas.Children.Add(path);

                // Etiquetas de datos
                double labelAngle = angle + sliceAngle / 2;
                var label = new TextBlock
                {
                    Text = $"{dato.Categoria} ({dato.Porcentaje:p1})",
                    RenderTransform = new TranslateTransform(
                        centerX + (radius + 10) * Math.Cos(labelAngle * Math.PI / 180),
                        centerY + (radius + 10) * Math.Sin(labelAngle * Math.PI / 180)),
                    Background = Brushes.White
                };

                canvas.Children.Add(label);

                angle += sliceAngle;
            }
        }
    }

    public class DatoGrafico
    {
        public string Categoria { get; set; }
        public double Importe { get; set; }
        public double Porcentaje { get; set; }
    }
}
