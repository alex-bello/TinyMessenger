using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyMessenger.Tests.TestData;

namespace TinyMessenger.Tests
{
    [TestClass]
    public class TinyMessageSubscriptionTokenTests
    {
        [TestMethod]
        public void Dispose_WithValidHubReference_UnregistersWithHub()
        {
            var messengerMock = new Moq.Mock<ITinyMessengerHub>();
            messengerMock.Setup((messenger) => messenger.Unsubscribe<TestMessage>(Moq.It.IsAny<TinyMessageSubscriptionToken>())).Verifiable();
            var token = new TinyMessageSubscriptionToken(messengerMock.Object, typeof(TestMessage));

            token.Dispose();

            messengerMock.VerifyAll();
        }

        [TestMethod]
        public void Dispose_WithInvalidHubReference_DoesNotThrow()
        {
            // Assign 
            
            var messenger = new TinyMessengerHub();
            var token = new TinyMessageSubscriptionToken(messenger, typeof(TestMessage));

            var weakTokenRef = new WeakReference(token);
            GC.Collect();
            GC.WaitForFullGCComplete(2000);

            token.Dispose();

            Assert.IsTrue(weakTokenRef.IsAlive);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullHub_ThrowsArgumentNullException()
        {
            var messenger = UtilityMethods.GetMessenger();

            var token = new TinyMessageSubscriptionToken(null, typeof(ITinyMessage));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ctor_InvalidMessageType_ThrowsArgumentOutOfRangeException()
        {
            var messenger = UtilityMethods.GetMessenger();

            var token = new TinyMessageSubscriptionToken(messenger, typeof(object));
        }

        [TestMethod]
        public void Ctor_ValidHubAndMessageType_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            var token = new TinyMessageSubscriptionToken(messenger, typeof(TestMessage));

            Assert.IsNotNull(token);
        }
    }
}
