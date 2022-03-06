using Newtonsoft.Json;
using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerCentralized
{
    public class WorkerClient
    {
            

        object ob = new object();

        public void RunCracking()
        {
            using (TcpClient socket = new TcpClient("localhost", 7777))
            {
                Wclientdoes(socket);
            }
        }

        public void Wclientdoes(TcpClient ns) 
        {
 
            List<UserInfo> userInfos =
                PasswordFileHandler.ReadPasswordFile("passwords.txt");
            Console.WriteLine("passwd opeend");

            List<UserInfoClearText> result = new List<UserInfoClearText>();
            using (StreamReader sr = new StreamReader(ns.GetStream()))
            using (StreamWriter sw = new StreamWriter(ns.GetStream()))
            {
                while (true)
                {

                    Console.WriteLine("Starting client");

                    List<string> info = GetUserInfo(sr, sw);

                    Console.WriteLine(JsonConvert.SerializeObject(info).ToString());
                    List<Task> listOfTasks = new List<Task>();
                    
                    foreach (string us in info)
                    {
                        listOfTasks.Add(Task.Run(() =>
                        {
                            IEnumerable<UserInfoClearText> partialResult = new Cracking().CheckWordWithVariations(us, userInfos);
                            result.AddRange(partialResult);

                        }));

                    }
                    Console.WriteLine("venter på task complete");
                    Task.WaitAll(listOfTasks.ToArray());
                    listOfTasks.Clear();
                    Console.WriteLine("Alle task færdig");
                    if (result.Count != 0)
                    {
                        SendUser(sr, sw, result);
                        result.Clear();
                    }
                }
            }
        }

        public void SendUser(StreamReader sr, StreamWriter sw, List<UserInfoClearText> res)
        {
            
                sw.WriteLine("2");
                sw.Flush();
                Console.WriteLine("sender list til server");
                sw.WriteLine(JsonConvert.SerializeObject(res));
                sw.Flush();
            
        }

        public List<string> GetUserInfo(StreamReader sr, StreamWriter sw)
        {
            
                sw.WriteLine("1");
                sw.Flush();
                string theline = sr.ReadLine();
              
                return JsonConvert.DeserializeObject<List<string>>(theline);
            

        }
    }
}
