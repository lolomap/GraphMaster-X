using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Serialization;

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

    }

    public enum GraphEditMode
    {
        Point,
        Line
    }



}
