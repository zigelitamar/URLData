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
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> UrlSessionDictionary { get;
            set;
        }
        public Dictionary<string, HashSet<string>>  UserIdUniqueUrlVisits { get; set; }
        public void Parse();
        

    }
}