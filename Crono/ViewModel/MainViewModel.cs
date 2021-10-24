using Crono.Configuration;
using Crono.Model;
using Crono.Service;
using Crono.Utility;
using Crono.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Crono.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        private int _resWidth;  //Window width
        private int _resHeight; //Window height
        private int _width; //Canvas width
        private ICronoConfig _config;
        private bool _commessaArgs = false;
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                ServiceBus.RaiseResizeHeightEvent(value);
                RaisePropertyChanged("Width");
            }
        }
        public int ResWidth
        {
            get { return _resWidth; }
            set
            {
                _resWidth = value;
                Width = value;
                ServiceBus.RaiseResizeWidthEvent(value);
                RaisePropertyChanged("ResWidth");
            }
        }
        public int ResHeight
        {
            get { return _resHeight; }
            set
            {
                _resHeight = value;
                RaisePropertyChanged("ResHeight");
            }
        }

        private IFrameNavigationService _navigationService;
        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand(
                    () =>
                    {
                        _navigationService.NavigateTo("Commesse");
                    }));
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ICronoConfig config, IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _config = config;
            ResHeight = config.ResHeight;
            ResWidth = config.ResWidth;
            Width = config.ResWidth;
            //Content = (CommessePage)AppContainer.Container.GetInstance(typeof(CommessePage));
            if (!string.IsNullOrEmpty(_config.CodiceCommessaArgs)) _commessaArgs = true;
            //ServiceBus.SubscribeToCommessaChange(this, LoadFasi);
            //ServiceBus.SubscribeToChangeContent(this, ChangeContent);
        }
        
    }
}