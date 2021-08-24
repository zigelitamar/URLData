using System;
using System.IO;
using URLdata.Data;
using Xunit;

namespace URLdata.Tests
{
    public class ReaderTests
    {
        private readonly CSVReader _sutCsvReader;

        public ReaderTests()
        {
            this._sutCsvReader = new CSVReader("/Users/aviv.amsellem/RiderProjects/URLData/URLdata/Resources");
        }

        /// <summary>
        /// This test check if the constructor can deal properly with not existed path
        /// given by the user.
        /// <example>
        /// path = "users/this_path_not_really_exists"
        /// </example>
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            string notRealPath = Path.Combine(Directory.GetCurrentDirectory(), @"not_a_real_directory/");
            var caughtException = Assert.Throws<DirectoryNotFoundException>(() => new CSVReader(notRealPath));
            Assert.Equal(caughtException.Message, $"Directory path: {notRealPath} is invalid.\n Please insert an existing directory path.");
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
            Assert.Equal(3, this._sutCsvReader.ReadData().Count);
        }
    }
}