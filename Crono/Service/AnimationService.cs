using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Service
{
    public class AnimationService : INotifyPropertyChanged, IAnimationService
    {
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyPropertyChanged("IsLoading"); }
        }
    }
}
