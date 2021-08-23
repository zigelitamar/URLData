using System.Collections.Generic;
using URLdata.Models;

namespace URLdata.Data
{
    public interface IReader
    {
        public List<IEnumerator<PageView>> ReadData();
    }
}