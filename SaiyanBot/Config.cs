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
    }
}
