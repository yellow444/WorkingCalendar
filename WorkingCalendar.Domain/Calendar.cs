using System;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace WorkingCalendar.Domain;

public sealed class Calendar
{
    private readonly XDocument _document;

    private Calendar(XDocument document)
    {
        _document = document;
    }

    public static Calendar FromXml(string xml)
    {
        return new Calendar(XDocument.Parse(xml));
    }

    public string DescribeDay(DateTime day, int workingDaysPerWeek)
    {
        var desc = _document.Root?.Element("holidays")?.Elements("holiday");
        var daysList = _document.Root?.Element("days")?.Elements("day");
        return EvaluateDay(desc, daysList, day, workingDaysPerWeek);
    }

    public string GenerateSql(int year, string type, int workingDaysPerWeek)
    {
        var builder = new StringBuilder();
        switch (type)
        {
            case "mssql":
                builder.Append("CREATE TABLE WorkingCalendar (id bigint IDENTITY NOT NULL, DateDay date NOT NULL, IsWorkingDay bit  NOT NULL);");
                break;
            case "mysql":
                builder.Append("CREATE TABLE WorkingCalendar (  id INTEGER PRIMARY KEY AUTO_INCREMENT,  DateDay DATE NOT NULL,  IsWorkingDay BIT NOT NULL);");
                break;
            case "postgresql":
                builder.Append("CREATE TABLE WorkingCalendar (id SERIAL PRIMARY KEY, DateDay date NOT NULL, IsWorkingDay bit NOT NULL);");
                break;
        }

        builder.AppendLine().AppendLine("INSERT INTO WorkingCalendar (DateDay, IsWorkingDay) VALUES ");
        for (var day = new DateTime(year, 1, 1); day <= new DateTime(year, 12, 31); day = day.AddDays(1))
        {
            var result = DescribeDay(day, workingDaysPerWeek);
            var bit = type == "postgresql" ? "'" : string.Empty;
            builder.Append($"('{day:yyyy-MM-dd}', {bit}{(result == "рабочий день" ? 1 : 0)}{bit}),");
        }
        if (builder[^1] == ',')
        {
            builder.Length--;
        }
        builder.Append(';');
        return builder.ToString();
    }

    private static string EvaluateDay(IEnumerable<XElement>? desc, IEnumerable<XElement>? daysList, DateTime day, int workingDaysPerWeek)
    {
        var dw = (int)day.DayOfWeek;
        var result = dw < workingDaysPerWeek + 1 && dw > 0 ? "рабочий день" : "выходной день";
        try
        {
            if (daysList == null || !daysList.Any(x => x.Attribute("d")?.Value == day.ToString("MM.dd")))
            {
                return result;
            }
            var t = daysList.Where(x => x?.Attribute("d")?.Value == day.ToString("MM.dd"));
            var h = t?.Attributes("h")?.FirstOrDefault()?.Value;
            var title = desc?.Where(x => x?.Attribute("id")?.Value == h)?.Attributes("title")?.FirstOrDefault()?.Value;
            return title ?? result;
        }
        catch
        {
            return result;
        }
    }
}
