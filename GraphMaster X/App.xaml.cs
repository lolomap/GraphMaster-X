using GraphMaster_X.Classes;
using GraphMaster_X.Views;
using System.Windows;

namespace GraphMaster_X
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly WindowControler controler = new WindowControler();
        readonly int LoadWinId;
        readonly int EditorWinId;
        public App()
        {

            ControlerEnventHandler.ShowWindowEvent += controler.ShowWindow;
            ControlerEnventHandler.CloseWindowEvent += controler.CloseWindow;


            LoadWinId = controler.CreateWindow<LoadV>();
            EditorWinId = controler.CreateWindow<EditorV>();
            controler.ShowWindow(LoadWinId);
        }
        ~App()
        {
            for (int i = 0; i <= controler.IdCount; i++)
                controler.CloseWindow(i);
        }
    }
}
