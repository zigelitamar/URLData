using System;
using System.Collections.Generic;
using System.IO;
using URLdata.Data;
using URLdata.Models;
using Xunit;

namespace SessionzingXUnitTests
{
    public class CsvDataParserTests
    {
        public readonly CsvDataParser _sutParser;
        
        
        public CsvDataParserTests()
        {
            // real path
            string path = Path.Combine(Directory.GetCurrentDirectory(), "TestFolder/");

            // creating a CSVReader
            CsvReader csvReader = new CsvReader(path);
        }

        /// <summary>
        /// This test check if the class handle correctly a null argument.
        /// </summary>
        [Fact]
        public void TestReadDataNullArgument()
        {
            IReader reader = null;
            
            var caughtException = Assert.Throws<NullReferenceException>(() => new CsvDataParser(reader).Parse());
            Assert.Equal("IReader object is null", caughtException.Message);
        }

    }
}