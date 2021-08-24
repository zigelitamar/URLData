using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using URLdata.Models;

namespace URLdata.Data
{
    public class CSVReader : IReader
    {
        private string directoryPath;
        private List<string> CSVfilesList = new List<string>();
        private int totalRecords { get; set; }

        public CSVReader(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(
                    $"Directory path: {path} is invalid.\n Please insert an existing directory path.");
            }
            this.directoryPath = path;
        }
        
        public List<IEnumerator<PageView>> ReadData()
        {
            this.totalRecords = 0;
            CSVfilesList = this.getCSVFileNames();
            List<IEnumerator<PageView>> allPageViewsListsIterators = new List<IEnumerator<PageView>>();
            
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            
            foreach (var currentCSVFileName in this.CSVfilesList)
            {
                var reader = new StreamReader(currentCSVFileName);
                var csv = new CsvReader(reader, config);
                
                IEnumerable<PageView> pageViewsListIterator = csv.GetRecords<PageView>();
                allPageViewsListsIterators.Add(pageViewsListIterator.GetEnumerator());
            }

            return allPageViewsListsIterators;
        }

        private List<string> getCSVFileNames()
        {
            return Directory.GetFiles(directoryPath, "*.csv").ToList();
        }
    }
}