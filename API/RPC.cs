using ArenaSimulator.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator.API
{
    public class RPC
    {
        public static async Task<List<ChainModel>> LoadRPCALL()
        {
            var client = new RestClient("https://api.9capi.com/rpc");
            var request = new RestRequest();
            RestResponse response = client.Execute(request);
            while (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Thread.Sleep(1000);
                response = client.Execute(request);
            }
            List<ChainModel> jsonobject = JsonConvert.DeserializeObject<List<ChainModel>>(response.Content);
            return jsonobject;
        }
    }
}
