using classroom_api.Models;
using classroom_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace classroom_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public partial class SubdivisionController : ControllerBase
    {
        private readonly ClassroomapiContext _context;
        public SubdivisionController(ClassroomapiContext context)
        {
            _context = context;
        }
        #region HTTP_GET


        [HttpGet("")]
        public async Task<ActionResult<List<SubdivisionModel>>> GetSubdivisionList()
        {
            return Ok(await _context.Subdivisions
                .Include(s => s.Courses)
                .Include(s => s.Moderators)
                .ToListAsync());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SubdivisionModel>> GetSubdivisionInfo(string id)
        {
            Guid.TryParse(id, out Guid subdivisionId);

            var subdivision = _context.Subdivisions
                .Include(s => s.Courses)
                .Include(s => s.Moderators)
                .FirstOrDefaultAsync(s => s.Id == subdivisionId);

            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }
            return Ok(subdivision);

        }
        [HttpGet("{id}/courses")]
        public async Task<ActionResult<List<CourseModel>>> GetSubdivisionCourses(string id)
        {
            Guid.TryParse(id, out Guid subdivisionId);

            var subdivision = await _context.Subdivisions.FirstOrDefaultAsync(s => s.Id == subdivisionId);
            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }
            return Ok(subdivision.Courses);

        }

        [HttpGet("{id}/moderators")]
        public async Task<ActionResult<List<UserModel>>> GetSubdivisionModerators(string id)
        {
            Guid.TryParse(id, out Guid subdivisionId);


            var subdivision = await _context.Subdivisions
                .Include(s => s.Moderators)
                .FirstOrDefaultAsync(s => s.Id == subdivisionId);

            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }
            return Ok(subdivision.Moderators);

        }
        #endregion

        #region HTTP_POST
        [HttpPost("")]
        public async Task<ActionResult<SubdivisionModel>> CreateSubdivision(string Name)
        {
            if (Name == null || Name == "")
            {
                return BadRequest("Name is empty");
            }

            var subdivision = new SubdivisionModel { Name = Name };
            _context.Subdivisions.Add(subdivision);
            await _context.SaveChangesAsync();
            return Ok(subdivision);
        }

        [HttpPost("{id}/courses")]
        public async Task<ActionResult<SubdivisionModel>> AddCoursesToSubdivision(string id, string courseId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(courseId, out Guid courseIdForDb);
            var subdivision = await _context.Subdivisions.FirstOrDefaultAsync(s => s.Id == subdivisionId);
            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseIdForDb);
            if (course == null)
            {
                return BadRequest("Course not found");
            }
            subdivision.Courses.Add(course);
            await _context.SaveChangesAsync();
            return Ok(subdivision);

        }

        [HttpPost("{id}/courses/{courseId}")]
        public async Task<ActionResult<SubdivisionModel>> AddCoursesToSubdivisionByUrl(string id, string courseId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(courseId, out Guid courseIdForDb);

            var subdivision = await _context.Subdivisions.FirstOrDefaultAsync(s => s.Id == subdivisionId);
            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseIdForDb);
            if (course == null)
            {
                return BadRequest("Course not found");
            }
            subdivision.Courses.Add(course);
            await _context.SaveChangesAsync();
            return Ok(subdivision);

        }

        [HttpPost("{id}/moderators")]
        public async Task<ActionResult<UserModel>> AddSubdivisionModerators(string id, string moderatorId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(moderatorId, out Guid moderatorIdForDb);
            var subdivision = await _context.Subdivisions
                .Include(s => s.Moderators)
                .FirstOrDefaultAsync(s => s.Id == subdivisionId);

            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }

            var checkIfModeratorAlreadyExists = subdivision.Moderators.FirstOrDefault(m => m.Id == moderatorIdForDb);
            if (checkIfModeratorAlreadyExists != null)
            {
                return BadRequest("This moderator already been added");
            }

            var moderator = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == moderatorIdForDb);

            if (moderator == null)
            {
                return BadRequest("Moderator not found");
            }
            subdivision.Moderators.Add(moderator);

            await _context.SaveChangesAsync();

            return Ok(moderator);

        }

        [HttpPost("{id}/moderators/{moderatorId}")]
        public async Task<ActionResult<UserModel>> AddSubdivisionModeratorsByUrl(string id, string moderatorId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(moderatorId, out Guid moderatorIdForDb);

            var subdivision = await _context.Subdivisions
                .Include(s => s.Moderators)
                .FirstOrDefaultAsync(s => s.Id == subdivisionId);

            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }

            var checkIfModeratorAlreadyExists = subdivision.Moderators.FirstOrDefault(m => m.Id == moderatorIdForDb);
            if (checkIfModeratorAlreadyExists != null)
            {
                return BadRequest("This moderator already been added");
            }

            var moderator = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == moderatorIdForDb);

            if (moderator == null)
            {
                return BadRequest("Moderator not found");
            }
            subdivision.Moderators.Add(moderator);

            await _context.SaveChangesAsync();

            return Ok(moderator);

        }
        #endregion

        #region HTTP_PATCH
        [HttpPatch("{id}")]
        public ActionResult<SubdivisionModel> EditSubdivision(string id, string Name)
        {
            Guid.TryParse(id, out Guid subdivisionId);

            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions.FirstOrDefault(s => s.Id == subdivisionId);
                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }

                if (Name != null && Name != "")
                {
                    subdivision.Name = Name;
                }
                db.SaveChanges();
                return Ok(subdivision);
            }
        }
        #endregion

        #region HTTP_DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult<SubdivisionModel>> DeleteSubdivision(string id)
        {
            Guid.TryParse(id, out Guid subdivisionId);


            var subdivision = await _context.Subdivisions.FirstOrDefaultAsync(s => s.Id == subdivisionId);
            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }

            _context.Subdivisions.Remove(subdivision);
            await _context.SaveChangesAsync();
            return Ok($"{id} deleted");

        }

        [HttpDelete("{id}/courses/{courseId}")]
        public async Task<ActionResult<SubdivisionModel>> RemoveCourseFromSubdivision(string id, string courseId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(courseId, out Guid courseIdForDb);

            var subdivision = await _context.Subdivisions.FirstOrDefaultAsync(s => s.Id == subdivisionId);
            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseIdForDb);
            if (course == null)
            {
                return BadRequest("Course not found");
            }
            subdivision.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok(subdivision);

        }

        [HttpDelete("{id}/moderators/{moderatorId}")]
        public async Task<ActionResult<UserModel>> RemoveSubdivisionModerator(string id, string moderatorId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(moderatorId, out Guid moderatorIdForDb);

            var subdivision = await _context.Subdivisions
                .Include(s => s.Moderators)
                .FirstOrDefaultAsync(s => s.Id == subdivisionId);

            if (subdivision == null)
            {
                return BadRequest("Subdivision not found");
            }

            var moderator = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == moderatorIdForDb);

            if (moderator == null)
            {
                return BadRequest("Moderator not found");
            }
            subdivision.Moderators.Remove(moderator);
            moderator.Subdivisions.Remove(subdivision);
            await _context.SaveChangesAsync();

            return Ok(subdivision);
        }
        #endregion
    }
}
