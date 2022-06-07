using classroom_api.Constants;
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
        private readonly ITSUService _TSUService;

        private HttpClient LKStudentClient;
        private HttpClient PersonaTSUClient;
        public TSUController(IHttpClientService httpClientService,
            ITSUService TSUService)
        {
            _httpClientService = httpClientService;
            _TSUService = TSUService;
            LKStudentClient = _httpClientService.InitLKStudentHttpClient();
            PersonaTSUClient = _httpClientService.InitPersonaTSUHttpClient();
        }
        #region LK_Students
        [HttpGet("faculties")]
        public async Task<ActionResult<List<TSUDepartament>>> GetFaculties()
        {
            var uri = TSUSitePaths.LKStudentAPIPath + "departments";                                             
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
            var uri = TSUSitePaths.LKStudentAPIPath + "departments/groups_by_faculty/" + facultyId.ToString();
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
            try
            {
                return Ok(await _TSUService.GetStudents(groupNumber));
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(ex.StatusCode);
            }
        }
        #endregion

        #region PersonaTSU

        [HttpGet("teachers/find/{namePart}")]
        public async Task<ActionResult<List<TSUTeacher>>> GetTeachers(string namePart)
        {
            var uri = TSUSitePaths.PersonaTSUAPIPath + "/FindUsers?namePart=" + namePart;
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
