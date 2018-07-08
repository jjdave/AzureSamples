namespace Storage
{
    using Microsoft.WindowsAzure.Storage.Queue;
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