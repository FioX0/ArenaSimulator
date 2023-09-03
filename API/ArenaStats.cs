using ArenaSimulator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator.API
{
    public class ArenaStats
    {
        public static async Task<List<ArenaLeaderBoard>> GetStats() 
        {
            var client = new RestClient("https://api.9capi.com/arenaLeaderboard");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            //var body = @"{""avatarAddress"": ""0x3b7a47daaece48807fc00a310b05bd9f5d26736e"",""enemyAddress"": ""0xab44635462880666daa7f2be5a21c71c1590ff2b""}";
            //request.AddParameter("application/json", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            var result = JsonConvert.DeserializeObject<List<ArenaLeaderBoard>>(response.Content);

            return result;
        }

        public static async Task<string> Simulate(string avatar, string enemy)
        {
            var client = new RestClient("https://api.9capi.com/arenaSim");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            var body = "{\"avatarAddress\": \""+avatar+"\",\"enemyAddress\": \""+enemy+"\"}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            RestResponse response = client.ExecutePost(request);
            try
            {
                JObject result2 = JObject.Parse(response.Content);
                float result = float.Parse((string)result2["winPercentage"], CultureInfo.InvariantCulture);
                return result.ToString();
            }
            catch(Exception ex)
            {
                for(int i=0; i<5; i++)
                {
                    response = client.ExecutePost(request);
                    if(response.Content != "\"Failed\"\n")
                    {
                        float result = float.Parse(response.Content.Replace(".", ","));
                        return result.ToString();
                    }
                }
                throw new Exception("I was Unable to get a result for you. Something went very wrong");
            }
        }
    }
}
