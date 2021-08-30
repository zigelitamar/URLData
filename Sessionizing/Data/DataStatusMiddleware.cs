using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace URLdata.Data
{
    /// <summary>
    ///
    /// 
    /// </summary>
    public class DataStatusMiddleware :IMiddleware
    {
        private readonly IParser _parser ;

        public DataStatusMiddleware(IParser parser)
        {
            _parser = parser;
        }
        public async  Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (_parser.UrlSessionDictionary == null)
            {
                // Console.WriteLine("bad parser / somthing went wrong with the file reading");
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync(" service is currentlly unavailable");
            }
            else
            {
                Console.WriteLine("good parser");
                await next(context);
                

            }
        }
    }
}