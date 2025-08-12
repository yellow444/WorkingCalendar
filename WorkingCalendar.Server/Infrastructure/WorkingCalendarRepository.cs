using System;
using System.Globalization;
using System.Xml.Linq;

namespace WorkingCalendar.Server.Infrastructure
{
    public interface IWorkingCalendarRepository
    {
        string? CheckDayWorkingCalendar(string data, string days);

        string GetYearWorkingCalendar(string year, string type, string days);
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

        public string? CheckDayWorkingCalendar(string data, string days)
        {
            string format = "dd.MM.yyyy";
            DateTime.TryParseExact(data, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var day);
            var dw = (int)day.DayOfWeek;
            _workingCalendarXml.TryGetValue(day.Year.ToString(), out var xDocument);
            var desc = xDocument?.Root?.Element("holidays")?.Elements("holiday");
            var daysList = xDocument?.Root?.Element("days")?.Elements("day");
            var result = CheckDay(desc, daysList, day, dw, days);
            return result;
        }

        public string GetYearWorkingCalendar(string year, string type, string days)
        {
            var ret = "";//type + Environment.NewLine + Environment.NewLine;

            switch (type)
            {
                case "mssql":
                    {
                        ret += "CREATE TABLE WorkingCalendar (id bigint IDENTITY NOT NULL, DateDay date NOT NULL, IsWorkingDay bit  NOT NULL);";
                        break;
                    }
                case "mysql":
                    {
                        ret += "CREATE TABLE WorkingCalendar (  id INTEGER PRIMARY KEY AUTO_INCREMENT,  DateDay DATE NOT NULL,  IsWorkingDay BIT NOT NULL);";
                        break;
                    }
                case "postgresql":
                    {
                        ret += "CREATE TABLE WorkingCalendar (id SERIAL PRIMARY KEY, DateDay date NOT NULL, IsWorkingDay bit NOT NULL);";
                        break;
                    }
                default:
                    { break; }
            }

            ret += Environment.NewLine + Environment.NewLine + "INSERT INTO WorkingCalendar (DateDay, IsWorkingDay) VALUES ";
            var all = year;
            year = all == "all" ? "2013" : year;
            do
            {
                _workingCalendarXml.TryGetValue(year, out var xDocument);
                var desc = xDocument?.Root?.Element("holidays")?.Elements("holiday");
                var daysList = xDocument?.Root?.Element("days")?.Elements("day");
                for (DateTime day = new DateTime(int.Parse(year), month: 1, day: 1); day <= new DateTime(year: int.Parse(year), 12, 31); day = day.AddDays(1))
                {
                    var dw = (int)day.DayOfWeek;

                    var result = CheckDay(desc, daysList, day, dw, days);
                    var bit = type != "postgresql" ? "" : "'";
                    if (result == "рабочий день")
                    {                        
                        ret += $"('{day.ToString("yyyy-MM-dd")}', {bit}1{bit}),";
                    }
                    else
                    {
                        ret += $"('{day.ToString("yyyy-MM-dd")}', {bit}0{bit}),";
                    }
                }
                year = (int.Parse(year) + 1).ToString();
            }
            while (all == "all" && year != "2025");
            ret = ret.Substring(0, ret.Length - 1) + ";";
            return ret;
        }

        private static string CheckDay(IEnumerable<XElement>? desc, IEnumerable<XElement>? daysList, DateTime day, int dw, string days)
        {
            var _days = int.Parse(days);
            var result = dw < _days + 1 && dw > 0 ? "рабочий день" : "выходной день";
            try
            {
               if ( !daysList.Any(x => x.Attribute("d")?.Value == $"{day.ToString("MM.dd")}"))
                {
                    return result;
                }
                var t = daysList.Where(x => x?.Attribute("d")?.Value == $"{day.ToString("MM.dd")}");
                //var d = t.Attributes("d");
                var h = t?.Attributes("h")?.FirstOrDefault()?.Value;
                if (desc?.Where(x => x?.Attribute("id")?.Value == h)?.Attributes("title")?.FirstOrDefault()?.Value == null)
                {
                    return result;
                }
                result = desc?.Where(x => x?.Attribute("id")?.Value == h)?.Attributes("title")?.FirstOrDefault()?.Value;
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