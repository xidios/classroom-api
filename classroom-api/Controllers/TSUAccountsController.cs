using Microsoft.AspNetCore.Mvc;

namespace classroom_api.Controllers
{
    [Route("tsu_accounts")]
    [ApiController]
    //на данный момент есть
    public class TSUAccountsController : ControllerBase
    {
        public TSUAccountsController()
        {

        }
        [HttpGet("save_token")]
        public async Task<ActionResult> SaveToken()
        {
            return Ok();
        }
    }
}
