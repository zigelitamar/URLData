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
            try
            {
                return Ok(await _dataManager.GetUniqueSites($"visitor_{id}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NoContent();
            }
           
        }
        
    }
}