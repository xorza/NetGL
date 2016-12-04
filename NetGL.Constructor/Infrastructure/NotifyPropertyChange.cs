using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetGL.Constructor.Infrastructure {
    public class NotifyPropertyChange : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propname = null) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
        }
    }
}
