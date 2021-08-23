using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using URLdata.Models;

namespace URLdata.Data
{
    
    public class CsvDataParser : IParser
    {
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary { get; set; }
        public Dictionary<string, HashSet<string>>  userIdUniqueURLVisits  { get; set; }
        

        public CsvDataParser()
        {
            Console.WriteLine("hello");

        }
      
        
        public void Parse(List<IEnumerator<PageView>> pageIterators)
        {
            List<long> timeStampsList = new List<long>();
            
            for (int i = 0; i < pageIterators.Count; i++)
            {
                if (pageIterators[i].MoveNext())
                {
                    timeStampsList.Add(pageIterators[i].Current.timestamp);
                    pageIterators[i].MoveNext();
                }
                else
                {
                    pageIterators.RemoveAt(i);
                }
            }

            
            Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary = new Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>>(); 
            Dictionary<string, HashSet<string>>  userIdUniqueURLVisits = new Dictionary<string, HashSet<string>>();
            

            while (pageIterators.Count != 0)
            {
                long minTimeStamp = timeStampsList.Min();
                int indexOfMinTimeStamp = timeStampsList.IndexOf(minTimeStamp);

                PageView currentPageView = pageIterators[indexOfMinTimeStamp].Current;

                string visitorID = currentPageView.visitor;
                
                if (!userIdUniqueURLVisits.ContainsKey(visitorID))
                {
                    userIdUniqueURLVisits[visitorID] = new HashSet<string>();
                }
                userIdUniqueURLVisits[visitorID].Add(currentPageView.mainUrl);
                
                //check of the current url exist in the dictionary
                if (urlSessionDictionary.ContainsKey(currentPageView.mainUrl))
                {
                    //check if the current visitor already have a session for the current url
                    if (urlSessionDictionary[currentPageView.mainUrl].Item1.ContainsKey(currentPageView.visitor))
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
                    else // the current visitor page view its first appearance
                    {
                        // set to the user a new session
                        Dictionary<string, Session> visitorSessionDictionary = urlSessionDictionary[currentPageView.mainUrl].Item1;
                        visitorSessionDictionary[currentPageView.visitor] = new Session(currentPageView.timestamp);
                        
                        // get  the session counter and increment it
                        int sessionCounter = urlSessionDictionary[currentPageView.mainUrl].Item2 + 1;
                        
                        List<long> sessionsLengthList = urlSessionDictionary[currentPageView.mainUrl].Item3;
                        
                        urlSessionDictionary[currentPageView.mainUrl] = Tuple.Create(visitorSessionDictionary, sessionCounter, sessionsLengthList);
                    }
                }
                else //url not exist yet in the data structure -> set first session timestamp, set sessions counter to 1, initial new list for sessions lengths
                {
                    Dictionary<string, Session> visitorSessionDictionary = new Dictionary<string, Session>();
                    visitorSessionDictionary[currentPageView.visitor] = new Session(currentPageView.timestamp);
                    
                    urlSessionDictionary[currentPageView.mainUrl] = Tuple.Create(visitorSessionDictionary, 1, new List<long>());
                }

                
                if (!pageIterators[indexOfMinTimeStamp].MoveNext())
                {
                    pageIterators.RemoveAt(indexOfMinTimeStamp);
                    timeStampsList.RemoveAt(indexOfMinTimeStamp);
                }
                else
                {
                    timeStampsList[indexOfMinTimeStamp] =
                        pageIterators[indexOfMinTimeStamp].Current.timestamp;
                }
            }

            this.urlSessionDictionary = urlSessionDictionary;
            this.userIdUniqueURLVisits = userIdUniqueURLVisits;

        }
        
    }
}