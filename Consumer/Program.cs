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
        static bool singleMessage = true;

        public static async Task Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Consumer - reading messages, processing and deleting from Azure queue 500 ms.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Press 1 to read One message at a time, or any other key to read Five messages at a time: ");
            var key = Console.ReadKey(true);
            singleMessage = key.Key == ConsoleKey.D1;

            // Using a SAS Key that is valid for a fixed period only.
            // "Connection String" value when Generare SAS and connection string button in Azure 
            // OR - use from app.config "appSettings" -> AzureQueueStorageConnectionString
            var connectionString = CloudConfigurationManager.GetSetting("AzureQueueStorageConnectionString");

            IQueueService queueService = new QueueService(connectionString, "samplequeue");

            if (key.Key == ConsoleKey.D1)
            {
                await ProcessOneMessage(queueService);
            }
            else
            {
                await ProcessFiveMessages(queueService);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Completed. Press a key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        ///  Clear screen and print titles.
        /// </summary>
        private static void ClearPrintTitles()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Consumer - reading messages, processing and deleting from Azure queue 500 ms.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(singleMessage ? "[One Message Mode, 30 second invisbility]" : "[Five Messages Mode, 1 hour invisibilty]");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Press CTRL+C to stop the consumer reading, processing and deleting queue messages.");
            Console.WriteLine();
        }

        /// <summary>
        /// Reads one message at a time and then deletes it from the <paramref name="queueService"/>
        /// </summary>
        /// <param name="queueService">valid queue.</param>
        /// <returns><see cref="Task"/></returns>
        private static async Task ProcessOneMessage(IQueueService queueService)
        {
            var messagesRead = 0;
            var emptyCleared = true;

            while (!cancelConsumer)
            {
                var message = await queueService.GetMessageAsync();

                if (message != null)
                {
                    if (!emptyCleared)
                    {
                        emptyCleared = true;
                    }

                    ClearPrintTitles();

                    messagesRead++;
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Messages read: " + messagesRead.ToString());
                    Console.WriteLine("Last message as JSON:");
                    var json = JsonConvert.SerializeObject(message);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(json);
                    await queueService.DeleteMessageAsync(message);
                }
                else
                {
                    if (emptyCleared)
                    {
                        ClearPrintTitles();
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[Queue is Empty, Waiting]");
                        emptyCleared = false;
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Reads five messages at a time and then deletes them from the <paramref name="queueService"/>
        /// </summary>
        /// <param name="queueService">valid queue.</param>
        /// <returns><see cref="Task"/></returns>
        private static async Task ProcessFiveMessages(IQueueService queueService)
        {
            var messagesRead = 0;
            var emptyCleared = true;

            while (!cancelConsumer)
            {
                var messages = await queueService.GetMessagesAsync(5, TimeSpan.FromHours(1));
                
                if (messages != null && messages.Count()> 0)
                {
                    if (!emptyCleared)
                    {
                        emptyCleared = true;
                    }

                    ClearPrintTitles();

                    messagesRead += messages.Count();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Total Messages read: " + messagesRead.ToString() + ", GetMessagesAsync: "+ messages.Count().ToString());
                    Console.WriteLine("Last messages as JSON:");
                    var json = JsonConvert.SerializeObject(messages);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(json);

                    foreach (var message in messages)
                    {
                        await queueService.DeleteMessageAsync(message);
                    }
                }
                else
                {
                    if (emptyCleared)
                    {
                        ClearPrintTitles();
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[Queue is Empty, Waiting]");
                        emptyCleared = false;
                    }
                }

                Thread.Sleep(500);
            }
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
