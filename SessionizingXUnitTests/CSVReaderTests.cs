using System;
using System.Configuration;
using System.IO;
using URLdata.Data;
using Xunit;

namespace SessionzingXUnitTests
{
    public class ReaderTests
    {
        private readonly CsvReader _sutCsvReader;

        public ReaderTests()
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"TestFolder");
            
            this._sutCsvReader = new CsvReader(directoryPath);
        }

        /// <summary>
        /// This test check if the class raise DirectoryNotFoundException exception when a given path is not real
        /// given by the user.
        /// <example>
        /// path = "users/this_path_not_really_exists"
        /// </example>
        /// </summary>
        [Theory]
        [InlineData("not_a_real_directory/")]
        [InlineData(null)]
        public void TestPath2(string path)
        {
            var caughtException = Assert.Throws<DirectoryNotFoundException>(() => new CsvReader(path).ReadData());
            Assert.Equal(caughtException.Message,
                $"Directory path: {path} is invalid.\n Please insert an existing directory path.");
            
        }

        /// <summary>
        /// This test check if the class raise FileNotFoundException when no CSV are in the given path  
        /// </summary>
        [Fact]
        public void TestNoCsvFilesInPath()
        {
            string noCsvFilesDirectoryPath = Directory.GetCurrentDirectory();
            var caughtException = Assert.Throws<FileNotFoundException>(() => new CsvReader(noCsvFilesDirectoryPath).ReadData());
            Assert.Equal(caughtException.Message,
                $"There are no CSV files in the given directory path: {noCsvFilesDirectoryPath}");
        }
        
        /// <summary>
        /// This test check if the class raise 
        /// <example>
        /// path = "Resources folder inside the project."
        /// </example>
        /// </summary>
        [Fact]
        public void TestUnauthorizedFiles()
        {
            string unauthorizedFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestFolder/UnauthorizedCSVFiles");
            var caughtException = Assert.Throws<UnauthorizedAccessException>(() => new CsvReader(unauthorizedFileDirectory).ReadData());
            Assert.Equal(caughtException.Message,
                $"Cannot access the file: input_1.csv.");
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
            var iteratorsList = _sutCsvReader.ReadData();
            Assert.Equal(3, iteratorsList.Count);
        }
    }
}