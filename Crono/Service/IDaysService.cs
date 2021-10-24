using System;
using System.Collections.ObjectModel;
using Crono.ViewModel;

namespace Crono.Service
{
    public interface IDaysService
    {
        DateTime EndDate { get; set; }
        DateTime StartDate { get; set; }

        double DateToPixel(DateTime day);
        ObservableCollection<DayBlockViewModel> DrawDays(int startRow, int canvasMarginLeft);
        DayBlockViewModel GetDayByDay(DateTime date);
        DayBlockViewModel GetEndDayBetween(double x1, double x2);
        DayBlockViewModel GetStartDayBetween(double x1, double x2);
        void SetNormalBackground();
        void UpdateDays(int shift);
    }
}