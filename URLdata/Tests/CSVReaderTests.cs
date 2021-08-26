using System;
using System.Configuration;
using System.IO;
using URLdata.Data;
using URLdata.Models;
using Xunit;

namespace URLdata.Tests
{
    public class ReaderTests
    {
        private readonly CSVReader _sutCsvReader;

        public ReaderTests()
        {
            _sutCsvReader = new CSVReader("/Users/itamarsigel/RiderProjects/URLdata/URLdata/Resources");
        }

        /// <summary>
        /// This test check if the constructor can deal properly with not existed path
        /// given by the user.
        /// <example>
        /// path = "users/this_path_not_really_exists"
        /// </example>
        /// </summary>
        [Fact]
        public void TestBadPath()
        {
            string notRealPath = "123";
            Assert.Throws<FileLoadException>(() => new CSVReader(notRealPath).ReadData());
        }

        /// <summary>
        /// This test check the read data return the exact amount of files in the resources file.
        /// <example>
        /// path = "Resources folder inside the project."
        /// </example>
        /// </summary>
        [Fact]
        public void TestReadData()
        {
            Assert.Equal(3, _sutCsvReader.ReadData().Count);
        }

        [Fact]
        public void TestFileReadingType()
        {
            var type = _sutCsvReader.ReadData()[0];
            while (type.MoveNext())
            {
                var type2 = type.Current;
                Assert.IsType<PageView>(type2);
            }
        }
    }
}