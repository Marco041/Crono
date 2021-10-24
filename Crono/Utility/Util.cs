using Crono.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Utility
{
    public static class Util
    {
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public static bool BetweenDate(DateTime d, DateTime dLow, DateTime dHight) => d >= dLow && d <= dHight;

        //List of public holidays
        private static List<Tuple<int, int>> _holidays = new List<Tuple<int, int>>()
        {
            new Tuple<int, int>(01,01),
            new Tuple<int, int>(06,01),
            new Tuple<int, int>(15,02),
            new Tuple<int, int>(25,04),
            new Tuple<int, int>(01,05),
            new Tuple<int, int>(02,06),
            new Tuple<int, int>(15,08),
            new Tuple<int, int>(01,11),
            new Tuple<int, int>(08,12),
            new Tuple<int, int>(25,12),
            new Tuple<int, int>(26,12),
        };

        private static DateTime EasterSunday(int year)
        {
            int month = 0;
            int day = 0;
            EasterSunday(year, ref month, ref day);
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Easter
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        private static void EasterSunday(int year, ref int month, ref int day)
        {
            int g = year % 19;
            int c = year / 100;
            int h = h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));
            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;
            if (day > 31)
            {
                month++;
                day -= 31;
            }
        }

        public static bool IsHoliday(DateTime day)
        {
            if (_holidays.Contains(new Tuple<int, int>(day.Day, day.Month)) ||
                day.Equals(EasterSunday(day.Year).AddDays(1)) ||
                day.DayOfWeek.Equals(DayOfWeek.Saturday) ||
                day.DayOfWeek.Equals(DayOfWeek.Sunday) )
                return true;
            return false;
        }

        /// <summary>
        /// Number of holidays between 2 dates
        /// </summary>
        public static int HolidaysNumber(DateTime d1, DateTime d2)
        {
            int cnt=0;
            foreach (DateTime item in EachDay(new DateTime(Math.Min(d1.Ticks,d2.Ticks)), new DateTime(Math.Max(d1.Ticks,d2.Ticks))))
            {
                if (IsHoliday(item)) cnt++;
            }
            return cnt;
        }

       
    }

    public static class ObservableCollectionExtensions
    {
        public static void RemoveAll<T>(this ObservableCollection<T> collection,
                                                           Func<T, bool> condition)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (condition(collection[i]))
                {
                    collection.RemoveAt(i);
                }
            }
        }        
    }
}
