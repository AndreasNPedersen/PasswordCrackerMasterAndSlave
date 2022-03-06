namespace PasswordCrackerCentralized
{
    class Program
    {
        static void Main()
        {
            WorkerClient client = new WorkerClient();
            client.RunCracking();
        }


    }
}
