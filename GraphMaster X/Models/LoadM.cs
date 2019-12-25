using GraphMaster_X.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Xml.Serialization;

namespace GraphMaster_X.Models
{
    public class LoadM
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Graph));

        

        public void OpenFile()
        {
            Graph graph = new Graph();
            WindowControler.windowsParams.Add(graph);
            WindowControler.windowsParams.Add(WindowControler.EditorOpenParams.OPEN);
            WindowControler.CallParamsUpdatedEvent();
            ControlerEnventHandler.CallShowWindow(1);
            ControlerEnventHandler.CallCloseWindow(0);
        }

        public void Create()
        {
            Graph graph = new Graph();
            WindowControler.windowsParams.Add(graph);
            WindowControler.windowsParams.Add(WindowControler.EditorOpenParams.NO);
            ControlerEnventHandler.CallShowWindow(1);
            ControlerEnventHandler.CallCloseWindow(0);
        }

        public void Convert()
        {
            Graph graph = new Graph();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel таблица (*.xlsx; *xls)|*xlsx;*xls";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
               graph = ConvertToGraph(fileName);
            }
            WindowControler.windowsParams.Add(graph);
            WindowControler.windowsParams.Add(WindowControler.EditorOpenParams.NO);
            ControlerEnventHandler.CallShowWindow(1);
            ControlerEnventHandler.CallCloseWindow(0);
        }



        private Graph ConvertToGraph(string fileName)
        {
            Graph graph = new Graph();

            var table = GetTable(fileName, "Таблица 1");


            var rows = table.Rows;
            List<string> RowPoints = new List<string>();
            List<string> rowCaptions = new List<string>();
            foreach(DataRow row in rows)
            {
                if (row.ItemArray[0].GetType() == typeof(DBNull))
                { rowCaptions.Add(string.Empty); continue; }
                rowCaptions.Add((string)row.ItemArray[0]);
            }
            RowPoints.AddRange(rowCaptions);

            var columns = table.Columns;
            List<string> columnsCaptions = new List<string>();
            List<string> ColumnPoints = new List<string>();
            columns.RemoveAt(0);
            foreach(DataColumn column in columns)
            {
                columnsCaptions.Add(column.Caption);
            }
            ColumnPoints.AddRange(columnsCaptions);


            List<GraphPoint> rp = new List<GraphPoint>();
            List<GraphPoint> cp = new List<GraphPoint>();

            foreach(var point in RowPoints)
            {
                GraphPoint p = new GraphPoint
                {
                    Name = point,
                    //TODO: Придумать адекватный алгоритм генерации положения точки
                    X = 0,
                    Y = 0
                };
                rp.Add(p);
                graph.points.Add(p);
            }
            foreach (var point in ColumnPoints)
            {
                GraphPoint p = new GraphPoint
                {
                    Name = point,
                    //TODO: Придумать адекватный алгоритм генерации положения точки
                    X = 0,
                    Y = 0
                };
                cp.Add(p);
                graph.points.Add(p);
            }

            //Врооде как это должно добавить линии соединения всех вершин если есть вес
            //UPD: TODO: не работает, исправь
            int i = 0;
            foreach (DataRow row in rows)
            {
                List<string> rowContent = new List<string>();//((string[])row.ItemArray);
                foreach(var k in row.ItemArray)
                {
                    if (k.GetType() != typeof(DBNull))
                        rowContent.Add((string)k);
                    else rowContent.Add(string.Empty);
                }
                rowContent.RemoveAt(0);

                int j = 0;
                foreach (var item in rowContent)
                {
                    if (item != string.Empty)
                    {
                        graph.SetLine(rp[i].ID, cp[j].ID, System.Convert.ToDouble(item));
                        
                    }

                    j++;
                }
                i++;
            }
            //TODO: проверить работоспособность
            List<GraphPoint> LayedPoints = Graph.GraphLaying(graph, new Point(300, 300), 50);
            graph.points = LayedPoints;

            return graph;
        }

        

        private DataTable GetTable(string fileName, string sheetName)
        {
            OleDbConnection MyConnection;
            DataSet DtSet;
            OleDbDataAdapter MyCommand;

            MyConnection = new OleDbConnection(@"provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                                fileName + ";Extended Properties=\"excel 8.0;hdr=no;IMEX=1\";");
            MyCommand = new OleDbDataAdapter("select * from ["+sheetName+"$]", MyConnection);
            MyCommand.TableMappings.Add("Table", "Net");
            DtSet = new DataSet();
            MyCommand.Fill(DtSet);

            var table = DtSet.Tables[0];

            MyConnection.Close();

            return table;
        }
    }
}
