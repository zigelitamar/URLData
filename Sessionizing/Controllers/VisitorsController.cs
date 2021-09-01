using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using URLdata.Data;
using URLdata.Models;

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

        [HttpGet()]
        public async Task<ActionResult<int>> UniqueSites([FromQuery]VisitorId visitor)
        {
            var uniqueSites = await _dataManager.GetUniqueSites($"visitor_{visitor.visitorId}");
            return Ok(uniqueSites);
            
        }
    }
}