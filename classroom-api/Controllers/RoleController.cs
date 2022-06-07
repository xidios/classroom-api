using classroom_api.Models;
using classroom_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace classroom_api.Controllers
{
    [Route("[controller]")]
    public partial class RoleController : ControllerBase
    {
        private readonly ClassroomapiContext _context;
        public RoleController(ClassroomapiContext context)
        {
            _context = context;
        }
        #region HTTP_GET

        [HttpGet("")]
        public async Task<ActionResult<List<RoleModel>>> GetRoleList()
        {
            return (await _context.Roles
                .Include(r => r.Permissions)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleModel>> GetRole(string id)
        {
            Guid.TryParse(id, out var roleId);


            RoleModel? role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return BadRequest("Role not found");
            }
            await _context.SaveChangesAsync();
            return (role);

        }

        [HttpGet("permissions")]
        public async Task<ActionResult<List<PermissionModel>>> GetPermissionList()
        {
            return Ok(await _context.Permissions.ToListAsync());
        }
        #endregion

        #region HTTP_POST
        [HttpPost("")]
        public async Task<ActionResult<List<RoleModel>>> CreateRole()
        {
            return (await _context.Roles
                .Include(r => r.Permissions)
                .ToListAsync());
        }

        [HttpPost("{roleId}/{userId}")]
        public async Task<ActionResult<UserModel>> AddUserRole(string roleId, string userId)
        {
            Guid.TryParse(roleId, out var roleIdForDb);
            Guid.TryParse(userId, out var userIdForDb);

            RoleModel? role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleIdForDb);

            if (role == null)
            {
                return NotFound("Role not found");
            }
            UserModel? user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userIdForDb);
            if (user == null)
            {
                return NotFound("User not found");
            }
            user.Roles.Add(role);
            role.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;

        }
        #endregion

        #region HTTP_DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult<RoleModel>> DeleteRole(string id)
        {
            Guid.TryParse(id, out var roleId);

            RoleModel? role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return BadRequest("Role not found");
            }
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return (role);

        }

        [HttpDelete("{roleId}/{userId}")]
        public async Task<ActionResult<UserModel>> RemoveUserRole(string roleId, string userId)
        {
            Guid.TryParse(roleId, out var roleIdForDb);
            Guid.TryParse(userId, out var userIdForDb);

            UserModel? user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userIdForDb);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            RoleModel? role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleIdForDb);

            if (role == null)
            {
                return BadRequest("Role not found");
            }

            user.Roles.Remove(role);
            role.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;

        }
        #endregion

        #region HTTP_PATCH
        [HttpPatch("{id}")]
        public async Task<ActionResult<RoleModel>> EditRole(string id, string Name)
        {
            if (Name == null || Name == "")
            {
                return BadRequest("Name is empty");
            }

            Guid.TryParse(id, out var roleId);

                RoleModel? role = await _context.Roles
                    .Include(r => r.Permissions)
                    .FirstOrDefaultAsync(r => r.Id == roleId);
                if (role == null)
                {
                    return BadRequest("Role not found");
                }
                role.Name = Name;
                await _context.SaveChangesAsync();
                return role;
            
        }
        #endregion
    }
}
