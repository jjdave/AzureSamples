namespace Producer
{
    using Storage;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure;
    using Newtonsoft.Json;
    using Microsoft.WindowsAzure.Storage.Queue;
    using System.Linq;

    class Consumer
    {
        volatile static bool cancelConsumer = false;

        public static async Task Main(string[] args)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Consumer - reading messages, processing and deleting from Azure queue 500 ms.");
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press CTRL+C to stop the consumer reading, processing and deleting queue messages.");
            Console.WriteLine();

            // Using a SAS Key that is valid for a fixed period only.
            // "Connection String" value when Generare SAS and connection string button in Azure 
            // OR - use from app.config "appSettings" -> AzureQueueStorageConnectionString
            var connectionString = CloudConfigurationManager.GetSetting("AzureQueueStorageConnectionString");

            var messagesRead = 0;
            var emptyCleared = true;
            IQueueService queueService = new QueueService(connectionString, "samplequeue");

            while (!cancelConsumer)
            {
                var message = await queueService.GetMessageAsync();

                if (message != null)
                {
                    if (!emptyCleared)
                    {
                        emptyCleared = true;
                        Console.SetCursorPosition(0, 4);
                        Console.WriteLine(Enumerable.Repeat(string.Empty, 25));
                    }

                    messagesRead++;
                    Console.SetCursorPosition(0, 6);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Messages read: " + messagesRead.ToString());
                    Console.SetCursorPosition(0, 7);
                    Console.WriteLine("Last message as JSON:");
                    var json = JsonConvert.SerializeObject(message);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(json);
                    await queueService.DeleteMessageAsync(message);
                }
                else
                {
                    Console.SetCursorPosition(0, 4);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Queue is Empty, Waiting]");
                    emptyCleared = false;
                }

                Thread.Sleep(500);
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
            cancelConsumer = true;
        }
    }
}
