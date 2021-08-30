using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using URLdata.Models;

namespace URLdata.Data
{
    /// <summary>
    /// Interface class to parse from different data sources.
    /// </summary>
    public interface IParser
    {
        public Dictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long>
            allUrlSessionsList)> UrlSessionDictionary { get;
            set;
        }
        public Dictionary<string, HashSet<string>>  UserIdUniqueUrlVisits { get; set; }
        public Task Parse();
   
        

    }
}