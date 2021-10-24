using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Crono.ViewModel
{
    /// <summary>
    /// Rappresent a day column in the canvas
    /// </summary>
    public class DayBlockViewModel : ViewModelBase
    {
        private DateTime _day;
        private double _x;
        private double _y;
        private double _width;
        private Brush _dayBackground;   
        private int _zIndex;

        public double X
        {
            get { return _x; }
            set { _x = value; RaisePropertyChanged("X"); }
        }
        public double Y
        {
            get { return _y; }
            set { _y = value; RaisePropertyChanged("Y"); }
        }
        public double Width
        {
            get { return _width; }
            set { _width = value; RaisePropertyChanged("Width"); }
        }
        public DateTime Day
        {
            get { return _day; }
            set { _day = value; RaisePropertyChanged("Day"); }
        }


        public Brush DayBackground
        {
            get
            {
                return _dayBackground;
            }
            set
            {
                _dayBackground = value;
                RaisePropertyChanged("DayBackground");
            }
        }

        public int Zindex
        {
            get
            {
                return _zIndex;
            }
            set
            {
                _zIndex = value;
                RaisePropertyChanged("Zindex");
            }
        }

        public void SetBackground(bool isHoliday)
        {
            if (isHoliday)  //Holyday days are orange
            {
                DayBackground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f9ae7f"));
                Zindex = 99999999;
            }
            else
            {
                DayBackground = Brushes.White;
                Zindex = 0;
            }
        }

        public void SetSelectedBackground()
        {

                DayBackground = Brushes.Purple;
            Zindex = 99999999;

        }

        public DayBlockViewModel(double y, double x, double width, DateTime date, bool isHoliday = false)
        {
            Y = y;
            X = x;
            Day = date;
            Width = width;
            SetBackground(isHoliday);
        }

        [PreferredConstructor]
            public DayBlockViewModel() { }
    }
}
