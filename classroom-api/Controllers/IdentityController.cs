using classroom_api.Models;
using classroom_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace classroom_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ClassroomapiContext _context;
        public IdentityController(ClassroomapiContext context)
        {
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var identity = await GetIdentity(email, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }
            var user  = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromDays(2)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                email = identity.Name
            };
            user.Token = encodedJwt;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(response);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string password, string name)
        {
            if(email == null||password == null || name == null)
            {
                return BadRequest(new { errorText = "Bad params" });
            }

            var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            if(user != null)
            {
                return BadRequest("User already exists");
            }
            var newUser = new UserModel
            {
                Email = email,
                Password = password,
                Name = name
            };
            await _context.Users.AddAsync(newUser);            
            await _context.SaveChangesAsync();
            var response = new
            {
                Email = email,
                Message = "registered"
            };
            return Ok(response);
        }


        private async Task<ClaimsIdentity> GetIdentity(string email,string? password = null)
        {
            UserModel? user;
            if (password == null)
            {
                 user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(x => x.Email == email);
            }
            else
            {
                 user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            }

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Roles.First().Name)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}
