using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using URLdata.Exceptions;
using URLdata.Models;
using Xunit.Sdk;

namespace URLdata.Data
{
    /// <summary>
    /// Parser class for CSV files.
    /// Implements the IParser interface.
    /// </summary>
    public class CsvDataParser : IParser
    {
        private readonly IReader _reader;
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> UrlSessionDictionary { get;
            set;
        }
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
        public void Parse()
        {
            if (_reader == null)
            {
                throw new NullReferenceException("IReader object is null");
            }
            // get iterators list
            List<IEnumerator<PageView>> csvFilesIterators = null;
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
            
            // create a list of the current timestamp of each iterator.
            List<long> timeStampsList = new List<long>();
            
            // get the timestamp of each iterator (if the csv file is empty/no records in the csv file - remove the iterator)
            GetFirstTimeStamps(csvFilesIterators, timeStampsList);

            // initial the data structures to save the necessary data from the csv files.
            Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary = new Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>>(); 
            Dictionary<string, HashSet<string>>  userIdUniqueUrlVisits = new Dictionary<string, HashSet<string>>();
            
            // iterate on chronologically (by time stamps) on all iterators together.
            while (csvFilesIterators.Count != 0)
            {
                //get index of the minimal timestamp between all iterators anf its record.
                var minTimeStamp = timeStampsList.Min();
                var indexOfMinTimeStamp = timeStampsList.IndexOf(minTimeStamp);
                var currentPageView = csvFilesIterators[indexOfMinTimeStamp].Current;
                
                string visitorId = currentPageView.visitor;

                AddUrlToVisitor(visitorId, userIdUniqueUrlVisits, currentPageView);

                //check if the current url exist in the dictionary
                if (urlSessionDictionary.ContainsKey(currentPageView.mainUrl))
                {
                    //check if the current visitor already have a session for the current url
                    if (urlSessionDictionary[currentPageView.mainUrl].Item1.ContainsKey(currentPageView.visitor))
                    {
                       UpdateVisitorSessions(urlSessionDictionary, currentPageView);
                    }
                    else // the current visitor page view is its first appearance
                    {
                        InitialFirstVisitorSession(urlSessionDictionary, currentPageView);
                    }
                }
                else //url not exist yet in the data structure -> set first session timestamp, set sessions counter to 1, initial new list for sessions lengths
                {
                    Dictionary<string, Session> visitorSessionDictionary = new Dictionary<string, Session>();
                    visitorSessionDictionary[currentPageView.visitor] = new Session(currentPageView.timestamp);
                    
                    urlSessionDictionary[currentPageView.mainUrl] = Tuple.Create(visitorSessionDictionary, 1, new List<long>());
                }

                // promote the chosen iterator with the minimal time stamp
                PromoteIterator(csvFilesIterators, timeStampsList, indexOfMinTimeStamp);
            }

            UrlSessionDictionary = urlSessionDictionary;
            UserIdUniqueUrlVisits = userIdUniqueUrlVisits;
            Console.WriteLine("DONE PROCESSING");
        }

        public Task ParseAsync()
        {
              // get iterators list
            List<IEnumerator<PageView>> csvFilesIterators = null;
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
            
            // create a list of the current timestamp of each iterator.
            List<long> timeStampsList = new List<long>();
            
            // get the timestamp of each iterator (if the csv file is empty/no records in the csv file - remove the iterator)
            GetFirstTimeStamps(csvFilesIterators, timeStampsList);

            // initial the data structures to save the necessary data from the csv files.
            Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary = new Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>>(); 
            Dictionary<string, HashSet<string>>  userIdUniqueUrlVisits = new Dictionary<string, HashSet<string>>();
            
            // iterate on chronologically (by time stamps) on all iterators together.
            while (csvFilesIterators.Count != 0)
            {
                //get index of the minimal timestamp between all iterators anf its record.
                var minTimeStamp = timeStampsList.Min();
                var indexOfMinTimeStamp = timeStampsList.IndexOf(minTimeStamp);
                var currentPageView = csvFilesIterators[indexOfMinTimeStamp].Current;
                
                string visitorId = currentPageView.visitor;

                AddUrlToVisitor(visitorId, userIdUniqueUrlVisits, currentPageView);

                //check if the current url exist in the dictionary
                if (urlSessionDictionary.ContainsKey(currentPageView.mainUrl))
                {
                    //check if the current visitor already have a session for the current url
                    if (urlSessionDictionary[currentPageView.mainUrl].Item1.ContainsKey(currentPageView.visitor))
                    {
                       UpdateVisitorSessions(urlSessionDictionary, currentPageView);
                    }
                    else // the current visitor page view is its first appearance
                    {
                        InitialFirstVisitorSession(urlSessionDictionary, currentPageView);
                    }
                }
                else //url not exist yet in the data structure -> set first session timestamp, set sessions counter to 1, initial new list for sessions lengths
                {
                    Dictionary<string, Session> visitorSessionDictionary = new Dictionary<string, Session>();
                    visitorSessionDictionary[currentPageView.visitor] = new Session(currentPageView.timestamp);
                    
                    urlSessionDictionary[currentPageView.mainUrl] = Tuple.Create(visitorSessionDictionary, 1, new List<long>());
                }

                // promote the chosen iterator with the minimal time stamp
                PromoteIterator(csvFilesIterators, timeStampsList, indexOfMinTimeStamp);
            }

            UrlSessionDictionary = urlSessionDictionary;
            UserIdUniqueUrlVisits = userIdUniqueUrlVisits;
            Console.WriteLine("DONE PROCESSING");

        }

        /// <summary>
        /// The method promote the iterator with the minimal timestamp
        /// and checks if this current iterator reached to its end.
        /// If it does - remove the iterator from the iterators list and and its timestamp
        /// from the the timestamps list as well.
        /// </summary>
        /// <param name="csvFilesIterators">
        /// All iterators list.
        /// </param>
        /// <param name="timeStampsList">
        /// All current iterators time stamps.
        /// </param>
        /// <param name="indexOfMinTimeStamp">
        /// The index of the iterator with the minimal time stamp.
        /// </param>
        private void PromoteIterator(List<IEnumerator<PageView>> csvFilesIterators, List<long> timeStampsList, int indexOfMinTimeStamp)
        {
            if (!csvFilesIterators[indexOfMinTimeStamp].MoveNext())
            {
                csvFilesIterators.RemoveAt(indexOfMinTimeStamp);
                timeStampsList.RemoveAt(indexOfMinTimeStamp);
            }
            else
            {
                timeStampsList[indexOfMinTimeStamp] =
                    csvFilesIterators[indexOfMinTimeStamp].Current.timestamp;
            }
        }

        /// <summary>
        /// The method initial the the first session of a visitor entring a new
        /// website url.
        /// </summary>
        /// <param name="urlSessionDictionary">
        /// The data structure of the urls sessions.
        /// </param>
        /// <param name="currentPageView">
        /// The current PageView record with the minimal time stamp.
        /// </param>
        private void InitialFirstVisitorSession(Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary, PageView currentPageView)
        {
            // set to the user a new session
            Dictionary<string, Session> visitorSessionDictionary = urlSessionDictionary[currentPageView.mainUrl].Item1;
            visitorSessionDictionary[currentPageView.visitor] = new Session(currentPageView.timestamp);
                        
            // get  the session counter and increment it
            int sessionCounter = urlSessionDictionary[currentPageView.mainUrl].Item2 + 1;
                        
            List<long> sessionsLengthList = urlSessionDictionary[currentPageView.mainUrl].Item3;
                        
            urlSessionDictionary[currentPageView.mainUrl] = Tuple.Create(visitorSessionDictionary, sessionCounter, sessionsLengthList);
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
        private void UpdateVisitorSessions(Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary, PageView currentPageView)
        {
           long userLastSessionTimeStamp = urlSessionDictionary[currentPageView.mainUrl]
                        .Item1[currentPageView.visitor].endTime;

            //check if the current page view belong to the last session (no longer than 30 minute).
            if (currentPageView.timestamp - userLastSessionTimeStamp <= (30 * 60))
            {
                //update the last time stamp of the current session.
                urlSessionDictionary[currentPageView.mainUrl]
                    .Item1[currentPageView.visitor].endTime = currentPageView.timestamp;
            }
            else
            {
                // get the start session
                long userStartSessionTimeStamp = urlSessionDictionary[currentPageView.mainUrl]
                    .Item1[currentPageView.visitor].startTime;
                
                // add the session time to the sessions length list
                List<long> sessionsLengthList = urlSessionDictionary[currentPageView.mainUrl].Item3;
                sessionsLengthList.Add(userLastSessionTimeStamp - userStartSessionTimeStamp);
                
                //set the new session by the current page view.
                Dictionary<string, Session> visitorSessionDictionary = urlSessionDictionary[currentPageView.mainUrl].Item1;
                visitorSessionDictionary[currentPageView.visitor] = new Session(currentPageView.timestamp);
                
                // increment the counter of the sessions amount
                int sessionCounter = urlSessionDictionary[currentPageView.mainUrl].Item2 + 1;
                
                urlSessionDictionary[currentPageView.mainUrl] = Tuple.Create(visitorSessionDictionary, sessionCounter, sessionsLengthList);
            }
        }

        /// <summary>
        /// The method get from all the iterators their first page view time stamps.
        /// </summary>
        /// <param name="csvFilesIterators">
        /// All iterators list.
        /// </param>
        /// <param name="timeStampsList">
        /// all first time stamps from all the iterators.
        /// </param>
        private void GetFirstTimeStamps(List<IEnumerator<PageView>> csvFilesIterators, List<long> timeStampsList)
        {
            for (int i = 0; i < csvFilesIterators.Count; i++)
            {
                if (csvFilesIterators[i].MoveNext())
                {
                    var pageView = csvFilesIterators[i].Current;
                    if (pageView != null) timeStampsList.Add(pageView.timestamp);
                    csvFilesIterators[i].MoveNext();
                }
                else
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
            if (!userIdUniqueUrlVisits.ContainsKey(visitorId))
            {
                userIdUniqueUrlVisits[visitorId] = new HashSet<string>();
            }
            userIdUniqueUrlVisits[visitorId].Add(currentPageView.mainUrl);
        }
    }
}