using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

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
             _parser.Parse();
             await Task.Delay(20000);
             Console.WriteLine("Done parsing");
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}