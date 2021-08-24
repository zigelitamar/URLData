using System;
using System.Collections.Generic;
using URLdata.Models;
using MathNet.Numerics.Statistics;

namespace URLdata.Data
{
    public class DataHandler : IDataHandler
    {
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary { get; set; }

        public Dictionary<string, HashSet<string>>  userIdUniqueURLVisits { get; set; }

        public DataHandler()
        {
            // IReader reader = new CSVReader("/Users/itamarsigel/Desktop/Itamar's things");
            IReader reader = new CSVReader("/Users/aviv.amsellem/Downloads/");
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

        //todo: implement  hash set for medians calculated
        public long getMedian(string url)
        {
            if (urlSessionDictionary.ContainsKey(url))
            {
                urlSessionDictionary[url].Item3.Sort();
                Dictionary<String, Session> userIdSessions = urlSessionDictionary[url].Item1;
                return urlSessionDictionary[url].Item3[urlSessionDictionary[url].Item3.Count / 2];
            }

            return 0;
        }
    }
}