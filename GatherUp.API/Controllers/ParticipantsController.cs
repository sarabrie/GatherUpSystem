using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GatherUp.API.Controllers
{
    [Authorize]
    public class ParticipantsController : BaseApiController
    {
        private readonly EventManagerService _eventManagerService;

        public ParticipantsController(EventManagerService eventManagerService, EventsService eventsService)
            : base(eventsService)
        {
            _eventManagerService = eventManagerService;
        }

        [HttpGet("event/{eventId}")]
        public ActionResult<IEnumerable<Participant>> GetParticipants(int eventId)
        {
            if (!IsUserInEvent(eventId)) return Forbid(); // מנהל, מארח או משתתף רשום בלבד

            IEnumerable<Participant> participants = _eventManagerService.GetParticipantsForEvent(eventId);
            return Ok(participants);
        }

        [HttpPost("event/{eventId}")]
        public ActionResult AddParticipant(int eventId, [FromBody] Participant participant)
        {
            if (participant == null) return BadRequest("נתוני משתתף אינם תקינים.");
            if (!IsUserManager(eventId)) return Forbid(); // רק מנהל או מארח יכולים להוסיף

            _eventManagerService.AddParticipantToEvent(eventId, participant);
            return Ok(new { Message = "המשתתף נוסף בהצלחה לאירוע." });
        }

        [HttpPost("event/{eventId}/remind")]
        public ActionResult SendReminder(int eventId, [FromQuery] string reminderType)
        {
            if (string.IsNullOrEmpty(reminderType)) return BadRequest("יש לספק סוג תזכורת.");
            if (!IsUserManager(eventId)) return Forbid(); // רק מנהל או מארח יכולים לשלוח תזכורות

            _eventManagerService.SendReminderToParticipants(eventId, reminderType);
            return Ok(new { Message = "התזכורות נשלחו בהצלחה." });
        }
    }
}