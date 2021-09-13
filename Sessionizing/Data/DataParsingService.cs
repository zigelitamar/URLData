using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using URLdata.Exceptions;

namespace URLdata.Data
{
    public class DataParsingService : IHostedService
    {
        private readonly IParser _parser;

        public DataParsingService(IParser parser)
        {
            _parser = parser;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("on start action");
         
             try
             {
                 
                 await _parser.ParseBySessions();
                 Console.WriteLine("Done parsing");
             }
             catch (ParsingException e)
             {
                 Console.WriteLine(e);
             }
  
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}