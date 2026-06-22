using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO;
using GatherUp.Core.DO.Polls;
using GatherUp.Core.DO.Users;
using GatherUp.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GatherUp.API.Controllers
{
    public class CreateEventRequest
    {
        public Event Event { get; set; } = new Event();
        public bool IAmManager { get; set; } = true;
        public string? ManagerEmail { get; set; }
        public string? HostEmail { get; set; }
        public List<string> ParticipantEmails { get; set; } = new List<string>();
        public Poll? LocationPoll { get; set; }
        public Poll? DatePoll { get; set; }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : BaseApiController
    {
        private readonly EventManagerService _eventManagerService;
        private readonly PollService _pollService;
        private readonly IRepository<Person> _personRepo;
        private readonly IMailNotificationBridge _notificationBridge;

        public EventsController(
            EventsService eventsService,
            EventManagerService eventManagerService,
            PollService pollService,
            IRepository<Person> personRepo,
            IMailNotificationBridge notificationBridge) : base(eventsService)
        {
            _eventManagerService = eventManagerService;
            _pollService = pollService;
            _personRepo = personRepo;
            _notificationBridge = notificationBridge;
        }

        [HttpPost("create")]
        public IActionResult CreateEvent([FromBody] CreateEventRequest request)
        {
            int userId = GetCurrentUserId();
            if (userId <= 0) return Unauthorized(new { error = "מזהה משתמש לא חוקי." });

            if (request.IAmManager)
            {
                request.Event.EventManagerId = userId;
            }
            else
            {
                Person? manager = _personRepo.GetAll().FirstOrDefault(p => p.Email == request.ManagerEmail);
                if (manager == null) return BadRequest(new { error = $"משתמש עם מייל {request.ManagerEmail} לא נמצא." });
                request.Event.EventManagerId = manager.Id;
            }

            if (string.IsNullOrEmpty(request.HostEmail))
            {
                request.Event.EventHostId = userId;
            }
            else
            {
                Person? host = _personRepo.GetAll().FirstOrDefault(p => p.Email == request.HostEmail);
                if (host == null) return BadRequest(new { error = $"משתמש עם מייל {request.HostEmail} לא נמצא." });
                request.Event.EventHostId = host.Id;
            }

            var polls = new[] { request.LocationPoll, request.DatePoll }
                .Where(p => p != null)
                .ToList();

            var participants = (request.ParticipantEmails ?? new List<string>())
                .Select(email => _personRepo.GetAll().FirstOrDefault(x => x.Email == email))
                .Where(p => p != null)
                .Select(p => new Participant { Id = p!.Id, Email = p.Email, Name = p.Name })
                .ToList();

            _eventsService.AddEvent(
                request.Event,
                participants.Any() ? participants : null,
                polls.Any() ? polls : null
            );

            return Ok(new { message = "האירוע נוצר בהצלחה.", participantsAdded = participants.Count, pollsAdded = polls.Count });
        }

        [HttpGet("{eventId}")]
        public IActionResult GetEventDetails(int eventId)
        {
            Event ev = _eventsService.GetEventDetails(eventId);
            if (ev == null) return NotFound(new { error = "האירוע המבוקש לא נמצא." });
            return Ok(ev);
        }

        [HttpGet("my-events")]
        public IActionResult GetMyEvents()
        {
            int userId = GetCurrentUserId();
            if (userId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });
            return Ok(_eventsService.GetEventsByUser(userId));
        }

        [HttpPut("{eventId}/edit")]
        public IActionResult EditEvent(int eventId, [FromBody] Event updatedEvent)
        {
            int userId = GetCurrentUserId();
            if (userId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });

            Event existingEvent = _eventsService.GetEventDetails(eventId);
            if (existingEvent == null) return NotFound(new { error = "האירוע לא נמצא." });
            if (existingEvent.EventManagerId != userId) return Forbid();

            _eventManagerService.UpdateEventDetails(eventId, updatedEvent);
            _notificationBridge.TriggerEventAction(eventId, "עדכון פרטי אירוע");
            return Ok(new { message = "האירוע עודכן בהצלחה." });
        }

        [HttpPost("{eventId}/coordinate-schedule")]
        public IActionResult CoordinateSchedule(int eventId, [FromBody] Poll schedulePoll)
        {
            _pollService.CreatePoll(eventId, schedulePoll);
            return Ok(new { message = "סקר לתיאום לוחות זמנים נוצר בהצלחה." });
        }

        [HttpPost("{eventId}/subscribe")]
        public IActionResult SubscribeToUpdates(int eventId, [FromBody] Participant participant)
        {
            _eventManagerService.AddParticipantToEvent(eventId, participant);
            return Ok(new { message = "נרשמת בהצלחה לערוצי העדכונים של האירוע." });
        }
    }
}
