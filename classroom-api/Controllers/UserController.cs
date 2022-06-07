using classroom_api.Models;
using classroom_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace classroom_api.Controllers
{
    [Route("[controller]")]

    public class UserController : ControllerBase
    {
        private readonly ClassroomapiContext _context;
        public UserController(ClassroomapiContext context)
        {
            _context = context;
        }
        #region HTTP_GET

        [HttpGet("/info/{userId}")]
        public async Task<ActionResult<List<UserModel>>> GetShortUserData(string userId)
        {
            Guid.TryParse(userId, out var userGuid);

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userGuid);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);

        }

        [HttpGet("/list")]
        public async Task<ActionResult<List<UserModel>>> GetUserList()
        {

            var user = await _context.Users
                .Include(u => u.Roles)
                .ToListAsync();

            return Ok(user);

        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<UserModel>>> GetUserInfo(string userId)
        {
            Guid.TryParse(userId, out var userGuid);

            var user = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Subdivisions)
                .FirstOrDefaultAsync(u => u.Id == userGuid);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [HttpGet("{userId}/permissions")]
        public async Task<ActionResult<List<PermissionModel>>> GetUserPermissionList(string userId)
        {

            Guid.TryParse(userId, out var userGuid);

            var user = await _context.Users
                .Include(u => u.Roles)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Id == userGuid);

            if (user == null)
            {
                return NotFound("User not found");
            }
            List<PermissionModel> permissions = new List<PermissionModel>();
            foreach (var role in user.Roles)
            {
                permissions.AddRange(role.Permissions);
            }

            return Ok(permissions);

        }
        #endregion

        #region HTTP_PATCH

        [HttpPatch("{userId}")]
        public async Task<ActionResult<List<UserModel>>> EditUserInfo(string userId, string Name)
        {
            Guid.TryParse(userId, out var userGuid);

            var user = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Subdivisions)
                .FirstOrDefaultAsync(u => u.Id == userGuid);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Name = Name;
            await _context.SaveChangesAsync();
            return Ok(user);

        }
        #endregion

        #region HTTP_POST
        [HttpPost("{userId}/email")]
        public async Task<ActionResult<List<UserModel>>> AddEmailToUser(string userId, string email)
        {
            if (email == null || email == "")
            {
                return BadRequest("Email is empty");
            }

            MailAddress m;
            try
            {
                m = new MailAddress(email);
            }
            catch (FormatException)
            {
                return BadRequest("Incorrect email");
            }

            Guid.TryParse(userId, out var userGuid);

                var user = await _context.Users
                    .Include(u => u.Emails)
                    .FirstOrDefaultAsync(u => u.Id == userGuid);

                if (user == null)
                {
                    return NotFound("User not found");
                }
                EmailModel? oldUserEmail = await _context.Emails.FirstOrDefaultAsync(e => e.IsMain == true);
                if (oldUserEmail != null)
                {
                    oldUserEmail.IsMain = false;
                }

                EmailModel emailModel = new EmailModel
                {
                    Email = m.Address,
                    User = user,
                    IsMain = true
                };
                _context.Emails.Add(emailModel);
                await _context.SaveChangesAsync();
                return Ok(user);
            
        }

        [HttpPost("{userId}/email/{emailId}/main")]
        public async Task<ActionResult<List<UserModel>>> SetUserEmailAsMain(string userId, string emailId)
        {
            Guid.TryParse(userId, out var userGuid);
            Guid.TryParse(emailId, out var emailGuid);

                var user = await _context.Users
                    .Include(u => u.Emails)
                    .FirstOrDefaultAsync(u => u.Id == userGuid);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                EmailModel? userEmailForUpdateIsMain = await _context.Emails.FirstOrDefaultAsync(e => e.Id == emailGuid);
                if (userEmailForUpdateIsMain == null)
                {
                    return NotFound("Email not found");
                }
                userEmailForUpdateIsMain.IsMain = true;

                EmailModel? odlUserEmail = await _context.Emails.FirstOrDefaultAsync(e => e.IsMain == true);
                if (odlUserEmail != null)
                {
                    odlUserEmail.IsMain = false;
                }

                await _context.SaveChangesAsync();
                return Ok(user);
            
        }
        #endregion

        #region HTTP_DELETE
        [HttpDelete("{userId}/email/{emailId}")]
        public async Task<ActionResult<List<UserModel>>> RemoveEmailFromUser(string userId, string emailId)
        {
            Guid.TryParse(userId, out var userGuid);
            Guid.TryParse(emailId, out var emailGuid);

                var user = await _context.Users
                    .Include(u => u.Emails)
                    .FirstOrDefaultAsync(u => u.Id == userGuid);

                if (user == null)
                {
                    return NotFound("User not found");
                }
                EmailModel? userEmail = await _context.Emails.FirstOrDefaultAsync(e => e.Id == emailGuid);
                if (userEmail == null)
                {
                    return NotFound("Email not found");
                }

                _context.Emails.Remove(userEmail);
                await _context.SaveChangesAsync();
                return Ok(user);
            
        }
        #endregion
    }
}
