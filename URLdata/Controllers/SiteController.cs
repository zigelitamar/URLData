using System;
using System.Collections.Generic;
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
        }

        [HttpGet("median/{url}")]
        public ActionResult<double> getSessionsMedian(string url)
        {
            
            var watch = new System.Diagnostics.Stopwatch();
            
            watch.Start();
            double median = 0;
            try
            {
                median = _dataManager.getMedian(url);
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