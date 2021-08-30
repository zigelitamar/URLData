using System.Threading.Tasks;

namespace URLdata.Data
{
    /// <summary>
    /// Interface class to handle API requests that require the parsed data structures.
    /// </summary>
    public interface IDataHandler
    {
        public Task<int> GetSessionsAmount(string url);

        public int GetUniqueSites(string visitorId);

        public double GetMedian(string url);
    }
}