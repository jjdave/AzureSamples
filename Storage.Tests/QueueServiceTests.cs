namespace QueueTests
{
    using System;
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
