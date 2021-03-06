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
    public class CSVReader : IReader
    {
        private readonly string _directoryPath;
        private List<string> _csvFilesList = new();

        /// <summary>
        /// Constructor.
        /// receives the path from the app.config file to the directory
        /// where all the CSV files located.
        /// </summary>
        /// <param name="path">
        /// string argument - a path to the directory where the
        /// CSV files located in.
        /// </param>
        public CSVReader(string path)
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
        public List<IAsyncEnumerator<PageView>> ReadData()
        {
            if (_directoryPath is null || !Directory.Exists(_directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory path: {_directoryPath} is invalid.\n Please insert an existing directory path.");
            }
            
            // get all csv files in the given directory path
            GetCsvFileNames();
            
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

             var listOfAsyncEnumerators = _csvFilesList
                 .Select(async(file) =>
                {
                    var reader = new StreamReader(file);
                    var csv = new CsvReader(reader, config);
                    var pageViewsListIterator = csv.GetRecordsAsync<PageView>().GetAsyncEnumerator();
                    await pageViewsListIterator.MoveNextAsync();
                    
                    return pageViewsListIterator;
                }
            )
                 .Where( iter => iter.Result.Current is not null)
                 .Select(iterTask => iterTask.Result)
                 .ToList();
             

             if(listOfAsyncEnumerators is null || listOfAsyncEnumerators.Count == 0)
            {
                throw new NullReferenceException($"iterators list is null or empty.");
            }
            
            return listOfAsyncEnumerators;
        }

        /// <summary>
        /// The method scan the directory path and return all the
        /// CSV files name.
        /// </summary>
        /// <returns>
        /// The method returns a list of strings containing all the CSV files names
        /// in the given path.
        /// </returns>
        private void GetCsvFileNames()
        {

            _csvFilesList = Directory.GetFiles(_directoryPath, "*.csv").ToList();
            if (_csvFilesList.Count == 0)
            {
                throw new FileNotFoundException(
                    $"There are no CSV files in the given directory path: {_directoryPath}");
            }
        }

       
    }
}