using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using URLdata.Models;

namespace URLdata.Data
{
    /// <summary>
    /// Reader class for CSV files.
    /// Implements the IReader interface.
    /// </summary>
    public class CsvReader : IReader
    {
        private readonly string _directoryPath;
        private List<string> _csvFilesList = new List<string>();

        /// <summary>
        /// Constructor.
        /// receives the path from the app.config file to the directory
        /// where all the CSV files located.
        /// </summary>
        /// <param name="path">
        /// string argument - a path to the directory where the
        /// CSV files located in.
        /// </param>
        public CsvReader(string path)
        {
   
            _directoryPath = path;
        }
        
        /// <summary>
        /// IReader interface method.
        /// The method taking all the CSV files located in the given path and
        /// generates to each CSV file an iterator for later use.
        /// </summary>
        /// <returns>
        /// The method return a list of iterators for each of the CSV files.
        /// </returns>
        public List<IEnumerator<PageView>> ReadData()
        {
            //TODO: * * * check about IEnumerableAsync and change. * * *
            
            _csvFilesList = GetCsvFileNames();
            if (_csvFilesList == null)
            {
                throw new FileLoadException();
            }
            List<IEnumerator<PageView>> allPageViewsListsIterators = new List<IEnumerator<PageView>>();
            
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            
            foreach (var currentCsvFileName in this._csvFilesList)
            {
                var reader = new StreamReader(currentCsvFileName);
                var csv = new CsvHelper.CsvReader(reader, config);
                
                IEnumerable<PageView> pageViewsListIterator = csv.GetRecords<PageView>();
                allPageViewsListsIterators.Add(pageViewsListIterator.GetEnumerator());
            }

            return allPageViewsListsIterators;
        }

        /// <summary>
        /// The method scan the directory path and return all the
        /// CSV files name.
        /// </summary>
        /// <returns>
        /// The method returns a list of strings containing all the CSV files names
        /// in the given path.
        /// </returns>
        private List<string> GetCsvFileNames()
        {

            try
            {
                return Directory.GetFiles(_directoryPath, "*.csv").ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("Either reading files not Exist or you dont have permissions for the reading files");
                return null;
            }
        }
    }
}