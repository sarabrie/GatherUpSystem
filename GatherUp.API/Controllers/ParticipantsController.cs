using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users;
using System.Collections.Generic;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly EventManagerService _eventManagerService;

        public ParticipantsController(EventManagerService eventManagerService)
        {
            _eventManagerService = eventManagerService;
        }

        // שליפת כל המשתתפים באירוע ספציפי
        [HttpGet("event/{eventId}")]
        public ActionResult<IEnumerable<Participant>> GetParticipants(int eventId)
        {
            var participants = _eventManagerService.GetParticipantsForEvent(eventId);
            return Ok(participants);
        }

        // הוספת משתתף חדש לאירוע קיים
        [HttpPost("event/{eventId}")]
        public ActionResult AddParticipant(int eventId, [FromBody] Participant participant)
        {
            if (participant == null)
            {
                return BadRequest("נתוני משתתף אינם תקינים.");
            }

            _eventManagerService.AddParticipantToEvent(eventId, participant);
            return Ok(new { Message = "המשתתף נוסף בהצלחה לאירוע." });
        }

        // שליחת תזכורת במייל למשתתפי האירוע - מתוקן עם FromQuery
        [HttpPost("event/{eventId}/remind")]
        public ActionResult SendReminder(int eventId, [FromQuery] string reminderType)
        {
            if (string.IsNullOrEmpty(reminderType))
            {
                return BadRequest("יש לספק סוג תזכורת.");
            }

            _eventManagerService.SendReminderToParticipants(eventId, reminderType);
            return Ok(new { Message = "התזכורות נשלחו בהצלחה." });
        }
    }
}