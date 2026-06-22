//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using GatherUp.BL.Services;
//using GatherUp.Core.DO.Users;

//namespace GatherUp.API.Controllers
//{
//    public class PersonController : BaseApiController
//    {
//        private readonly AuthService _authService;

//        public PersonController(AuthService authService, EventsService eventsService)
//            : base(eventsService)
//        {
//            _authService = authService;
//        }

//        [HttpPost("register")]
//        [AllowAnonymous]
//        public IActionResult Register([FromBody] Person newUser)
//        {
//            _authService.AddUser(newUser);
//            return Ok(new { message = "המשתמש נוצר בהצלחה." });
//        }

//        [HttpPut("edit")]
//        [Authorize]
//        public IActionResult EditUser([FromBody] Person updatedUser)
//        {
//            int currentUserId = GetCurrentUserId();
//            if (currentUserId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });
//            updatedUser.Id = currentUserId;
//            _authService.UpdateUser(updatedUser);
//            return Ok(new { message = "פרטי המשתמש עודכנו בהצלחה." });
//        }
//    }
////}
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using GatherUp.BL.Services;
//using GatherUp.Core.DO.Users;
//using GatherUp.Core.Interfaces;
//using System.Linq;

//namespace GatherUp.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class PersonController : BaseApiController
//    {
//        private readonly AuthService _authService;
//        private readonly IRepository<Person> _personRepo;

//        // קונסטרקטור מאוחד המזריק את השירותים ואת ה-Repository של המשתמשים
//        public PersonController(AuthService authService, EventsService eventsService, IRepository<Person> personRepo)
//            : base(eventsService)
//        {
//            _authService = authService;
//            _personRepo = personRepo;
//        }

//        [HttpPost("register")]
//        [AllowAnonymous]
//        public IActionResult Register([FromBody] Person newUser)
//        {
//            _authService.AddUser(newUser);
//            // 🌟 הוחזר כאובייקט JSON כדי להתאים ל-apiCall
//            return Ok(new { message = "המשתמש נוצר בהצלחה." });
//        }

//        [HttpGet("all")]
//        [Authorize]
//        public IActionResult GetAllPersons()
//        {
//            int currentUserId = GetCurrentUserId();
//            if (currentUserId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });

//            var persons = _personRepo.GetAll()
//                .Where(p => p.Id != currentUserId)
//                .Select(p => new { p.Id, p.Name, p.Email })
//                .ToList();

//            return Ok(persons);
//        }

//        [HttpPut("edit")]
//        [Authorize]
//        public IActionResult EditUser([FromBody] Person updatedUser)
//        {
//            int currentUserId = GetCurrentUserId();
//            if (currentUserId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });

//            updatedUser.Id = currentUserId;
//            _authService.UpdateUser(updatedUser);

//            // 🌟 הוחזר כאובייקט JSON עקבי
//            return Ok(new { message = "פרטי המשתמש עודכנו בהצלחה." });
//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users;
using GatherUp.Core.Interfaces;
using System.Linq;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : BaseApiController
    {
        private readonly AuthService _authService;
        private readonly IRepository<Person> _personRepo;

        public PersonController(AuthService authService, EventsService eventsService, IRepository<Person> personRepo)
            : base(eventsService)
        {
            _authService = authService;
            _personRepo = personRepo;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] Person newUser)
        {
            _authService.AddUser(newUser);
            // 🌟 הוחזר כאובייקט JSON עקבי כדי שה-apiCall לא יישבר
            return Ok(new { message = "המשתמש נוצר בהצלחה." });
        }

        [HttpGet("all")]
        [Authorize]
        public IActionResult GetAllPersons()
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });

            var persons = _personRepo.GetAll()
                .Where(p => p.Id != currentUserId)
                .Select(p => new { p.Id, p.Name, p.Email })
                .ToList();

            return Ok(persons);
        }

        [HttpPut("edit")]
        [Authorize]
        public IActionResult EditUser([FromBody] Person updatedUser)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return Unauthorized(new { error = "משתמש לא מזוהה." });

            updatedUser.Id = currentUserId;
            _authService.UpdateUser(updatedUser);

            // 🌟 הוחזר כאובייקט JSON עקבי
            return Ok(new { message = "פרטי המשתמש עודכנו בהצלחה." });
        }
    }
}