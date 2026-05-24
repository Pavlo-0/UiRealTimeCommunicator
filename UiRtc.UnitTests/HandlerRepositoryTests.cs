using UiRtc.Domain.Repository;
using UiRtc.Domain.Repository.Records;

namespace UiRtc.UnitTests
{
    [TestClass]
    public sealed class HandlerRepositoryTests
    {
        [TestMethod]
        public void Add_AllowsMultipleHandlersWithSameSignature()
        {
            var repository = new HandlerRepository();
            var hubName = UniqueHubName();

            repository.Add(CreateRecord(hubName, "Update", typeof(string)));
            repository.Add(CreateRecord(hubName, "Update", typeof(string)));

            Assert.AreEqual(2, repository.Get(hubName, "Update").Count());
            Assert.AreEqual(1, repository.GetBuilderList(hubName).Count());
        }

        [TestMethod]
        public void Add_RejectsHandlersWithSameNameAndDifferentPayloadTypes()
        {
            var repository = new HandlerRepository();
            var hubName = UniqueHubName();

            repository.Add(CreateRecord(hubName, "Update", typeof(string)));

            try
            {
                repository.Add(CreateRecord(hubName, "Update", typeof(int)));
                Assert.Fail("Expected ambiguous handler registration to throw.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void Add_RejectsHandlersWithSameNameWhenOnlyOneHasPayload()
        {
            var repository = new HandlerRepository();
            var hubName = UniqueHubName();

            repository.Add(CreateRecord(hubName, "Update", null));

            try
            {
                repository.Add(CreateRecord(hubName, "Update", typeof(string)));
                Assert.Fail("Expected ambiguous handler registration to throw.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        private static HandlerRecord CreateRecord(string hubName, string methodName, Type? genericModel) =>
            new(
                hubName,
                methodName,
                typeof(object),
                typeof(object),
                typeof(object),
                false,
                genericModel);

        private static string UniqueHubName() => $"Hub{Guid.NewGuid():N}";
    }
}
