using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GatherUp.BL.Services;
using GatherUp.Core.DO.Users;

namespace GatherUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(AuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous] // 🌟 פתוח לכולם - מאפשר התחברות ללא טוקן קיים
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || request.Id <= 0)
            {
                return BadRequest("נתוני התחברות לא תקינים.");
            }

            // פנייה לשכבת ה-BL לאימות המשתמש
            Person user = _authService.AuthenticateUser(request.Email, request.Id);

            if (user == null)
            {
                return Unauthorized("פרטי התחברות שגויים.");
            }

            // יצירת טוקן ומענה ללקוח
            string token = GenerateJwtToken(user);
            return Ok(new { Token = token, Message = "התחברות הצליחה." });
        }

        // פונקציית עזר פרטית ליצירת הטוקן
        private string GenerateJwtToken(Person user)
        {
            string jwtKey = _config["Jwt:Key"] ?? "default-secret-key-replace-in-production";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // הגדרת המידע שיישמר בתוך הטוקן (Claims)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Name", user.Name ?? ""),
                // 🌟 תוספת: התאמה מלאה ל-ClaimTypes.NameIdentifier שבו השתמשנו בשאר הקונטרולרים
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // תוקף הטוקן לשעתיים
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // מחלקת עזר לקבלת נתוני הבקשה
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public int Id { get; set; } // משמש כסיסמה על פי דרישות ה-BL
    }
}