using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users; // ודאי שזהו הנתיב למחלקת Person שלך

namespace GatherUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _config;

        // הזרקת שכבת הלוגיקה וההגדרות (עבור מפתח ה-JWT)
        public AuthController(AuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous] // מאפשר לכולם לגשת לנתיב הזה בלי טוקן קיים
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 1. פנייה לשכבת ה-BL לאימות המשתמש (מייל = שם משתמש, ת.ז = סיסמה)
            Person user = _authService.AuthenticateUser(request.Email, request.Id);

            // אם הפונקציה מחזירה null, סימן שהפרטים שגויים
            if (user == null)
            {
                return Unauthorized("שם משתמש או סיסמה שגויים.");
            }

            // 2. יצירת הטוקן אם האימות הצליח
            string token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        // פונקציית עזר פרטית ליצירת הטוקן
        private string GenerateJwtToken(Person user)
        {
            // שליפת המפתח הסודי מקובץ ההגדרות (appsettings.json) או ה-Program.cs
            string jwtKey = _config["Jwt:Key"] ?? "default-secret-key-replace-in-production";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // הגדרת המידע שיישמר בתוך הטוקן (Claims)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Name", user.Name), // בהנחה שיש שדה שם במחלקת Person
                new Claim(ClaimTypes.Role, "User") // ניתן לשנות בהמשך לפי תפקיד אמיתי
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // תוקף הטוקן
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // מחלקת עזר פנימית לקבלת הנתונים מהלקוח (גוף הבקשה - Body)
    public class LoginRequest
    {
        public string Email { get; set; }
        public int Id { get; set; }
    }
}