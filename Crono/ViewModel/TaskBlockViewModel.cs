using Crono.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Crono.ViewModel
{
    /// <summary>
    /// Single phase
    /// </summary>
    public class TaskBlockViewModel : ViewModelBase
    {
        private int _group; //phase with the same contraint have the same group number
        private double _x;  
        private double _y;
        private double _xRightConstraint;
        private double _yTopConstraint;
        private double _width;
        private double _height;
        private int _zIndex;
        private Brush _background;
        public CronoTask TaskModel { get; set; }
        public double PositionOrder { get; set; }
        private ObservableCollection<ConstraintViewModel> _constraintList {get;set;}
        private ObservableCollection<ConstraintViewModel> _masters {get;set;}   //Task from which the constaint starts
        private bool _constraintLine;
        private bool _openDetails;
        private string _tempName;
        private DateTime _tempStartDate;
        private DateTime _tempEndDate;

        
        public bool Deletable { get; set; }

        public string TempName
        {
            get { return _tempName; }
            set
            {
                _tempName = value;
                RaisePropertyChanged("TempName");
            }
        }
        public DateTime TempStartDate
        {
            get { return _tempStartDate; }
            set
            {
                _tempStartDate = value;
                RaisePropertyChanged("TempStartDate");
            }
        }
        public DateTime TempEndDate
        {
            get { return _tempEndDate; }
            set
            {
                _tempEndDate = value;
                RaisePropertyChanged("TempEndDate");
            }
        }
        public int Group
        {
            get { return _group; }
            set
            {
                _group = value;
                RaisePropertyChanged("Group");
            }
        }

        public bool OpenDetails
        {
            get { return _openDetails; }
            set
            {
                _openDetails = value;
                RaisePropertyChanged("OpenDetails");
            }
        }

        public bool ConstraintLine
        {
            get { return _constraintLine; }
            set
            {
                _constraintLine = value;
                RaisePropertyChanged("ConstraintLine");
            }
        }
        
        public ObservableCollection<ConstraintViewModel> Masters
        {
            get { return _masters; }
            set
            {
                _masters = value;
                RaisePropertyChanged("Masters");
            }
        }

        public ObservableCollection<ConstraintViewModel> ConstraintList
        {
            get { return _constraintList; }
            set
            {
                _constraintList = value;
                if (_constraintList.Count == 0) ConstraintLine = false;
                RaisePropertyChanged("ConstraintList");
            }
        }


        public int Zindex
        {
            get { return _zIndex; }
            set
            {
                _zIndex = value;
                RaisePropertyChanged("Zindex");
            }
        }

        public double XRightConstraint
        {
            get { return _xRightConstraint; }
            set
            {
                _xRightConstraint = value;
                RaisePropertyChanged("XRightConstraint");
            }
        }
        public double YTopConstraint
        {
            get { return _yTopConstraint; }
            set { _yTopConstraint = value; RaisePropertyChanged("YTopConstraint"); }
        }

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                RaisePropertyChanged("X");
            }
        }
        public double Y
        {
            get { return _y; }
            set { _y = value; RaisePropertyChanged("Y"); }
        }
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged("Width");
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
            }
        }
        public Brush Background
        {
            get { return _background; }
            set { _background = value; RaisePropertyChanged("Background"); }
        }

        private Brush _constraintbackground;
        public Brush ConstraintBackground
        {
            get { return _constraintbackground; }
            set { _constraintbackground = value; RaisePropertyChanged("ConstraintBackground"); }
        }

        public RelayCommand<object> HighlightConstraintThumbCommand { get; set; }
        public RelayCommand<object> RestoreConstraintThumbCommand { get; set; }

        public RelayCommand CloseDetails { get => new RelayCommand(() => {
            OpenDetails = false;
        }
        ); }

        public RelayCommand DetailsOpened
        {
            get => new RelayCommand(() => {
                TempEndDate = TaskModel.EndDate;
                TempStartDate = TaskModel.StartDate;
                TempName = TaskModel.Intervento;
            });
        }

        


        public TaskBlockViewModel(CronoTask model)
        {

        }

        public TaskBlockViewModel(double x, double y, double width, int zIndex, Brush background, CronoTask model, int height, double positionOrder=-1)
        {
            TempEndDate = model.EndDate;
            TempStartDate = model.StartDate;
            TempName = model.Intervento;
            OpenDetails = false;
            Group = model.Id;
            Deletable = true;
            Height = height;
            Zindex = zIndex;
            _x = x;
            _y = y;
            _xRightConstraint = x + width;
            _yTopConstraint = _y;
            _width = width;
            _background = background;
            TaskModel = model;
            _constraintbackground = Brushes.Black;
            ConstraintList = new ObservableCollection<ConstraintViewModel>();
            ConstraintLine = false;

            Masters = new ObservableCollection<ConstraintViewModel>();
            HighlightConstraintThumbCommand = new RelayCommand<object>(ChangeBackgroundThumb);
            RestoreConstraintThumbCommand = new RelayCommand<object>(RestoreBackgroundThumb);
            PositionOrder = positionOrder;
        }

        public void SaveDetails()
        {
            TaskModel.Intervento = TempName;
            this.RaisePropertyChanged("TaskModel");
        }

        public void ChangeBackgroundThumb(object o)
        {

            ConstraintBackground = Brushes.Orange; 
        }

        public void RestoreBackgroundThumb(object o)
        {
            ConstraintBackground = Brushes.Black;
        }

        public void MoveLeft(double newX)
        {
            this.Width += this.X - newX;
            this.X = newX;
        }

        public void MoveRight(double newX, double dayWidth)
        {
            this.Width += -(this.X + this.Width) + newX + dayWidth;
            this.XRightConstraint = newX + dayWidth;
        }

        public void Move(double newX)
        {
            this.X = newX;
            this.XRightConstraint = newX + this.Width;
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType().Equals(typeof(TaskBlockViewModel)))
                return ((TaskBlockViewModel)obj).TaskModel.Equals(this.TaskModel);
            return false;
        }

        public override int GetHashCode()
        {
            return -130219519 + TaskModel.GetHashCode();
        }
    }
}
