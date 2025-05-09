using System;

namespace AndroidDataExtractor.Models
{
    public class Message
    {
        public string Sender { get; set; }
        public string MessageContent { get; set; }
        public DateTime Timestamp { get; set; }
    }
}