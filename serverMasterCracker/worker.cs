using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Net;

namespace serverMasterCracker
{
    public class Worker
    {

        private List<UserInfoClearText> userInfos = new List<UserInfoClearText>();
        List<UserInfoClearText> _result = new List<UserInfoClearText>();
        BlockingCollection<List<string>> _blockingList;

        public Worker()
        {
            _blockingList = BlockList();
        }


        public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 7777);
            listener.Start();
            while (true)
            {
                Task.Run(() =>
                {
                    TcpClient socket = listener.AcceptTcpClient();
                    SendPackets(socket.GetStream());
                });
            }
        }

        public static BlockingCollection<List<string>> BlockList()
        {
            List<string> webster = new List<string>();
            BlockingCollection<List<string>> blockcol = new BlockingCollection<List<string>>();
            int counter = 1;
            using(FileStream fs = new FileStream("webster-dictionary.txt", FileMode.Open, FileAccess.Read))
            using (StreamReader dictionary = new StreamReader(fs))
            {
                while (!dictionary.EndOfStream)
                {
                    String dictionaryEntry = dictionary.ReadLine();
                    if (counter%1000 != 0)
                    {
                        webster.Add(dictionaryEntry);
                        counter++;
                    }
                    else
                    {
                        counter = 1;
                        blockcol.Add(webster);
                        webster = new List<string>();
                    }
                }
            }
            return blockcol;
        }

        public void SendPackets(NetworkStream socket)
        {
            using (StreamReader sr = new StreamReader(socket))
            using (StreamWriter sw = new StreamWriter(socket))
            {
                while (true)
                {
                    string se = sr.ReadLine();
                    switch (se)
                    {
                        case "1":
                            if (_blockingList.Count != 0 )
                            {

                            sw.WriteLine(JsonConvert.SerializeObject(_blockingList.Take()));
                            Console.WriteLine("Sender til socket");
                            sw.Flush();
                            }
                            break;
                        case "2":
                            foreach (UserInfoClearText us in JsonConvert.DeserializeObject<List<UserInfoClearText>>(sr.ReadLine()))
                            {
                                _result.Add(us);
                                Console.WriteLine("blev sendt: " + us.ToString());
                            }
                            break;
                        default:
                            Console.WriteLine("wrong input" + se);
                            break;
                    }
                }
                
            }
        }
    }
}
