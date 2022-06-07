using Google.Apis.Classroom.v1;
using Microsoft.AspNetCore.Mvc;
using classroom_api.Services;
using Google.Apis.Classroom.v1.Data;
using System.Net;
using classroom_api.Models;
using classroom_api.Enums;
using Google;
using System;
using classroom_api.FromBodyModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace classroom_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private ClassroomService classroomService = ClassRoomOAuth.GetClassroomService();
        private readonly ClassroomapiContext _context;
        public ClassroomController(ClassroomapiContext context)
        {
            _context = context;
        }
        #region HTTP_GET

        [HttpGet("list")]
        public async Task<ActionResult<List<CourseModel>>> GetClassroomList()
        {
            return Ok(await _context.Courses.ToListAsync());
        }
        [HttpGet("list/active")]
        public async Task<ActionResult<List<CourseModel>>> GetClassroomActiveList()
        {

            return Ok(await _context.Courses.Where(c => c.CourseState.ToLower() == "active").ToListAsync());

        }
        [HttpGet("list/archived")]
        public async Task<ActionResult<List<CourseModel>>> GetClassroomArchiveList()
        {
            return Ok(await _context.Courses.Where(c => c.CourseState.ToLower() == "archived").ToListAsync());

        }
        [HttpGet("info/{userId}")]
        public ActionResult GetUserInformation(string userId)
        {
            try
            {
                UserProfile user = classroomService.UserProfiles.Get(userId).Execute();
                return Ok(user);
            }
            catch (GoogleApiException ex)
            {
                return BadRequest(GoogleApiExceptionReturnMessage(ex));
            }
        }

        [HttpGet("updateInvitation")]
        public async Task<ActionResult<List<InvitationModel>>> UpdateInvitationStatus()
        {
            List<InvitationModel> invitations;

            invitations = await _context.Invitations.ToListAsync();

            foreach (var invitation in invitations)
            {
                try
                {
                    var invitationResponse = classroomService.Invitations.Get(invitation.GoogleInvitationId).Execute();
                }
                catch (GoogleApiException ex)
                {
                    if (ex.HttpStatusCode == HttpStatusCode.NotFound)
                    {
                        invitation.Status = "NOT FOUND";
                    }
                    continue;
                }
            }
            await _context.SaveChangesAsync();
            return Ok(invitations);
        }
        [HttpGet("invite/delete/{id}")]
        public async Task<ActionResult<Invitation>> DeleteInvitation(string id)
        {
            Guid invitationId;
            if (id == null || id == "")
            {
                return BadRequest("Id is empty");
            }
            try
            {
                invitationId = Guid.Parse(id);
            }
            catch
            {
                return BadRequest("Id isn't correct");
            }
            try
            {
                InvitationModel? invitation = await _context.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId);
                if (invitation == null)
                {
                    return BadRequest("INVITATION NOT FOUND");
                }
                var invitationDeleteResponse = classroomService.Invitations.Delete(invitation.GoogleInvitationId);
                _context.Invitations.Remove(invitation);
                await _context.SaveChangesAsync();
                return Ok(invitationDeleteResponse);
            }
            catch (GoogleApiException ex)
            {
                return BadRequest(GoogleApiExceptionReturnMessage(ex));
            }
        }
        [HttpGet("stats")]
        public async Task<ActionResult<StatisticModel>> GetTechStatistic()
        {

            List<InvitationModel> invitations = await _context.Invitations.ToListAsync();
            List<CourseModel> courses = await _context.Courses.ToListAsync();
            StatisticModel statistic = new StatisticModel
            {
                ClassesCreatedCount = courses.Count,
                StudentsInvitationCount = invitations.Where(i => i.Role.ToUpper() == "STUDENT" && i.Status.ToUpper() == "OK").Count(),
                TeacherInvitationCount = invitations.Where(i => i.Role.ToUpper() == "TEACHER" && i.Status.ToUpper() == "OK").Count()
            };
            return Ok(statistic);

        }

        [HttpGet("tasks")]
        public async Task<ActionResult<List<CourseWork>>> CheckCourseworks()
        {
            List<CourseModel> courses = await _context.Courses.Where(c => c.CourseState.ToUpper() == "ACTIVE").ToListAsync();
            List<CourseWork> courseWorks = new List<CourseWork>();
            foreach (var course in courses)
            {
                try
                {
                    IList<CourseWork> courseWorksResponse = classroomService.Courses.CourseWork.List(course.GoogleId).Execute().CourseWork;
                    if (courseWorksResponse != null && courseWorksResponse.Count() != 0)
                        courseWorks.AddRange(courseWorksResponse);
                }
                catch (GoogleApiException ex)
                {
                    Console.WriteLine(GoogleApiExceptionReturnMessage(ex));
                    continue;
                }
            }
            return Ok(courseWorks);
        }

        [HttpGet("tasks/substring")]
        public async Task<ActionResult<List<StudentSubmission>>> GetAllCourseWorksSubstrings()
        {
            List<CourseModel> courses = await _context.Courses.Where(c => c.CourseState.ToUpper() == "ACTIVE").ToListAsync();
            List<CourseWork> courseWorks = new List<CourseWork>();
            List<StudentSubmission> studentSubmissions = new List<StudentSubmission>();
            foreach (var course in courses)
            {
                try
                {
                    IList<CourseWork> courseWorksResponse = classroomService.Courses.CourseWork.List(course.GoogleId).Execute().CourseWork;
                    if (courseWorksResponse != null && courseWorksResponse.Count() != 0)
                        courseWorks.AddRange(courseWorksResponse);
                }
                catch (GoogleApiException ex)
                {
                    Console.WriteLine(GoogleApiExceptionReturnMessage(ex));
                    continue;
                }
            }

            foreach (var courseWork in courseWorks)
            {
                try
                {
                    var studentSubmissionsResponse = classroomService.Courses.CourseWork.StudentSubmissions.List(courseWork.CourseId, courseWork.Id).Execute().StudentSubmissions;
                    if (studentSubmissionsResponse != null && studentSubmissionsResponse.Count() != 0)
                        studentSubmissions.AddRange(studentSubmissionsResponse);
                }
                catch (GoogleApiException ex)
                {
                    Console.WriteLine(GoogleApiExceptionReturnMessage(ex));
                    continue;
                }
            }
            return Ok(studentSubmissions);
        }

        #endregion

        #region HTTP_POST

        [HttpPost("create")]
        public async Task<ActionResult<CourseModel>> CreateClassroomCourse([FromBody] ClassroomCreateFromBodyModel model)
        {
            if (model.Name == null)
            {
                return BadRequest("A name cannot be empty");
            }
            //if (!CheckClassroomState(model.CourseState))
            //{
            //    return BadRequest("Wrong value of course state");
            //}
            Course course = new Course()
            {
                Name = model.Name,
                Section = model.Section,
                Description = model.Description,
                DescriptionHeading = model.DescriptionHeading,
                Room = model.Room,
                //CourseState = model.CourseState,
                OwnerId = "me"
            };
            try
            {
                course = classroomService.Courses.Create(course).Execute();

                CourseModel courseModel = new CourseModel
                {
                    Name = course.Name,
                    Description = course.Description,
                    DescriptionHeading = course.DescriptionHeading,
                    Section = course.Section,
                    CourseState = course.CourseState,
                    GoogleId = course.Id
                };
                _context.Courses.Add(courseModel);
                await _context.SaveChangesAsync();

                return Ok(courseModel);

            }
            catch (GoogleApiException ex)
            {
                return BadRequest(GoogleApiExceptionReturnMessage(ex));
            }

        }

        [HttpPost("invite/student")]
        public async Task<ActionResult<List<InvitationModel>>> InviteStudents([FromBody] InvitePersonModel model)
        {
            return await InviteToClassroomByRole(model, "STUDENT");
        }

        [HttpPost("invite/teacher")]
        public async Task<ActionResult<List<InvitationModel>>> InviteTeachers([FromBody] InvitePersonModel model)
        {
            return await InviteToClassroomByRole(model, "TEACHER");
        }

        [HttpPost("invite/group")]
        public async Task<ActionResult<List<Student>>> InviteGroup([FromBody] InviteGroupModel model)
        {
            if (model.AccountIdList.Count() == 0)
            {
                return BadRequest("Id list is empty");
            }
            List<InvitationModel> InvitationsResult = new List<InvitationModel>();
            foreach (var accountId in model.AccountIdList)
            {
                try
                {
                    Invitation invite = new Invitation
                    {
                        CourseId = model.CourseId.ToString(),
                        UserId = accountId,
                        Role = "STUDENT"
                    };
                    var inviteResponse = classroomService.Invitations.Create(invite).Execute();

                    InvitationModel invitationModel = new InvitationModel
                    {
                        CourseId = model.CourseId,
                        Email = accountId,
                        Role = "STUDENT",
                        GoogleInvitationId = inviteResponse.Id
                    };
                    _context.Invitations.Add(invitationModel);
                    await _context.SaveChangesAsync();

                    InvitationsResult.Add(invitationModel);

                }
                catch (GoogleApiException ex)
                {
                    string errorException = GoogleApiExceptionReturnMessage(ex);
                    InvitationsResult.Add(new InvitationModel
                    {
                        CourseId = model.CourseId,
                        Email = accountId,
                        GoogleInvitationId = errorException,
                        Role = errorException,
                        Status = errorException
                    });
                    continue;
                }
            }
            return Ok(InvitationsResult);
        }

        #endregion

        #region HTTP_PATCH

        [HttpPatch("update")]
        public async Task<ActionResult<CourseModel>> UpdateClassroom([FromBody] ClassroomPutchFromBodyModel model)
        {
            CourseModel? courseFromDb;
            Course course;
            try
            {
                courseFromDb = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.Id);
                if (courseFromDb == null)
                {
                    return BadRequest("COURSE NOT FOUND");
                }
                course = classroomService.Courses.Get(courseFromDb.GoogleId).Execute();

            }
            catch (GoogleApiException ex)
            {
                return BadRequest(GoogleApiExceptionReturnMessage(ex));
            }

            if (model.Name != null)
            {
                course.Name = model.Name;
                courseFromDb.Name = course.Name;
            }

            if (model.Description != null)
            {
                course.Description = model.Description;
                courseFromDb.Description = course.Description;
            }

            if (model.Section != null)
            {
                course.Section = model.Section;
                courseFromDb.Section = course.Section;
            }

            if (model.DescriptionHeading != null)
            {
                course.DescriptionHeading = model.DescriptionHeading;
                courseFromDb.DescriptionHeading = course.DescriptionHeading;
            }

            if (model.Room != null) // TODO: MB UPDATE DB MODEL
                course.Room = model.Room;

            if (model.CourseState != null)
            {
                if (CheckClassroomState(model.CourseState))
                {
                    course.CourseState = model.CourseState;
                    courseFromDb.CourseState = course.CourseState;
                }
            }
            try
            {
                course = classroomService.Courses.Update(course, courseFromDb.GoogleId).Execute();
                await _context.SaveChangesAsync();
                return Ok(courseFromDb);

            }
            catch (GoogleApiException ex)
            {
                return BadRequest(GoogleApiExceptionReturnMessage(ex));
            }
        }

        #endregion

        private string GoogleApiExceptionReturnMessage(GoogleApiException ex)
        {
            switch (ex.HttpStatusCode)
            {
                case HttpStatusCode.NotFound: return "NOT FOUND";
                case HttpStatusCode.Forbidden: return "PERMISSION DENIED";
                case HttpStatusCode.BadRequest: return "FAILED PRECONDITION";
                case HttpStatusCode.Conflict: return "ALREADY EXISTS";
                case HttpStatusCode.UnprocessableEntity: return "INVALID ARGUMENT"; // не уверен
                default: return "UNKNOWN ERROR";
            }
        }
        private bool CheckClassroomState(string? classroomStatus)
        {
            if (classroomStatus == null)
            {
                return false;
            }
            var enumParams = Enum.GetNames(typeof(CourseStateEnum));

            string? a = enumParams.FirstOrDefault(p => p.ToLower() == classroomStatus.ToLower());

            if (a == null)
            {
                return false;
            }
            return true;
        }

        private async Task<ActionResult> InviteToClassroomByRole(InvitePersonModel model, string role)
        {
            if (model.CourseId == null)
            {
                return BadRequest("Course id is empty");
            }

            if (model.AccountId == null && model.AccountId == "")
            {
                return BadRequest("Student id is empty");
            }
            try
            {
                Invitation invite = new Invitation
                {
                    CourseId = model.CourseId.ToString(),
                    UserId = model.AccountId,
                    Role = role
                };
                var inviteResponse = classroomService.Invitations.Create(invite).Execute();

                InvitationModel invitationModel = new InvitationModel
                {
                    CourseId = model.CourseId,
                    Email = model.AccountId,
                    Role = role,
                    GoogleInvitationId = inviteResponse.Id
                };
                _context.Invitations.Add(invitationModel);
                await _context.SaveChangesAsync();

                return Ok(invite);

            }
            catch (GoogleApiException ex)
            {
                return BadRequest(GoogleApiExceptionReturnMessage(ex));
            }

        }
    }
}
