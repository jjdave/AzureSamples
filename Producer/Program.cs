namespace Producer
{
    using Storage;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure;

    class Program
    { 
        volatile static bool cancelProducer = false;

        public static async Task Main(string[] args)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Producer - adding messages to Azure queue 250 ms.");
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press CTRL+C to stop the producer adding queue messages.");
            Console.WriteLine();
                        
            // Using a SAS Key that is valid for a fixed period only.
            // "Connection String" value when Generare SAS and connection string button in Azure 
            // OR - use from app.config "appSettings" -> AzureQueueStorageConnectionString
            var connectionString = CloudConfigurationManager.GetSetting("AzureQueueStorageConnectionString");

            var messagesAdded = 0;
            IQueueService queueService = new QueueService(connectionString, "samplequeue");

            while (!cancelProducer)
            {
                var message = string.Format("Sample message at {0}", DateTime.Now.ToUniversalTime());
                await queueService.AddMessageAsync(message);
                messagesAdded++;
                Console.SetCursorPosition(0, 4);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Messages added: " + messagesAdded.ToString());
                Console.SetCursorPosition(0, 5);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Added to queue: " + message);
                Thread.Sleep(250);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Completed. Press a key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Handles a Cancel Press and sets a static boolean to signal the main loop to exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            cancelProducer = true;
        }
    }
}
