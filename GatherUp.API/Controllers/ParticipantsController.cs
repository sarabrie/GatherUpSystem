using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users;

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
        public ActionResult GetParticipants(int eventId)
        {
            if (!IsUserInEvent(eventId)) return Forbid();
            return Ok(_eventManagerService.GetParticipantsForEvent(eventId));
        }

        [HttpPost("event/{eventId}")]
        public ActionResult AddParticipant(int eventId, [FromBody] Participant participant)
        {
            if (participant == null) return BadRequest(new { error = "נתוני משתתף אינם תקינים." });
            if (!IsUserManager(eventId)) return Forbid();
            _eventManagerService.AddParticipantToEvent(eventId, participant);
            return Ok(new { message = "המשתתף נוסף בהצלחה לאירוע." });
        }

        [HttpPatch("event/{eventId}/attendance")]

        //לעדכון הגעה של משתתף ש.ב
        public ActionResult UpdateAttendance(int eventId, [FromBody] AttendanceRequest request)
        {
            if (request == null) return BadRequest(new { error = "נתוני בקשה לא תקינים." });
            int userId = GetCurrentUserId();
            _eventManagerService.UpdateParticipantAttendance(eventId, userId, request.IsAttending);
            string msg = request.IsAttending ? "הגעתך אושרה בהצלחה!" : "ביטול הגעתך נשלח בהצלחה.";
            return Ok(new { message = msg });
        }

        [HttpPost("event/{eventId}/remind")]
        public ActionResult SendReminder(int eventId, [FromQuery] string reminderType)
        {
            if (string.IsNullOrEmpty(reminderType)) return BadRequest(new { error = "יש לספק סוג תזכורת." });
            if (!IsUserManager(eventId)) return Forbid();
            _eventManagerService.SendReminderToParticipants(eventId, reminderType);
            return Ok(new { message = "התזכורות נשלחו בהצלחה." });
        }
    }

    public class AttendanceRequest
    {
        public bool IsAttending { get; set; }
    }
}
