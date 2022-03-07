﻿using Google.Apis.Classroom.v1;
using Microsoft.AspNetCore.Mvc;
using classroom_api.Services;
using Google.Apis.Classroom.v1.Data;
using System.Net;
using classroom_api.Models;
using classroom_api.Enums;
using Google;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace classroom_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private ClassroomService classroomService = ClassRoomOAuth.GetClassroomService();

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
            CoursesResource.ListRequest request = classroomService.Courses.List();
            ListCoursesResponse response = request.Execute();

            return Ok(response.Courses);
        }

        [HttpGet("list/active")]
        public ActionResult<IEnumerable<Course>> GetClassroomActiveList()
        {
            CoursesResource.ListRequest request = classroomService.Courses.List();
            ListCoursesResponse response = request.Execute();

            return Ok(response.Courses.Where(c => c.CourseState == "ACTIVE"));
        }
        [HttpGet("list/archived")]
        public ActionResult<IEnumerable<Course>> GetClassroomArchivedList()
        {
            CoursesResource.ListRequest request = classroomService.Courses.List();
            ListCoursesResponse response = request.Execute();

            return Ok(response.Courses.Where(c => c.CourseState == "ARCHIVED"));
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
                    textResponse += " Invited";
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
    }
}
