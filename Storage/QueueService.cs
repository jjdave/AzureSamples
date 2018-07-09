namespace Storage
{
    using System;
    using System.Threading.Tasks;
    using System.Configuration;

    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using System.Collections;
    using System.Collections.Generic;


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
        /// Gets the number of messages in the queue as defined by <paramref name="messageCount"/>. Azure will make the message invisible to all consumers for a default of 30 seconds.
        /// </summary>
        /// <param name="messageCount">number of messages to retrieve. Must be greater than zero and a maximum of 32.</param>
        /// <param name="invisibleFor"><see cref="TimeSpan"/> that indicates how long to keep the messages invisible from other consumers for.</param>
        /// <returns><see cref="Task{CloudQueueMessage}"/></returns>
        /// <exception cref="ArgumentException">thrown if <paramref name="messageCount"/> is less than 1.</exception>
        public async Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount, TimeSpan invisibleFor)
        {
            if (messageCount < 1 || messageCount > 32)
            {
                throw new ArgumentException("must be > 0 and < = 32", nameof(messageCount));
            }

            // Creates the azure queue service client.
            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();

            // Gets a reference to the queue. Queue names must be lowercase.
            CloudQueue sampleQueue = queueClient.GetQueueReference(queueName);

            // Get the queue, creating it if it does not exist.
            await sampleQueue.CreateIfNotExistsAsync();

            var operationContext = new OperationContext();

            // Gets the set of messages from the queue. 
            // Azure will also make the messages invisble to other consumers for the value defined in invisibleFor. 
            return await sampleQueue.GetMessagesAsync(messageCount, invisibleFor, queueClient.DefaultRequestOptions, operationContext);
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
