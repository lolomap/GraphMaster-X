using System;
using System.Collections.Generic;
using System.Windows;


namespace GraphMaster_X.Classes
{
    #region GraphElements
    [Serializable]
    public class GraphPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }

        static private int IDsCount = 0;
        public GraphPoint()
        {

        }
        public GraphPoint(double x, double y, string name)
        {
            this.X = x;
            this.Y = y;
            this.Name = name;

            ID = IDsCount++;
        }

        public GraphPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Name = "";
            ID = IDsCount++;
        }

        public static GraphPoint GetPointByName(string name, List<GraphPoint> points)
        {
            foreach (var point in points)
            {
                if (point.Name == name)
                    return point;
            }
            return null;
        }
        public static GraphPoint GetPointByPos(Point pos, List<GraphPoint> points)
        {
            foreach (var point in points)
            {
                if (pos.X - 20 < point.X && point.X < pos.X + 20 &&
                    pos.Y - 20 < point.Y && point.Y < pos.Y + 20)
                    return point;
            }
            return null;
        }

        public static GraphPoint GetPointByID(int ID, List<GraphPoint> points)
        {
            foreach (var point in points)
            {
                if (point.ID == ID)
                    return point;
            }
            return null;
        }

        public List<GraphLine> GetLines(List<GraphLine> graphLines)
        {
            List<GraphLine> result = new List<GraphLine>();
            foreach (var line in graphLines)
            {
                if (line.PointID1 == ID || line.PointID2 == ID)
                    result.Add(line);
            }

            return result;
        }

        public void SetCoordinates(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public Point GetCoordinates()
        {
            return new Point(X, Y);
        }

        public static GraphLine GetFarthestPoints(List<GraphLine> lines, List<GraphPoint> points)
        {
            GraphLine resultLine = null;
            double maxLength = 0;
            foreach(var line in lines)
            {
                if(line.GetLength(points) > maxLength)
                {
                    maxLength = line.GetLength(points);
                    resultLine = line;
                }
            }
            return resultLine;
        }
    }
    [Serializable]
    public class GraphLine
    {
        public int PointID1 { get; set; }
        public int PointID2 { get; set; }
        public int ID { get; set; }

        static private int IDsCount = 0;

        public GraphLine(int pID1, int pID2)
        {
            PointID1 = pID1;
            PointID2 = pID2;
            ID = IDsCount++;
        }
        public GraphLine()
        {

        }

        public static GraphLine GetLineByPoints(int id1, int id2, List<GraphLine> lines)
        {
            foreach (var line in lines)
            {
                if (line.PointID1 == id1 && line.PointID2 == id2)
                    return line;
            }
            return null;
        }

        public double GetLength(List<GraphPoint> points)
        {
            GraphPoint p1 = GraphPoint.GetPointByID(PointID1, points);
            GraphPoint p2 = GraphPoint.GetPointByID(PointID2, points);
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p2.Y, 2));
        }
    }
    [Serializable]
    public class Weight
    {
        public int LineID { get; set; }
        public double Value { get; set; }

        public Weight(int lID, double v)
        {
            LineID = lID;
            Value = v;
        }
        public Weight()
        {

        }

        public static Weight GetWeightByLine(int id, List<Weight> weights)
        {
            foreach (var weight in weights)
            {
                if (weight.LineID == id)
                    return weight;
            }
            return null;
        }
        public static Point CalculatePos(Point p1, Point p2)
        {
            if (p2.Y > p1.Y)
                return new Point((p1.X + p2.X) / 2 - 15, (p1.Y + p2.Y) / 2 + 15);
            else
                return new Point((p1.X + p2.X) / 2 + 15, (p1.Y + p2.Y) / 2 - 15);
        }
        public static Point CalculatePos(double x1, double y1, double x2, double y2)
        {
            if (y2 > y1)
                return new Point((x1 + x2) / 2 - 15, (y1 + y2) / 2 + 15);
            else
                return new Point((x1 + x2) / 2 + 15, (y1 + y2) / 2 - 15);
        }
    }
    #endregion


    [Serializable]
    public class Graph
    {
        bool IsDirectional = false;

        public string FileName = null;
        public string ShortFileName = null;

        public List<GraphPoint> points = new List<GraphPoint>();
        public List<GraphLine> lines = new List<GraphLine>();
        public List<Weight> weights = new List<Weight>();



        #region Events
        public delegate void PointSettedEventHandler(double x, double y, string name, Graph graph);
        public static event PointSettedEventHandler PointSettedEvent;

        public delegate void LineSettedEventHandler(Point point1, Point point2, Graph graph, double? weight = null);
        public static event LineSettedEventHandler LineSettedEvent;

        public static void CallPointSettedEvent(double x, double y, string name, Graph graph)
        {
            PointSettedEvent(x, y, name, graph);
        }
        public static void CallLineSettedEvent(Point point1, Point point2, Graph graph, double? weight = null)
        {
            LineSettedEvent(point1, point2, graph, weight);
        }
        #endregion

        #region PointMethods
        public void SetPoint(double x, double y, string name)
        {
            points.Add(new GraphPoint(x, y, name));
        }
        public void RemovePoint(string name)
        {
            GraphPoint p = GraphPoint.GetPointByName(name, points);
            if (p != null)
                points.Remove(p);
        }
        public void RemovePoint(GraphPoint point)
        {
            points.Remove(point);
        }
        public void RenamePoint(string current, string updated)
        {
            GraphPoint p = GraphPoint.GetPointByName(current, points);
            if (p != null)
            {
                p.Name = updated;
            }
        }
        #endregion

        #region LineMethods
        public void SetLine(string p1, string p2, double? weight = null)
        {
            GraphPoint P1 = GraphPoint.GetPointByName(p1, points);
            GraphPoint P2 = GraphPoint.GetPointByName(p2, points);
            if (P1 == null || P2 == null)
                return;
            GraphLine line = new GraphLine(P1.ID, P2.ID);
            lines.Add(line);

            if (weight != null)
            {
                weights.Add(new Weight(line.ID, weight.Value));
            }
        }
        public void SetLine(Point p1, Point p2, double? weight = null)
        {
            GraphPoint P1 = GraphPoint.GetPointByPos(p1, points);
            GraphPoint P2 = GraphPoint.GetPointByPos(p2, points);
            if (P1 == null || P2 == null)
                return;
            GraphLine line = new GraphLine(P1.ID, P2.ID);
            lines.Add(line);

            if (weight != null)
            {
                weights.Add(new Weight(line.ID, weight.Value));
            }
        }
        public void SetLine(int ID1, int ID2, Weight weight)
        {
            lines.Add(new GraphLine(ID1, ID2));
            weights.Add(weight);
        }
        public void SetLine(int ID1, int ID2, double? weight = null)
        {
            if (weight != null)
            {
                GraphLine line = new GraphLine(ID1, ID2);
                lines.Add(line);
                weights.Add(new Weight(line.ID, weight.Value));
            }
        }
        public void RemoveLine(string p1, string p2)
        {
            GraphPoint P1 = GraphPoint.GetPointByName(p1, points);
            GraphPoint P2 = GraphPoint.GetPointByName(p2, points);
            if (P1 != null && P2 != null)
            {
                GraphLine line = GraphLine.GetLineByPoints(P1.ID, P2.ID, lines);

                lines.Remove(line);

                Weight weight = Weight.GetWeightByLine(line.ID, weights);
                if (weight != null)
                    weights.Remove(weight);
            }

        }
        public void RemoveLine(GraphLine line)
        {
            lines.Remove(line);
        }
        #endregion


        #region FileIO


        [Serializable]
        public class GraphPropsFields : PropsFields
        {
            public Graph prop = null;
        }
        static BinProps<GraphPropsFields> GraphIOBin = new BinProps<GraphPropsFields>();
        static XmlProps<GraphPropsFields> GraphIOXml = new XmlProps<GraphPropsFields>();
        public void SaveGraph(string fileName, string ext)
        {
            if (ext == "dat")
            {
                GraphIOBin.Fields.BinFileName = fileName;
                GraphIOBin.Fields.prop = this;
                GraphIOBin.WriteBin();
            }
            else if (ext == "xml")
            {
                //TODO: Починить чертово сохранение в XML (какието проблемы с xmlns но я хз)
                GraphIOXml.Fields.XMLFileName = fileName;
                GraphIOXml.Fields.prop = this;
                GraphIOXml.WriteXml();

            }
        }
        public static Graph LoadGraph(string fileName, string ext)
        {
            if (ext == "dat")
            {
                GraphIOBin.Fields.BinFileName = fileName;
                GraphIOBin.ReadBin();
                return GraphIOBin.Fields.prop;
            }
            else if (ext == "xml")
            {
                GraphIOXml.Fields.XMLFileName = fileName;
                GraphIOXml.ReadXml();
                return GraphIOXml.Fields.prop;
            }
            else return null;
        }


        #endregion

        #region Algorithms
        Dictionary<GraphPoint, bool> used = new Dictionary<GraphPoint, bool>();
        bool dfs(GraphPoint u, GraphPoint p = null)
        {
            used[u] = true;

            GraphPoint w = new GraphPoint();

            foreach (var line in u.GetLines(lines))
            {
                if (line.PointID1 == u.ID)
                {
                    w = GraphPoint.GetPointByID(line.PointID2, points);
                }
                else if (line.PointID2 == u.ID)
                {
                    w = GraphPoint.GetPointByID(line.PointID1, points);
                }

                if (!used[w])
                {
                    if (dfs(w, u))
                        return true;
                }
                else if (w != p)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCycled()
        {
            foreach (var point in points)
            {
                used[point] = false;
            }
            if (!IsDirectional)
            {
                foreach (var point in points)
                {
                    if (!used[point])
                    {
                        if (dfs(point))
                            return true;
                    }
                }


            }



            return false;
        }

        public static List<GraphPoint> GraphLaying(Graph graph, Point StartPoint, double length)
        {
            //Деревья рисуем через правильные треугольники
            List<GraphPoint> result = new List<GraphPoint>();

            if (!graph.IsCycled())
            {
                Dictionary<GraphPoint, int> pointLinesCount = new Dictionary<GraphPoint, int>();
                foreach (var point in graph.points)
                {
                    pointLinesCount[point] = point.GetLines(graph.lines).Count;
                }
                GraphPoint maxLinedPoint = new GraphPoint();
                int count = graph.points.Count;
                if (count > 0) maxLinedPoint = graph.points[0];
                else return null;
                int maxLines = 0;
                foreach (var point in graph.points)
                {
                    //TODO: переделать для случая когда несколько точек с максмальным колиеством смежных
                    if (pointLinesCount[point] >= maxLines)
                    {
                        maxLinedPoint = point;
                        maxLines = pointLinesCount[point];
                    }
                }
                maxLinedPoint.X = StartPoint.X;
                maxLinedPoint.Y = StartPoint.Y;

                result.Add(maxLinedPoint);

                //TODO: напсиать расположние слоев для последующих точек
                List<GraphPoint> neighbors = new List<GraphPoint>();
                foreach(var line in maxLinedPoint.GetLines(graph.lines))
                {
                    if (line.PointID1 == maxLinedPoint.ID)
                        neighbors.Add(GraphPoint.GetPointByID(line.PointID2, graph.points));
                    else if (line.PointID2 == maxLinedPoint.ID)
                        neighbors.Add(GraphPoint.GetPointByID(line.PointID1, graph.points));

                }

                if (neighbors.Count > 0)
                    neighbors[0].SetCoordinates(
                        new Point(StartPoint.X - length / 2, StartPoint.Y + Math.Sqrt(0.75) * length));
                else goto otherPoints;
                if (neighbors.Count > 1)
                    neighbors[1].SetCoordinates(
                        new Point(StartPoint.X + length / 2, StartPoint.Y + Math.Sqrt(0.75) * length));
                else goto otherPoints;
                if (neighbors.Count > 2)
                    neighbors[2].SetCoordinates(
                        new Point(StartPoint.X - length / 2, StartPoint.Y - Math.Sqrt(0.75) * length));
                else goto otherPoints;
                if (neighbors.Count > 3)
                    neighbors[3].SetCoordinates(
                        new Point(StartPoint.X + length / 2, StartPoint.Y - Math.Sqrt(0.75) * length));
                else goto otherPoints;
                if (neighbors.Count > 4)
                    neighbors[4].SetCoordinates(new Point(StartPoint.X - length, StartPoint.Y));
                else goto otherPoints;
                if (neighbors.Count > 5)
                    neighbors[5].SetCoordinates(new Point(StartPoint.X + length, StartPoint.Y));
                result.AddRange(neighbors);

                otherPoints:
                if(count > 6)
                {
                    List<GraphPoint> otherNeighbors = new List<GraphPoint>();
                    int i = 0;
                    foreach(var p in neighbors)
                    {
                        if (i > 5)
                            otherNeighbors.Add(p);
                        i++;
                    }
                    i = 6;
                    foreach(var p in otherNeighbors)
                    {
                        GraphLine l = GraphPoint.GetFarthestPoints(graph.lines, neighbors);
                        GraphPoint p1 = GraphPoint.GetPointByID(l.PointID1, neighbors);
                        GraphPoint p2 = GraphPoint.GetPointByID(l.PointID2, neighbors);

                        p.X = (p1.X + p2.X) / 2;
                        p.Y = (p1.Y + p2.Y) / 2;

                        neighbors[i] = p;
                        i++;
                    }
                    result.AddRange(otherNeighbors);
                }
                
                foreach(var p in  neighbors)
                {
                    graph.RemovePoint(p);
                }
                List<GraphLine> usedLines = new List<GraphLine>();
                foreach(var p in neighbors)
                {
                    foreach(var line in p.GetLines(graph.lines))
                    {
                        usedLines.Add(line);
                    }
                }
                foreach (var line in usedLines)
                    graph.RemoveLine(line);

                //ОСТОРОЖНО РЕКУРСИЯ ДЛЯ ТОГО ЧТОБЫ СДЕЛАТЬ ТОЖЕ САМОЕ ДЛЯ ВСЕХ ВЕРШИН
                foreach(var point in neighbors)
                {
                   result.AddRange(GraphLaying(graph, new Point(point.X, point.Y), length));
                }
            }


            return result;
        }

        #endregion


    }

    public enum GraphEditMode
    {
        Point,
        Line
    }



}
