using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace GraphTest
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private void DrawCircle(float x, float y, float radius, Windows.UI.Color color)
        {
            var ellipse = new Ellipse();
            ellipse.Stroke = new SolidColorBrush(color);
            ellipse.Width = ellipse.Height = radius * 2.0f;
            Canvas.SetLeft(ellipse, x - (ellipse.Width * 0.5f));
            Canvas.SetTop(ellipse, y - (ellipse.Height * 0.5f));
            Main_canvas.Children.Add(ellipse);
        }
        private void DrawPoint(float x, float y, Windows.UI.Color color)
        {
            float size = 4.0f;
            var ellipse = new Ellipse();
            ellipse.Fill = new SolidColorBrush(color);
            ellipse.Width = size;
            ellipse.Height = size;
            Canvas.SetLeft(ellipse, x - (size * 0.5f));
            Canvas.SetTop(ellipse, y - (size * 0.5f));
            Main_canvas.Children.Add(ellipse);
        }
        private void DrawLine(DTPoint p1, DTPoint p2, Windows.UI.Color color)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush(color);
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
            Main_canvas.Children.Add(line);
        }
        private void DrawSquare(DTPoint p1, DTPoint p2, DTPoint p3, DTPoint p4, Windows.UI.Color color)
        {
            this.DrawLine(p1, p2, color);
            this.DrawLine(p2, p3, color);
            this.DrawLine(p3, p4, color);
            this.DrawLine(p4, p1, color);
        }
        private void DrawTriangle(DTPoint p1, DTPoint p2, DTPoint p3, Windows.UI.Color color)
        {
            this.DrawLine(p1, p2, color);
            this.DrawLine(p2, p3, color);
            this.DrawLine(p3, p1, color);
        }
        private void HaltonSequence(Int32 dimension, Int32 start, Int32 count, List<DTPoint> points)
        {
            Main_canvas.Children.Clear();
            {
                List<float> hs = new List<float>();
                for (Int32 index1 = 0; index1 < count; index1++)
                {
                    hs.Clear();
                    global::HaltonSequence.Get(hs, start + index1, dimension);
                    hs[0] *= (float)(Main_canvas.Width);
                    hs[1] *= (float)(Main_canvas.Height);
                    points.Add(new DTPoint(hs[0], hs[1]));
                }
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            List<DTPoint> points = new List<DTPoint>();
            {
                Int32 dimension = 3;
                Random rand = new Random();
                Int32 start = rand.Next(20, 256);
                Int32 count = 64;
                this.HaltonSequence(dimension, start, count, points);
            }
            {
                DelaunayTriangulation delaunay = new DelaunayTriangulation();
                {
                    DateTime startTime = DateTime.Now;
                    delaunay.Build(points);
                    DateTime endTime = DateTime.Now;
                    TimeSpan diffTime = (endTime - startTime);
                    Text_ElapsedTime.Text = string.Format("time : {0} seconds.", diffTime.TotalSeconds);
                }
                DTBound bound = delaunay.Bound;
                // bound.
                if (false)
                {
                    DTPoint p1 = new DTPoint((bound.Center.X - bound.WidthHalf), (bound.Center.Y - bound.HeightHalf));
                    DTPoint p2 = new DTPoint((bound.Center.X - bound.WidthHalf), (bound.Center.Y + bound.HeightHalf));
                    DTPoint p3 = new DTPoint((bound.Center.X + bound.WidthHalf), (bound.Center.Y + bound.HeightHalf));
                    DTPoint p4 = new DTPoint((bound.Center.X + bound.WidthHalf), (bound.Center.Y - bound.HeightHalf));
                    this.DrawSquare(p1, p2, p3, p4, Windows.UI.Colors.Gray);
                }
                // circle.
                if (false)
                {
                    float radius = delaunay.GetRadiusOfBound();
                    this.DrawCircle(bound.Center.X, bound.Center.Y, radius, Windows.UI.Colors.Gray);
                }
                // super triangle.
                if (false)
                {
                    DTTriangle st = delaunay.GetSuperTriangle();
                    this.DrawTriangle(st.Point1, st.Point2, st.Point3, Windows.UI.Colors.Gray);
                }
                // triangles.
                foreach (DTTriangle tri in delaunay.Triangles)
                {
                    this.DrawTriangle(tri.Point1, tri.Point2, tri.Point3, Windows.UI.Colors.Gray);
                    if (false)
                    {
                        DTCircle c = tri.GetCircumscribedCircle();
                        this.DrawCircle(c.Center.X, c.Center.Y, c.Radius, Windows.UI.Colors.Gray);
                    }
                }
                // points.
                for (Int32 index1 = 0; index1 < delaunay.Points.Count; index1++)
                {
                    Windows.UI.Color color = Windows.UI.Colors.Red;
                    if (100 <= index1)
                    {
                        color = Windows.UI.Colors.Green;
                    }
                    else if (10 <= index1)
                    {
                        color = Windows.UI.Colors.Blue;
                    }
                    this.DrawPoint(delaunay.Points[index1].X, delaunay.Points[index1].Y, color);
                }
            }
        }
    }
}
