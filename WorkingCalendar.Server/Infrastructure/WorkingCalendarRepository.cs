using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.VisualBasic;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkingCalendar.Server.Infrastructure
{
    public interface IWorkingCalendarRepository
    {
        string? CheckDayWorkingCalendar(string data);
        string GetYearWorkingCalendar(string year);
    }

    public class WorkingCalendarRepository : IWorkingCalendarRepository
    {
        private readonly ILogger<WorkingCalendarRepository> _logger;
        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private readonly Dictionary<string, XDocument> _workingCalendarXml = new Dictionary<string, XDocument>();
        //private enum langCalendar
        //{
        //    by,
        //    kz,
        //    ru,
        //    ua,
        //    uz
        //}

        public WorkingCalendarRepository(ILogger<WorkingCalendarRepository> logger)
        {
            _logger = logger;
            var paths = Directory.GetDirectories(_path, "ru");
            foreach (var path in paths)
            {
                var folders = Directory.GetDirectories(path);
                foreach (var folder in folders)
                {
                    var files = Directory.GetFiles(folder, "calendar.xml");
                    foreach (var file in files)
                    {
                        XDocument xdoc = XDocument.Load(file);
                        _workingCalendarXml.Add(folder.Split(Path.DirectorySeparatorChar).Last(), xdoc);
                    }
                }
            }
        }

        public string? CheckDayWorkingCalendar(string data)
        {
            string format = "dd.MM.yyyy";
            DateTime.TryParseExact(data, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var day);
            var dw = (int)day.DayOfWeek;
            _workingCalendarXml.TryGetValue(day.Year.ToString(), out var xDocument);
            var desc = xDocument.Root.Element("holidays").Elements("holiday");
            var days = xDocument.Root.Element("days").Elements("day");
            var result = CheckDay(desc, days, day, dw);
            return result;
        }

        public string GetYearWorkingCalendar(string year)
        {
            var ret = "INSERT INTO [WorkingCalendar] ([DateDay], [IsWorkingDay]) VALUES (";
            _workingCalendarXml.TryGetValue(year, out var xDocument);
            var desc = xDocument.Root.Element("holidays").Elements("holiday");
            var days = xDocument.Root.Element("days").Elements("day");
            var y = DateTime.IsLeapYear(int.Parse(year)) ? 366 : 365;
            for (DateTime day = new DateTime(int.Parse(year), 1, 1); day <= new DateTime(year: int.Parse(year), 12, 31); day = day.AddDays(1))
            {
                var dw = (int)day.DayOfWeek;

                var result = CheckDay(desc, days, day, dw);
                if (result == "рабочий день")
                {
                    ret += $"('{day.ToString("yyyy-MM-dd")}', 1),";
                }
                else
                {
                    ret += $"('{day.ToString("yyyy-MM-dd")}', 0),";
                }
            }
            ret = ret.Substring(0, ret.Length - 1) + ")";
            return ret;
        }

        private static string CheckDay(IEnumerable<XElement> desc, IEnumerable<XElement> days, DateTime day, int dw)
        {
            var result = dw < 6 ? "рабочий день" : "выходной день";
            try
            {
                var t = days.Where(x => x.Attribute("d").Value == $"{day.ToString("MM.dd")}");
                var d = t.Attributes("d");
                var h = t.Attributes("h").FirstOrDefault().Value;
                result = desc.Where(x => x.Attribute("id").Value == h).Attributes("title").FirstOrDefault().Value;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.Message);
#endif
            }
            return result;
        }
    }
}