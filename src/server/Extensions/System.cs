
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Toucan.Contract;

namespace Toucan.Server
{
    public static partial class Extensions
    {
        public static DateTime? ToSourceUtc(this string value, CultureInfo culture, TimeZoneInfo sourceTimeZone)
        {
            string clean = Sanitize(value);

            if (DateTime.TryParse(clean, culture, DateTimeStyles.AssumeUniversal, out DateTime date))
                return date.ToSourceUtc(sourceTimeZone);
            else
                return null;
        }

        public static DateTime? ToSourceUtc(this DateTime date, TimeZoneInfo sourceTimeZone)
        {
            DateTime? dateTime = null;

            if (sourceTimeZone.Id == TimeZoneInfo.Local.Id)
            {
                dateTime = date.ToUniversalTime();
            }
            else
            {
                DateTime sourceDateTime = TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local, sourceTimeZone);

                dateTime = TimeZoneInfo.ConvertTimeToUtc(sourceDateTime, sourceTimeZone);
            }

            return dateTime;
        }

        private static string Sanitize(string value)
        {
            if (value.EndsWith("GMT"))
                return value; //  Javascript = new Date().toUTCString()

            if (value.EndsWith("Z") && value.Contains("T"))
                return value; //  Javascript = new Date().toISOString()

            if (Regex.IsMatch(value, @"(GMT\+[\d]+\s\([\w\s]+\))+"))
            {
                // Javascript = new Date().toString()
                var values = value.Split(" ").Take(5).ToArray();
                return $"{values[0]}, {values[2]} {values[1]} {values[3]} {values[4]} GMT";
            }

            return value;
        }
    }
}