using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using URLdata.Models;
using MathNet.Numerics.Statistics;

namespace URLdata.Data
{
    public class DataHandler : IDataHandler
    {
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary { get; set; }

        public Dictionary<string, HashSet<string>>  userIdUniqueURLVisits { get; set; }

        public Dictionary<string, double > mediansCalculated { get; set; }



        public DataHandler()
        {
            mediansCalculated = new Dictionary<string, double>();
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"Resources/");
            IReader reader = new CSVReader(path);
            
            CsvDataParser parser = new CsvDataParser();
            parser.Parse(reader.ReadData());
            this.urlSessionDictionary = parser.urlSessionDictionary;
            this.userIdUniqueURLVisits = parser.userIdUniqueURLVisits;
            Console.WriteLine("DONE PROCESSING");
        }

        public int getsize()
        {
            return urlSessionDictionary.Count;
        }

        public int getSessionsAmount(string url)
        {
            if (urlSessionDictionary.ContainsKey(url))
            {
                return urlSessionDictionary[url].Item2;
            }
            return -1;
            
        }

        public int getUniqueSites(string visitorID)
        {
            if (userIdUniqueURLVisits.ContainsKey(visitorID))
            {
                return userIdUniqueURLVisits[visitorID].Count;
            }

            return -1;
        }

      
        public double getMedian(string url)
        {
            if (!urlSessionDictionary.ContainsKey(url))
            {
                throw new KeyNotFoundException("Could not find record");
            }
            if (mediansCalculated.ContainsKey(url))
            {
                return mediansCalculated[url];
            }
            //calculating median of sessions length and update the value in the medians map
            var lengths = urlSessionDictionary[url].Item3;
            lengths.Sort();
            int lengthsSize = lengths.Count;
            int midElement = lengthsSize / 2;
            
            double median = (lengthsSize % 2 != 0) ? lengths[midElement] : ((double)lengths[midElement] + lengths[midElement - 1]) / 2;
            mediansCalculated[url] = median;
            return median;



        }
    }
}