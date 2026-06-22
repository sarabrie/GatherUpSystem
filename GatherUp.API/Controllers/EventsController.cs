using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Polls;
using GatherUp.Core.DO.Users;

namespace GatherUp.API.Controllers
{
    [Authorize]
    public class EventsController : BaseApiController
    {
        private readonly EventManagerService _eventManagerService;
        private readonly PollService _pollService;

        public EventsController(EventsService eventsService, EventManagerService eventManagerService, PollService pollService)
            : base(eventsService)
        {
            _eventManagerService = eventManagerService;
            _pollService = pollService;
        }

        [HttpPost("create")]
        public IActionResult CreateEvent([FromBody] Event newEvent)
        {
            int userId = GetCurrentUserId(); // שימוש בחילוץ ה-ID המשותף מהאבא
            if (userId <= 0) return Unauthorized("מזהה משתמש לא חוקי.");

            if (newEvent.EventManagerId <= 0)
                newEvent.EventManagerId = userId;

            _eventsService.AddEvent(newEvent);
            return Ok("האירוע נוצר בהצלחה.");
        }

        [HttpGet("{eventId}")]
        public IActionResult GetEventDetails(int eventId)
        {
            if (!IsUserInEvent(eventId)) return Forbid(); // בדיקת אבטחה מורחבת מהאבא

            Event ev = _eventsService.GetEventDetails(eventId);
            return Ok(ev);
        }

        [HttpGet("my-events")]
        public IActionResult GetMyEvents()
        {
            int userId = GetCurrentUserId();
            if (userId <= 0) return Unauthorized();

            var userEvents = _eventsService.GetEventsByUser(userId);
            return Ok(userEvents);
        }

        [HttpPut("{eventId}/edit")]
        public IActionResult EditEvent(int eventId, [FromBody] Event updatedEvent)
        {
            int userId = GetCurrentUserId();
            if (userId <= 0) return Unauthorized();

            Event existingEvent = _eventsService.GetEventDetails(eventId);
            if (existingEvent.EventManagerId != userId) return Forbid();

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