using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GraphUtility
{
    class VoronoiNode
    {
        public GTriangle Triangle = null;
        public GCircle Circle = null;
        public VoronoiNode(GTriangle triangle)
        {
            this.Triangle = triangle;
            this.Circle = this.Triangle.GetCircumscribedCircle();
        }
    }
    class VoronoiDiagram
    {
        public List<VoronoiNode> Nodes = null;
        public List<GEdge> OriginalEdges = null;
        public List<GEdge> Edges = null;
        public GBound Bound = null;
        public VoronoiDiagram()
        {
            this.Nodes = new List<VoronoiNode>();
            this.OriginalEdges = new List<GEdge>();
            this.Edges = new List<GEdge>();
        }
        private void AddUniqueEdge(VoronoiNode node1, VoronoiNode node2)
        {
            bool contains = false;
            foreach (GEdge edge in this.OriginalEdges)
            {
                if ((true == edge.HasPoint(node1.Circle.Center)) && (true == edge.HasPoint(node2.Circle.Center)))
                {
                    contains = true;
                    break;
                }
            }
            if (false == contains)
            {
                this.OriginalEdges.Add(new GEdge(node1.Circle.Center, node2.Circle.Center));
            }
        }
        public void Build(DelaunayTriangulation delaunay)
        {
            this.Nodes.Clear();
            this.OriginalEdges.Clear();
            this.Bound = delaunay.Bound;
            foreach (GTriangle tri in delaunay.OriginalTriangles)
            {
                this.Nodes.Add(new VoronoiNode(tri));
            }
            for (Int32 index1 = 0; index1 < this.Nodes.Count; index1++)
            {
                for (Int32 index2 = 0; index2 < this.Nodes.Count; index2++)
                {
                    if (index1 != index2)
                    {
                        VoronoiNode n1 = this.Nodes[index1];
                        VoronoiNode n2 = this.Nodes[index2];
                        if (2 == n1.Triangle.GetCommonPoinCount(n2.Triangle))
                        {
                            this.AddUniqueEdge(n1, n2);
                        }
                    }
                }
            }
            // clip.
            foreach (GEdge edge in this.OriginalEdges)
            {
                this.Edges.Add(new GEdge(edge));
            }
            for (Int32 index1 = (this.Edges.Count - 1); index1 >= 0; index1--)
            {
                if (true == this.Bound.Outside(this.Edges[index1]))
                {
                    this.Edges.RemoveAt(index1);
                }
            }
            List<GEdge> tmpEdges = new List<GEdge>();
            foreach (GEdge edge in this.Edges)
            {
                if (true == this.Bound.Cross(edge))
                {
                    tmpEdges.Add(edge);
                }
            }
            {
                float minX = (this.Bound.Center.X - this.Bound.WidthHalf);
                float minY = (this.Bound.Center.Y - this.Bound.HeightHalf);
                float maxX = (this.Bound.Center.X + this.Bound.WidthHalf);
                float maxY = (this.Bound.Center.Y + this.Bound.HeightHalf);
                GEdge tEdge = new GEdge(minX, minY, maxX, minY);
                GEdge bEdge = new GEdge(minX, maxY, maxX, maxY);
                GEdge lEdge = new GEdge(minX, minY, minX, maxY);
                GEdge rEdge = new GEdge(maxX, minY, maxX, maxY);
                List<GEdge> edgesCrossOnT = new List<GEdge>();
                List<GEdge> edgesCrossOnB = new List<GEdge>();
                List<GEdge> edgesCrossOnL = new List<GEdge>();
                List<GEdge> edgesCrossOnR = new List<GEdge>();
                foreach (GEdge edge in tmpEdges)
                {
                    if (true == tEdge.IsCross(edge)) { edgesCrossOnT.Add(edge); }
                    else if (true == bEdge.IsCross(edge)) { edgesCrossOnB.Add(edge); }
                    else if (true == lEdge.IsCross(edge)) { edgesCrossOnL.Add(edge); }
                    else if (true == rEdge.IsCross(edge)) { edgesCrossOnR.Add(edge); }
                }
                List<GPoint> pointsOnT = new List<GPoint>();
                List<GPoint> pointsOnB = new List<GPoint>();
                List<GPoint> pointsOnL = new List<GPoint>();
                List<GPoint> pointsOnR = new List<GPoint>();
                for (Int32 index1 = 0; index1 < edgesCrossOnT.Count; index1++)
                {
                    pointsOnT.Add(this.ClipEdges(tEdge, edgesCrossOnT[index1]));
                }
                for (Int32 index1 = 0; index1 < edgesCrossOnB.Count; index1++)
                {
                    pointsOnB.Add(this.ClipEdges(bEdge, edgesCrossOnB[index1]));
                }
                for (Int32 index1 = 0; index1 < edgesCrossOnL.Count; index1++)
                {
                    pointsOnL.Add(this.ClipEdges(lEdge, edgesCrossOnL[index1]));
                }
                for (Int32 index1 = 0; index1 < edgesCrossOnR.Count; index1++)
                {
                    pointsOnR.Add(this.ClipEdges(rEdge, edgesCrossOnR[index1]));
                }
                GPoint lt = new GPoint(this.Bound.Center.X - this.Bound.WidthHalf, this.Bound.Center.Y - this.Bound.HeightHalf);
                GPoint lb = new GPoint(this.Bound.Center.X - this.Bound.WidthHalf, this.Bound.Center.Y + this.Bound.HeightHalf);
                GPoint rt = new GPoint(this.Bound.Center.X + this.Bound.WidthHalf, this.Bound.Center.Y - this.Bound.HeightHalf);
                GPoint rb = new GPoint(this.Bound.Center.X + this.Bound.WidthHalf, this.Bound.Center.Y + this.Bound.HeightHalf);
                pointsOnT.Sort((a, b) => (int)(a.X - b.X));
                pointsOnB.Sort((a, b) => (int)(a.X - b.X));
                pointsOnL.Sort((a, b) => (int)(a.Y - b.Y));
                pointsOnR.Sort((a, b) => (int)(a.Y - b.Y));
                pointsOnT.Insert(0, lt);
                pointsOnT.Add(rt);
                pointsOnB.Insert(0, lb);
                pointsOnB.Add(rb);
                pointsOnL.Insert(0, lt);
                pointsOnL.Add(lb);
                pointsOnR.Insert(0, rt);
                pointsOnR.Add(rb);
                this.AddEdgeFromList(pointsOnT);
                this.AddEdgeFromList(pointsOnB);
                this.AddEdgeFromList(pointsOnL);
                this.AddEdgeFromList(pointsOnR);
            }
        }
        private void AddEdgeFromList(List<GPoint> points)
        {
            Debug.Assert(2 <= points.Count);
            for (Int32 index1 = 1; index1 < points.Count; index1++)
            {
                GEdge edge = new GEdge(points[index1 - 1], points[index1]);
                this.Edges.Add(edge);
            }
        }
        private GPoint ClipEdges(GEdge boundEdge, GEdge edge)
        {
            GPoint point = boundEdge.GetCrossPoint(edge);
            Debug.Assert(null != point);
            if (null != point)
            {
                if (true == this.Bound.Contains(edge.Point1))
                {
                    edge.Point2 = point;
                }
                else
                {
                    edge.Point1 = point;
                }
            }
            return (point);
        }
    }
}