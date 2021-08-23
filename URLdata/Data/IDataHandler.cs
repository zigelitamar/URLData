namespace URLdata.Data
{
    public interface IDataHandler
    {
        public int getSessionsAmount(string url);

        public int getUniqueSites(string visitorID);

        public long getMedian(string url);
    }
}