using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Dtos;
using TaskManager.Api.Entities;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController(AppDbContext db, TokenService tokens) : ControllerBase
    {
        [HttpPost("register")]

        public async Task<ActionResult<UserResponse>> Register(RegisterRequest req)
        {
            var email = req.Email.Trim().ToLowerInvariant();

            if (await db.Users.AnyAsync(u => u.Email == email))
                return Conflict(new { error = "Этот email уже зарегистрирован" });

            var user = new User
            {
                Email = email,
                Name = req.Name.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return StatusCode(201, new UserResponse(user.Id, user.Email, user.Name));
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login(LoginRequest req)
        {
            var email = req.Email.Trim().ToLowerInvariant();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Unauthorized(new { error = "Неверный email или пароль" });

            return new TokenResponse(tokens.CreateToken(user));
        }

    }
}
