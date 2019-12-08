using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GraphMaster_X.Classes
{
    public class WindowControler
    {
        public enum EditorOpenParams
        {
            NO,
            OPEN
        }

        public delegate void ParamsUpdatedEventHandler();
        public static event ParamsUpdatedEventHandler ParamsUpdatedEvent;
        public static void CallParamsUpdatedEvent() { ParamsUpdatedEvent(); }

        public static List<object> windowsParams = new List<object>();
        public Dictionary<int, Window> windows { get; private set; } = new Dictionary<int, Window>();
        public int IdCount { get; private set; } = -1;

        public int CreateWindow<T>() where T : Window, new()
        {
            windows.Add(++IdCount, new T());
            return IdCount;
        }
        public void ShowWindow(int id)
        {
            if (windows.Keys.Contains(id))
                windows[id].Show();
        }
        public void CloseWindow(int id)
        {
            if (!windows.Keys.Contains(id))
                return;
            windows[id].Close();
            windows.Remove(id);
            IdCount--;
        }
        public void HideWindow(int id)
        {
            if (windows.Keys.Contains(id))
                windows[id].Hide();
        }

    }
    public class ControlerEnventHandler
    {

        public delegate void ShowWindowEventHandler(int id);
        public static event ShowWindowEventHandler ShowWindowEvent;

        public delegate void CloseWindowEventHandler(int id);
        public static event CloseWindowEventHandler CloseWindowEvent;

        public static void CallShowWindow(int id)
        {
            ShowWindowEvent(id);
        }

        public static void CallCloseWindow(int id)
        {
            CloseWindowEvent(id);
        }
    }
}
