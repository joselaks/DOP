using System.ComponentModel;

namespace DataObra.Sistema
{
    public class ObjetoNotificable : INotifyPropertyChanged
    {
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
