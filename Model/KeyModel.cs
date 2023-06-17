using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GQLMLauncher
{
    public class KeyModels
    {
        [JsonProperty("keyid")]
        public string keyid { get; set; }

        [JsonProperty("publickey")]
        public string publickey { get; set; }
    }
}
