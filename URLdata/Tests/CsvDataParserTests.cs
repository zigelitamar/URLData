using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using URLdata.Data;
using URLdata.Models;
using Xunit;
using Xunit.Sdk;

namespace URLdata.Tests
{
    [Collection("Sequential")]
    public class CsvDataParserTests
    {
        public readonly CsvDataParser _sutParser;

        public CsvDataParserTests()
        {
            // real path
            //string path = Path.Combine(Directory.GetCurrentDirectory(), @"Resources/");
            var path = "";
            try
            {
                path = Path.Combine(Directory.GetCurrentDirectory(),
                    $"{ConfigurationManager.AppSettings["resource directory"]}");
            }
            catch (Exception e)
            {
                throw new FileNotFoundException("repository dose not exist!");
            }
            
            // creating a CSVReader
            CSVReader csvReader = new CSVReader(path);
            _sutParser = new CsvDataParser(csvReader);
        }
        
        /// <summary>
        /// This test check if the method handle correctly a null argument.
        /// </summary>
        [Fact]
        public void TestReadDataNullArgument()
        {
          
            var caughtException = Assert.Throws<NullReferenceException>(() => _sutParser.Parse());
            Assert.Equal(caughtException.Message, $"iterators list is null or empty.");
        }

        /// <summary>
        /// This test check if the method handle correctly an empty list argument.
        /// </summary>
        [Fact]
        public void TestReadDataEmptyListArgument()
        {
            List<IEnumerator<PageView>> allPageViewsListsIterators = new List<IEnumerator<PageView>>();
            var caughtException = Assert.Throws<NullReferenceException>(() => _sutParser.Parse());
            Assert.Equal(caughtException.Message, $"iterators list is null or empty.");
        }
        
        
    }
}