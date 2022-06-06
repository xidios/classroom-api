using System.Net.Http.Headers;
using System.Text;

namespace classroom_api.Services
{
    public interface IMiddlewareService
    {
        public HttpClient InitLKStudentHttpClient();

    }
    public class MiddlewareService : IMiddlewareService
    {
        private readonly IConfiguration _configuration;
        public MiddlewareService(IConfiguration configuration)
        {
             _configuration = configuration;
        }
        public HttpClient InitLKStudentHttpClient()
        {
            HttpClient client = new HttpClient();
            var lk = _configuration.GetSection("LKStudent");
            string lk_username = lk.GetSection("Loggin").Value;
            string lk_password = lk.GetSection("Password").Value;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Host", "api.lk.student.tsu.ru");
            string basicAuth = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(lk_username + ":" + lk_password));
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", basicAuth);
            return client;
        }
    }
}
