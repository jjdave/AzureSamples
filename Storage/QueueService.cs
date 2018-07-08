namespace Storage
{
    using System;
    using System.Threading.Tasks;
    using System.Configuration;

    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;


    /// <summary>
    /// Encapsulates an Azure Storage Queue
    /// </summary>
    public class QueueService : IQueueService
    {
        /// <summary>
        /// Defines the name of the queue. Queue names must be all lowercase.
        /// </summary>
        private readonly string queueName;

        /// <summary>
        /// Reference to the cloud storage account for the queue.
        /// </summary>
        private readonly CloudStorageAccount cloudStorageAccount;

        /// <summary>
        /// Returns an instance of the <see cref="QueueService"/>
        /// </summary>
        /// <param name="queueStorageConnectionString">Azure storage queue configuration.</param>
        /// <param name="queueName">Queue name, must be all lowercase.</param>
        /// <exception cref="ArgumentException">if <paramref name="queueStorageConnectionString"/> or <paramref name="queueName"/> are null, empty or whitespace.</exception>
        public QueueService(string queueStorageConnectionString, string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueStorageConnectionString))
            {
                throw new ArgumentException("must not be empty, null or whitespace.", nameof(queueStorageConnectionString));
            }

            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("must not be empty, null or whitespace.", nameof(queueName));
            }

            this.queueName = queueName.ToLower();

            cloudStorageAccount = CloudStorageAccount.Parse(queueStorageConnectionString);           
        }

        /// <summary>
        /// Returns an instance of the <see cref="QueueService"/>
        /// </summary>
        /// <param name="queueStorageConnectionString">Azure storage queue configuration.</param>
        /// <param name="queueName">Queue name, must be all lowercase.</param>
        /// <exception cref="ArgumentException">if <paramref name="queueStorageConnectionString"/> or <paramref name="queueName"/> are null, empty or whitespace.</exception>
        public QueueService(bool a, string sas, string queueName)
        {

            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("must not be empty, null or whitespace.", nameof(queueName));
            }

            this.queueName = queueName.ToLower();

            // Connection string for Queue storage from the config file of the respective application using this library.
            // var queueConfig = CloudConfigurationManager.GetSetting(QueueStorageConnectionKeyName);


            // Create new storage credentials using the SAS token.
           // StorageCredentials accountSAS = new StorageCredentials(sasToken);

            // Use these credentials and the account name to create a Blob service client.
            //CloudStorageAccount accountWithSAS = new CloudStorageAccount(accountSAS, "account-name", endpointSuffix: null, useHttps: true);

            //cloudStorageAccount = CloudStorageAccount.Parse(queueStorageConnectionString);
        }

        /// <summary>
        /// Adds a message to the queue
        /// </summary>
        /// <param name="message">a valid message string.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, whitespace or greater than 65536 bytes.</exception>
        /// <returns><see cref="Task"/></returns>
        public async Task AddMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("must contain a valid string", nameof(message));
            }

            // Creates the azure queue service client.
            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();

            // Gets a reference to the queue. Queue names must be lowercase.
            CloudQueue sampleQueue = queueClient.GetQueueReference(queueName);
            
            // Get the queue, creating it if it does not exist.
            await sampleQueue.CreateIfNotExistsAsync();
            
            // Create a queue message and then add it to the queue.
            CloudQueueMessage queueMessage = new CloudQueueMessage(message);
            await sampleQueue.AddMessageAsync(queueMessage);
        }

        /// <summary>
        /// Peeks at the next message in the queue. Peeking keeps the message visible to all consumers.
        /// </summary>
        /// <returns><see cref="Task{CloudQueueMessage}"/></returns>
        public async Task<CloudQueueMessage> PeekMessageAsync()
        {
            // Creates the azure queue service client.
            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();

            // Gets a reference to the queue. Queue names must be lowercase.
            CloudQueue sampleQueue = queueClient.GetQueueReference(queueName);

            // Get the queue, creating it if it does not exist.
            await sampleQueue.CreateIfNotExistsAsync();

            // Peek at the next message in the queue. 
            return await sampleQueue.PeekMessageAsync();
        }

        /// <summary>
        /// Gets the next message in the queue. Azure will make the message invisible to all consumers for a default of 30 seconds.
        /// </summary>
        /// <returns><see cref="Task{CloudQueueMessage}"/></returns>
        public async Task<CloudQueueMessage> GetMessageAsync()
        {
            // Creates the azure queue service client.
            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();

            // Gets a reference to the queue. Queue names must be lowercase.
            CloudQueue sampleQueue = queueClient.GetQueueReference(queueName);

            // Get the queue, creating it if it does not exist.
            await sampleQueue.CreateIfNotExistsAsync();

            // Gets the next message in the queue. 
            // Azure will also make this message invisble to other consumers for a default of 30 seconds. 
            return await sampleQueue.GetMessageAsync();
        }


        /// <summary>
        /// Deletes the given <paramref name="message"/> from the queue.
        /// </summary>
        /// <param name="message">a valid queue message.</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteMessageAsync(CloudQueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // Creates the azure queue service client.
            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();

            // Gets a reference to the queue. Queue names must be lowercase.
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // Get the queue, creating it if it does not exist.
            await queue.CreateIfNotExistsAsync();

            // Deletes the message from the queue
            await queue.DeleteMessageAsync(message);
        }
    }
}
