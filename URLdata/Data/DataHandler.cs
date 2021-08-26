using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using URLdata.Models;
using MathNet.Numerics.Statistics;

namespace URLdata.Data
{
    /// <summary>
    /// Data handler class.
    /// Implements the IDataHandler interface.
    /// </summary>
    public class DataHandler : IDataHandler
    {
        public Dictionary<string, Tuple<Dictionary<string, Session>, int, List<long>>> urlSessionDictionary { get; }

        public Dictionary<string, HashSet<string>>  userIdUniqueUrlVisits { get; }

        public Dictionary<string, double > mediansCalculated { get; }



        /// <summary>
        /// Constructor.
        /// Receives an IParser implementation object.
        /// the constructor makes a placement from the parser data structure
        /// and initialize a Dictionary for calculating the medians
        /// of websites by caching user request for later use of other users. 
        /// </summary>
        /// <param name="parser">
        /// IReader implementation object.
        /// </param>
        public DataHandler(IParser parser)
        {
            var parser1 = parser;
            mediansCalculated = new Dictionary<string, double>();
            urlSessionDictionary = parser1.urlSessionDictionary;
            userIdUniqueUrlVisits = parser1.userIdUniqueUrlVisits;

        }
        
        /// <summary>
        /// The method returns the amount of sessions of a given website.
        /// </summary>
        /// <param name="url"></param>
        /// String argument - a url website.
        /// <returns>
        /// int - the amount of sessions of the given website.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        ///  An exception if the given url in not exists. 
        /// </exception>
        public int GetSessionsAmount(string url)
        {
            if (urlSessionDictionary.ContainsKey(url))
            {
                return urlSessionDictionary[url].Item2;
            }
            else
            {
                throw new KeyNotFoundException("no records found");
            }
            
        }
        /// <summary>
        /// The method returns the amount of unique websites
        /// a given visitor has visited.
        /// </summary>
        /// <param name="visitorId">
        /// String argument - a visitor ID.  
        /// </param>
        /// <returns>
        /// int - the amount of unique website the user has visited.
        /// </returns>
        public int GetUniqueSites(string visitorId)
        {
            if (userIdUniqueUrlVisits.ContainsKey(visitorId))
            {
                return userIdUniqueUrlVisits[visitorId].Count;
            }

            return -1;
        }

      
        /// <summary>
        /// the method receives a url and checks if the the median of
        /// the url has already calculated before.
        /// if not - the method will calculate the median of the given url
        /// and will keep the outcome for later requests.
        /// </summary>
        /// <param name="url">
        /// String argument - a url website.
        /// </param>
        /// <returns>
        ///  double - the median of he given url.
        /// </returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public double GetMedian(string url)
        {
            
            if (!urlSessionDictionary.ContainsKey(url))
            {
                throw new KeyNotFoundException("Could not find record");
            }
            if (mediansCalculated.ContainsKey(url))
            {
                return mediansCalculated[url];
            }
            //calculating median of sessions length and update the value in the medians map
            var lengths = urlSessionDictionary[url].Item3;
            lengths.Sort();
            int lengthsSize = lengths.Count;
            int midElement = lengthsSize / 2;
            double median = (lengthsSize % 2 != 0) ? lengths[midElement] : ((double)lengths[midElement] + lengths[midElement - 1]) / 2;
            mediansCalculated[url] = median;
            return median;
        }
    }
}