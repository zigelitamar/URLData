using System;
using System.Collections.Generic;
using URLdata.Models;

namespace URLdata.Data
{
    /// <summary>
    /// Interface class to parse from different data sources.
    /// </summary>
    public interface IParser
    {
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary { get;
            set;
        }
        public Dictionary<string, HashSet<string>>  userIdUniqueUrlVisits { get; set; }
        public void Parse();
        

    }
}