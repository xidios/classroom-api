using classroom_api.Models;
using classroom_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace classroom_api.Controllers
{
    [Route("[controller]")]
    public partial class RoleController : ControllerBase
    {
        #region HTTP_GET

        [HttpGet("")]
        public ActionResult<List<RoleModel>> GetRoleList()
        {
            using (var db = new ClassroomapiContext())
            {
                return (db.Roles
                    .Include(r=>r.Permissions)
                    .ToList());
            }
        }

        [HttpGet("{id}")]
        public ActionResult<RoleModel> GetRole(string id)
        {
            Guid.TryParse(id, out var roleId);

            using (var db = new ClassroomapiContext())
            {
                RoleModel? role = db.Roles
                    .Include(r => r.Permissions)
                    .FirstOrDefault(r => r.Id == roleId);
                if (role == null)
                {
                    return BadRequest("Role not found");
                }
                db.SaveChanges();
                return (role);
            }
        }

        [HttpGet("permissions")]
        public ActionResult<List<PermissionModel>> GetPermissionList()
        {
            using (var db = new ClassroomapiContext())
            {
                return Ok(db.Permissions.ToList());
            }            
        }        
        #endregion

        #region HTTP_POST
        [HttpPost("")]
        public ActionResult<List<RoleModel>> CreateRole()
        {
            using (var db = new ClassroomapiContext())
            {
                return (db.Roles
                    .Include(r => r.Permissions)
                    .ToList());
            }
        }

        [HttpPost("{roleId}/{userId}")]
        public ActionResult<UserModel> AddUserRole(string roleId, string userId)
        {
            Guid.TryParse(roleId, out var roleIdForDb);
            Guid.TryParse(userId, out var userIdForDb);

            using (var db = new ClassroomapiContext())
            {
                RoleModel? role = db.Roles
                    .Include(r => r.Permissions)
                    .FirstOrDefault(r => r.Id == roleIdForDb);

                if (role == null)
                {
                    return NotFound("Role not found");
                }
                UserModel? user = db.Users
                    .FirstOrDefault(u => u.Id == userIdForDb);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                user.Roles.Add(role);
                role.Users.Add(user);
                db.SaveChanges();
                return user;
            }
        }
        #endregion

        #region HTTP_DELETE
        [HttpDelete("{id}")]
        public ActionResult<RoleModel> DeleteRole(string id)
        {
            Guid.TryParse(id, out var roleId);

            using (var db = new ClassroomapiContext())
            {
                RoleModel? role = db.Roles
                    .Include(r=>r.Permissions)
                    .FirstOrDefault(r=>r.Id == roleId);
                if (role == null) 
                {
                    return BadRequest("Role not found");
                }
                db.Roles.Remove(role);
                db.SaveChanges();
                return (role);
            }
        }

        [HttpDelete("{roleId}/{userId}")]
        public ActionResult<UserModel> RemoveUserRole(string roleId, string userId)
        {
            Guid.TryParse(roleId, out var roleIdForDb);
            Guid.TryParse(userId, out var userIdForDb);

            using (var db = new ClassroomapiContext())
            {

                UserModel? user = db.Users
                    .FirstOrDefault(u => u.Id == userIdForDb);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                RoleModel? role = db.Roles
                    .Include(r => r.Permissions)
                    .FirstOrDefault(r => r.Id == roleIdForDb);

                if (role == null)
                {
                    return BadRequest("Role not found");
                }
                
                user.Roles.Remove(role);
                role.Users.Remove(user);
                db.SaveChanges();
                return user;
            }
        }
        #endregion

        #region HTTP_PATCH
        [HttpPatch("{id}")]
        public ActionResult<RoleModel> EditRole(string id, string Name)
        {
            if (Name == null || Name == "")
            {
                return BadRequest("Name is empty");
            }

            Guid.TryParse(id, out var roleId);

            using (var db = new ClassroomapiContext())
            {
                RoleModel? role = db.Roles
                    .Include(r => r.Permissions)
                    .FirstOrDefault(r => r.Id == roleId);
                if (role == null)
                {
                    return BadRequest("Role not found");
                }
                role.Name = Name;
                db.SaveChanges();
                return role;
            }
        }      
        #endregion
    }
}
