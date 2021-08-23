using Microsoft.AspNetCore.Mvc;
using URLdata.Data;

namespace URLdata.Controllers
{
    [Route("visitors")]
    [ApiController]
    public class VisitorsController : ControllerBase
    {
        private readonly IDataHandler _dataManager;

        public VisitorsController(IDataHandler dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("{id}")]
        public ActionResult<int> uniqueSites(string id)
        {
            return Ok(_dataManager.getUniqueSites($"visitor_{id}"));
        }
        
    }
}