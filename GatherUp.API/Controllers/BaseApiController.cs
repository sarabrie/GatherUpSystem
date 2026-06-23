using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GatherUp.BL.Services;
using GatherUp.Core.DO;
using System.Linq;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly EventsService _eventsService;

        protected BaseApiController(EventsService eventsService)
        {
            _eventsService = eventsService;
        }

        protected int GetCurrentUserId()
        {
            string? userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdStr, out int userId);
            return userId;
        }

        protected bool IsUserManager(int eventId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return false;

            var ev = _eventsService.GetEventDetails(eventId);
            return ev.EventManagerId == currentUserId;
        }

        protected bool IsUserInEvent(int eventId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return false;

            try
            {
                Event ev = _eventsService.GetEventDetails(eventId);
                bool isParticipant = ev.ParticipantIds != null && ev.ParticipantIds.Contains(currentUserId);
                return ev.EventManagerId == currentUserId || ev.EventHostId == currentUserId || isParticipant;
            }
            catch { return false; }
        }
    }
}