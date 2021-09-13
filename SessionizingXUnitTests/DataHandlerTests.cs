using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using URLdata.Data;
using Xunit;

namespace SessionizingXUnitTests
{
    public class DataHandlerTests : IAsyncLifetime
    {
        private IDataHandler _dataHandler;


        /// <summary>
        /// The method checks the getSessionsAmount method
        /// </summary>
        /// <param name="url">
        /// Url string to test the method.
        /// </param>
        [Theory]
        [InlineData("www.not_real_url.com")]
        [InlineData("www.s_1.com")]
        public async Task GetSessionsAmount(string url)
        {
            try
            {
                int sessionsAmount = await _dataHandler.GetSessionsAmount(url);
                Assert.Equal(3684, sessionsAmount);
            }
            catch (KeyNotFoundException exception)
            {
                Assert.Equal("No records found.", exception.Message);
            }
        }

        /// <summary>
        /// The method test the GetMedian method.
        /// </summary>
        /// <param name="url">
        /// Url string to test the method.
        /// </param>
        [Theory]
        [InlineData("www.not_real_url.com")]
        [InlineData("www.s_5.com")]
        public async Task GetMedianTest(string url)
        {
            try
            {
                var sessionsAmount = await _dataHandler.GetMedian(url);
                Assert.Equal(1383.5, sessionsAmount);
            }
            catch (KeyNotFoundException exception)
            {
                Assert.Equal("No records found.", exception.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">
        /// visitor ID to test the method.
        /// </param>
        [Theory]
        [InlineData("not_real_ID")]
        [InlineData("visitor_4541")]
        public async Task GetUniqueSitesTest(string userId)
        {
            try
            {
                var sessionsAmount = await _dataHandler.GetUniqueSites(userId);
                Assert.Equal(4, sessionsAmount);
            }
            catch (KeyNotFoundException exception)
            {
                Assert.Equal("No records found.", exception.Message);
            }
        }

        public async Task InitializeAsync()
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"TestFolder");
            IReader reader = new CSVReader(directoryPath);
            
            IParser parser = new CsvDataParser(reader);
            await parser.ParseBySessions();
            _dataHandler = new DataHandler(parser);
        }

        public async Task DisposeAsync()
        {

        }
    }
}