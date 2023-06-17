using ArenaSimulator.Model;
using GQLMLauncher;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator.GQL
{
    public class PullData
    {
        public static async Task<List<AvatarModel>> GetAvatar(string agent, string host)
        {
            try
            {
                List<AvatarModel> avatarList = new List<AvatarModel>();
                var client = new RestClient(host);
                var request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"query {\\r\\n  stateQuery {\\r\\n    agent(address: \\\"" + agent + "\\\") {\\r\\n      avatarStates {\\r\\n        address, actionPoint, name, dailyRewardReceivedIndex, level\\r\\n      }\\r\\n    }\\r\\n  }\\r\\n}\\r\\n\",\"variables\":{}}",
                           ParameterType.RequestBody);
                RestResponse response = client.ExecutePost(request);
                JObject joResponse = JObject.Parse(response.Content);
                var resultObjects = AllChildren(JObject.Parse(response.Content))
                    .First(c => c.Type == JTokenType.Array && c.Path.Contains("avatarStates"))
                    .Children<JObject>();

                foreach (var resultObject in resultObjects)
                {
                    AvatarModel avatarModel = new AvatarModel();
                    avatarModel.address = (string)resultObject["address"];
                    avatarModel.name = (string)resultObject["name"];
                    avatarModel.level = (int)resultObject["level"];
                    avatarList.Add(avatarModel);
                }
                return avatarList;
            }
            catch (Exception ex) { return null; }
        }

        public static List<KeyModels> GetAddresses()
        {
            /// <summary>
            /// Login
            /// </summary>
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "planet.exe";
            p.StartInfo.Arguments = "key list";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            // Read the output stream first and then wait.
            //p.StandardInput.WriteLine("planet key list");
            string output = p.StandardOutput.ReadToEnd();

            List<KeyModels> KeyList = new List<KeyModels>();

            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = output.Split(stringSeparators, StringSplitOptions.None);

            foreach (string s in lines)
            {
                if (s != string.Empty)
                {
                    KeyModels model = new KeyModels();

                    model.keyid = s.Split(' ')[0];
                    model.publickey = s.Split(' ')[1];
                    KeyList.Add(model);
                }
            }
            return KeyList;
        }

        public static IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
        }
    }
}
