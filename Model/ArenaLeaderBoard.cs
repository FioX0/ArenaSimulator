using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator.Model
{
    public class ArenaLeaderBoard
    {
        [JsonProperty("avataraddress")]
        public string avataraddress { get; set; }

        [JsonProperty("avatarname")]
        public string avatarname { get; set; }

        [JsonProperty("rankid")]
        public int rankid { get; set; }

        [JsonProperty("cp")]
        public int cp { get; set; }

        [JsonProperty("score")]
        public int score { get; set; }
    }
}
