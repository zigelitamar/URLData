using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using URLdata.Data;
using URLdata.Models;
using Xunit;

namespace SessionizingXUnitTests
{
    public class CsvDataParserTests
    {
        public CsvDataParserTests()
        {
            // real path
            string path = Path.Combine(Directory.GetCurrentDirectory(), "TestFolder/");

            // creating a CSVReader
            IReader csvReader = new CSVReader(path);
        }

        /// <summary>
        /// This test check if the class handle correctly a null argument.
        /// </summary>
        [Fact]
        public  void TestReadDataNullArgument()
        {
            var csvDataParser = new CsvDataParser(null);
            var caughtException = Assert.ThrowsAsync<NullReferenceException>( ()=> csvDataParser.Parse());
            Assert.Equal("IReader object is null", caughtException.Result.Message);
        }

    }
}