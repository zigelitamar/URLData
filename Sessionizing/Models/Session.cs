namespace URLdata.Models
{
    /// <summary>
    /// the class represent a session for a visitor.
    /// </summary>
    public class Session
    {
        public long startTime { get; set; }
        public long endTime { get; set; }

        public Session(long startTime)
        {
            this.startTime = startTime;
            endTime = startTime;
        }

        public void ResetSession(long newTimestamp)
        {
            startTime = newTimestamp;
            endTime = newTimestamp;
        }
    }
}