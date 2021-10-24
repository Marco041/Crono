using Crono.Configuration;
using Crono.Configuration.Log;
using Crono.Model;
using Crono.Repository;
using Crono.Service;
using Crono.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.ViewModel
{
    public class FasiViewModel : ViewModelBase
    {
        #region Properties
        private IFrameNavigationService _navService;
        private readonly int _canvasMarginLeft; //Left list width 
        private readonly int startWidth;    //Initial canvas width
        private ICronoConfig _config;    
        private ObservableCollection<DayBlockViewModel> _dayBlocks; //List of days to show
        private IRepository _repository;    
        private int _startRow;  //Row index of the first task
        private int _rowHeight; //height of the task
        private int _rowMargin; //Vertical margin of 2 tasks
        private double _dayWidth;   //Day column width
        private DateTime _dateStart;    //First day of the timespan
        private DateTime _dateEnd;  //Lst day of the timespan
        private int _dayShift;  //Timespan shift
        private int _resWidth;  //Window width
        private int _resHeight; //Window height
        private int _width; //Canvas width
        private int _height;    //Canvas height
        private double _scaleCanvasX;   //Value to expand canvas on window resize     
        private Commessa _commessaCorrente; //Commission selected
        private double _opacity;    //Scrollbar opacity
        private string _message;    //Modal message
        private bool _showMessageModal; //Show modal mssage flag
        private IDaysService _dayService { get; set; }   //Day columns
        private ITempRepositoryService _tempRepoService { get; set; }    //Im memery repositoty for edit. Flushed on save
        private IAnimationService _aniService { get; set; }  //Loading animation service
        private LogSplitter _log;   //logger
        private DateTime _newStart;
        private bool _readOnlyMode;

        public ObservableCollection<DayBlockViewModel> DayBlocks
        {
            get { return _dayBlocks; }
            set { _dayBlocks = value; RaisePropertyChanged("DayBlocks"); }
        }

        public DateTime NewStartDate
        {
            get { return _newStart; }
            set { _newStart = value; RaisePropertyChanged("NewStartDate"); }
        }

        public bool IsLoading
        {
            get { return _aniService.IsLoading; }
        }

        public double ScaleCanvasX
        {
            get { return _scaleCanvasX; }
            set { _scaleCanvasX = value; RaisePropertyChanged("ScaleCanvasX"); }
        }
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                if (_dayBlocks != null)
                {
                    while (_dayBlocks.Last().X + _dayWidth < value)
                        AddDayOnResize();
                    while (_dayBlocks.Last().X + _dayWidth > value)
                        RemoveDayOnResize();
                    _dateEnd = _dayBlocks.Last().Day;
                    _dayService.EndDate = _dateEnd;
                    ServiceBus.RaiseDateEndChange(_dateEnd);
                }
                RaisePropertyChanged("Width");
            }
        }
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
            }
        }
        public int ResWidth
        {
            get { return _resWidth; }
            set
            {
                _resWidth = value;
                Width = value;
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
        public double Opacity
        {
            get { return _opacity; }
            set { _opacity = value; RaisePropertyChanged("Opacity"); }
        }
        public bool ShowMessageModal
        {
            get { return _showMessageModal; }
            set { _showMessageModal = value; RaisePropertyChanged("ShowMessageModal"); }
        }
        public string Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged("Message"); }
        }
        
        public bool ReadOnlyMode
        {
            get { return _readOnlyMode; }
            set
            {
                _readOnlyMode = value;
                RaisePropertyChanged("ReadOnlyMode");
            }
        }

        public RelayCommand BackCommand { get; set; }
        public RelayCommand PreviousCommand { get; set; }
        public RelayCommand NextCommand { get; set; }
        public TaskBlockViewModel ConstraintSource { get; set; }
        public RelayCommand SetIsOpenModalNewTask { get; set; }   //New commission creation
        public RelayCommand SaveNewTaskCommand { get; set; }   //New commission save
        public RelayCommand FlushCommand { get; set; }   
        public RelayCommand SetShowMessageModal { get; set; }   
        public RelayCommand SetNewStartDateCommand { get; set; } 
        public List<Commessa> ListaCommesse { get; set; }
        public Commessa CommessaCorrente
        {
            get
            {
                return _commessaCorrente;
            }
            set
            {
                _commessaCorrente = value;
                RaisePropertyChanged("CommessaCorrente");
            }
        }
        #endregion

        public FasiViewModel(ICronoConfig config, IDaysService service, ITempRepositoryService tempRepoService, IAnimationService aniService, IFrameNavigationService navService)
        {
            _navService = navService;
            _aniService = aniService;
            _aniService.PropertyChanged += (e, o) =>
                RaisePropertyChanged("IsLoading");
            _canvasMarginLeft = config.CanvasReduceWidth; //horizontal timespan shift
            _scaleCanvasX = 1;
            _startRow = 0;
            Opacity = 1.0;
            _config = config;
            _dateStart = _config.DateStart;
            _dateEnd = _config.DateEnd;
            _rowHeight = config.RowHeight;
            _dayShift = config.DayShift;
            ResHeight = config.ResHeight;
            ResWidth = config.ResWidth;
            Width = config.ResWidth;
            startWidth = config.ResWidth;
            _rowMargin = config.RowMargin;
            _dayWidth = config.DayWidth;
            _repository = config.Repository;
            _dayService = service;
            _tempRepoService = tempRepoService;
            _log = config.Logger;
            NewStartDate = _dateStart;
            ListaCommesse = new List<Commessa>();
            NextCommand = new RelayCommand(ShiftRightTime);
            PreviousCommand = new RelayCommand(ShiftLeftTime);
            FlushCommand = new RelayCommand(FlushAll);
            SetIsOpenModalNewTask = new RelayCommand(SetModalNewTask);
            SetShowMessageModal = new RelayCommand(ShowModal);
            SetNewStartDateCommand = new RelayCommand(ChangeStartDate);
            BackCommand = new RelayCommand(GoBack);
            ServiceBus.SubscribeToResizeHeightEvent(this, (int wh) => { ResHeight = wh; });
            ServiceBus.SubscribeToResizeWidthEvent(this, (int wh) => { ResWidth = wh; });
            ServiceBus.SubscribeToCommessaChange(this, CommessaChanged);
            DayBlocks = _dayService.DrawDays(_startRow, _canvasMarginLeft);  //days loading
        }

        public void CommessaChanged(CommessaDto c)
        {
            NewStartDate = c.From;
            ChangeStartDate();
            ReadOnlyMode = c.ReadOnlyMode;
            CommessaCorrente = c.Comm;
            ReadOnlyMode = c.ReadOnlyMode;
        }

        private void GoBack()
        {
            _navService.GoBack();
            //ServiceBus.RaiseChangeContent();
        }

        /// <summary>
        /// Add a new day to timespan after window resize
        /// </summary>
        private void AddDayOnResize()
        {
            var lastBlock = _dayBlocks.Last();
            _dayBlocks.Add(new DayBlockViewModel(lastBlock.Y, lastBlock.X + _dayWidth, lastBlock.Width, lastBlock.Day.AddDays(1), Util.IsHoliday(lastBlock.Day.AddDays(1))));
            RaisePropertyChanged("DayBlocks");
        }

        /// <summary>
        /// Remove a day after window resize
        /// </summary>
        private void RemoveDayOnResize()
        {
            _dayBlocks.RemoveAt(_dayBlocks.Count - 1);
            RaisePropertyChanged("DayBlocks");
        }

        /// <summary>
        /// Message modal close
        /// </summary>
        private void ShowModal()
        {
            ShowMessageModal = false;
            Message = string.Empty;
        }

        /// <summary>
        /// Open new task modal using message bus
        /// </summary>
        private async void SetModalNewTask()
        {
            try
            {
                if (CommessaCorrente != null)
                {
                    var newTask = new CronoTask() { Id = _tempRepoService.GetNewMaxId(), IdFase = _tempRepoService.GetNewMaxId(), StartDate = DateTime.Now.Date, EndDate = DateTime.Now.Date, CodiceCommessa = CommessaCorrente.Codice };
                    var taskList = await _repository.GetAllTask(CommessaCorrente.Codice);
                    ServiceBus.RaiseOpenModal(new TaskDetailsDto(new List<CronoTask> { newTask }, taskList, true, ReadOnlyMode));
                }
                else
                    ShowMessage("Selezionare prima una commessa");
            }
            catch (Exception e)
            {
                ShowMessage("Errore");
                _log.Error("Errore durante l'apertura della modale nuova fase", e);
            }
        }

       
        public void ChangeStartDate()
        {
            int timespan = (int)(_dateEnd - _dateStart).TotalDays;
            int difference = (int)(NewStartDate - _dateStart).TotalDays;
            _dateStart = NewStartDate;
            _dateEnd = _dateStart.AddDays(timespan);
            _dayService.StartDate = _dateStart;
            _dayService.EndDate = _dateEnd;
            _dayService.UpdateDays(difference);
            ServiceBus.RaiseDateStartChange(difference);
        }

        /// <summary>
        /// Right shift timespan
        /// </summary>
        public void ShiftRightTime()
        {
            try
            {
                DateTime old = _dateStart;
                _dateStart = _dateStart.AddDays(_dayShift);
                _dateEnd = _dateEnd.AddDays(_dayShift);
                _dayService.UpdateDays(_dayShift);
                _dayService.StartDate = _dateStart;
                _dayService.EndDate = _dateEnd;
                NewStartDate = _dateStart;
                ServiceBus.RaiseDateShiftChange(old);            
            }
            catch (Exception e)
            {
                ShowMessage("Errore durante il cambio data");
                _log.Error("Errore durante il cambio data", e);
            }
        }

        /// <summary>
        /// Left shift timespan
        /// </summary>
        public void ShiftLeftTime()
        {
            try
            {
                DateTime old = _dateStart;
                _dateStart = _dateStart.AddDays(-_dayShift);
                _dateEnd = _dateEnd.AddDays(-_dayShift);
                _dayService.UpdateDays(-_dayShift);
                _dayService.StartDate = _dateStart;
                _dayService.EndDate = _dateEnd;
                NewStartDate = _dateStart;
                ServiceBus.RaiseDateShiftChange(old);    
            }
            catch (Exception e)
            {
                ShowMessage("Errore durante il cambio data");
                _log.Error("Errore durante il cambio data", e);
            }
        }

        /// <summary>
        /// Salvataggio dei cambiamenti fatti nel repository
        /// </summary>
        private async void FlushAll()
        {
            _aniService.IsLoading = true;
            bool success = true;
            try
            {
                foreach (CronoTask item in _tempRepoService.GetAllAdded())
                {
                    int id = await _repository.InsertTask(item);
                    if (id > 0)
                    {
                        item.Id = id;
                    }
                    else
                        success = false;
                }

                _tempRepoService.GetAllAdded().ForEach(async i => await _repository.InsertConstraints(i));  //New constraint insertion        

                foreach (CronoTask item in _tempRepoService.GetAllModifed())
                {
                    if (await _repository.UpdateTask(item) == 0)
                        success = false;

                }
                foreach (CronoTask item in _tempRepoService.GetAllDeleted())
                {
                    if (!await _repository.DeleteTask(item)) success = false;
                }
                if (success) _tempRepoService.ClearAll();
            }
            catch (Exception e)
            {
                success = false;
                _log.Error("Errore durante il salvataggio", e);
            }
            if (success)
                ShowMessage("Modifiche salvate con successo");
            else
                ShowMessage("Errore durante il salvataggio");

            _aniService.IsLoading = false;
        }

        /// <summary>
        /// Show message modal
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMessage(string msg)
        {
            Message = msg;
            ShowMessageModal = true;
        }
    }
}
