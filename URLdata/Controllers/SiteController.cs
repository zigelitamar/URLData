using System;
using System.Collections.Generic;
using System.Net;
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
        public  ActionResult<int> getSessionAmount(string url)
        {
            int sessionsAmount = 0;
            try
            {
                sessionsAmount =  _dataManager.getSessionsAmount(url);
                return Ok(sessionsAmount);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e);
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