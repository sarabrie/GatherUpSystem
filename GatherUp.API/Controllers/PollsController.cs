using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Polls;
using GatherUp.Core.DO.Users;
using GatherUp.Core.Interfaces;

namespace GatherUp.API.Controllers
{
    [Authorize]
    public class PollsController : BaseApiController
    {
        private readonly PollService _pollService;
        private readonly IRepository<Person> _personRepo;
        private readonly IMailNotificationBridge _notificationBridge;

        public PollsController(PollService pollService, EventsService eventsService, IRepository<Person> personRepo, IMailNotificationBridge notificationBridge)
            : base(eventsService)
        {
            _pollService = pollService;
            _personRepo = personRepo;
            _notificationBridge = notificationBridge;
        }

        [HttpPost("event/{eventId}")]
        public ActionResult CreatePoll(int eventId, [FromBody] Poll newPoll)
        {
            if (newPoll == null) return BadRequest(new { error = "נתוני סקר אינם תקינים." });
            if (!IsUserManager(eventId)) return Forbid();
            _pollService.CreatePoll(eventId, newPoll);
            _notificationBridge.TriggerNewPoll(eventId, newPoll.Title);
            return Ok(new { message = "הסקר נוצר בהצלחה." });
        }

        [HttpGet("event/{eventId}")]
        public ActionResult GetEventPolls(int eventId)
        {
            if (!IsUserInEvent(eventId)) return Forbid();
            return Ok(_pollService.GetEventPolls(eventId));
        }

        [HttpPost("{pollId}/answer")]
        public ActionResult SubmitAnswer(int pollId, [FromBody] SubmitAnswerRequest req)
        {
            if (req == null) return BadRequest(new { error = "נתונים לא תקינים." });
            var user = _personRepo.GetById(GetCurrentUserId());
            _pollService.SubmitAnswer(pollId, user.Name, req.Answers);
            return Ok(new { message = "תשובותך נשמרו בהצלחה." });
        }

        [HttpGet("{pollId}/questions/{questionId}/results")]
        public ActionResult GetQuestionResults(int pollId, int questionId)
        {
            if (!_pollService.IsPollValidAndActive(pollId))
                return BadRequest(new { error = "הסקר המבוקש אינו פעיל או תקין." });
            return Ok(_pollService.CalculateQuestionResultsPercentages(pollId, questionId));
        }
    }

    public class SubmitAnswerRequest
    {
        public Dictionary<int, int> Answers { get; set; } = new();
    }
}
