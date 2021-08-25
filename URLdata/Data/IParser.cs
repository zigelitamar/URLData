using System;
using System.Collections.Generic;
using URLdata.Models;

namespace URLdata.Data
{
    public interface IParser
    {
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary { get;
            set;
        }
        public Dictionary<string, HashSet<string>>  userIdUniqueURLVisits { get; set; }
        public void Parse();
        

    }
}