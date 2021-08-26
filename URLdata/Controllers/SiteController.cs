using System;
using System.Collections.Generic;
using System.Net;
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
        public  async Task<ActionResult<int>> getSessionAmount(string url)
        {
            int sessionsAmount = 0;
            try
            {
                sessionsAmount = await Task.Run(()=>  _dataManager.GetSessionsAmount(url));
                return Ok(sessionsAmount);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e);
                return NoContent();
            }
            
        }

        [HttpGet("median/{url}")]
        public async Task<ActionResult<double>> getSessionsMedian(string url)
        {
            
            var watch = new System.Diagnostics.Stopwatch();
            
            watch.Start();
            double median = 0;
            try
            {
                median = await Task.Run(()=>_dataManager.GetMedian(url));
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