using GalaSoft.MvvmLight.Command;
using GraphMaster_X.Classes;
using GraphMaster_X.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace GraphMaster_X.ViewModels
{
    public class EditorVM : INotifyPropertyChanged
    {
        const string DefaultWindowTitle = "Graph Master X";

        #region Properties
        private string _WindowTitle;
        public string WindowTitle
        {
            get { return _WindowTitle; }
            set { _WindowTitle = value; NotifyOfPropertyChange(); }
        }

        private Graph _CurrentGraph;
        public Graph CurrentGraph
        {
            get { return _CurrentGraph; }
            set { _CurrentGraph = value; NotifyOfPropertyChange(); }
        }

        private GraphEditMode _Mode;
        public GraphEditMode Mode
        {
            get { return _Mode; }
            set
            {
                _Mode = value;
                NotifyOfPropertyChange();
            }
        }

        private bool _PointChoosed;
        public bool PointChoosed
        {
            get { return _PointChoosed; }
            set
            {
                if (_PointChoosed != value)
                {
                    _PointChoosed = value;
                    LineChoosed = !value;
                }
                NotifyOfPropertyChange();
            }
        }

        private bool _LineChoosed;
        public bool LineChoosed
        {
            get { return _LineChoosed; }
            set
            {
                if (_LineChoosed != value)
                {
                    _LineChoosed = value;
                    PointChoosed = !value;
                }
                NotifyOfPropertyChange();
            }
        }

        private Visibility _IsNameInputShow;
        public Visibility IsNameInputShow
        {
            get { return _IsNameInputShow; }
            set { IsNameInputShowSetter(value); }
        }
        public void IsNameInputShowSetter(Visibility value)
        {
            _IsNameInputShow = value; NotifyOfPropertyChange("IsNameInputShow");
        }

        private Thickness _NameInputPos;
        public Thickness NameInputPos
        {
            get { return _NameInputPos; }
            set { NameInputPosSetter(value); }
        }
        public void NameInputPosSetter(Thickness value)
        {
            _NameInputPos = value; NotifyOfPropertyChange("NameInputPos");
        }

        private bool _IsInput;
        public bool IsInput
        {
            get { return _IsInput; }
            set { IsInputSetter(value); }
        }
        public void IsInputSetter(bool value)
        {
            _IsInput = value; NotifyOfPropertyChange("IsInput");
        }

        private string _InputText;
        public string InputText
        {
            get { return _InputText; }
            set
            {
                _InputText = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Commands
        public ICommand PointModeCmd { get; private set; }
        public ICommand LineModeCmd { get; private set; }
        public ICommand KeyDownHandler { get; private set; }

        public ICommand SaveGraph { get; private set; }
        public ICommand SaveGraphAs { get; private set; }

        public ICommand OpenGraph { get; private set; }
        #endregion

        public EditorVM()
        {

            #region DefaultValues

            WindowTitle = DefaultWindowTitle;
            CurrentGraph = new Graph();
            IsNameInputShow = Visibility.Hidden;
            IsInput = false;
            PointChoosed = true;

            #endregion

            #region EventInitialize

            ControlerEnventHandler.ShowWindowEvent += LoadGraph;
            
            EditorM.SetNameInputVisEvent += IsNameInputShowSetter;
            EditorM.SetNameInputPosEvent += NameInputPosSetter;
            EditorM.SetIsInputEvent += IsInputSetter;

            PointNameInputingEvent += EditorM.PointNameInputing;
            LineNameInputingEvent += EditorM.LineNameInputing;

            Model.FileSavedEvent += () => WindowTitle = DefaultWindowTitle + " ("
                                            + CurrentGraph.ShortFileName + ")";
            Model.FileNotSavedEvent += () =>
            {
                if (CurrentGraph.ShortFileName == null || CurrentGraph.ShortFileName == string.Empty)
                    WindowTitle = DefaultWindowTitle + " *";
                else WindowTitle = DefaultWindowTitle + " (" + CurrentGraph.ShortFileName + "*)";
            };
            Model.FileOpenedEvent += BuildOpenedGraph;

            #endregion

            #region CommandInitialize

            PointModeCmd = new RelayCommand<GraphEditMode>(param => Mode = GraphEditMode.Point);
            LineModeCmd = new RelayCommand<GraphEditMode>(param => Mode = GraphEditMode.Line);
            
            SaveGraphAs = new RelayCommand<Graph>(param => Model.SaveGraphAs(CurrentGraph));
            SaveGraph = new RelayCommand<Graph>(param => Model.SaveGraph(CurrentGraph));
            OpenGraph = new RelayCommand(Model.OpenGraph);

            #endregion

        }

        private void LoadGraph(int id)
        {
            if (id == 1)
                CurrentGraph = (Graph)WindowControler.windowsParams[0];
        }

        private void BuildOpenedGraph(Graph graph)
        {
            CurrentGraph.FileName = graph.FileName;
            CurrentGraph.ShortFileName = graph.ShortFileName;

            CurrentGraph.points.Clear();
            CurrentGraph.lines.Clear();
            CurrentGraph.weights.Clear();
            
            foreach (var point in graph.points)
            {
                CurrentGraph.SetPoint(point.X, point.Y, point.Name);
                PointNamePlacementEvent(point.Name, new Point(point.X, point.Y));
            }
            foreach (var line in graph.lines)
            {
                Weight weight = Weight.GetWeightByLine(line.ID, graph.weights);
                CurrentGraph.SetLine(line.PointID1, line.PointID2, weight);
                Point pos1 = new Point(GraphPoint.GetPointByID(line.PointID1, graph.points).X,
                    GraphPoint.GetPointByID(line.PointID1, graph.points).Y);
                Point pos2 = new Point(GraphPoint.GetPointByID(line.PointID2, graph.points).X,
                    GraphPoint.GetPointByID(line.PointID2, graph.points).Y);
                PointNamePlacementEvent(weight.Value.ToString(), Weight.CalculatePos(pos1, pos2));
            }
        }

        #region Events
        public delegate void PointNameInputingEventHandler(Point point);
        public static event PointNameInputingEventHandler PointNameInputingEvent;

        public delegate void PointNamePlacementEventHandler(string Text, Point point);
        public event PointNamePlacementEventHandler PointNamePlacementEvent;

        public void CallInputCompleteEvent(Point pointPlace)
        {
            CurrentGraph.SetPoint(pointPlace.X, pointPlace.Y, InputText);
            Model.NameInputClose();
            PointNamePlacementEvent(InputText, pointPlace);
            Model.CallFileNotSavedEvent();
        }

        public void CallLineInputCompleteEvent(Point p1, Point p2)
        {
            if (InputText == "")
                CurrentGraph.SetLine(p1, p2);
            else
                try
                { CurrentGraph.SetLine(p1, p2, Convert.ToDouble(InputText)); }
                catch (Exception e)
                { MessageBox.Show("Error" + e.Message); CurrentGraph.SetLine(p1, p2); }
            Model.NameInputClose();
            PointNamePlacementEvent(InputText, Weight.CalculatePos(p1, p2));
            Model.CallFileNotSavedEvent();
        }

        public void CallPointNameInputingEvent(Point point)
        {
            PointNameInputingEvent(point);
        }


        public delegate void LineNameInputingEventHandler(Point point);
        public static event LineNameInputingEventHandler LineNameInputingEvent;

        public void CallLineNameInputingEvent(Point point)
        {
            LineNameInputingEvent(point);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private EditorM Model = new EditorM();
    }
}
