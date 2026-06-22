using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Polls;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

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
            if (newPoll == null) return BadRequest("נתוני סקר אינם תקינים.");
            if (!IsUserManager(eventId)) return Forbid(); // רק מנהל או מארח יוצרים סקר

            _pollService.CreatePoll(newPoll);
            return Ok(new { Message = "הסקר נוצר בהצלחה." });
        }

        [HttpGet("event/{eventId}")]
        public ActionResult<IEnumerable<Poll>> GetEventPolls(int eventId)
        {
            if (!IsUserInEvent(eventId)) return Forbid(); // כל מי שקשור לאירוע רשאי לראות סקרים

            var polls = _pollService.GetEventPolls(eventId);
            return Ok(polls);
        }

        [HttpGet("{pollId}/questions/{questionId}/results")]
        public ActionResult<Dictionary<string, double>> GetQuestionResults(int pollId, int questionId)
        {
            if (!_pollService.IsPollValidAndActive(pollId))
            {
                return BadRequest("הסקר המבוקש אינו פעיל או תקין.");
            }

            var results = _pollService.CalculateQuestionResultsPercentages(pollId, questionId);
            return Ok(results);
        }
    }
}