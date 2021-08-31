using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [HttpGet("sessions_amount/{url}")]
        public async Task<ActionResult<int>> GetSessionAmount(string url)
        {
            try
            {
                //TODO: remove the task.run and change the IDataHandler methods to async.
                var sessionsAmount = await _dataManager.GetSessionsAmount(url);
                return Ok(sessionsAmount);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e);
                return NoContent();
            }
            
        }

        [HttpGet("median/{url}")]
        public async Task<ActionResult<double>> GetSessionsMedian(string url)
        {
            
            var watch = new System.Diagnostics.Stopwatch();
            
            watch.Start();
            double median = 0;
            try
            {
                median = await _dataManager.GetMedian(url);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return NoContent();
            }

            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            return Ok(median);
        }



    }
}