using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using URLdata.Data;
using URLdata.Models;

namespace URLdata.Controllers
{
    [Route("urls")]
    [ApiController]
    [BindProperties(SupportsGet = true)]
    public class SiteController : ControllerBase
    {
        private readonly IDataHandler _dataManager;

        public SiteController(IDataHandler dataManager)
        {
            _dataManager = dataManager;
        }
         

        [HttpGet("sessions_amount/")]
        public async Task<ActionResult<int>> GetSessionAmount([FromQuery]Url url)
        {
            
            var sessionsAmount = await _dataManager.GetSessionsAmount(url.address);
            return Ok(sessionsAmount);
        }

        [HttpGet("median/")]
        public async Task<ActionResult<double>> GetSessionsMedian([FromQuery]Url url)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var median = await _dataManager.GetMedian(url.address);
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            return Ok(median);
        }


    }
}