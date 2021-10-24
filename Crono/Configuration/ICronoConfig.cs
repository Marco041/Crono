using System;
using Crono.Configuration.Log;
using Crono.Repository;

namespace Crono.Configuration
{
    public interface ICronoConfig
    {
        int AttractionRange { get; }
        int CanvasReduceWidth { get; }
        DateTime DateEnd { get; }
        DateTime DateStart { get; }
        int DayShift { get; }
        double DayWidth { get; }
        LogSplitter Logger { get; }
        IRepository Repository { get; }
        int ResHeight { get; }
        int ResWidth { get; }
        int RowHeight { get; }
        int RowMargin { get; }
        int RowStart { get; }
        bool UseFakeRepository { get; set; }
        string CodiceCommessaArgs { get; }
    }
}