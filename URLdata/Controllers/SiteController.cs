using Microsoft.AspNetCore.Mvc;
using URLdata.Data;

namespace URLdata.Controllers
{
    [Route("urls")]
    [ApiController]
    public class SiteController : ControllerBase
    {
        private readonly IDataHandler _dataManager;

        public SiteController(IDataHandler dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet]
        public ActionResult<string> test()
        {
            return Ok("hi");
        }

        [HttpGet("sessions_amount/{url}")]
        public ActionResult<int> getSessionAmount(string url)
        {
            int sessionsAmount = _dataManager.getSessionsAmount(url);
            if (sessionsAmount != -1)
            {
                return Ok(sessionsAmount);
            }
            else
            {
                return NoContent();
            }
            return Ok(_dataManager.getSessionsAmount(url));
        }

    }
}