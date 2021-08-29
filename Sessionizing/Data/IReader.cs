using System.Collections.Generic;
using URLdata.Models;

namespace URLdata.Data
{
    /// <summary>
    /// Interface class to read data from different sources.
    /// </summary>
    public interface IReader
    {
        public List<IEnumerator<PageView>> ReadData();
    }
}