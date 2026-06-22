using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GatherUp.BL.Services;
using System.Linq;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly EventsService _eventsService;

        // הבנאי של מחלקת הבסיס
        protected BaseApiController(EventsService eventsService)
        {
            _eventsService = eventsService;
        }

        // 🌟 1. חילוץ מהיר של ה-ID מהטוקן
        protected int GetCurrentUserId()
        {
            string? userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdStr, out int userId);
            return userId;
        }

        // 🌟 2. בדיקה האם המשתמש המחובר הוא מנהל או מארח
        protected bool IsUserManager(int eventId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return false;

            var ev = _eventsService.GetEventDetails(eventId);
            return ev.EventManagerId == currentUserId;
        }

        // 🌟 3. בדיקה האם המשתמש קשור לאירוע (מנהל, מארח או משתתף רשום)
        protected bool IsUserInEvent(int eventId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return false;

            var ev = _eventsService.GetEventDetails(eventId);
            bool isParticipant = ev.ParticipantIds != null && ev.ParticipantIds.Contains(currentUserId);

            return ev.EventManagerId == currentUserId || ev.EventHostId == currentUserId || isParticipant;
        }
    }
}