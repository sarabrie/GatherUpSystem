using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users;
using GatherUp.Core.Enums;
using GatherUp.Core.Interfaces;

namespace GatherUp.API.Controllers
{
    public class PersonController : BaseApiController
    {
        private readonly AuthService _authService;
        private readonly IRepository<Person> _personRepo;
        private readonly IRepository<EventManager> _managerRepo;
        private readonly IRepository<EventHost> _hostRepo;

        public PersonController(AuthService authService, EventsService eventsService, IRepository<Person> personRepo, IRepository<EventManager> managerRepo, IRepository<EventHost> hostRepo)
            : base(eventsService)
        {
            _authService = authService;
            _personRepo = personRepo;
            _managerRepo = managerRepo;
            _hostRepo = hostRepo;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            Person user = req.Role switch
            {
                "manager" => new EventManager { Id = req.Id, Name = req.Name, Email = req.Email },
                "host"    => new EventHost    { Id = req.Id, Name = req.Name, Email = req.Email },
                _         => new Person       { Id = req.Id, Name = req.Name, Email = req.Email }
            };
            _authService.AddUser(user);
            if (user is EventManager m) _managerRepo.Add(m);
            else if (user is EventHost h) _hostRepo.Add(h);
            return Ok(new { message = "המשתמש נוצר בהצלחה." });
        }

        [HttpGet("all")]
        [Authorize]
        public IActionResult GetAllPersons()
        {
            int currentUserId = GetCurrentUserId();
            var persons = _personRepo.GetAll()
                .Where(p => p.Id != currentUserId)
                .Select(p => new { p.Id, p.Name, p.Email });
            return Ok(persons);
        }

        [HttpGet("notifications")]
        [Authorize]
        public IActionResult GetNotifications()
        {
            int userId = GetCurrentUserId();
            EventManager? manager = _managerRepo.GetById(userId);
            if (manager == null) return NotFound();
            return Ok(new { notificationSettings = (int)manager.NotificationSettings });
        }

        [HttpPut("notifications")]
        [Authorize]
        public IActionResult SaveNotifications([FromBody] SaveNotificationsRequest req)
        {
            int userId = GetCurrentUserId();
            EventManager? manager = _managerRepo.GetById(userId);
            if (manager == null) return NotFound();
            manager.NotificationSettings = (NotificationPreferences)req.NotificationSettings;
            _managerRepo.Update(manager);
            return Ok("הגדרות ההתראות נשמרו בהצלחה.");
        }

        [HttpPut("edit")]
        [Authorize]
        public IActionResult EditUser([FromBody] Person updatedUser)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0) return Unauthorized("משתמש לא מזוהה.");
            updatedUser.Id = currentUserId;
            _authService.UpdateUser(updatedUser);
            return Ok("פרטי המשתמש עודכנו בהצלחה.");
        }
    }

    public class RegisterRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "person";
    }

    public class SaveNotificationsRequest
    {
        public int NotificationSettings { get; set; }
    }
}
