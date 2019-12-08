﻿using GraphMaster_X.Classes;
using Microsoft.Win32;
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                Graph graph = ConvertToGraph(fileName);
            }
        }



        private Graph ConvertToGraph(string fileName)
        {
            Graph graph = new Graph();

            return graph;
        }
    }
}
