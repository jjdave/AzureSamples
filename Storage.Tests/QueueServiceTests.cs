namespace QueueTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Storage;

    [TestClass]
    public class QueueServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifyQueueService_Creation_ThrowsException_With_ConnectionStringNull()
        {
            // Arrange and Act
            IQueueService queueService = new QueueService(null, "queue");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifyQueueService_Creation_ThrowsException_With_QueueEmptyString()
        {
            // Arrange and Act
            IQueueService queueService = new QueueService("conn", string.Empty);
        }

        [TestMethod]
        public void VerifyQueueService_Creation_Returns_Valid_Instance()
        {
            // Arrange
            IQueueService queueService = new QueueService("UseDevelopmentStorage=true", "testqueue");

            // Assert
            Assert.IsNotNull(queueService);
        }

        [TestMethod]
        public async Task VerifyQueueService_AddMessage_GetMessage_DeleteMessage()
        {
            // Arrange
            IQueueService queueService = new QueueService("UseDevelopmentStorage=true", "testqueue");

            // Act
            await queueService.AddMessageAsync("test");
            var message = await queueService.GetMessageAsync();

            // Assert
            Assert.IsNotNull(queueService);
            Assert.IsNotNull(message);

            // Cleanup 
            await queueService.DeleteMessageAsync(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task VerifyQueueService_Get0Messages()
        {
            // Arrange
            IQueueService queueService = new QueueService("UseDevelopmentStorage=true", "testqueue");

            // Act
            var messages = await queueService.GetMessagesAsync(0, TimeSpan.FromHours(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task VerifyQueueService_Get35Messages()
        {
            // Arrange
            IQueueService queueService = new QueueService("UseDevelopmentStorage=true", "testqueue");

            // Act
            var messages = await queueService.GetMessagesAsync(35, TimeSpan.FromHours(1));
        }

        [TestMethod]
        public async Task VerifyQueueService_Add10Messages_Get5Messages_DeleteMessages()
        {
            // Arrange
            IQueueService queueService = new QueueService("UseDevelopmentStorage=true", "testqueue");

            // Act
            for (var messageCount = 0; messageCount < 10; messageCount++)
            {
                await queueService.AddMessageAsync("test "+ messageCount.ToString());
            }

            var messages = await queueService.GetMessagesAsync(5, TimeSpan.FromHours(1));

            // Assert
            Assert.IsNotNull(queueService);
            Assert.IsNotNull(messages);
            Assert.AreEqual(5, messages.Count());

            // Cleanup 
            foreach (var message in messages)
            {
                await queueService.DeleteMessageAsync(message);
            }
        }

        [TestMethod]
        public async Task VerifyQueueService_AddMessage_PeekMessage_DeleteMessage()
        {
            // Arrange
            IQueueService queueService = new QueueService("UseDevelopmentStorage=true", "testqueue");

            // Act
            await queueService.AddMessageAsync("test");
            var message = await queueService.PeekMessageAsync();

            // Assert
            Assert.IsNotNull(queueService);
            Assert.IsNotNull(message);

            // Cleanup
            message = await queueService.GetMessageAsync();
            Assert.IsNotNull(message, "should not be null, in cleanup stage");
            await queueService.DeleteMessageAsync(message);
        }
    }
}
