using GraphMaster_X.Classes;
using GraphMaster_X.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphMaster_X.Views
{
    /// <summary>
    /// Логика взаимодействия для EditorV.xaml
    /// </summary>
    public partial class EditorV : Window
    {
        EditorVM ViewModel = new EditorVM();
        public EditorV()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.PointNamePlacementEvent += SetTextOnCanvas;
        }

        Ellipse savedPoint = null;
        bool isPointNameInputing = false;
        bool isLineNameInputing = false;
        Point TempPointData;
        Line TempLineData;

        //TODO: вынести установку точек и линий в ивент для использования в других классах
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPointNameInputing || isLineNameInputing)
                return;

            double x = Mouse.GetPosition(Scene).X;
            double y = Mouse.GetPosition(Scene).Y;
            if (ViewModel.Mode == GraphEditMode.Point)
            {
                Ellipse point = new Ellipse
                {
                    Height = 15,
                    Width = 15,
                    Stroke = Brushes.Black,
                    Fill = Brushes.White,
                    StrokeThickness = 3
                };
                Scene.Children.Add(point);
                Canvas.SetLeft(point, x);
                Canvas.SetTop(point, y);
                Panel.SetZIndex(point, 1);

                isPointNameInputing = true;
                ViewModel.CallPointNameInputingEvent(new Point(x, y));


                TempPointData = new Point(x, y);
            }
            else if (ViewModel.Mode == GraphEditMode.Line)
            {
                List<Ellipse> points = new List<Ellipse>();
                foreach (var i in Scene.Children)
                {
                    if (i.GetType() == typeof(Ellipse))
                    {
                        points.Add((Ellipse)i);
                    }
                }


                if (savedPoint == null)
                {
                    foreach (var point in points)
                    {
                        if (point.IsMouseOver)
                        {
                            savedPoint = point;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var point in points)
                    {
                        if (point.IsMouseOver && point != savedPoint)
                        {
                            Line line = new Line
                            {
                                X1 = Canvas.GetLeft(savedPoint) + 5,
                                Y1 = Canvas.GetTop(savedPoint) + 5,

                                X2 = Canvas.GetLeft(point) + 5,
                                Y2 = Canvas.GetTop(point) + 5,

                                Stroke = Brushes.Black,
                                StrokeThickness = 3
                            };

                            Scene.Children.Add(line);
                            Panel.SetZIndex(line, 0);

                            isLineNameInputing = true;
                            ViewModel.CallLineNameInputingEvent
                                      (
                                      Weight.CalculatePos(line.X1, line.Y1, line.X2, line.Y2)
                                      );
                            TempLineData = line;


                        }
                    }
                    savedPoint = null;
                }

            }
        }

        private void GraphPropInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && GraphPropInput.Text != string.Empty)
            {
                if (isPointNameInputing)
                {
                    ViewModel.CallInputCompleteEvent(TempPointData);
                    GraphPropInput.Text = "";
                    isPointNameInputing = false;
                }
                else if (isLineNameInputing)
                {
                    Point p1 = new Point(TempLineData.X1, TempLineData.Y1);
                    Point p2 = new Point(TempLineData.X2, TempLineData.Y2);
                    ViewModel.CallLineInputCompleteEvent(p1, p2);
                    GraphPropInput.Text = "";
                    isLineNameInputing = false;
                }
            }
        }

        private void SetTextOnCanvas(string Text, Point point)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = Text,
                FontSize = 18,
                FontWeight = FontWeights.Bold
            };

            Scene.Children.Add(textBlock);
            Canvas.SetLeft(textBlock, point.X - 12);
            Canvas.SetTop(textBlock, point.Y - 15);

        }
    }
}
