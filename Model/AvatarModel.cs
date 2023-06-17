using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator.Model
{
    public class AvatarModel
    {
        [JsonProperty("address")]
        public string address { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("level")]
        public int level { get; set; }
    }
}
