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
            allUrlSessionsList)> urlSessionDictionary { get;
            set;
        }
        public Dictionary<string, HashSet<string>>  userIdUniqueUrlVisits { get; set; }
        public Task ParseBySessions();
   
        

    }
}