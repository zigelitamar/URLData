using System.Collections.Generic;
using URLdata.Models;

namespace URLdata.Data
{
    public interface IParser
    {
        public void Parse(List<IEnumerator<PageView>> pageIterators);
        

    }
}