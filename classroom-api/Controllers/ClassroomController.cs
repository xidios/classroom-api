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
using classroom_api.Models.TSU;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace classroom_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private ClassroomService classroomService = ClassRoomOAuth.GetClassroomService();
        private readonly ClassroomapiContext _context;
        private readonly ITSUService _TSUService;
        public ClassroomController(ClassroomapiContext context,
            ITSUService TSUService)
        {
            _context = context;
            _TSUService = TSUService;
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
                StudentsInvitationCount = invitations.Where(i => i.Role == CourseRoleEnum.Student && i.Status.ToUpper() == "OK").Count(),
                TeacherInvitationCount = invitations.Where(i => i.Role == CourseRoleEnum.Teacher && i.Status.ToUpper() == "OK").Count()
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
        //TODO: Доделайть проблему с инвайтами
        [HttpPost("invite/student")]
        public async Task<ActionResult<List<InvitationModel>>> InviteStudents([FromBody] InvitePersonModel model)
        {
            if (model.CourseId == null)
            {
                return BadRequest("Course id is empty");
            }

            if (model.AccountId == null)
            {
                return BadRequest("Student id is empty");
            }
            var course = await _context.Courses
                .Include(c=>c.Invations)
                .Where(c=>c.Id == model.CourseId).FirstOrDefaultAsync();
            if(course == null)
            {
                return NotFound("Course not found");
            }
            var localStudent =await _context.CourseUsers
                .Where(s=>s.AccountId == model.AccountId).FirstOrDefaultAsync();
            if(localStudent == null) 
            {
                var student = await _TSUService.GetTSUNameAndEmail(model.AccountId);
                if(student == null)
                {
                    return NotFound("Student not found");
                }
                localStudent = new CourseUserModel
                {
                    AccountId = student.AccountId,
                    Email = student.Email,
                    Name = student.FullName,
                };
                await _context.AddAsync(localStudent);
                await _context.SaveChangesAsync();
            }
            try
            {
                Invitation invite = new Invitation
                {
                    CourseId = course.GoogleId,
                    UserId = localStudent.Email,
                    Role = "STUDENT"
                };
                var inviteResponse = classroomService.Invitations.Create(invite).Execute();
                var localInvitation =  course.Invations.Where(i => i.AccountId == localStudent.AccountId).FirstOrDefault();
                if (localInvitation!=null)
                {                    
                    course.Invations.Remove(localInvitation);
                    _context.Remove(localInvitation);
                    course.CourseUsers.Remove(localStudent);
                    await _context.SaveChangesAsync();
                }
                InvitationModel invitationModel = new InvitationModel
                {
                    CourseId = model.CourseId,
                    AccountId = localStudent.AccountId,
                    Course = course,
                    Email = localStudent.Email,
                    Role = CourseRoleEnum.Student,
                    GoogleInvitationId = inviteResponse.Id
                };
                _context.Invitations.Add(invitationModel);
                localStudent.Courses.Add(course);
                course.CourseUsers.Add(localStudent);
                course.Invations.Add(invitationModel);
                _context.UpdateRange(course, localStudent);
                await _context.SaveChangesAsync();

                return Ok(invite);

            }
            catch (GoogleApiException ex)
            {
                return BadRequest(GoogleApiExceptionReturnMessage(ex));
            }
        }

        [HttpPost("invite/teacher")]
        public async Task<ActionResult<List<InvitationModel>>> InviteTeachers([FromBody] InvitePersonModel model)
        {
            if (model.CourseId == null)
            {
                return BadRequest("Course id is empty");
            }

            if (model.AccountId == null)
            {
                return BadRequest("Teacher id is empty");
            }
            var course = await _context.Courses.Where(c => c.Id == model.CourseId).FirstOrDefaultAsync();
            if (course == null)
            {
                return NotFound("Course not found");
            }
            var localTeacher = await _context.CourseUsers.Where(s => s.AccountId == model.AccountId).FirstOrDefaultAsync();
            if (localTeacher == null)
            {
                var student = await _TSUService.GetTSUNameAndEmail(model.AccountId);
                if (student == null)
                {
                    return NotFound("Teacher not found");
                }
                localTeacher = new CourseUserModel
                {
                    AccountId = student.AccountId,
                    Email = student.Email,
                    Name = student.FullName,
                };
                await _context.AddAsync(localTeacher);
                await _context.SaveChangesAsync();
            }
            try
            {
                Invitation invite = new Invitation
                {
                    CourseId = course.GoogleId,
                    UserId = localTeacher.Email,
                    Role = "TEACHER"
                };
                var inviteResponse = classroomService.Invitations.Create(invite).Execute();

                InvitationModel invitationModel = new InvitationModel
                {
                    CourseId = model.CourseId,
                    AccountId = localTeacher.AccountId,
                    Course = course,
                    Email = localTeacher.Email,
                    Role = CourseRoleEnum.Teacher,
                    GoogleInvitationId = inviteResponse.Id
                };
                _context.Invitations.Add(invitationModel);
                localTeacher.Courses.Add(course);
                course.CourseUsers.Add(localTeacher);
                course.Invations.Add(invitationModel);
                _context.UpdateRange(course, localTeacher);
                await _context.SaveChangesAsync();

                return Ok(invite);

            }
            catch (GoogleApiException ex)
            {
                return BadRequest(GoogleApiExceptionReturnMessage(ex));
            }
        }

        //[HttpPost("invite/group")]
        //public async Task<ActionResult<List<Student>>> InviteGroup([FromBody] InviteGroupModel model)
        //{
        //    if (model.AccountIdList.Count() == 0)
        //    {
        //        return BadRequest("Id list is empty");
        //    }
        //    List<InvitationModel> InvitationsResult = new List<InvitationModel>();
        //    foreach (var accountId in model.AccountIdList)
        //    {
        //        try
        //        {
        //            Invitation invite = new Invitation
        //            {
        //                CourseId = model.CourseId.ToString(),
        //                UserId = accountId,
        //                Role = "STUDENT"
        //            };
        //            var inviteResponse = classroomService.Invitations.Create(invite).Execute();

        //            InvitationModel invitationModel = new InvitationModel
        //            {
        //                CourseId = model.CourseId,
        //                Email = accountId,
        //                Role = "STUDENT",
        //                GoogleInvitationId = inviteResponse.Id
        //            };
        //            _context.Invitations.Add(invitationModel);
        //            await _context.SaveChangesAsync();

        //            InvitationsResult.Add(invitationModel);

        //        }
        //        catch (GoogleApiException ex)
        //        {
        //            string errorException = GoogleApiExceptionReturnMessage(ex);
        //            InvitationsResult.Add(new InvitationModel
        //            {
        //                CourseId = model.CourseId,
        //                Email = accountId,
        //                GoogleInvitationId = errorException,
        //                Role = errorException,
        //                Status = errorException
        //            });
        //            continue;
        //        }
        //    }
        //    return Ok(InvitationsResult);
        //}

        [HttpPost("invite/group")]
        public async Task<ActionResult<List<string>>> InviteGroup(string groupNumber, Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.CourseUsers)
                .Include(c=>c.Invations)
                .Where(c => c.Id == courseId).FirstOrDefaultAsync();
            

            if (course == null)
            {
                return NotFound("Course not found");
            }
            List<string> InvitationsResult = new List<string>();
            List<TSUStudent> students;

            try
            {
                var kek = new TSUStudent
                {
                    AccountId = Guid.Parse("f43c6ea0-b2a2-48a7-91bc-1334b0cd3312"),
                    Email = "redbull-18@bk.ru",
                    FirstName = "Prisyach",
                    LastName = "Vladimir",
                    Patronymic = "Olegovich",
                    Status = 0
                };
                //students = await _TSUService.GetStudents(groupNumber);
                students = new List<TSUStudent>() { kek };
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            var allStudents = await _context.CourseUsers.ToListAsync();
            foreach (var student in students)
            {
                try
                {
                    if(student.Status == 2) // отчислен
                    {
                        continue;
                    }
                    if (course.CourseUsers.Any(s => s.AccountId == student.AccountId) && course.Invations.Any(i=>i.AccountId == student.AccountId && i.Status == "OK"))
                    {
                        InvitationsResult.Add($"{student.AccountId}: already invited");
                        continue;
                    }
                    Invitation invite = new Invitation
                    {
                        CourseId = course.GoogleId,
                        UserId = student.Email,
                        Role = "STUDENT"
                    };
                    var inviteResponse = classroomService.Invitations.Create(invite).Execute();

                    InvitationModel invitationModel = new InvitationModel
                    {
                        CourseId = courseId,
                        Course = course,
                        Email = student.Email,
                        Role = CourseRoleEnum.Student,
                        GoogleInvitationId = inviteResponse.Id,
                        AccountId = student.AccountId
                    };
                    var currentStudent = allStudents.Where(s => s.AccountId == student.AccountId).FirstOrDefault();
                    if (currentStudent == null)
                    {
                        currentStudent = new CourseUserModel
                        {
                            AccountId = student.AccountId,
                            Email = student.Email,
                            Name = student.LastName + " " + student.FirstName + " " + student.Patronymic
                            
                        };
                        _context.CourseUsers.Add(currentStudent);
                        await _context.SaveChangesAsync();
                    }
                    currentStudent.Courses.Add(course);
                    _context.Invitations.Add(invitationModel);
                    _context.CourseUsers.Add(currentStudent);
                    course.CourseUsers.Add(currentStudent);
                    course.Invations.Add(invitationModel);
                    _context.UpdateRange(course, currentStudent);
                    await _context.SaveChangesAsync();
                    InvitationsResult.Add($"{student.AccountId}: invited");
                }
                catch (GoogleApiException ex)
                {
                    string errorException = GoogleApiExceptionReturnMessage(ex);
                    InvitationsResult.Add($"{student.AccountId}: {errorException}");
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

        //TODO: invite
        //private async Task<ActionResult> InviteToClassroomByRole(InvitePersonModel model, string role)
        //{
        //    if (model.CourseId == null)
        //    {
        //        return BadRequest("Course id is empty");
        //    }

        //    if (model.AccountId == null && model.AccountId == "")
        //    {
        //        return BadRequest("Student id is empty");
        //    }
        //    try
        //    {
        //        Invitation invite = new Invitation
        //        {
        //            CourseId = model.CourseId.ToString(),
        //            UserId = model.AccountId,
        //            Role = role
        //        };
        //        var inviteResponse = classroomService.Invitations.Create(invite).Execute();

        //        InvitationModel invitationModel = new InvitationModel
        //        {
        //            CourseId = model.CourseId,
        //            Email = model.AccountId,
        //            Role = role,
        //            GoogleInvitationId = inviteResponse.Id
        //        };
        //        _context.Invitations.Add(invitationModel);
        //        await _context.SaveChangesAsync();

        //        return Ok(invite);

        //    }
        //    catch (GoogleApiException ex)
        //    {
        //        return BadRequest(GoogleApiExceptionReturnMessage(ex));
        //    }

        //}
    }
}
