using Microsoft.AspNetCore.Mvc;
using WorkingCalendar.Application;

namespace WorkingCalendar.Server.Controllers;

/// <summary>
/// API controller for working calendar operations.
/// </summary>
[ApiController]
[Route("WorkingCalendar")]
public class WorkingCalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkingCalendarController"/> class.
    /// </summary>
    /// <param name="calendarService">Service for working with calendars.</param>
    public WorkingCalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    /// <summary>
    /// Generates SQL for the specified year.
    /// </summary>
    /// <param name="year">Calendar year or "all" for all years.</param>
    /// <param name="type">Type of the generated SQL script.</param>
    /// <param name="days">Working days per week.</param>
    /// <returns>SQL script for the requested year.</returns>
    [HttpGet("GetYearWorkingCalendar")]
    public async Task<ActionResult<string>> GetYearWorkingCalendar(string year, string type, int days)
    {
        var sql = await _calendarService.GetYearSqlAsync(year, type, days);
        return Ok(sql);
    }
}
