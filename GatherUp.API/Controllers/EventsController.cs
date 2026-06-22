//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using GatherUp.BL.Services;
//using GatherUp.Core.DO;
//using GatherUp.Core.DO.Polls;
//using GatherUp.Core.DO.Users;

//namespace GatherUp.API.Controllers
//{
//    [Authorize]
//    public class EventsController : BaseApiController
//    {
//        private readonly EventManagerService _eventManagerService;
//        private readonly PollService _pollService;

//        public EventsController(EventsService eventsService, EventManagerService eventManagerService, PollService pollService)
//            : base(eventsService)
//        {
//            _eventManagerService = eventManagerService;
//            _pollService = pollService;
//        }

//        [HttpPost("create")]
//        public IActionResult CreateEvent([FromBody] Event newEvent)
//        {
//            int userId = GetCurrentUserId();
//            if (userId <= 0) return Unauthorized(new { error = "מזהה משתמש לא חוקי." });
//            if (newEvent.EventManagerId <= 0) newEvent.EventManagerId = userId;
//            _eventsService.AddEvent(newEvent);
//            return Ok(new { message = "האירוע נוצר בהצלחה." });
//        }

//        [HttpGet("{eventId}")]
//        public IActionResult GetEventDetails(int eventId)
//        {
//            Event ev = _eventsService.GetEventDetails(eventId);
//            return Ok(ev);
//        }

//        [HttpGet("my-events")]
//        public IActionResult GetMyEvents()
//        {
//            int userId = GetCurrentUserId();
//            if (userId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });
//            return Ok(_eventsService.GetEventsByUser(userId));
//        }

//        [HttpPut("{eventId}/edit")]
//        public IActionResult EditEvent(int eventId, [FromBody] Event updatedEvent)
//        {
//            int userId = GetCurrentUserId();
//            if (userId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });
//            Event existing = _eventsService.GetEventDetails(eventId);
//            if (existing.EventManagerId != userId) return Forbid();
//            _eventManagerService.UpdateEventDetails(eventId, updatedEvent);
//            return Ok(new { message = "האירוע עודכן בהצלחה." });
//        }

//        [HttpPost("{eventId}/coordinate-schedule")]
//        public IActionResult CoordinateSchedule(int eventId, [FromBody] Poll schedulePoll)
//        {
//            _pollService.CreatePoll(eventId, schedulePoll);
//            return Ok(new { message = "סקר לתיאום לוחות זמנים נוצר בהצלחה." });
//        }

//        [HttpPost("{eventId}/subscribe")]
//        public IActionResult SubscribeToUpdates(int eventId, [FromBody] Participant participant)
//        {
//            _eventManagerService.AddParticipantToEvent(eventId, participant);
//            return Ok(new { message = "נרשמת בהצלחה לערוצי העדכונים של האירוע." });
//        }
//    }
//}

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using GatherUp.BL.Services;
//using GatherUp.Core.DO;
//using GatherUp.Core.DO.Polls;
//using GatherUp.Core.DO.Users;
//using GatherUp.Core.Interfaces;
//using System.Collections.Generic;
//using System.Linq;

//namespace GatherUp.API.Controllers
//{
//    // מחלקת עזר (DTO) לקבלת נתוני יצירת אירוע מורכב
//    public class CreateEventRequest
//    {
//        public Event Event { get; set; } = new Event();
//        public bool IAmManager { get; set; } = true;
//        public string? ManagerEmail { get; set; }
//        public string? HostEmail { get; set; }
//        public List<string> ParticipantEmails { get; set; } = new List<string>();
//        public Poll? LocationPoll { get; set; }
//        public Poll? DatePoll { get; set; }
//    }

//    [Authorize]
//    [ApiController]
//    [Route("api/[controller]")]
//    public class EventsController : BaseApiController
//    {
//        private readonly EventManagerService _eventManagerService;
//        private readonly PollService _pollService;
//        private readonly IRepository<Person> _personRepo;

//        // קונסטרקטור מאוחד המזריק את כל השירותים הנדרשים
//        public EventsController(
//            EventsService eventsService,
//            EventManagerService eventManagerService,
//            PollService pollService,
//            IRepository<Person> personRepo) : base(eventsService)
//        {
//            _eventManagerService = eventManagerService;
//            _pollService = pollService;
//            _personRepo = personRepo;
//        }

//        [HttpPost("create")]
//        public IActionResult CreateEvent([FromBody] CreateEventRequest request)
//        {
//            int userId = GetCurrentUserId();
//            if (userId <= 0) return Unauthorized(new { error = "מזהה משתמש לא חוקי." });

//            // קביעת מנהל האירוע
//            if (request.IAmManager)
//            {
//                request.Event.EventManagerId = userId;
//            }
//            else
//            {
//                Person? manager = _personRepo.GetAll().FirstOrDefault(p => p.Email == request.ManagerEmail);
//                if (manager == null) return BadRequest(new { error = $"משתמש עם מייל {request.ManagerEmail} לא נמצא." });
//                request.Event.EventManagerId = manager.Id;
//            }

//            // קביעת מארח האירוע
//            if (string.IsNullOrEmpty(request.HostEmail))
//            {
//                request.Event.EventHostId = userId;
//            }
//            else
//            {
//                Person? host = _personRepo.GetAll().FirstOrDefault(p => p.Email == request.HostEmail);
//                if (host == null) return BadRequest(new { error = $"משתמש עם מייל {request.HostEmail} לא נמצא." });
//                request.Event.EventHostId = host.Id;
//            }

//            // איסוף סקרים התחלתיים במידה וקיימים
//            List<Poll> polls = new List<Poll>();
//            if (request.LocationPoll != null) polls.Add(request.LocationPoll);
//            if (request.DatePoll != null) polls.Add(request.DatePoll);

//            // חילוץ משתתפים מתוך רשימת אימיילים
//            List<Participant> participants = request.ParticipantEmails
//                ?.Select(email => {
//                    Person? p = _personRepo.GetAll().FirstOrDefault(x => x.Email == email);
//                    if (p == null) return null;
//                    return new Participant { Id = p.Id, Email = p.Email, Name = p.Name };
//                })
//                .Where(p => p != null)
//                .Select(p => p!)
//                .ToList() ?? new List<Participant>();

//            _eventsService.AddEvent(request.Event, participants.Count > 0 ? participants : null, polls.Count > 0 ? polls : null);

//            return Ok(new { message = "האירוע נוצר בהצלחה." });
//        }

//        [HttpGet("{eventId}")]
//        public IActionResult GetEventDetails(int eventId)
//        {
//            Event ev = _eventsService.GetEventDetails(eventId);
//            if (ev == null) return NotFound(new { error = "האירוע המבוקש לא נמצא." });
//            return Ok(ev);
//        }

//        [HttpGet("my-events")]
//        public IActionResult GetMyEvents()
//        {
//            int userId = GetCurrentUserId();
//            if (userId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });

//            var userEvents = _eventsService.GetEventsByUser(userId);
//            return Ok(userEvents);
//        }

//        [HttpPut("{eventId}/edit")]
//        public IActionResult EditEvent(int eventId, [FromBody] Event updatedEvent)
//        {
//            int userId = GetCurrentUserId();
//            if (userId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });

//            Event existingEvent = _eventsService.GetEventDetails(eventId);
//            if (existingEvent == null) return NotFound(new { error = "האירוע לא נמצא." });
//            if (existingEvent.EventManagerId != userId) return Forbid();

//            _eventManagerService.UpdateEventDetails(eventId, updatedEvent);
//            return Ok(new { message = "האירוע עודכן בהצלחה." });
//        }

//        [HttpPost("{eventId}/coordinate-schedule")]
//        public IActionResult CoordinateSchedule(int eventId, [FromBody] Poll schedulePoll)
//        {
//            _pollService.CreatePoll(eventId, schedulePoll);
//            return Ok(new { message = "סקר לתיאום לוחות זמנים נוצר בהצלחה." });
//        }

//        [HttpPost("{eventId}/subscribe")]
//        public IActionResult SubscribeToUpdates(int eventId, [FromBody] Participant participant)
//        {
//            _eventManagerService.AddParticipantToEvent(eventId, participant);
//            return Ok(new { message = "נרשמת בהצלחה לערוצי העדכונים של האירוע." });
//        }
//    }
//}
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
    // מחלקת עזר (DTO) לקבלת נתוני יצירת אירוע מורכב
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

        // קונסטרקטור מאוחד המזריק את כל השירותים הנדרשים
        public EventsController(
            EventsService eventsService,
            EventManagerService eventManagerService,
            PollService pollService,
            IRepository<Person> personRepo) : base(eventsService)
        {
            _eventManagerService = eventManagerService;
            _pollService = pollService;
            _personRepo = personRepo;
        }

        [HttpPost("create")]
        public IActionResult CreateEvent([FromBody] CreateEventRequest request)
        {
            int userId = GetCurrentUserId();
            if (userId <= 0) return Unauthorized(new { error = "מזהה משתמש לא חוקי." });

            // קביעת מנהל האירוע
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

            // קביעת מארח האירוע
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

            // איסוף סקרים התחלתיים במידה וקיימים
            List<Poll> polls = new();
            if (request.LocationPoll != null) polls.Add(request.LocationPoll);
            if (request.DatePoll != null) polls.Add(request.DatePoll);

            // 🌟 לולאת ה-foreach החדשה והקריאה שלכן לחילוץ משתתפים
            List<Participant> participants = new();
            foreach (var email in request.ParticipantEmails ?? new List<string>())
            {
                Person? p = _personRepo.GetAll().FirstOrDefault(x => x.Email == email);
                if (p != null)
                {
                    participants.Add(new Participant { Id = p.Id, Email = p.Email, Name = p.Name });
                }
            }

            _eventsService.AddEvent(
                request.Event,
                participants.Count > 0 ? participants : null,
                polls.Count > 0 ? polls : null
            );

            // 🌟 החזרת האובייקט המורחב שלכן בפורמט JSON תקני
            return Ok(new
            {
                message = "האירוע נוצר בהצלחה.",
                participantsAdded = participants.Count,
                pollsAdded = polls.Count
            });
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

            var userEvents = _eventsService.GetEventsByUser(userId);
            return Ok(userEvents);
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