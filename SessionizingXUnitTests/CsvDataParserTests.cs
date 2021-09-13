using System;
using System.IO;
using URLdata.Data;
using Xunit;

namespace SessionizingXUnitTests
{
    public class CsvDataParserTests
    {
        private readonly IReader _reader;
        public CsvDataParserTests()
        {
            // real path
            var path = Path.Combine(Directory.GetCurrentDirectory(), "TestFolder/");

            // creating a CSVReader
            _reader = new CSVReader(path);
        }

        /// <summary>
        /// This test check if the class handle correctly a null argument.
        /// </summary>
        [Fact]
        public  void TestReadDataNullArgument()
        {
            var csvDataParser = new CsvDataParser(null);
            var caughtException = Assert.ThrowsAsync<NullReferenceException>( ()=> csvDataParser.ParseBySessions());
            Assert.Equal("IReader object is null", caughtException.Result.Message);
        }

        [Fact]
        public void TestSessionCreationConflicts()
        {
            
        }

    }
}