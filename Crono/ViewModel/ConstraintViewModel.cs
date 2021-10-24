using Crono.Model;
using Crono.Service;
using Crono.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
    /// Thid ViewModel contains the state of a constraint between two tasks
    /// </summary>
    public class ConstraintViewModel : ViewModelBase
    {
        public TaskBlockViewModel ConstraintTask { get; set; }
        public TaskBlockViewModel MasterTask { get; set; }
        private double _xh1;
        private double _xh2;
        private double _xl1;
        private double _yh;
        private double _yl;
        private Brush _color;

        private readonly int initialMargin = 6;
        private readonly int endConstraintMargin = 10;

        /// <summary>
        /// Color line
        /// </summary>          
        public Brush Color
        {
            get { return _color; }
            set { _color = value; RaisePropertyChanged("Color"); }
        }

        /// <summary>
        /// X coordinate of the first vertex of the contraint line
        /// </summary>   

        public double Xh1
        {
            get { return _xh1; }
            set { _xh1 = value; RaisePropertyChanged("Xh1"); }
        }

        /// <summary>
        /// X coordinate of the second vertex of the constraint line
        /// </summary>        
        public double Xh2
        {
            get { return _xh2; }
            set { _xh2 = value; RaisePropertyChanged("Xh2"); }
        }
        /// <summary>
        /// Third vertex, is equals to the x coordinate of the second task
        /// </summary>

        public double Xl1
        {
            get { return _xl1; }
            set { _xl1 = value; RaisePropertyChanged("Xl1"); }
        }

        /// <summary>
        /// Y coord of first vertex
        /// </summary>
        public double Yh
        {
            get { return _yh; }
            set { _yh = value; RaisePropertyChanged("Yh"); }
        }

        /// <summary>
        /// Y coord of secodn vertex
        /// </summary>
        public double Yl
        {
            get { return _yl; }
            set { _yl = value; RaisePropertyChanged("Yl"); }
        }

        /// <summary>
        /// Highlight a constraint line
        /// </summary>
        public RelayCommand<TaskBlockViewModel> SelectItemCommand { get {
                return new RelayCommand<TaskBlockViewModel>((TaskBlockViewModel o) => {
                    Color = Brushes.Orange;
                    o.Zindex = 99999999;
                });
            } }
        public RelayCommand<TaskBlockViewModel> DeselectItemCommand { get {
                return new RelayCommand<TaskBlockViewModel>((TaskBlockViewModel o) => {
                    Color = Brushes.Black;
                    o.Zindex = 0;
                });
            } }

        private IDaysService _dayService;


        public ConstraintViewModel(IDaysService dayService, TaskBlockViewModel task, TaskBlockViewModel master)
        {
            _dayService = dayService;
            ConstraintTask = task;
            MasterTask = master;
            UpdateConstraint();
            Color = Brushes.Black;
        }

        /// <summary>
        /// Shift a constraint
        /// </summary>
        /// <param name="task"></param>
        /// <param name="value"></param>
        public void ShiftWithoutDate(TaskBlockViewModel task)
        {
            if (Math.Abs(ConstraintTask.X - DateToPixel(ConstraintTask.TaskModel.StartDate)) > 1)           
               ConstraintTask.X = DateToPixel(ConstraintTask.TaskModel.StartDate);            
            UpdateConstraint();   
        }


        /// <summary>
        /// This method calculate the coordinats of the constraint line based on the position of two tasks.
        /// Taks 1 is the task where the constaint line starts and task 2 is where constraint ends.
        /// The are 7 possibile cases:
        /// Type 1: both taks are in timespan
        /// Type 2: Task 1 is inside the timespan and task 2 start before the start date of the timespan (left)
        /// Type 3: Task 1 end date if after end date of the timespan and task 2 start inside the timespan
        /// Type 4: Task 1 is inside the timespan and task 2 start after end date of the timespan 
        /// Tipo 5: Task 1 ends before start date of the timespan and task 2 starts inside the timespan
        /// Tipo 6: Task 1 ends after end date of the time stan and task 2 start before start date of the timespan
        /// Tipo 7: Both task are off the timestpan
        /// </summary>
        /// <param name="MasterTask"></param>
        public void UpdateConstraint()
        {

            DateTime endDate = EndDate();
            DateTime startDate = StartDate();
            Xh1 = MasterTask.X + MasterTask.Width + initialMargin;
            Xh2 = DateToPixel(ConstraintTask.TaskModel.StartDate) - endConstraintMargin;
            Xl1 = DateToPixel(ConstraintTask.TaskModel.StartDate);
            Yh = MasterTask.Y + MasterTask.Height + initialMargin;
            Yl = ConstraintTask.Y + ConstraintTask.Height / 2;

            if (ConstraintTask.TaskModel.StartDate > endDate && MasterTask.TaskModel.EndDate < endDate)    //vincolo tipo 4
            {
                this.Xh1 = MasterTask.X + MasterTask.Width + initialMargin;
                this.Xh2 = DateToPixel(endDate) - endConstraintMargin;
                this.Xl1 = ConstraintTask.X;
            }else
            if (ConstraintTask.TaskModel.StartDate < startDate)  //vincolo tipo 2
            {
                this.Xh1 = MasterTask.X + MasterTask.Width + initialMargin;
                this.Xh2 = DateToPixel(startDate) + endConstraintMargin;
                this.Xl1 = DateToPixel(startDate);
                Yl = ConstraintTask.Y-2;
            }
            if (MasterTask.TaskModel.EndDate < startDate && Util.BetweenDate(ConstraintTask.TaskModel.StartDate, startDate, endDate))    //vincolo tipo 5
            {
                this.Xh1 = DateToPixel(startDate);
                this.Xh2 = DateToPixel(ConstraintTask.TaskModel.StartDate) - endConstraintMargin;
                this.Xl1 = DateToPixel(ConstraintTask.TaskModel.StartDate);
            }
            if (MasterTask.TaskModel.EndDate > endDate && Util.BetweenDate(ConstraintTask.TaskModel.StartDate, startDate, endDate)) //vincolo tipo 3
            {
                this.Xh1 = DateToPixel(endDate);
                this.Xh2 = DateToPixel(ConstraintTask.TaskModel.StartDate) - endConstraintMargin;
                this.Xl1 = DateToPixel(ConstraintTask.TaskModel.StartDate);
            }
            if (MasterTask.TaskModel.EndDate < startDate && (ConstraintTask.TaskModel.StartDate < startDate)) //vincolo tipo 7
            {
                this.Xh1 = MasterTask.X + MasterTask.Width+initialMargin;
                this.Xh2 = ConstraintTask.X- endConstraintMargin;
                this.Xl1 = ConstraintTask.X;
            }
            if (MasterTask.TaskModel.EndDate > endDate && (ConstraintTask.TaskModel.StartDate < startDate)) //vincolo tipo 6
            {
                this.Xh1 = MasterTask.X + MasterTask.Width + endConstraintMargin;
                this.Xh2 = DateToPixel(startDate)+ endConstraintMargin;
                this.Xl1 = ConstraintTask.X;
            }
        }
        
        /// <summary>
        /// Metodo per sistemare le linee dei vincoli in ingresso ad un task che viene spostato
        /// </summary>
        /// <param name="newDate"></param>
        public void AdjstConstraintLine(DateTime newDate)
        {
            var end = DateToPixel(newDate);
            Xl1 = end;
            Xh2 = end - endConstraintMargin;
            Yl = ConstraintTask.Y + ConstraintTask.Height/2;
            if (ConstraintTask.TaskModel.StartDate > EndDate())
            {
                end = DateToPixel(EndDate());
                Xl1 = end;
                Xh2 = end - endConstraintMargin;
            }
            if (ConstraintTask.TaskModel.StartDate < StartDate())
            {
                end = DateToPixel(StartDate());
                Xl1 = end;
                Xh2 = end + endConstraintMargin;
                Yl = ConstraintTask.Y - 2;
            }
        }

        private double DateToPixel(DateTime day) => _dayService.DateToPixel(day);
        private DateTime StartDate() => _dayService.StartDate;
        private DateTime EndDate() => _dayService.EndDate;       
    }
}
