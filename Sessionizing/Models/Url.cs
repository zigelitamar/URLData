namespace URLdata.Models
{
    public class Url
    {
        public string address { get; }

        public Url(string url)
        {
            this.address = url;
        }
    }
}