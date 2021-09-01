using Microsoft.AspNetCore.Mvc;

namespace URLdata.Models
{
    public class Url
    {
        [FromQuery] 
        public string address { get; set; }
    }
}