using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Swashbuckle.AspNetCore.Annotations;

using WorkingCalendar.Server.Infrastructure.Services;

namespace WorkingCalendar.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkingCalendarController : ControllerBase
    {
        private readonly ILogger<WorkingCalendarController> _logger;
        private readonly IWorkingCalendarService _workingCalendarService;

        public WorkingCalendarController(ILogger<WorkingCalendarController> logger, IWorkingCalendarService workingCalendarService)
        {
            _logger = logger;
            _workingCalendarService = workingCalendarService;
        }

        /// <summary>
        /// dd.MM.yyyy
        /// </summary>
        /// <param name="data">dd.MM.yyyy</param>
        /// <returns>string</returns>
        [HttpGet]
        [Route("CheckDayWorkingCalendar")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces("text/plain")]
        public async Task<IActionResult> CheckDayWorkingCalendar(string data)
        {
            try
            {
                var result = await _workingCalendarService.CheckDayWorkingCalendar(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding folder.");
                return BadRequest();
            }
        }

        /// <summary>
        /// yyyy
        /// </summary>
        /// <param name="year">yyyy</param>
        /// <returns>string</returns>
        [HttpGet]
        [Route("GetYearWorkingCalendar")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces("text/plain")]
        public async Task<IActionResult> GetYearWorkingCalendar(string year)
        {
            try
            {
                var result = await _workingCalendarService.GetYearWorkingCalendar(year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding folder.");
                return BadRequest();
            }
        }
    }
}