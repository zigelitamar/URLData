using System;
using System.Threading.Tasks;
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
        public async Task<ActionResult<int>> UniqueSites(string id)
        {
            var uniqueSites = await _dataManager.GetUniqueSites($"visitor_{id}");
            return Ok(uniqueSites);
            
        }
    }
}