using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace URLdata.Filters
{
    public class NotValidKeyFilter :Attribute, IExceptionFilter
    {



        public void OnException(ExceptionContext context)
        {
            if (context.Exception is KeyNotFoundException)
            {
                Console.WriteLine("here is a typical noo key exanple");
                context.HttpContext.Response.StatusCode = 400;
                context.HttpContext.Response.WriteAsync("ID is not valid in the system");
            
            }
            else
            {
                Console.WriteLine("here is just a regular expetion");
            }
        }
    }
}