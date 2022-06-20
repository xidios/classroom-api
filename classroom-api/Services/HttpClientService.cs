using System.Net.Http.Headers;
using System.Text;

namespace classroom_api.Services
{
    public interface IHttpClientService
    {
        public HttpClient InitLKStudentHttpClient();
        public HttpClient InitPersonaTSUHttpClient();
        public HttpClient InitTSUAccountsHttpClient();

    }
    public class HttpClientService : IHttpClientService
    {
        private readonly IConfiguration _configuration;
        public HttpClientService(IConfiguration configuration)
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

        public HttpClient InitPersonaTSUHttpClient()
        {
            HttpClient client = new HttpClient();
            var lk = _configuration.GetSection("PersonaTSU");
            string lk_username = lk.GetSection("Loggin").Value;
            string lk_password = lk.GetSection("Password").Value;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Host", "persona.tsu.ru");
            string basicAuth = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(lk_username + ":" + lk_password));
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", basicAuth);
            return client;
        }
        public HttpClient InitTSUAccountsHttpClient()
        {
            HttpClient TSUAccounts = new HttpClient();
            string basicAuth = _configuration.GetSection("TSUAccounts:Basic").Value;
            TSUAccounts.DefaultRequestHeaders.Accept.Clear();
            TSUAccounts.DefaultRequestHeaders.Add("Host", "accounts.tsu.ru");
            TSUAccounts.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            TSUAccounts.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "YWNjb3VudHM6RXdjemN2MEE/cTkjbzZI");
            return TSUAccounts;
        }
    }
}
