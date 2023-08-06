using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator.Helpers
{
    public class SimHandler
    {
        public static string currentPath = Environment.CurrentDirectory+"/sims";
        public static async Task<bool> SaveSim(string address, string percentage, string currentAvatar)
        {
            // Check if file already exists. If yes, delete it.     
            if (File.Exists(currentPath+"/"+currentAvatar+"/"+address+".txt"))
            {
                File.Delete(currentPath + "/" + currentAvatar + "/" + address + ".txt");
            }

            // Create a new file     
            using (StreamWriter sw = File.CreateText(currentPath + "/" + currentAvatar + "/" + address + ".txt"))
            {
                sw.WriteLine(percentage);
            }

            return true;
        }

        public static async Task<string> LoadSim(string address, string currentAvatar)
        {
            // Check if file already exists. If yes, delete it.     
            if (File.Exists(currentPath + "/" + currentAvatar+ "/" + address + ".txt"))
            {
                // Write file contents on console.     
                using (StreamReader sr = File.OpenText(currentPath + "/" + currentAvatar+"/" + address + ".txt"))
                {
                    return (sr.ReadLine());
                }
            }

            return String.Empty;
        }
    }
}
