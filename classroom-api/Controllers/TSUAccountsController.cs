using Microsoft.AspNetCore.Mvc;

namespace classroom_api.Controllers
{
    [Route("tsu_accounts")]
    [ApiController]
    //на данный момент есть
    public class TSUAccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TSUAccountsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("loggin")]
        public async Task<IActionResult> Loggin()
        {
            var serviceId = _configuration.GetSection("TSUAccounts:ServiceId").Value;
            return Redirect("https://accounts.tsu.ru/Account/Login2/?applicationId=" + serviceId);
        }
        [HttpGet("save_token")]
        
        public async Task<ActionResult> SaveToken()
        {
            return Ok();
        }
    }
}
