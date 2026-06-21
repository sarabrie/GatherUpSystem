using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users;

namespace GatherUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly AuthService _authService;

        public PersonController(AuthService authService)
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

        // --- הפעולה החדשה לעריכת משתמש ---
        [HttpPut("edit")]
        [Authorize] // רק משתמש שיש לו טוקן יכול לגשת לכאן
        public IActionResult EditUser([FromBody] Person updatedUser)
        {
            // ה-Middleware הגלובלי שיצרת קודם יתפוס אוטומטית שגיאות אם ה-BL יזרוק אותן
            _authService.UpdateUser(updatedUser);

            return Ok("פרטי המשתמש עודכנו בהצלחה.");
        }
    }
}