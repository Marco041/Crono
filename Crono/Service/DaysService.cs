using Crono.Configuration;
using Crono.Utility;
using Crono.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Service
{
    public class DaysService : IDaysService
    {

        private ObservableCollection<DayBlockViewModel> _dayBlocks;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        private double _dayWidth;


        public DaysService(ICronoConfig config)
        {
            _dayBlocks = new ObservableCollection<DayBlockViewModel>();
            StartDate = config.DateStart;
            EndDate = config.DateEnd;
            _dayWidth = config.DayWidth;
        }

        public DayBlockViewModel GetStartDayBetween(double x1, double x2) =>
            _dayBlocks.FirstOrDefault(f => f.X >= x1 && f.X < x2);        

        public DayBlockViewModel GetEndDayBetween(double x1, double x2) =>     
            _dayBlocks.FirstOrDefault(f => f.X >= x1 && f.X < x2);

        public DayBlockViewModel GetDayByDay(DateTime date) =>
            _dayBlocks.FirstOrDefault(f => f.Day.Equals(date));

        /// Metodo per convertire la data in pixel in base al timespan corrente
        public double DateToPixel(DateTime day)
        {
            var value = _dayBlocks.FirstOrDefault(f => f.Day.Equals(day));
            if (value == null)
            {
                if (day <= StartDate)
                    return _dayBlocks.First().X - (StartDate - day).TotalDays * _dayWidth;
                if (day >= EndDate)
                    return _dayBlocks.Last().X + (day - EndDate).TotalDays * _dayWidth;
            }
            return value.X;
        }

        /// <summary>
        /// Update days os the timespan
        /// </summary>
        public void UpdateDays(int shift)
        {
            foreach (var day in _dayBlocks)
            {
                day.Day = day.Day.AddDays(shift);
                day.SetBackground(Util.IsHoliday(day.Day));
            }

        }

        public void SetNormalBackground()
        {
            _dayBlocks.ToList().ForEach(i => i.SetBackground(Util.IsHoliday(i.Day)));
        }

        /// <summary>
        /// Creates days os the timespan 
        /// </summary>
        public ObservableCollection<DayBlockViewModel> DrawDays(int startRow, int canvasMarginLeft)
        {
            int i = 0;
            foreach (var day in Util.EachDay(StartDate, EndDate))
            {
                _dayBlocks.Add(new DayBlockViewModel(startRow / 2, (i++ * _dayWidth) + canvasMarginLeft, _dayWidth, day, Util.IsHoliday(day)));
            }
            return _dayBlocks;
        }
    }
}
