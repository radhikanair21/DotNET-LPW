using System;

namespace AndroidDataExtractor.Models
{
    public class CallLog
    {
        public string PhoneNumber { get; set; }
        public string CallType { get; set; }
        public int Duration { get; set; }
        public DateTime CallTime { get; set; }

        public string FormattedDuration
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(Duration);
                return time.Hours > 0
                    ? $"{time.Hours}h {time.Minutes}m {time.Seconds}s"
                    : time.Minutes > 0
                        ? $"{time.Minutes}m {time.Seconds}s"
                        : $"{time.Seconds}s";
            }
        }
    }
}