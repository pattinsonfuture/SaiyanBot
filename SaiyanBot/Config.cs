using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SaiyanBot
{
    internal struct Config
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("lavalink_hostname")] 
        public string LaravelLinkHostName { get; private set; }

        [JsonProperty("lavalink_port")]
        public int LaravelLinkPort { get; private set; }

        [JsonProperty("lavalink_password")]
        public string LaravelLinkPassword { get; private set; }
    }
}
