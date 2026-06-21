using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Polls;
using System.Collections.Generic;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PollsController : ControllerBase
    {
        private readonly PollService _pollService;

        public PollsController(PollService pollService)
        {
            _pollService = pollService;
        }

        // יצירת סקר חדש במערכת
        [HttpPost]
        public ActionResult CreatePoll([FromBody] Poll newPoll)
        {
            if (newPoll == null)
            {
                return BadRequest("נתוני סקר אינם תקינים.");
            }

            _pollService.CreatePoll(newPoll);
            return Ok(new { Message = "הסקר נוצר בהצלחה." });
        }

        // שליפת כל הסקרים המשויכים לאירוע מסוים
        [HttpGet("event/{eventId}")]
        public ActionResult<IEnumerable<Poll>> GetEventPolls(int eventId)
        {
            var polls = _pollService.GetEventPolls(eventId);
            return Ok(polls);
        }

        // הפקת תוצאות באחוזים עבור שאלה ספציפית בתוך סקר
        [HttpGet("{pollId}/questions/{questionId}/results")]
        public ActionResult<Dictionary<string, double>> GetQuestionResults(int pollId, int questionId)
        {
            // שימוש בפונקציית הולידציה הקיימת אצלכן ב-BL
            if (!_pollService.IsPollValidAndActive(pollId))
            {
                return BadRequest("הסקר המבוקש אינו פעיל, לא נמצא או שלא קיימות בו שאלות.");
            }

            var results = _pollService.CalculateQuestionResultsPercentages(pollId, questionId);
            return Ok(results);
        }
    }
}