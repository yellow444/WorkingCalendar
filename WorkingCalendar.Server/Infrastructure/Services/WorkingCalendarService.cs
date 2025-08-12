using System.Reflection;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace WorkingCalendar.Server.Infrastructure.Services
{
    public interface IWorkingCalendarService
    {
        Task<string?> CheckDayWorkingCalendar(string data, string days);

        Task<string?> GetYearWorkingCalendar(string year, string type, string days);
    }

    public class WorkingCalendarService : IWorkingCalendarService
    {
        private readonly ILogger<WorkingCalendarService> _logger;
        private readonly IWorkingCalendarRepository _workingCalendarRepository;

        public WorkingCalendarService(ILogger<WorkingCalendarService> logger, IWorkingCalendarRepository workingCalendarRepository)
        {
            _logger = logger;
            _workingCalendarRepository = workingCalendarRepository;
        }

        public async Task<string?> CheckDayWorkingCalendar(string data, string days)
        {
            return await Task.FromResult(_workingCalendarRepository.CheckDayWorkingCalendar(data, days));
        }

        public async Task<string?> GetYearWorkingCalendar(string year, string type, string days)
        {
            return await Task.FromResult(_workingCalendarRepository.GetYearWorkingCalendar(year, type, days));
        }
    }
}