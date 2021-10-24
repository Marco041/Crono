using Crono.Configuration.Log;
using Crono.Repository;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Configuration
{
    public class CronoConfig : ICronoConfig
    {
        public bool UseFakeRepository { get; set; }
        private readonly ILog _infoLogger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);        
        public LogSplitter Logger { get; }
        public IRepository Repository { get; }
        public int ResWidth { get; }
        public int ResHeight { get; }
        public DateTime DateStart { get; }
        public DateTime DateEnd { get; }
        public int RowHeight { get; }
        public int RowMargin { get; }
        public int DayShift { get; }
        public int AttractionRange { get; }
        public double DayWidth { get; }
        public int RowStart { get; }
        public int CanvasReduceWidth => 255;    //Width of left panel with phase list        
        public string CodiceCommessaArgs { get; }

        public CronoConfig(string codiceCommessa=null)
        {
            CodiceCommessaArgs = codiceCommessa;
            var logTraceDaysCapacity = ConfigurationManager.AppSettings["LogTraceDaysCapacity"];
            UseFakeRepository = bool.Parse(ConfigurationManager.AppSettings["fakeRepository"]);
            if (Logger == null)
            {
                Logger = new LogSplitter(_infoLogger, _infoLogger);
                XmlConfigurator.Configure();
            }
            if (UseFakeRepository)
                Repository = new FakeRepository();
            else
                Repository = new Repository.MSSqlRepository(ConfigurationManager.AppSettings["DbName"], Logger);

            var resolution = ConfigurationManager.AppSettings["Resolution"];
            ResWidth = int.Parse(resolution.Split('x')[0]);
            ResHeight = int.Parse(resolution.Split('x')[1]);
            var timeSpan = int.Parse(ConfigurationManager.AppSettings["TimeSpan"]);
            var start = ConfigurationManager.AppSettings["Start"];

            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            if (start.ToLower().Equals("today"))
                DateStart = new DateTime(currentYear, currentMonth, 1);
            else
                DateStart = DateTime.Parse(start);

            DateEnd = new DateTime(DateStart.Year, DateStart.Month+ timeSpan-1, DateTime.DaysInMonth(DateStart.Year, DateStart.Month+ timeSpan-1));
            RowHeight = int.Parse(ConfigurationManager.AppSettings["RowHeight"]);
            RowMargin = int.Parse(ConfigurationManager.AppSettings["RowMargin"]);
            DayShift = int.Parse(ConfigurationManager.AppSettings["DayShift"]);
            AttractionRange = 5;
            DayWidth = (ResWidth - CanvasReduceWidth) / ((DateEnd - DateStart).TotalDays + 1);  //Days column width
            RowStart = 0;
        }
    }
}
