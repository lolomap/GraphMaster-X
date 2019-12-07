using GraphMaster_X.Classes;
using Microsoft.Win32;
using System.Windows;

namespace GraphMaster_X.Models
{
    public class EditorM
    {
        #region Methods
        public void SetPoint(double x, double y, string name, Graph graph)
        {
            graph.SetPoint(x, y, name);
        }
        public void SetLine(Point point1, Point point2, Graph graph, double? weight = null)
        {
            graph.SetLine(point1, point2, weight);
        }
        
        public static void PointNameInputing(Point point)
        {
            SetNameInputVisEvent(Visibility.Visible);
            SetNameInputPosEvent(new Thickness(point.X + 20, point.Y - 10, 100, 20));
            SetIsInputEvent(true);
        }

        public static void LineNameInputing(Point point)
        {
            SetNameInputVisEvent(Visibility.Visible);
            SetNameInputPosEvent(new Thickness(point.X, point.Y, 100, 20));
            SetIsInputEvent(true);
        }

        public void NameInputClose()
        {
            SetNameInputVisEvent(Visibility.Hidden);
            SetIsInputEvent(false);
        }


        public void SaveGraph(Graph graph)
        {
            if (graph.FileName == null || graph.FileName == string.Empty)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = "dat";
                saveFileDialog.Filter = "Бинарный файл (*.dat)|*.dat|XML файл (*.xml)|*.xml";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    graph.FileName = fileName;
                    string ext = saveFileDialog.SafeFileName.Substring(saveFileDialog.SafeFileName.IndexOf('.') + 1);
                    graph.ShortFileName = saveFileDialog.SafeFileName;
                    graph.SaveGraph(fileName, ext);
                    FileSavedEvent();
                }
            }
            else
            {
                string ext = graph.ShortFileName.Substring(graph.ShortFileName.IndexOf('.') + 1);
                graph.SaveGraph(graph.FileName, ext);
                FileSavedEvent();
            }

        }
        public void SaveGraphAs(Graph graph)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = "dat";
            saveFileDialog.Filter = "Бинарный файл (*.dat)|*.dat|XML файл (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                graph.FileName = fileName;
                graph.ShortFileName = saveFileDialog.SafeFileName;
                string ext = saveFileDialog.SafeFileName.Substring(saveFileDialog.SafeFileName.IndexOf('.') + 1);
                graph.SaveGraph(fileName, ext);
                FileSavedEvent();
            }
        }
       
        public void OpenGraph()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Графы (*.xml;*.dat)|*.xml;*.dat";
            if(openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string ext = openFileDialog.SafeFileName.Substring(openFileDialog.SafeFileName.IndexOf('.') + 1);
                Graph.LoadGraph(fileName, ext);
            }
        }

        #endregion

        public EditorM()
        {
            Graph.PointSettedEvent += SetPoint;
            Graph.LineSettedEvent += SetLine;
        }

        #region Events
        public delegate void SetNameInputVisEventHandler(Visibility value);
        public static event SetNameInputVisEventHandler SetNameInputVisEvent;

        public delegate void SetNameInputPosEventHandler(Thickness value);
        public static event SetNameInputPosEventHandler SetNameInputPosEvent;

        public delegate void SetIsInputEventHandler(bool value);
        public static event SetIsInputEventHandler SetIsInputEvent;

        public delegate void FileSavedEventHandler();
        public event FileSavedEventHandler FileSavedEvent;

        public delegate void FileNotSavedEventHandler();
        public event FileNotSavedEventHandler FileNotSavedEvent;
        public void CallFileNotSavedEvent() { FileNotSavedEvent(); }
        #endregion

    }
}
