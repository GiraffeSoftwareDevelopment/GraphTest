using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GraphUtility
{
    class GPoint
    {
        public float X = 0.0f;
        public float Y = 0.0f;
        public GPoint(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public static float Distance(GPoint p1, GPoint p2)
        {
            float result = 0.0f;
            System.Numerics.Vector3 v1 = new System.Numerics.Vector3(p1.X, p1.Y, 0.0f);
            System.Numerics.Vector3 v2 = new System.Numerics.Vector3(p2.X, p2.Y, 0.0f);
            result = System.Numerics.Vector3.Distance(v1, v2);
            return (result);
        }
        public bool IsEqual(GPoint point)
        {
            bool result = false;
            if ((this.X == point.X) && (this.Y == point.Y))
            {
                result = true;
            }
            return (result);
        }
    }
    class GEdge
    {
        public GPoint Point1 = null;
        public GPoint Point2 = null;
        public GEdge(GEdge edge)
        {
            this.Point1 = edge.Point1;
            this.Point2 = edge.Point2;
        }
        public GEdge(GPoint p1, GPoint p2)
        {
            this.Point1 = p1;
            this.Point2 = p2;
        }
        public GEdge(float p1X, float p1Y, float p2X, float p2Y)
        {
            this.Point1 = new GPoint(p1X, p1Y);
            this.Point2 = new GPoint(p2X, p2Y);
        }
        public GEdge()
        {
            this.Point1 = new GPoint(0.0f, 0.0f);
            this.Point2 = new GPoint(0.0f, 0.0f);
        }
        public bool HasPoint(GPoint point)
        {
            bool result = false;
            if ((point.IsEqual(this.Point1)) || (point.IsEqual(this.Point2)))
            {
                result = true;
            }
            return (result);
        }
        public bool IsCross(GEdge edge)
        {
            bool result = false;

            Func<GPoint, GPoint, GPoint, float> f = (GPoint p1, GPoint p2, GPoint p3) => { return ((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X)); };
            float t1 = f(this.Point1, this.Point2, edge.Point1);
            float t2 = f(this.Point1, this.Point2, edge.Point2);
            float t3 = f(edge.Point1, edge.Point2, this.Point1);
            float t4 = f(edge.Point1, edge.Point2, this.Point2);
            if (((t1 * t2) < 0.0) && ((t3 * t4) < 0.0))
            {
                result = true;
            }
            return (result);
        }
        public GPoint GetCrossPoint(GEdge edge)
        {
            GPoint result = null;
            if (true == this.IsCross(edge))
            {
                float det = ((this.Point1.X - this.Point2.X) * (edge.Point2.Y - edge.Point1.Y) - (edge.Point2.X - edge.Point1.X) * (this.Point1.Y - this.Point2.Y));
                float t = (((edge.Point2.Y - edge.Point1.Y) * (edge.Point2.X - this.Point2.X) + (edge.Point1.X - edge.Point2.X) * (edge.Point2.Y - this.Point2.Y)) / det);
                float x = (t * this.Point1.X + (1.0f - t) * this.Point2.X);
                float y = (t * this.Point1.Y + (1.0f - t) * this.Point2.Y);
                result = new GPoint(x, y);
            }
            return (result);
        }
        public bool IsEqual(GEdge edge)
        {
            if (((true == this.Point1.IsEqual(edge.Point1)) && (true == this.Point2.IsEqual(edge.Point2))) ||
                ((true == this.Point1.IsEqual(edge.Point2)) && (true == this.Point2.IsEqual(edge.Point1))))
            {
                return (true);
            }
            return (false);
        }
        public static bool IsInList(GEdge edge, List<GEdge> list)
        {
            foreach (GEdge e in list)
            {
                if (true == edge.IsEqual(e))
                {
                    return (true);
                }
            }
            return (false);
        }
    }
    class GCircle
    {
        public GPoint Center = null;
        public float Radius = 0.0f;
        public GCircle(GPoint center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }
    }
    class GTriangle
    {
        public GEdge Edge1 = null;
        public GEdge Edge2 = null;
        public GEdge Edge3 = null;
        public GPoint Point1 = null;
        public GPoint Point2 = null;
        public GPoint Point3 = null;
        public GTriangle(GPoint p1, GPoint p2, GPoint p3)
        {
            this.Point1 = p1;
            this.Point2 = p2;
            this.Point3 = p3;
            this.Edge1 = new GEdge(p1, p2);
            this.Edge2 = new GEdge(p2, p3);
            this.Edge3 = new GEdge(p3, p1);
        }
        public bool IsEqual(GTriangle tri)
        {
            bool result = false;
            if ((true == this.HasPoint(tri.Point1)) && (true == this.HasPoint(tri.Point2)) && (true == this.HasPoint(tri.Point3)))
            {
                result = true;
            }
            return (result);
        }
        public bool HasPoint(GPoint point)
        {
            bool result = false;
            if ((true == this.Point1.IsEqual(point)) || (true == this.Point2.Equals(point)) || (this.Point3.Equals(point)))
            {
                result = true;
            }
            return (result);
        }
        public Int32 GetCommonPoinCount(GTriangle triangle)
        {
            Int32 result = 0;
            if (true == this.HasPoint(triangle.Point1))
            {
                result++;
            }
            if (true == this.HasPoint(triangle.Point2))
            {
                result++;
            }
            if (true == this.HasPoint(triangle.Point3))
            {
                result++;
            }
            return (result);
        }
        public GCircle GetCircumscribedCircle()
        {
            float x1 = this.Point1.X;
            float y1 = this.Point1.Y;
            float x2 = this.Point2.X;
            float y2 = this.Point2.Y;
            float x3 = this.Point3.X;
            float y3 = this.Point3.Y;
            float c = 2.0f * ((x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1));
            float x = ((y3 - y1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1) + (y1 - y2) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1)) / c;
            float y = ((x1 - x3) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1) + (x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1)) / c;
            GPoint center = new GPoint(x, y);
            float radius = GPoint.Distance(center, this.Point1);
            GCircle result = new GCircle(center, radius);
            return (result);
        }
    }
    class GBound
    {
        public GPoint Center = null;
        public float WidthHalf = 0.0f;
        public float HeightHalf = 0.0f;
        public GBound(List<GPoint> points)
        {
            Debug.Assert(0 < points.Count);
            GPoint min = new GPoint(points[0].X, points[0].Y);
            GPoint max = new GPoint(points[0].X, points[0].Y);
            for (Int32 index1 = 1; index1 < points.Count; index1++)
            {
                min.X = MathF.Min(min.X, points[index1].X);
                min.Y = MathF.Min(min.Y, points[index1].Y);
                max.X = MathF.Max(max.X, points[index1].X);
                max.Y = MathF.Max(max.Y, points[index1].Y);
            }
            this.WidthHalf = ((max.X - min.X) * 0.5f);
            this.HeightHalf = ((max.Y - min.Y) * 0.5f);
            this.Center = new GPoint(min.X + this.WidthHalf, min.Y + this.HeightHalf);
        }
        public bool Contains(GPoint point)
        {
            bool result = false;
            float minX = (Center.X - this.WidthHalf);
            float minY = (Center.Y - this.HeightHalf);
            float maxX = (Center.X + this.WidthHalf);
            float maxY = (Center.Y + this.HeightHalf);
            if ((minX < point.X) && (point.X < maxX)&&
                (minY < point.Y) && (point.Y < maxY))
            {
                result = true;
            }
            return (result);
        }
        public bool Cross(GEdge edge)
        {
            Int32 outside = 0;
            if (true == this.Contains(edge.Point1)){outside++;}
            if (true == this.Contains(edge.Point2)){outside++;}
            return (1 == outside);
        }
        public bool Outside(GEdge edge)
        {
            Int32 outside = 0;
            if (true == this.Contains(edge.Point1)){outside++;}
            if (true == this.Contains(edge.Point2)){outside++;}
            return (0 == outside);
        }
    }
    class GNode
    {
        public GPoint Point = null;
        public List<GNode> Neighbers = null;
        public GNode(GPoint point)
        {
            this.Point = point;
            this.Neighbers = new List<GNode>();
        }
        public bool IsEqual(GNode node)
        {
            if (this.Point.IsEqual(node.Point))
            {
                return (true);
            }
            return (false);
        }
        public bool IsEqual(GPoint point)
        {
            if (this.Point.IsEqual(point))
            {
                return (true);
            }
            return (false);
        }
        public static bool IsInList(GNode n, List<GNode> list)
        {
            foreach (GNode node in list)
            {
                if (true == node.IsEqual(n))
                {
                    return (true);
                }
            }
            return (false);
        }
        public static bool IsInList(GPoint p, List<GNode> list)
        {
            foreach (GNode node in list)
            {
                if (true == node.Point.IsEqual(p))
                {
                    return (true);
                }
            }
            return (false);
        }
        public static GNode GetNodeInListByPoint(GPoint p, List<GNode> list)
        {
            GNode result = null;
            foreach (GNode n in list)
            {
                if (true == n.IsEqual(p))
                {
                    result = n;
                    break;
                }
            }
            return (result);
        }
        public static GNode GetLeftTopNode(GBound bound, List<GNode> list)
        {
            GNode result = null;
            GPoint lt = new GPoint(bound.Center.X - bound.WidthHalf, bound.Center.Y - bound.HeightHalf);
            float min = float.MaxValue;
            foreach (GNode node in list)
            {
                float d = GPoint.Distance(lt, node.Point);
                if (d < min)
                {
                    min = d;
                    result = node;
                }
            }
            return (result);
        }
    }
}
