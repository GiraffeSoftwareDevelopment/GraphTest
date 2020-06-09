using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DTPoint
{
    public float X = 0.0f;
    public float Y = 0.0f;
    public DTPoint(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }
    public static float Distance(DTPoint p1, DTPoint p2)
    {
        float result = 0.0f;
        System.Numerics.Vector3 v1 = new System.Numerics.Vector3(p1.X, p1.Y, 0.0f);
        System.Numerics.Vector3 v2 = new System.Numerics.Vector3(p2.X, p2.Y, 0.0f);
        result = System.Numerics.Vector3.Distance(v1, v2);
        return (result);
    }
    public static bool operator ==(DTPoint p1, DTPoint p2)
    {
        return ((p1.X == p2.X) && (p1.Y == p2.Y));
    }
    public static bool operator !=(DTPoint p1, DTPoint p2)
    {
        return ((p1.X != p2.X) || (p1.Y != p2.Y));
    }
}
class DTEdge
{
    public DTPoint Point1 = null;
    public DTPoint Point2 = null;
    public DTEdge(DTPoint p1, DTPoint p2)
    {
        this.Point1 = p1;
        this.Point2 = p2;
    }
    public DTEdge(float p1X, float p1Y, float p2X, float p2Y)
    {
        this.Point1 = new DTPoint(p1X, p1Y);
        this.Point2 = new DTPoint(p2X, p2Y);
    }
}
class DTCircle
{
    public DTPoint Center = null;
    public float Radius = 0.0f;
    public DTCircle(DTPoint center, float radius)
    {
        this.Center = center;
        this.Radius = radius;
    }
}
class DTTriangle
{
    public DTEdge Edge1 = null;
    public DTEdge Edge2 = null;
    public DTEdge Edge3 = null;
    public DTPoint Point1 = null;
    public DTPoint Point2 = null;
    public DTPoint Point3 = null;
    public DTTriangle(DTPoint p1, DTPoint p2, DTPoint p3)
    {
        this.Point1 = p1;
        this.Point2 = p2;
        this.Point3 = p3;
        this.Edge1 = new DTEdge(p1, p2);
        this.Edge2 = new DTEdge(p2, p3);
        this.Edge3 = new DTEdge(p3, p1);
    }
    public bool IsEqual(DTTriangle tri)
    {
        bool result = false;
        if ((true == this.HasPoint(tri.Point1)) && (true == this.HasPoint(tri.Point2)) && (true == this.HasPoint(tri.Point3)))
        {
            result = true;
        }
        return (result);
    }
    public bool HasPoint(DTPoint point)
    {
        bool result = false;
        if ((point == this.Point1) || (point == this.Point2) || (point == this.Point3))
        {
            result = true;
        }
        return (result);
    }
    public DTCircle GetCircumscribedCircle()
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
        DTPoint center = new DTPoint(x, y);
        float radius = DTPoint.Distance(center, this.Point1);
        DTCircle result = new DTCircle(center, radius);
        return (result);
    }
}
class DTBound
{
    public DTPoint Center = null;
    public float WidthHalf = 0.0f;
    public float HeightHalf = 0.0f;
    public DTBound(List<DTPoint> points)
    {
        Debug.Assert(0 < points.Count);
        DTPoint min = new DTPoint(points[0].X, points[0].Y);
        DTPoint max = new DTPoint(points[0].X, points[0].Y);
        for (Int32 index1 = 1; index1 < points.Count; index1++)
        {
            min.X = MathF.Min(min.X, points[index1].X);
            min.Y = MathF.Min(min.Y, points[index1].Y);
            max.X = MathF.Max(max.X, points[index1].X);
            max.Y = MathF.Max(max.Y, points[index1].Y);
        }
        this.WidthHalf = ((max.X - min.X) * 0.5f);
        this.HeightHalf = ((max.Y - min.Y) * 0.5f);
        this.Center = new DTPoint(min.X + this.WidthHalf, min.Y + this.HeightHalf);
    }
}
class DelaunayTriangulation
{
    public List<DTPoint> Points = null;
    public List<DTTriangle> Triangles = null;
    public DTBound Bound = null;
    public DTTriangle SuperTriangle = null;
    public DelaunayTriangulation()
    {
    }
    public float GetRadiusOfBound()
    {
        float result = 0.0f;
        DTPoint lefttop = new DTPoint(this.Bound.Center.X - this.Bound.WidthHalf, this.Bound.Center.Y - this.Bound.HeightHalf);
        result = DTPoint.Distance(this.Bound.Center, lefttop);
        return (result);
    }
    public DTTriangle GetSuperTriangle()
    {
        DTPoint c = this.Bound.Center;
        float radius = this.GetRadiusOfBound();
        DTPoint p1 = null;
        DTPoint p2 = null;
        DTPoint p3 = null;
        {
            float sqrt = (MathF.Sqrt(3) * radius);
            float x1 = c.X - sqrt;
            float y1 = c.Y - radius;
            float x2 = c.X + sqrt;
            float y2 = c.Y - radius;
            float x3 = c.X;
            float y3 = c.Y + 2 * radius;
            p1 = new DTPoint(x1, y1);
            p2 = new DTPoint(x2, y2);
            p3 = new DTPoint(x3, y3);
        }
        DTTriangle result = new DTTriangle(p1, p2, p3);
        return (result);
    }
    private bool IsUniqueTriangle(DTTriangle triangle, List<DTTriangle> triangles)
    {
        Int32 count = 0;
        foreach (DTTriangle tri in triangles)
        {
            if(tri.IsEqual(triangle))
            {
                count++;
            }
        }
        Debug.Assert(0 < count);
        return (1 == count);
    }
    public void Build(List<DTPoint> points)
    {
        this.Points = points;
        this.Bound = new DTBound(this.Points);
        this.SuperTriangle = this.GetSuperTriangle();
        this.Triangles = new List<DTTriangle>();
        this.Triangles.Add(this.SuperTriangle);
        List<DTTriangle> tmpTriangles = new List<DTTriangle>();
        foreach (DTPoint point in this.Points)
        {
            tmpTriangles.Clear();
            for (Int32 index1 = (this.Triangles.Count - 1); 0 <= index1; index1--)
            {
                DTTriangle tri = this.Triangles[index1];
                DTCircle circle = tri.GetCircumscribedCircle();
                if (circle.Radius >= DTPoint.Distance(point, circle.Center))
                {
                    tmpTriangles.Add(new DTTriangle(point, tri.Point1, tri.Point2));
                    tmpTriangles.Add(new DTTriangle(point, tri.Point2, tri.Point3));
                    tmpTriangles.Add(new DTTriangle(point, tri.Point3, tri.Point1));
                    this.Triangles.RemoveAt(index1);
                }
            }
            foreach (DTTriangle tri in tmpTriangles)
            {
                if (this.IsUniqueTriangle(tri, tmpTriangles))
                {
                    this.Triangles.Add(tri);
                }
            }
        }
        for (Int32 index1 = (this.Triangles.Count - 1); 0 <= index1; index1--)
        {
            DTTriangle tri = this.Triangles[index1];
            if ((this.SuperTriangle.HasPoint(tri.Point1)) || (this.SuperTriangle.HasPoint(tri.Point2)) || (this.SuperTriangle.HasPoint(tri.Point3)))
            {
                this.Triangles.RemoveAt(index1);
            }
        }
    }
}
