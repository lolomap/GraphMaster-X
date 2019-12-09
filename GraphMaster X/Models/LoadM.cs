using GraphMaster_X.Classes;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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


            var s = table.Rows;
            List<string> points = new List<string>();
            foreach(DataRow row in s)
            {
                points.Add((string)row.ItemArray[0]);
            }

            var ss = table.Columns;
            List<string> columnsCaptions = new List<string>();
            int i = 0;
            foreach(DataColumn column in ss)
            {
                if (i != 0)
                    columnsCaptions.Add(column.Caption);
                i++;
            }
            points.AddRange(columnsCaptions);
            
            foreach(var point in points)
            {
                GraphPoint p = new GraphPoint
                {
                    Name = (string)point,
                    //TODO: Придумать адекватный алгоритм генерации положения точки
                    X = 0,
                    Y = 0
                };
                graph.points.Add(p);
            }


            return graph;
        }

        private DataTable GetTable(string fileName, string sheetName)
        {
            OleDbConnection MyConnection;
            DataSet DtSet;
            OleDbDataAdapter MyCommand;

            MyConnection = new OleDbConnection(@"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                                fileName + ";Extended Properties=Excel 8.0;");
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
