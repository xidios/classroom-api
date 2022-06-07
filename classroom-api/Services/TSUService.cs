using classroom_api.Constants;
using classroom_api.Models.TSU;
using Microsoft.AspNetCore.Mvc;

namespace classroom_api.Services
{
    public interface ITSUService
    {
        public Task<List<TSUStudent>> GetStudents(string groupNumber);
    }
    public class TSUService : ITSUService
    {
        private readonly IHttpClientService _httpClientService;
        private HttpClient LKStudentClient;
        public TSUService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            LKStudentClient = _httpClientService.InitLKStudentHttpClient();
        }

        public async Task<List<TSUStudent>> GetStudents(string groupNumber)
        {
            var uri = TSUSitePaths.LKStudentAPIPath + "students/group/" + groupNumber;
            var response = await LKStudentClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var students = await response.Content.ReadFromJsonAsync<List<TSUStudent>>();
                if (students == null)
                {
                    throw new NullReferenceException("Students not found");
                }
                return students;
            }
            throw new BadHttpRequestException("",(int)response.StatusCode);
        }
    }
}
