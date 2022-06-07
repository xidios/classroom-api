using classroom_api.Models;
using classroom_api.Models.TSU;
using classroom_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace classroom_api.Controllers
{
    [Route("tsu")]
    public class TSUController : ControllerBase
    {
        private readonly IHttpClientService _httpClientService;


        private string lk_path = "https://api.lk.student.tsu.ru/";
        private string persona_path = "https://persona.tsu.ru/api/";
        private HttpClient LKStudentClient;
        private HttpClient PersonaTSUClient;
        public TSUController(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            LKStudentClient = _httpClientService.InitLKStudentHttpClient();
            PersonaTSUClient = _httpClientService.InitPersonaTSUHttpClient();
        }
        #region LK_Students
        [HttpGet("faculties")]
        public async Task<ActionResult<List<TSUDepartament>>> GetFaculties()
        {
            var uri = lk_path + "departments";                                             
            var response = await LKStudentClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var departaments = await response.Content.ReadFromJsonAsync<List<TSUDepartament>>();
                return Ok(departaments);
            }
            else if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            return BadRequest();
        }
        [HttpGet("departments/groups_by_faculty")]
        public async Task<ActionResult<List<TSUGroup>>> GetGroups(Guid facultyId)
        {
            var uri = lk_path + "departments/groups_by_faculty/" + facultyId.ToString();
            var response = await LKStudentClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var groups = await response.Content.ReadFromJsonAsync<List<TSUGroup>>();
                return Ok(groups);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            return BadRequest();
        }
        [HttpGet("students/group/{groupNumber}")]
        public async Task<ActionResult<List<TSUStudent>>> GetStudents(string groupNumber)
        {
            var uri = lk_path + "students/group/" + groupNumber;
            var response = await LKStudentClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var students = await response.Content.ReadFromJsonAsync<List<TSUStudent>>();
                return Ok(students);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            return BadRequest();
        }
        #endregion

        #region PersonaTSU

        [HttpGet("teachers/find/{namePart}")]
        public async Task<ActionResult<List<TSUTeacher>>> GetTeachers(string namePart)
        {
            var uri = persona_path + "/FindUsers?namePart=" + namePart;
            var response = await PersonaTSUClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var teachers = await response.Content.ReadFromJsonAsync<List<TSUTeacher>>();
                return Ok(teachers);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            return BadRequest();
        }
        #endregion

    }

}
