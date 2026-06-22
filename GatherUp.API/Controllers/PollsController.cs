using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Polls;

namespace GatherUp.API.Controllers
{
    [Authorize]
    public class PollsController : BaseApiController
    {
        private readonly PollService _pollService;

        public PollsController(PollService pollService, EventsService eventsService)
            : base(eventsService)
        {
            _pollService = pollService;
        }

        [HttpPost("event/{eventId}")]
        public ActionResult CreatePoll(int eventId, [FromBody] Poll newPoll)
        {
            if (newPoll == null) return BadRequest(new { error = "נתוני סקר אינם תקינים." });
            if (!IsUserManager(eventId)) return Forbid();
            _pollService.CreatePoll(eventId, newPoll);
            return Ok(new { message = "הסקר נוצר בהצלחה." });
        }

        [HttpGet("event/{eventId}")]
        public ActionResult GetEventPolls(int eventId)
        {
            if (!IsUserInEvent(eventId)) return Forbid();
            return Ok(_pollService.GetEventPolls(eventId));
        }

        [HttpGet("{pollId}/questions/{questionId}/results")]
        public ActionResult GetQuestionResults(int pollId, int questionId)
        {
            if (!_pollService.IsPollValidAndActive(pollId))
                return BadRequest(new { error = "הסקר המבוקש אינו פעיל או תקין." });
            return Ok(_pollService.CalculateQuestionResultsPercentages(pollId, questionId));
        }
    }
}
