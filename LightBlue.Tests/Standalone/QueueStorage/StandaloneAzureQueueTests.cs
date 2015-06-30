﻿using System.IO;
using System.Threading.Tasks;

using AssertExLib;

using LightBlue.Standalone;

using Microsoft.WindowsAzure.Storage;

using Xunit;

namespace LightBlue.Tests.Standalone.QueueStorage
{
    public class StandaloneAzureQueueTests : StandaloneAzureTestsBase
    {
        private const string QueueName = "testqueue";

        private readonly string _queuePath;

        public StandaloneAzureQueueTests()
            : base(DirectoryType.QueueStorage)
        {
            _queuePath = Path.Combine(BasePath, QueueName);
        }

        [Fact]
        public void WillSetQueueUriOnConstructionFromQueuePath()
        {
            var queuePath = Path.Combine(BasePath, QueueName);

            var queue = new StandaloneAzureQueue(queuePath);

            Assert.Equal(QueueName, queue.Name);
        }

        [Fact]
        public void WillSetQueueUriOnConstructionFromStorageDirectoryAndQueueName()
        {
            var queue = new StandaloneAzureQueue(BasePath, QueueName);

            Assert.Equal(Path.Combine(BasePath, QueueName), queue.Uri.LocalPath);
        }

        [Fact]
        public void WillSetQueueNameOnConstructionFromQueuePath()
        {
            var queuePath = Path.Combine(BasePath, QueueName);

            var queue = new StandaloneAzureQueue(queuePath);

            Assert.Equal(queuePath, queue.Uri.LocalPath);
        }

        [Fact]
        public void WillSetQueueNameOnConstructionFromStorageDirectoryAndQueueName()
        {
            var queue = new StandaloneAzureQueue(BasePath, QueueName);

            Assert.Equal(QueueName, queue.Name);
        }
        
        [Fact]
        public void DoesNotCreateContainerDirectoryOnConstructionFromQueuePath()
        {
            new StandaloneAzureQueue(_queuePath);

            Assert.False(Directory.Exists(_queuePath));
        }

        [Fact]
        public void DoesNotCreateContainerDirectoryOnConstructionFromStorageDirectoryAndQueueName()
        {
            new StandaloneAzureQueue(BasePath, QueueName);

            Assert.False(Directory.Exists(_queuePath));
        }

        [Fact]
        public async Task WillCreateContainerPathOnCreateIfNotExists()
        {
            await new StandaloneAzureQueue(_queuePath).CreateIfNotExistsAsync();

            Assert.True(Directory.Exists(_queuePath));
        }

        [Fact]
        public async Task WillCreateContainerPathOnCreateIfNotExistsFromStorageDirectoryAndQueuePath()
        {
            await new StandaloneAzureQueue(BasePath, QueueName).CreateIfNotExistsAsync();

            Assert.True(Directory.Exists(_queuePath));
        }

        [Fact]
        public async Task CanDeleteQueue()
        {
            var queue = new StandaloneAzureQueue(BasePath, QueueName);
            await queue.CreateIfNotExistsAsync();

            await queue.DeleteAsync();

            Assert.False(Directory.Exists(Path.Combine(BasePath, QueueName)));
        }

        [Fact]
        public void WillThrowIfDeletingQueueThatDoesNotExist()
        {
            var queue = new StandaloneAzureQueue(BasePath, QueueName);

            Assert.Throws<StorageException>(() => queue.DeleteAsync());
        }

        [Fact]
        public async Task CanConditionallyDeleteQueue()
        {
            var queue = new StandaloneAzureQueue(BasePath, QueueName);
            await queue.CreateIfNotExistsAsync();

            await queue.DeleteIfExistsAsync();

            Assert.False(Directory.Exists(Path.Combine(BasePath, QueueName)));
        }

        [Fact]
        public void WillNotThrowIfConditionallyDeletingNonExistantQueue()
        {
            var queue = new StandaloneAzureQueue(BasePath, QueueName);

            Assert.DoesNotThrow(() => queue.DeleteIfExistsAsync());
        }
    }
}