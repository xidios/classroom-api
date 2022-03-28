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
        #region HTTP_GET
        
        [HttpGet("/info/{userId}")]
        public ActionResult<List<UserModel>> GetShortUserData(string userId)
        {
            Guid.TryParse(userId, out var userGuid);
            using (var db = new ClassroomapiContext())
            {
                var user = db.Users
                    .Include(u => u.Roles)
                    .FirstOrDefault(u => u.Id == userGuid);

                if(user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }           
        }

        [HttpGet("/list")]
        public ActionResult<List<UserModel>> GetUserList()
        {
            using (var db = new ClassroomapiContext())
            {
                var user = db.Users
                    .Include(u => u.Roles)
                    .ToList();

                return Ok(user);
            }
        }

        [HttpGet("{userId}")]
        public ActionResult<List<UserModel>> GetUserInfo(string userId)
        {
            Guid.TryParse(userId, out var userGuid);
            using (var db = new ClassroomapiContext())
            {
                var user = db.Users
                    .Include(u => u.Roles)
                    .Include(u=>u.Subdivisions)
                    .FirstOrDefault(u => u.Id == userGuid);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
        }

        [HttpGet("{userId}/permissions")]
        public ActionResult<List<PermissionModel>> GetUserPermissionList(string userId)
        {

            Guid.TryParse(userId, out var userGuid);
            using (var db = new ClassroomapiContext())
            {
                var user = db.Users
                    .Include(u => u.Roles)
                    .ThenInclude(r=>r.Permissions)
                    .FirstOrDefault(u => u.Id == userGuid);

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
        }
        #endregion

        #region HTTP_PATCH

        [HttpPatch("{userId}")]
        public ActionResult<List<UserModel>> EditUserInfo(string userId, string Name)
        {
            Guid.TryParse(userId, out var userGuid);
            using (var db = new ClassroomapiContext())
            {
                var user = db.Users
                    .Include(u => u.Roles)
                    .Include(u => u.Subdivisions)
                    .FirstOrDefault(u => u.Id == userGuid);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.Name = Name;
                db.SaveChanges();
                return Ok(user);
            }
        }
        #endregion

        #region HTTP_POST
        [HttpPost("{userId}/email")]
        public ActionResult<List<UserModel>> AddEmailToUser(string userId, string email)
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
            using (var db = new ClassroomapiContext())
            {
                var user = db.Users
                    .Include(u => u.Emails)
                    .FirstOrDefault(u => u.Id == userGuid);

                if (user == null)
                {
                    return NotFound("User not found");
                }
                EmailModel? oldUserEmail = db.Emails.FirstOrDefault(e => e.IsMain == true);
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
                db.Emails.Add(emailModel);
                db.SaveChanges();
                return Ok(user);
            }
        }

        [HttpPost("{userId}/email/{emailId}/main")]
        public ActionResult<List<UserModel>> SetUserEmailAsMain(string userId, string emailId)
        {
            Guid.TryParse(userId, out var userGuid);
            Guid.TryParse(emailId, out var emailGuid);
            using (var db = new ClassroomapiContext())
            {
                var user = db.Users
                    .Include(u => u.Emails)
                    .FirstOrDefault(u => u.Id == userGuid);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                EmailModel? userEmailForUpdateIsMain = db.Emails.FirstOrDefault(e => e.Id == emailGuid);
                if (userEmailForUpdateIsMain == null)
                {
                    return NotFound("Email not found");
                }
                userEmailForUpdateIsMain.IsMain = true;

                EmailModel? odlUserEmail = db.Emails.FirstOrDefault(e => e.IsMain == true);
                if (odlUserEmail != null)
                {
                    odlUserEmail.IsMain = false;
                }

                db.SaveChanges();
                return Ok(user);
            }
        }
        #endregion

        #region HTTP_DELETE
        [HttpDelete("{userId}/email/{emailId}")]
        public ActionResult<List<UserModel>> RemoveEmailFromUser(string userId, string emailId)
        {
            Guid.TryParse(userId, out var userGuid);
            Guid.TryParse(emailId, out var emailGuid);
            using (var db = new ClassroomapiContext())
            {
                var user = db.Users
                    .Include(u => u.Emails)
                    .FirstOrDefault(u => u.Id == userGuid);

                if (user == null)
                {
                    return NotFound("User not found");
                }
                EmailModel? userEmail = db.Emails.FirstOrDefault(e => e.Id == emailGuid);
                if (userEmail == null)
                {
                    return NotFound("Email not found");
                }
               
                db.Emails.Remove(userEmail);
                db.SaveChanges();
                return Ok(user);
            }
        }
        #endregion
    }
}
