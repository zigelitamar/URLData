namespace URLdata.Models
{
    public class Session
        {
            public long startTime { get; }
            public long endTime { get; set; }

            public Session(long startTime)
            {
                this.startTime = startTime;
                this.endTime = startTime;
            }
        }
}