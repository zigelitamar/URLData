using System;
using System.Collections;
using System.Collections.Generic;
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
            string path = "/Users/aviv.amsellem/RiderProjects/URLData/URLdata/Resources";
            
            // creating a CSVReader
            CSVReader csvReader = new CSVReader(path);
            List<IEnumerator<PageView>> pageViewIteratorsList = csvReader.ReadData();
            
            this._sutParser = new CsvDataParser();
        }
        
        /// <summary>
        /// This test check if the method handle correctly a null argument.
        /// </summary>
        [Fact]
        public void TestReadDataNullArgument()
        {
          
            var caughtException = Assert.Throws<NullReferenceException>(() => this._sutParser.Parse(null));
            Assert.Equal(caughtException.Message, $"iterators list is null or empty.");
        }

        /// <summary>
        /// This test check if the method handle correctly an empty list argument.
        /// </summary>
        [Fact]
        public void TestReadDataEmptyListArgument()
        {
            List<IEnumerator<PageView>> allPageViewsListsIterators = new List<IEnumerator<PageView>>();
            var caughtException = Assert.Throws<NullReferenceException>(() => this._sutParser.Parse(allPageViewsListsIterators));
            Assert.Equal(caughtException.Message, $"iterators list is null or empty.");
        }
        
        
    }
}