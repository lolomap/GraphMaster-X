using GalaSoft.MvvmLight.Command;
using GraphMaster_X.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GraphMaster_X.ViewModels
{
    public class LoadVM : INotifyPropertyChanged
    {
        //This class call methods by commands

        public ICommand OpenFileCmd { get; private set; }
        public ICommand CreateCmd { get; private set; }
        public ICommand ConvertCmd { get; private set; }





        public LoadVM()
        {
            OpenFileCmd = new RelayCommand(Model.OpenFile);
            CreateCmd = new RelayCommand(Model.Create);
            ConvertCmd = new RelayCommand(Model.Convert);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly LoadM Model = new LoadM();
    }
}
