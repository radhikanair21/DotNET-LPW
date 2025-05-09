using System;

namespace AndroidDataExtractor.Models
{
    public class DeviceInfo
    {
        public string DeviceID { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string CPUInfo { get; set; }
        public string MemoryInfo { get; set; }
    }
}