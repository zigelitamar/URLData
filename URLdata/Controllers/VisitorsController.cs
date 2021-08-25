using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task<ActionResult<int>> uniqueSites(string id)
        {
            return Ok( await Task.Run(()=> _dataManager.getUniqueSites($"visitor_{id}")));
        }
        
    }
}