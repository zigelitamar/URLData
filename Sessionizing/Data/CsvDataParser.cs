using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using URLdata.Models;

namespace URLdata.Data
{
    /// <summary>
    /// Parser class for CSV files.
    /// Implements the IParser interface.
    /// </summary>
    public class CsvDataParser : IParser
    {
        private readonly int _maxSessionTimeInterval = int.Parse(ConfigurationManager.AppSettings["session time in seconds"] ?? string.Empty);
        
        private readonly IReader _reader;

        public Dictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long> allUrlSessionsList)> urlSessionDictionary { get; set; }
        public Dictionary<string, HashSet<string>>  userIdUniqueUrlVisits  { get; set; }
        
        /// <summary>
        /// Constructor.
        /// Receives an IReader implementation object.
        /// </summary>
        /// <param name="reader"></param>
        public CsvDataParser(IReader reader)
        {
            _reader = reader;
            Console.WriteLine("CsvDataParser object created.");
        }
      
        /// <summary>
        /// IParser interface method.
        /// The method iterate chronologically (by time stamps) on all iterators
        /// and generates data structures to retrieve API requests as requested. 
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public async Task ParseBySessions()
        {
            // get iterators list
            var csvFilesIterators  = _reader.ReadData();

            // initial the data structures to save the necessary data from the csv files.
            Dictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long> allUrlSessionsList)> urlSessionsDictionary = new(); 
            var  userIdsUniqueUrlVisits = new Dictionary<string, HashSet<string>>();
            
            // iterate on chronologically (by time stamps) on all iterators together.
            while (csvFilesIterators.Count != 0)
            {
                //get index of the minimal timestamp between all iterators anf its record.
                var currentMinIterator = GetMinTimeStampIndex(csvFilesIterators);
                var currentMinPageView = currentMinIterator.Current;

                var visitorId = currentMinPageView.visitor;

                AddUrlToVisitor(visitorId, userIdsUniqueUrlVisits, currentMinPageView);

                //check if the current url exist in the dictionary
                if (urlSessionsDictionary.TryGetValue(key: currentMinPageView.mainUrl, value: out var currentUrlValue))
                {
                    //check if the current visitor already have a session for the current url
                    if (currentUrlValue.userSessions.TryGetValue(key: currentMinPageView.visitor, value: out var currentVisitorSession))
                    {
                        UpdateVisitorSessions(urlSessionsDictionary, currentMinPageView, currentUrlValue, currentVisitorSession);
                    }
                    else // the current visitor page view is its first appearance
                    {
                        InitialFirstVisitorSession(urlSessionsDictionary, currentUrlValue, currentMinPageView);
                    }
                }
                else //url not exist yet in the data structure -> set first session timestamp, set sessions counter to 1, initial new list for sessions lengths
                {
                    var visitorSessionDictionary = new Dictionary<string, Session>
                    {
                        [currentMinPageView.visitor] = new (currentMinPageView.timestamp)
                    };
                    
                    urlSessionsDictionary[currentMinPageView.mainUrl] = (visitorSessionDictionary, 1, new List<long>());
                }

                // promote the chosen iterator with the minimal time stamp. if the chosen iterator finished iterating all records - remove it from list.
                if (!await currentMinIterator.MoveNextAsync())
                {
                    csvFilesIterators.Remove(currentMinIterator);
                }
            }

            this.urlSessionDictionary = urlSessionsDictionary;
            this.userIdUniqueUrlVisits = userIdsUniqueUrlVisits;
            Console.WriteLine("DONE PROCESSING");
        }

        /// <summary>
        ///  The method return the index from the iterator list with
        ///  the minimal Time Stamp value.
        /// </summary>
        /// <param name="csvFilesIterators">
        ///  list of IEnumerator containing all the iterators.
        /// </param>
        /// <returns>
        /// int - the index with the minimal Time Stamp value.
        /// </returns>
        private static IAsyncEnumerator<PageView> GetMinTimeStampIndex(IEnumerable<IAsyncEnumerator<PageView>> csvFilesIterators)
        {
            return csvFilesIterators
                .OrderBy(pv => pv.Current.timestamp)
                .FirstOrDefault();
          
        }


        /// <summary>
        /// The method initial the the first session of a visitor entring a new
        /// website url.
        /// </summary>
        /// <param name="urlSessionDictionary">
        /// The data structure of the urls sessions.
        /// </param>
        /// <param name="currentUrlValue">
        /// The Tuple value of the url key.
        /// </param>
        /// <param name="currentPageView">
        /// The current PageView record with the minimal time stamp.
        /// </param>
        private static void InitialFirstVisitorSession(IDictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long> allUrlSessionsList)> urlSessionDictionary,
            (Dictionary<string, Session> userSessions, int sessionsCounter, List<long> allUrlSessionsList) currentUrlValue, PageView currentPageView)
        {
            // set to the visitor a new session
            currentUrlValue.userSessions[currentPageView.visitor] = new Session(currentPageView.timestamp);
            
            // get  the session counter and increment it
            int sessionCounter = currentUrlValue.sessionsCounter + 1;
            
            urlSessionDictionary[currentPageView.mainUrl] = (currentUrlValue.userSessions, sessionCounter, currentUrlValue.allUrlSessionsList);
        }

        /// <summary>
        /// The method checks if if the current page view of a visitor
        /// belong to his last recorded session.
        /// If it does -  update the current session, else - add the last session length to the sessions list
        /// and create a new session.
        /// </summary>
        /// <param name="urlSessionsDictionary">
        /// The data structure of the urls sessions.
        /// </param>
        /// <param name="currentPageView">
        /// The current PageView record with the minimal time stamp.
        /// </param>
        /// <param name="currentUrlValue"></param>
        /// <param name="currentVisitorSession"></param>
        private void UpdateVisitorSessions(IDictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long> allUrlSessionsList)> urlSessionsDictionary, PageView currentPageView, (Dictionary<string, Session> userSessions, int sessionsCounter,
            List<long> allUrlSessionsList) currentUrlValue, Session currentVisitorSession)
        {
            
            if (currentPageView.timestamp - currentVisitorSession.endTime <= _maxSessionTimeInterval)
            {
                currentVisitorSession.endTime = currentPageView.timestamp;
            }
            else
            {
                // add the session time to the sessions length list
                currentUrlValue.allUrlSessionsList.Add(currentVisitorSession.endTime - currentVisitorSession.startTime);
                
                //reset the visitor session by the current page view.
                currentVisitorSession.ResetSession(currentPageView.timestamp);
                
                // increment the counter of the sessions amount
                int sessionCounter = currentUrlValue.sessionsCounter + 1;
                
                urlSessionsDictionary[currentPageView.mainUrl] = (currentUrlValue.userSessions, sessionCounter, currentUrlValue.allUrlSessionsList);
            }
        }
        
        
        /// <summary>
        /// The method add the given url to the visitor's unique url websites he visited.
        /// </summary>
        /// <param name="visitorId">
        /// The visitor ID.
        /// </param>
        /// <param name="userIdsUniqueUrlVisits">
        /// The data structure of the visitor unique url website visits.
        /// </param>
        /// <param name="currentPageView">
        /// The current record with the minimal time stamp between all iterators.
        /// </param>
        private void AddUrlToVisitor(string visitorId, Dictionary<string, HashSet<string>> userIdsUniqueUrlVisits, PageView currentPageView)
        {
            if (userIdsUniqueUrlVisits.TryGetValue(key: visitorId, value: out var userUniqueUrlsVisit))
            {
                userUniqueUrlsVisit.Add(currentPageView.mainUrl);
            }
            else
            {
                userIdsUniqueUrlVisits[visitorId] = new HashSet<string> {currentPageView.mainUrl};
            }
        }
    }
}