using Google.Apis.Classroom.v1;
using Microsoft.AspNetCore.Mvc;
using classroom_api.Services;
using Google.Apis.Classroom.v1.Data;
using System.Net;
using classroom_api.Models;
using classroom_api.Enums;
using Google;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace classroom_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private ClassroomService classroomService = ClassRoomOAuth.GetClassroomService();
        #region HTTP_GET

        [HttpGet("list")]
        public ActionResult<List<Course>> GetClassroomList()
        {
            using (ClassroomapiContext context = new ClassroomapiContext())
            {
                var student = context.Students.SingleOrDefault(s => s.AccountId == userId || s.Email == userId);
                if (student == null)
                {
                    return BadRequest("Student not found");
                }
                return Ok(student.Courses);
            }
        }
        [HttpGet("info/{userId}")]
        public ActionResult GetUserInformation(string userId)
        {
            try
            {
                UserProfile user = classroomService.UserProfiles.Get(userId).Execute();
                return Ok(user);
            }
            catch
            {
                return BadRequest("Some problems with this user");
            }
        }


        #endregion
    }
    class ClassroomControllerOld : ControllerBase
    {
        private ClassroomService classroomService = ClassRoomOAuth.GetClassroomService();

        #region HttpGet
        [HttpGet("info/{userId}")]
        public ActionResult GetUserInformation(string userId)
        {
            try
            {
                UserProfile user = classroomService.UserProfiles.Get(userId).Execute();
                return Ok(user);
            }
            catch
            {
                return BadRequest("Some problems with this user");
            }
        }
        [HttpGet("list")]
        public ActionResult<IEnumerable<Course>> GetClassroomList()
        {
            return Ok(GetCoursesByCourseState(null));
        }

        [HttpGet("list/active")]
        public ActionResult<List<Course>> GetClassroomActiveList()
        {
            return Ok(GetCoursesByCourseState("ACTIVE"));
        }
        [HttpGet("list/archived")]
        public ActionResult<List<Course>> GetClassroomArchivedList()
        {
            return Ok(GetCoursesByCourseState("ARCHIVED"));
        }

        [HttpGet("{id}")]
        public ActionResult<Course> GetClassroomCourse(string id)
        {
            try
            {
                Course? course = classroomService.Courses.Get(id).Execute();
                return Ok(course);
            }
            catch
            {
                return BadRequest("Course not found");
            }
        }

        [HttpGet("stats")]
        public ActionResult<StatisticModel> GetTechStatistic()
        {
            return Ok("Will be later");
        }

        [HttpGet("invite/delete/{invitationId}")]
        public ActionResult<Invitation> DeleteInvitation(string invitationId)
        {
            if (invitationId == null)
            {
                BadRequest("Invitation id is empty");
            }
            try
            {
                var invitationResponse = classroomService.Invitations.Delete(invitationId).Execute();
                return Ok(invitationResponse);
            }
            catch (GoogleApiException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    return BadRequest("Invitation not found");
                }
                return BadRequest("Some problems with invitation");
            }
        }

        [HttpGet("invite/{invitationId}")]
        public ActionResult<Invitation> GetInvitation(string invitationId)
        {
            if (invitationId == null)
            {
                BadRequest("Invitation id is empty");
            }
            try
            {
                var invitationResponse = classroomService.Invitations.Get(invitationId).Execute();
                return Ok(invitationResponse);
            }
            catch (GoogleApiException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    return BadRequest("Invitation not found");
                }
                return BadRequest("Some problems with invitation");
            }
        }
        [HttpGet("tasks")]
        public ActionResult<List<CourseWork>> CheckCourseworks()
        {
            List<CourseWork> courseWorksList = new List<CourseWork>();
            IList<Course> activeCourses = GetCoursesByCourseState("ACTIVE");

            if (activeCourses == null)
            {
                return BadRequest("Active courses are NULL");
            }

            foreach (var course in activeCourses)
            {
                var courseWorksResponse = classroomService.Courses.CourseWork.List(course.Id).Execute();
                var courseWorks = courseWorksResponse.CourseWork;
                courseWorksList.AddRange(courseWorks);
            }
            return Ok(courseWorksList);
        }

        #endregion

        #region HttpPost
        [HttpPost("create")]
        public ActionResult<Course> CreateClassroomCourse([FromBody] ClassroomModel model)
        {
            if (model.Name == null)
            {
                return BadRequest("A name cannot be empty");
            }
            if (!CheckClassroomState(model.CourseState))
            {
                return BadRequest("Wrong value of course state");
            }
            Course course = new Course()
            {
                Name = model.Name,
                Section = model.Section,
                Description = model.Description,
                DescriptionHeading = model.DescriptionHeading,
                Room = model.Room,
                CourseState = model.CourseState,
                OwnerId = "me"
            };
            course = classroomService.Courses.Create(course).Execute();
            if (course == null)
            {
                return BadRequest("Problems in creating the course");
            }
            return Ok(course);
        }
        [HttpPost("invite/students")]
        public ActionResult<List<Student>> InviteStudents([FromBody] InviteModel model)
        {
            return InviteToClassroomByRole(model, "STUDENT");
        }
        [HttpPost("invite/teachers")]
        public ActionResult<List<Teacher>> InviteTeachers([FromBody] InviteModel model)
        {
            return InviteToClassroomByRole(model, "TEACHER");
        }

        #endregion

        #region HttpPatch

        [HttpPatch("update/{id}")]
        public ActionResult<Course> UpdateClassroom(string id, [FromBody] ClassroomModel model)
        {
            Course course;
            try
            {
                course = classroomService.Courses.Get(id).Execute();
            }
            catch
            {
                return BadRequest("Course not found");
            }

            if (model.Name != null)
                course.Name = model.Name;

            if (model.Description != null)
                course.Description = model.Description;

            if (model.Section != null)
                course.Section = model.Section;

            if (model.DescriptionHeading != null)
                course.DescriptionHeading = model.DescriptionHeading;

            if (model.Room != null)
                course.Room = model.Room;
            if (model.CourseState != null)
            {
                if (CheckClassroomState(model.CourseState))
                {
                    course.CourseState = model.CourseState;
                }
            }
            course = classroomService.Courses.Update(course, id).Execute();
            return Ok(course);
        }

        #endregion

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

        private ActionResult InviteToClassroomByRole(InviteModel model, string role)
        {
            if (model.CourseId == null)
            {
                return BadRequest("Course id is empty");
            }

            if (model.AccountIdList.Count() == 0)
            {
                return BadRequest("Empty student list");
            }

            List<string> invitations = new List<string>();
            string textResponse = "";
            foreach (string accountId in model.AccountIdList)
            {
                try
                {
                    Invitation invite = new Invitation
                    {
                        CourseId = model.CourseId,
                        UserId = accountId,
                        Role = role
                    };
                    textResponse = accountId;
                    var inviteResponse = classroomService.Invitations.Create(invite).Execute();
                    textResponse += " Invited" + " Invitation Id: " + inviteResponse.Id;
                    invitations.Add(textResponse);
                }
                catch (GoogleApiException e)
                {
                    textResponse += " " + e.Error.Message;
                    invitations.Add(textResponse);
                }
            }
            return Ok(invitations);
        }

        private IList<Course> GetCoursesByCourseState(string? courseState)
        {
            CoursesResource.ListRequest request = classroomService.Courses.List();
            ListCoursesResponse response = request.Execute();

            if (courseState == null)
            {
                return response.Courses;
            }

            return response.Courses.Where(c => c.CourseState == courseState).ToList();
        }
    } //OLD
}
