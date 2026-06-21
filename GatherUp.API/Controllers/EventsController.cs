using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GatherUp.BL.Services;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Polls;
using GatherUp.Core.DO.Users;

namespace GatherUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly EventsService _eventsService;
        private readonly EventManagerService _eventManagerService;
        private readonly PollService _pollService;

        public EventsController(
            EventsService eventsService,
            EventManagerService eventManagerService,
            PollService pollService)
        {
            _eventsService = eventsService;
            _eventManagerService = eventManagerService;
            _pollService = pollService;
        }

        [HttpPost("create")]
        public IActionResult CreateEvent([FromBody] Event newEvent)
        {
            string? userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("מזהה משתמש לא חוקי.");

            if (newEvent.EventManagerId <= 0)
                newEvent.EventManagerId = userId;

            _eventsService.AddEvent(newEvent);
            return Ok("האירוע נוצר בהצלחה.");
        }

        [HttpGet("{eventId}")]
        [AllowAnonymous]
        public IActionResult GetEvent(int eventId)
        {
            Event ev = _eventsService.GetEventDetails(eventId);
            return Ok(ev);
        }

        [HttpGet("my-events")]
        public IActionResult GetMyEvents()
        {
            string? userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            _eventsService.GetEventsByUser(userId);
            return Ok();
        }

        [HttpPut("{eventId}/edit")]
        public IActionResult EditEvent(int eventId, [FromBody] Event updatedEvent)
        {
            string? userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("מזהה משתמש לא חוקי.");

            Event existingEvent = _eventsService.GetEventDetails(eventId);

            if (existingEvent.EventManagerId != userId)
                return Forbid();

            _eventManagerService.UpdateEventDetails(eventId, updatedEvent);
            return Ok("האירוע עודכן בהצלחה.");
        }

        [HttpPost("{eventId}/coordinate-schedule")]
        public IActionResult CoordinateSchedule(int eventId, [FromBody] Poll schedulePoll)
        {
            _pollService.CreatePoll(schedulePoll);
            return Ok("סקר לתיאום לוחות זמנים נוצר בהצלחה.");
        }

        [HttpPost("{eventId}/subscribe")]
        public IActionResult SubscribeToUpdates(int eventId, [FromBody] Participant participant)
        {
            _eventManagerService.AddParticipantToEvent(eventId, participant);
            return Ok("נרשמת בהצלחה לערוצי העדכונים של האירוע.");
        }
    }
}
