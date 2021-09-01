using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using URLdata.Exceptions;
using URLdata.Models;

namespace URLdata.Data
{
    /// <summary>
    /// Parser class for CSV files.
    /// Implements the IParser interface.
    /// </summary>
    public class CsvDataParser : IParser
    {
        private readonly IReader _reader;

        public Dictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long> allUrlSessionsList)> UrlSessionDictionary { get; set; }
        public Dictionary<string, HashSet<string>>  UserIdUniqueUrlVisits  { get; set; }
        
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
        public async Task Parse()
        {
            if (_reader == null)
            {
                throw new NullReferenceException("IReader object is null");
            }
            
            // get iterators list
            List<IAsyncEnumerator<PageView>> csvFilesIterators = null;
            try
            {
                 csvFilesIterators  = _reader.ReadData();
            }
            catch (FileLoadException e)
            {
                Console.WriteLine(e);
                throw new ParsingException();
            }
            
            // checks if the list is not empty (no csv files)
            if(csvFilesIterators == null || csvFilesIterators.Count == 0)
            {
                throw new NullReferenceException($"iterators list is null or empty.");
            }
            
            
            // set iterators to point for the first record (if the csv file is empty/no records in the csv file - remove the iterator)
            await InitialIterators(csvFilesIterators);

            // initial the data structures to save the necessary data from the csv files.
            Dictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long> allUrlSessionsList)> urlSessionDictionary = new Dictionary<string,(Dictionary<string, Session>, int, List<long>)>(); 
            Dictionary<string, HashSet<string>>  userIdUniqueUrlVisits = new Dictionary<string, HashSet<string>>();
            
            // iterate on chronologically (by time stamps) on all iterators together.
            while (csvFilesIterators.Count != 0)
            {
                //get index of the minimal timestamp between all iterators anf its record.
                var indexOfMinTimeStamp = GetMinTimeStampIndex(csvFilesIterators);
                
                var currentPageView = csvFilesIterators[indexOfMinTimeStamp].Current;
                
                string visitorId = currentPageView.visitor;

                AddUrlToVisitor(visitorId, userIdUniqueUrlVisits, currentPageView);

                //check if the current url exist in the dictionary
                if (urlSessionDictionary.TryGetValue(key: currentPageView.mainUrl, value: out var currentUrlValue))
                {
                    //check if the current visitor already have a session for the current url
                    if (currentUrlValue.userSessions.TryGetValue(key: currentPageView.visitor, value: out var currentVisitorSession))
                    {
                        UpdateVisitorSessions(urlSessionDictionary, currentPageView, currentUrlValue, currentVisitorSession);
                    }
                    else // the current visitor page view is its first appearance
                    {
                        InitialFirstVisitorSession(urlSessionDictionary, currentUrlValue, currentPageView);
                    }
                }
                else //url not exist yet in the data structure -> set first session timestamp, set sessions counter to 1, initial new list for sessions lengths
                {
                    Dictionary<string, Session> visitorSessionDictionary = new Dictionary<string, Session>
                    {
                        [currentPageView.visitor] = new Session(currentPageView.timestamp)
                    };
                    
                    urlSessionDictionary[currentPageView.mainUrl] = (visitorSessionDictionary, 1, new List<long>());
                }

                // promote the chosen iterator with the minimal time stamp. if the chosen iterator finished iterating all records - remove it from list.
                if (!await csvFilesIterators[indexOfMinTimeStamp].MoveNextAsync())
                {
                    csvFilesIterators.RemoveAt(indexOfMinTimeStamp);
                }
            }

            UrlSessionDictionary = urlSessionDictionary;
            UserIdUniqueUrlVisits = userIdUniqueUrlVisits;
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
        private static int GetMinTimeStampIndex(IReadOnlyList<IAsyncEnumerator<PageView>> csvFilesIterators)
        {
            var minTimeStamp = csvFilesIterators[0].Current.timestamp;
            var indexOfMinTimeStamp = 0;
            for(var i = 0; i < csvFilesIterators.Count; i++)
            {
                if (csvFilesIterators[i].Current.timestamp < minTimeStamp)
                {
                    minTimeStamp = csvFilesIterators[i].Current.timestamp;
                    indexOfMinTimeStamp = i;
                }
            }
            return indexOfMinTimeStamp;
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
        private void InitialFirstVisitorSession(Dictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long> allUrlSessionsList)> urlSessionDictionary,
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
        /// <param name="urlSessionDictionary">
        /// The data structure of the urls sessions.
        /// </param>
        /// <param name="currentPageView">
        /// The current PageView record with the minimal time stamp.
        /// </param>
        /// <param name="currentUrlValue"></param>
        /// <param name="currentVisitorSession"></param>
        private void UpdateVisitorSessions(Dictionary<string, (Dictionary<string, Session> userSessions, int sessionsCounter, List<long>
            allUrlSessionsList)> urlSessionDictionary, PageView currentPageView, (Dictionary<string, Session> userSessions, int sessionsCounter,
            List<long> allUrlSessionsList) currentUrlValue, Session currentVisitorSession)
        {

            if (currentPageView.timestamp - currentVisitorSession.endTime <= (30 * 60))
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
                
                urlSessionDictionary[currentPageView.mainUrl] = (currentUrlValue.userSessions, sessionCounter, currentUrlValue.allUrlSessionsList);
            }
        }

        /// <summary>
        /// The method set all iterators to point for their first record.
        /// If an iterator is empty (no records) - remove it form the iterators list.
        /// </summary>
        /// <param name="csvFilesIterators">
        /// All iterators list.
        /// </param>
        private  async  Task InitialIterators(List<IAsyncEnumerator<PageView>> csvFilesIterators)
        {
            for (int i = 0; i < csvFilesIterators.Count; i++)
            {
                 if (!await csvFilesIterators[i].MoveNextAsync())
                 {
                     csvFilesIterators.RemoveAt(i);
                 }
            }
        }
        
        /// <summary>
        /// The method add the given url to the visitor's unique url websites he visited.
        /// </summary>
        /// <param name="visitorId">
        /// The visitor ID.
        /// </param>
        /// <param name="userIdUniqueUrlVisits">
        /// The data structure of the visitor unique url website visits.
        /// </param>
        /// <param name="currentPageView">
        /// The current record with the minimal time stamp between all iterators.
        /// </param>
        private void AddUrlToVisitor(string visitorId, Dictionary<string, HashSet<string>> userIdUniqueUrlVisits, PageView currentPageView)
        {
            if (userIdUniqueUrlVisits.TryGetValue(key: visitorId, value: out var userUniqueUrlsVisit))
            {
                userUniqueUrlsVisit.Add(currentPageView.mainUrl);
            }
            else
            {
                userIdUniqueUrlVisits[visitorId] = new HashSet<string>();
            }
        }
    }
}