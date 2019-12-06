using GraphMaster_X.ViewModels;
using System.Windows;

namespace GraphMaster_X
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class LoadV : Window
    {
        public LoadV()
        {
            LoadVM ViewModel = new LoadVM();
            InitializeComponent();
            DataContext = ViewModel;
        }


    }
}
