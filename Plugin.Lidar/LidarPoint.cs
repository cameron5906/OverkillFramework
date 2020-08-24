using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Plugin.Lidar
{
    public class LidarPoint
    {
        [JsonPropertyName("x")]
        public float X { get; set; }
        [JsonPropertyName("y")]
        public float Y { get; set; }
    }
}
