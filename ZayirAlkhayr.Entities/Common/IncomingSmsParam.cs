using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Common
{
    public class IncomingSmsParam
    {
        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("sentStamp")]
        public string SentStamp { get; set; }

        [JsonPropertyName("receivedStamp")]
        public string ReceivedStamp { get; set; }

        [JsonPropertyName("sim")]
        public string Sim { get; set; }
    }
}
