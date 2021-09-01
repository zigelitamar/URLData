using Microsoft.AspNetCore.Mvc;

namespace URLdata.Models
{
    public class VisitorId
    {
        [FromQuery] 
        public int visitorId { get; set; }
    }
}