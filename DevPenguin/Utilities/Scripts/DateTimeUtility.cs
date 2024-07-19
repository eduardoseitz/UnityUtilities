using System;
using System.Collections.Generic;

namespace DevPenguin.Utilities
{
    public static class DateTimeUtility
    {
        #region Declarations
        
        /// <summary>
        /// Use this to increase time artificially for testing.
        /// </summary>
        private static double debugIncreseDays = 0;
        private static double debugIncreseHours = 0;
        private static double debugIncreseMinutes = 0;

        private static HashSet<DateTime> Holidays = new()
        {
            // Feel free to add or remove holidays as per necessity.
            new DateTime(DateTime.Today.Year, 1, 1), // New years.
            new DateTime(DateTime.Today.Year, 04, 20), // Easter.
            new DateTime(DateTime.Today.Year, 12, 25), // Christmas.
            new DateTime(DateTime.Today.Year, 12, 1) // New years eve.
        };
        
        #endregion

        #region Getters and Setters

        public static DateTime LastYear => Today.AddYears(-1);
        
        public static DateTime LastMonth => Today.AddMonths(-1);
        
        public static DateTime Today => DateTime.Today.AddDays(debugIncreseDays);
        
        public static DateTime Now => DateTime.Now.AddDays(debugIncreseDays).AddHours(debugIncreseHours).AddMinutes(debugIncreseMinutes);
        
        public static DateTime Tomorrow => Today.AddDays(1);
        
        public static DateTime NextMonth => Today.AddMonths(1);
        
        public static DateTime NextYear => Today.AddYears(1);
        
        public static bool IsTodayAWeekend => IsDayAWeekend(Today);
        
        public static bool IsDayAWeekend(DateTime dateTime) => dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        public static bool IsTodayAWeekday => IsDayAWeekday(Today);
        
        public static bool IsDayAWeekday(DateTime dateTime) => dateTime.DayOfWeek != DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Sunday;

        public static bool IsDayAHoliday(DateTime date) => Holidays.Contains(date);
        
        public static bool IsTodayAHoliday => IsDayAHoliday(Today);
        
        public static DateTime FindNextDayOfWeek(DayOfWeek dayOfWeek)
        {
            DateTime _date = Today;
            do
                _date.AddDays(1);
            while (_date.DayOfWeek != dayOfWeek);
            return _date;
        }

        #endregion

    }
}
