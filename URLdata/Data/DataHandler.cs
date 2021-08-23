using System;
using System.Collections.Generic;
using URLdata.Models;

namespace URLdata.Data
{
    public class DataHandler : IDataHandler
    {
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary { get; set; }

        public Dictionary<string, HashSet<string>>  userIdUniqueURLVisits { get; set; }

        public DataHandler(Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary, Dictionary<string, HashSet<string>> userIdUniqueURLVisits)
        {
            this.urlSessionDictionary = urlSessionDictionary;
            this.userIdUniqueURLVisits = userIdUniqueURLVisits;
        }
    }
}