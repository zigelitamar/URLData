using System.Threading.Tasks;

namespace URLdata.Data
{
    /// <summary>
    /// Interface class to handle API requests that require the parsed data structures.
    /// </summary>
    public interface IDataHandler
    {
        public Task<int> GetSessionsAmount(string url);

        public Task<int> GetUniqueSites(string visitorId);

        public Task<double> GetMedian(string url);
    }
}