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
        #region HTTP_GET

        [HttpGet("")]
        public ActionResult<List<SubdivisionModel>> GetSubdivisionList()
        {
            using (var db = new ClassroomapiContext())
            {
                return Ok(db.Subdivisions
                    .Include(s => s.Courses)
                    .Include(s => s.Moderators)
                    .ToList());
            }
        }
        [HttpGet("{id}")]
        public ActionResult<SubdivisionModel> GetSubdivisionInfo(string id)
        {
            Guid.TryParse(id, out Guid subdivisionId);

            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions
                    .Include(s => s.Courses)
                    .Include(s => s.Moderators)
                    .FirstOrDefault(s => s.Id == subdivisionId);

                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }
                return Ok(subdivision);
            }
        }
        [HttpGet("{id}/courses")]
        public ActionResult<List<CourseModel>> GetSubdivisionCourses(string id)
        {
            Guid.TryParse(id, out Guid subdivisionId);

            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions.FirstOrDefault(s => s.Id == subdivisionId);
                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }
                return Ok(subdivision.Courses);
            }
        }

        [HttpGet("{id}/moderators")]
        public ActionResult<List<UserModel>> GetSubdivisionModerators(string id)
        {
            Guid.TryParse(id, out Guid subdivisionId);

            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions
                    .Include(s=>s.Moderators)
                    .FirstOrDefault(s => s.Id == subdivisionId);

                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }
                return Ok(subdivision.Moderators);
            }
        }
        #endregion

        #region HTTP_POST
        [HttpPost("")]
        public ActionResult<SubdivisionModel> CreateSubdivision(string Name)
        {
            if (Name == null || Name == "")
            {
                return BadRequest("Name is empty");
            }
            using (var db = new ClassroomapiContext())
            {
                var subdivision = new SubdivisionModel { Name = Name };
                db.Subdivisions.Add(subdivision);
                db.SaveChanges();
                return Ok(subdivision);
            }

        }

        [HttpPost("{id}/courses")]
        public ActionResult<SubdivisionModel> AddCoursesToSubdivision(string id, string courseId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(courseId, out Guid courseIdForDb);
            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions.FirstOrDefault(s => s.Id == subdivisionId);
                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }
                var course = db.Courses.FirstOrDefault(c => c.Id == courseIdForDb);
                if (course == null)
                {
                    return BadRequest("Course not found");
                }
                subdivision.Courses.Add(course);
                db.SaveChanges();
                return Ok(subdivision);
            }
        }

        [HttpPost("{id}/courses/{courseId}")]
        public ActionResult<SubdivisionModel> AddCoursesToSubdivisionByUrl(string id, string courseId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(courseId, out Guid courseIdForDb);
            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions.FirstOrDefault(s => s.Id == subdivisionId);
                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }
                var course = db.Courses.FirstOrDefault(c => c.Id == courseIdForDb);
                if (course == null)
                {
                    return BadRequest("Course not found");
                }
                subdivision.Courses.Add(course);
                db.SaveChanges();
                return Ok(subdivision);
            }
        }

        [HttpPost("{id}/moderators")]
        public ActionResult<UserModel> AddSubdivisionModerators(string id, string moderatorId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(moderatorId, out Guid moderatorIdForDb);
            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions
                    .Include(s=>s.Moderators)
                    .FirstOrDefault(s => s.Id == subdivisionId);

                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }

                var checkIfModeratorAlreadyExists = subdivision.Moderators.FirstOrDefault(m=>m.Id == moderatorIdForDb);
                if(checkIfModeratorAlreadyExists!= null)
                {
                    return BadRequest("This moderator already been added");
                }

                var moderator = db.Users
                    .FirstOrDefault(m => m.Id == moderatorIdForDb);

                if (moderator == null)
                {
                    return BadRequest("Moderator not found");
                }
                subdivision.Moderators.Add(moderator);

                db.SaveChanges();

                return Ok(moderator);
            }
        }

        [HttpPost("{id}/moderators/{moderatorId}")]
        public ActionResult<UserModel> AddSubdivisionModeratorsByUrl(string id, string moderatorId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(moderatorId, out Guid moderatorIdForDb);
            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions
                    .Include(s => s.Moderators)
                    .FirstOrDefault(s => s.Id == subdivisionId);

                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }

                var checkIfModeratorAlreadyExists = subdivision.Moderators.FirstOrDefault(m => m.Id == moderatorIdForDb);
                if (checkIfModeratorAlreadyExists != null)
                {
                    return BadRequest("This moderator already been added");
                }

                var moderator = db.Users
                    .FirstOrDefault(m => m.Id == moderatorIdForDb);

                if (moderator == null)
                {
                    return BadRequest("Moderator not found");
                }
                subdivision.Moderators.Add(moderator);

                db.SaveChanges();

                return Ok(moderator);
            }
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
        public ActionResult<SubdivisionModel> DeleteSubdivision(string id)
        {
            Guid.TryParse(id, out Guid subdivisionId);

            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions.FirstOrDefault(s => s.Id == subdivisionId);
                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }

                db.Subdivisions.Remove(subdivision);
                db.SaveChanges();
                return Ok($"{id} deleted");
            }
        }

        [HttpDelete("{id}/courses/{courseId}")]
        public ActionResult<SubdivisionModel> RemoveCourseFromSubdivision(string id, string courseId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(courseId, out Guid courseIdForDb);
            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions.FirstOrDefault(s => s.Id == subdivisionId);
                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }
                var course = db.Courses.FirstOrDefault(c => c.Id == courseIdForDb);
                if (course == null)
                {
                    return BadRequest("Course not found");
                }
                subdivision.Courses.Remove(course);
                db.SaveChanges();
                return Ok(subdivision);
            }
        }

        [HttpDelete("{id}/moderators/{moderatorId}")]
        public ActionResult<UserModel> RemoveSubdivisionModerator(string id, string moderatorId)
        {
            Guid.TryParse(id, out Guid subdivisionId);
            Guid.TryParse(moderatorId, out Guid moderatorIdForDb);
            using (var db = new ClassroomapiContext())
            {
                var subdivision = db.Subdivisions
                    .Include(s => s.Moderators)
                    .FirstOrDefault(s => s.Id == subdivisionId);

                if (subdivision == null)
                {
                    return BadRequest("Subdivision not found");
                }

                var moderator = db.Users
                    .FirstOrDefault(m => m.Id == moderatorIdForDb);

                if (moderator == null)
                {
                    return BadRequest("Moderator not found");
                }
                subdivision.Moderators.Remove(moderator);
                moderator.Subdivisions.Remove(subdivision);
                db.SaveChanges();

                return Ok(subdivision);
            }
        }
        #endregion


    }
}
