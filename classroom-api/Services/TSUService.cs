using classroom_api.Constants;
using classroom_api.Models.TSU;
using Microsoft.AspNetCore.Mvc;

namespace classroom_api.Services
{
    public interface ITSUService
    {
        public Task<List<TSUStudent>> GetStudents(string groupNumber);
        public Task<TSUNameAndEmail> GetTSUNameAndEmail(Guid accountId);
    }
    public class TSUService : ITSUService
    {
        private readonly IHttpClientService _httpClientService;
        private HttpClient LKStudentClient;
        private HttpClient TSUAccounts;
        public TSUService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            LKStudentClient = _httpClientService.InitLKStudentHttpClient();
            TSUAccounts = _httpClientService.InitTSUAccountsHttpClient();
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
        public async Task<TSUNameAndEmail> GetTSUNameAndEmail(Guid accountId)
        {
            var uri = "https://accounts.tsu.ru/api/Profile/GetUserModel/?id=" + accountId.ToString();
            var response = await TSUAccounts.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var info = await response.Content.ReadFromJsonAsync<TSUNameAndEmail>();
                if (info == null)
                {
                    throw new NullReferenceException("User not found");
                }
                return info;
            }
            throw new BadHttpRequestException("", (int)response.StatusCode);
        }

    }
}
