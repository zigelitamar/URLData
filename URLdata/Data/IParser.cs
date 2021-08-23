using System.Collections.Generic;
using URLdata.Models;

namespace URLdata.Data
{
    public interface IParser
    {
        public DataHandler Parse(List<IEnumerator<PageView>> pageIterators);
        

    }
}