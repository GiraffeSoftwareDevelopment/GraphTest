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

using GraphUtility;

namespace GraphTest
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            TextBox_point_size.Text = string.Format("{0}", 32);
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
        private void DrawLine(GPoint p1, GPoint p2, Windows.UI.Color color)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush(color);
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
            Main_canvas.Children.Add(line);
        }
        private void DrawSquare(GPoint p1, GPoint p2, GPoint p3, GPoint p4, Windows.UI.Color color)
        {
            this.DrawLine(p1, p2, color);
            this.DrawLine(p2, p3, color);
            this.DrawLine(p3, p4, color);
            this.DrawLine(p4, p1, color);
        }
        private void DrawTriangle(GPoint p1, GPoint p2, GPoint p3, Windows.UI.Color color)
        {
            this.DrawLine(p1, p2, color);
            this.DrawLine(p2, p3, color);
            this.DrawLine(p3, p1, color);
        }
        private void DrawGraph(GNode node, Windows.UI.Color color, List<(GNode, GNode)> tupleList)
        {
            bool isRoot = false;
            if (null == tupleList)
            {
                isRoot = true;
                tupleList = new List<(GNode, GNode)>();
            }
            foreach (GNode neighber in node.Neighbers)
            {
                bool skip = false;
                foreach (var t in tupleList)
                {
                    if (((node == t.Item1) && (neighber == t.Item2)) ||
                        ((node == t.Item2) && (neighber == t.Item1)))
                    {
                        skip = true;
                        break;
                    }
                }
                if (false == skip)
                {
                    tupleList.Add((node, neighber));
                    this.DrawLine(node.Point, neighber.Point, color);
                    this.DrawGraph(neighber, color, tupleList);
                }
            }
            if (true == isRoot)
            {
                this.DrawPoint(node.Point.X, node.Point.Y, Windows.UI.Colors.Black);
            }
        }
        private void HaltonSequence(Int32 dimension, Int32 start, Int32 count, List<GPoint> points)
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
                    points.Add(new GPoint(hs[0], hs[1]));
                }
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            List<GPoint> points = new List<GPoint>();
            {
                Int32 dimension = 3;
                Random rand = new Random();
                Int32 start = rand.Next(20, 256);
                Int32 count = 0;
                if ((false == int.TryParse(TextBox_point_size.Text, out count)) || (1024 < count))
                {
                    count = 32;
                    TextBox_point_size.Text = string.Format("{0}", count);
                }
                this.HaltonSequence(dimension, start, count, points);
            }
            TimeSpan delaunayDiffTime;
            DelaunayTriangulation delaunay = new DelaunayTriangulation();
            {
                {
                    DateTime delaunayStartTime = DateTime.Now;
                    delaunay.Build(points);
                    DateTime delaunayEndTime = DateTime.Now;
                    delaunayDiffTime = (delaunayEndTime - delaunayStartTime);
                }
                GBound bound = delaunay.Bound;
                // bound.
                if (true)
                {
                    GPoint p1 = new GPoint((bound.Center.X - bound.WidthHalf), (bound.Center.Y - bound.HeightHalf));
                    GPoint p2 = new GPoint((bound.Center.X - bound.WidthHalf), (bound.Center.Y + bound.HeightHalf));
                    GPoint p3 = new GPoint((bound.Center.X + bound.WidthHalf), (bound.Center.Y + bound.HeightHalf));
                    GPoint p4 = new GPoint((bound.Center.X + bound.WidthHalf), (bound.Center.Y - bound.HeightHalf));
                    this.DrawSquare(p1, p2, p3, p4, Windows.UI.Colors.LightGray);
                }
                // circle.
                if (false)
                {
                    float radius = delaunay.GetRadiusOfBound();
                    this.DrawCircle(bound.Center.X, bound.Center.Y, radius, Windows.UI.Colors.LightGray);
                }
                // super triangle.
                if (false)
                {
                    GTriangle st = delaunay.GetSuperTriangle();
                    this.DrawTriangle(st.Point1, st.Point2, st.Point3, Windows.UI.Colors.LightGray);
                }
                // delaunay original triangles.
                if (false)
                {
                    foreach (GTriangle tri in delaunay.OriginalTriangles)
                    {
                        this.DrawTriangle(tri.Point1, tri.Point2, tri.Point3, Windows.UI.Colors.LightGray);
                        if (false)
                        {
                            GCircle c = tri.GetCircumscribedCircle();
                            this.DrawCircle(c.Center.X, c.Center.Y, c.Radius, Windows.UI.Colors.LightGray);
                        }
                    }
                }
                // delaunay triangles.
                if (false)
                {
                    foreach (GTriangle tri in delaunay.Triangles)
                    {
                        this.DrawTriangle(tri.Point1, tri.Point2, tri.Point3, Windows.UI.Colors.LightBlue);
                    }
                }
                // delaunay graph.
                if (true)
                {
                    this.DrawGraph(delaunay.RootGraphNode, Windows.UI.Colors.LightBlue, null);
                }
                // delaunay graph spanning tree.
                if (true)
                {
                    this.DrawGraph(delaunay.SpanningTreeRoot, Windows.UI.Colors.Blue, null);
                }
                // points.
                if (false)
                {
                    for (Int32 index1 = 0; index1 < delaunay.Points.Count; index1++)
                    {
                        Windows.UI.Color color = Windows.UI.Colors.LightGray;
                        if (false)
                        {
                            color = Windows.UI.Colors.Red;
                            if (100 <= index1)
                            {
                                color = Windows.UI.Colors.Green;
                            }
                            else if (10 <= index1)
                            {
                                color = Windows.UI.Colors.Blue;
                            }
                        }
                        this.DrawPoint(delaunay.Points[index1].X, delaunay.Points[index1].Y, color);
                    }
                }
            }
            TimeSpan voronoiDiffTime;
            VoronoiDiagram voronoi = new VoronoiDiagram();
            {
                DateTime voronoiStartTime = DateTime.Now;
                voronoi.Build(delaunay);
                DateTime voronoiEndTime = DateTime.Now;
                voronoiDiffTime = (voronoiEndTime - voronoiStartTime);
                if (false)
                {
                    foreach (GEdge edge in voronoi.OriginalEdges)
                    {
                        this.DrawLine(edge.Point1, edge.Point2, Windows.UI.Colors.LightGray);
                    }
                }
                foreach (GEdge edge in voronoi.Edges)
                {
                    this.DrawLine(edge.Point1, edge.Point2, Windows.UI.Colors.Pink);
                }
            }
            Text_ElapsedTime.Text = string.Format("time : Delaunay {0} seconds / Voronoi {1} seconds ",
                delaunayDiffTime.TotalSeconds,
                voronoiDiffTime.TotalSeconds);
        }
    }
}
