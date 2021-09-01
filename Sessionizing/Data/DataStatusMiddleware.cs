using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using URLdata.Exceptions;
using URLdata.Models;

namespace URLdata.Data
{
    /// <summary>
    ///
    /// 
    /// </summary>
    public class DataStatusMiddleware :IMiddleware
    {
        private readonly IParser _parser ;
        private readonly ILogger<DataStatusMiddleware> _log;

        public DataStatusMiddleware(IParser parser, ILogger<DataStatusMiddleware> logger)
        {
            _log = logger;
            _parser = parser;
        }
        public async  Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (KeyNotFoundException ke)
            {
                _log.LogError("Some one asked for a key that could not be found");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync(" invalid ID given");

            }
            catch (ParsingException pe)
            {
                _log.LogError("Our parsing is having issues");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("service is currently unavailable");
            }
            catch (Exception e)
            {
                var err = e.Message;
                _log.LogError($"Some unexpected error has occoured {err}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("service is currently unavailable");
            }
       
        }
    }
}