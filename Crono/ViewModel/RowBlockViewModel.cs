using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.ViewModel
{
    /// <summary>
    /// Row with 0 or more phases
    /// </summary>
    public class RowBlockViewModel : ViewModelBase
    {
        private int _order; //row number
        private double _y;  
        private double _x;  

        public int Order
        {
            get{
                return _order;
            }
            set{
                _order = value; RaisePropertyChanged("Order");
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value; RaisePropertyChanged("Y");
            }
        }

        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value; RaisePropertyChanged("X");
            }
        }

        private List<TaskBlockViewModel> _task; //list of phases in the row
        public List<TaskBlockViewModel> Task{
            get
            {
                return _task;
            }
            set
            {
                _task = value;
                if(value!=null)
                    _task.ForEach(i=>i.Y = this.Y + 6);
            }
        }

        [PreferredConstructor]
        public RowBlockViewModel()
        {

        }

        public RowBlockViewModel(double x, double y, int order)
        {
            _x = x;
            _y = y;
            _order = order;
            Task = new List<TaskBlockViewModel>();
        }
    }
}
