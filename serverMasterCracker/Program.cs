using System;

namespace serverMasterCracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            Worker worker = new Worker();
            worker.Start();
        }
    }
}
