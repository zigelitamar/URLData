using Microsoft.AspNetCore.Mvc;
using URLdata.Data;

namespace URLdata.Controllers
{
    [Route("sites")]
    [ApiController]
    public class SiteController : ControllerBase
    {
        private readonly IParser _dataManager;

        public SiteController(IParser dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet]
        public ActionResult<string> getbla()
        {
            return Ok("hello");
        }
        
    }
}