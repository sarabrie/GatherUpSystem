using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users;

namespace GatherUp.API.Controllers
{
    public class PersonController : BaseApiController
    {
        private readonly AuthService _authService;

        // מכיוון שהוא יורש מ-BaseApiController, חובה להעביר את ה-EventsService לאבא בבנאי
        public PersonController(AuthService authService, EventsService eventsService)
            : base(eventsService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] Person newUser)
        {
            _authService.AddUser(newUser);
            return Ok("המשתמש נוצר בהצלחה.");
        }

        [HttpPut("edit")]
        [Authorize]
        public IActionResult EditUser([FromBody] Person updatedUser)
        {
            int currentUserId = GetCurrentUserId(); // שליפת ה-ID מהאבא
            if (currentUserId <= 0) return Unauthorized("משתמש לא מזוהה.");

            // 🌟 אבטחה קריטית: הכרחת העריכה לערוך רק את המשתמש המחובר עצמו!
            updatedUser.Id = currentUserId;

            _authService.UpdateUser(updatedUser);
            return Ok("פרטי המשתמש עודכנו בהצלחה.");
        }
    }
}