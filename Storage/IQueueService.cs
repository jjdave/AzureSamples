namespace Storage
{
    using Microsoft.WindowsAzure.Storage.Queue;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Queue Service Interface
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Adds a message to the queue
        /// </summary>
        /// <param name="message">a valid message string.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, whitespace or greater than 65536 bytes.</exception>
        Task AddMessageAsync(string message);

        /// <summary>
        /// Gets the number of messages in the queue as defined by <paramref name="messageCount"/>. Azure will make the message invisible to all consumers for a default of 30 seconds.
        /// </summary>
        /// <param name="messageCount">number of messages to retrieve. Must be greater than zero and a maximum of 32.</param>
        /// <param name="invisibleFor"><see cref="TimeSpan"/> that indicates how long to keep the messages invisible from other consumers for.</param>
        /// <returns><see cref="Task{CloudQueueMessage}"/></returns>
        /// <exception cref="ArgumentException">thrown if <paramref name="messageCount"/> is less than 1.</exception>
        Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount, TimeSpan invisibleFor);

        /// <summary>
        /// Peeks at the next message in the queue. Peeking keeps the message visible to all consumers.
        /// </summary>
        /// <returns>null if there is no message, otherwise a valid message.</returns>
        Task<CloudQueueMessage> PeekMessageAsync();

        /// <summary>
        /// Gets the next message in the queue. Azure will make the message invisible to all consumers for a default of 30 seconds.
        /// </summary>
        /// <returns>null if there is no message, otherwise a valid message.</returns>
        Task<CloudQueueMessage> GetMessageAsync();

        /// <summary>
        /// Deletes the given <paramref name="message"/> from the queue.
        /// </summary>
        /// <param name="message">a valid queue message.</param>
        /// <returns><see cref="Task"/></returns>
        Task DeleteMessageAsync(CloudQueueMessage message);
    }
}