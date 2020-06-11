using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphUtility
{
    class DelaunayTriangulation
    {
        public List<GPoint> Points = null;
        public List<GTriangle> OriginalTriangles = null;
        public List<GTriangle> Triangles = null;
        public List<GNode> GraphNodes = null;
        public GNode RootGraphNode = null;
        public GBound Bound = null;
        public GTriangle SuperTriangle = null;
        public DelaunayTriangulation()
        {
        }
        public float GetRadiusOfBound()
        {
            float result = 0.0f;
            GPoint lefttop = new GPoint(this.Bound.Center.X - this.Bound.WidthHalf, this.Bound.Center.Y - this.Bound.HeightHalf);
            result = GPoint.Distance(this.Bound.Center, lefttop);
            return (result);
        }
        public GTriangle GetSuperTriangle()
        {
            GPoint c = this.Bound.Center;
            float radius = this.GetRadiusOfBound();
            GPoint p1 = null;
            GPoint p2 = null;
            GPoint p3 = null;
            {
                float sqrt = (MathF.Sqrt(3) * radius);
                float x1 = c.X - sqrt;
                float y1 = c.Y - radius;
                float x2 = c.X + sqrt;
                float y2 = c.Y - radius;
                float x3 = c.X;
                float y3 = c.Y + 2 * radius;
                p1 = new GPoint(x1, y1);
                p2 = new GPoint(x2, y2);
                p3 = new GPoint(x3, y3);
            }
            GTriangle result = new GTriangle(p1, p2, p3);
            return (result);
        }
        private bool IsUniqueTriangle(GTriangle triangle, List<GTriangle> triangles)
        {
            Int32 count = 0;
            foreach (GTriangle tri in triangles)
            {
                if(tri.IsEqual(triangle))
                {
                    count++;
                }
            }
            Debug.Assert(0 < count);
            return (1 == count);
        }
        public void Build(List<GPoint> points)
        {
            this.Points = points;
            this.Bound = new GBound(this.Points);
            this.SuperTriangle = this.GetSuperTriangle();
            this.OriginalTriangles = new List<GTriangle>();
            this.OriginalTriangles.Add(this.SuperTriangle);
            this.Triangles = new List<GTriangle>();
            List<GTriangle> tmpTriangles = new List<GTriangle>();
            foreach (GPoint point in this.Points)
            {
                tmpTriangles.Clear();
                for (Int32 index1 = (this.OriginalTriangles.Count - 1); 0 <= index1; index1--)
                {
                    GTriangle tri = this.OriginalTriangles[index1];
                    GCircle circle = tri.GetCircumscribedCircle();
                    if (circle.Radius >= GPoint.Distance(point, circle.Center))
                    {
                        tmpTriangles.Add(new GTriangle(point, tri.Point1, tri.Point2));
                        tmpTriangles.Add(new GTriangle(point, tri.Point2, tri.Point3));
                        tmpTriangles.Add(new GTriangle(point, tri.Point3, tri.Point1));
                        this.OriginalTriangles.RemoveAt(index1);
                    }
                }
                foreach (GTriangle tri in tmpTriangles)
                {
                    if (this.IsUniqueTriangle(tri, tmpTriangles))
                    {
                        this.OriginalTriangles.Add(tri);
                    }
                }
            }
            // Clean up.
            foreach (GTriangle tri in this.OriginalTriangles)
            {
                if ((false == this.SuperTriangle.HasPoint(tri.Point1)) &&
                    (false == this.SuperTriangle.HasPoint(tri.Point2)) &&
                    (false == this.SuperTriangle.HasPoint(tri.Point3)))
                {
                    this.Triangles.Add(tri);
                }
            }
            // Graph.
            this.GraphNodes = new List<GNode>();
            List<GEdge> uniqueEdges = new List<GEdge>();
            foreach (GTriangle tri in this.Triangles)
            {
                if (false == GEdge.IsInList(tri.Edge1, uniqueEdges)) { uniqueEdges.Add(tri.Edge1); }
                if (false == GEdge.IsInList(tri.Edge2, uniqueEdges)) { uniqueEdges.Add(tri.Edge2); }
                if (false == GEdge.IsInList(tri.Edge3, uniqueEdges)) { uniqueEdges.Add(tri.Edge3); }
            }
            foreach (GEdge e in uniqueEdges)
            {
                GNode node1 = GNode.GetNodeInListByPoint(e.Point1, this.GraphNodes);
                GNode node2 = GNode.GetNodeInListByPoint(e.Point2, this.GraphNodes);
                if (null == node1) { node1 = new GNode(e.Point1); this.GraphNodes.Add(node1); }
                if (null == node2) { node2 = new GNode(e.Point2); this.GraphNodes.Add(node2); }
                node1.Neighbers.Add(node2);
                node2.Neighbers.Add(node1);
            }
            this.RootGraphNode = GNode.GetLeftTopNode(this.Bound, this.GraphNodes);
            Debug.Assert(null != this.RootGraphNode);
        }
    }
}