using FluentAssertions;
using Interview.Domain.Events;
using Interview.Domain.Events.Events;
using Interview.Domain.Events.Events.Serializers;

namespace Interview.Test.Units.Events.Serializers
{
    public class JsonRoomEventSerializerTest
    {
        public static IEnumerable<object?[]> SerializeAsStringData
        {
            get
            {
                yield return new object?[] { "{}", null };

                yield return new object?[]
                {
                    """{"RoomId":"81ad40ec-c89d-11ed-ac80-463da0479b2d","Type":"Type1"}""",
                    new RoomEvent(Guid.Parse("81ad40ec-c89d-11ed-ac80-463da0479b2d"), "Type1", null)
                };

                yield return new object?[]
                {
                    """{"RoomId":"81ad40ec-c89d-11ed-ac80-463da0479b2d","Type":"Type2","Value":"Hello world"}""",
                    new RoomEvent(Guid.Parse("81ad40ec-c89d-11ed-ac80-463da0479b2d"), "Type2", "Hello world")
                };

                yield return new object?[]
                {
                    """{"RoomId":"81ad40ec-c89d-11ed-ac80-463da0479b2d","Type":"EventType3"}""",
                    new RoomEvent<RoomEventUserPayload>(Guid.Parse("81ad40ec-c89d-11ed-ac80-463da0479b2d"), "EventType3", null)
                };

                yield return new object?[]
                {
                    """{"RoomId":"81ad40ec-c89d-11ed-ac80-463da0479b2d","Type":"EventType4","Value":{"UserId":"81ad40ec-c89d-11ed-ac80-9acce9101761"}}""",
                    new RoomEvent<RoomEventUserPayload>(Guid.Parse("81ad40ec-c89d-11ed-ac80-463da0479b2d"), "EventType4", new RoomEventUserPayload(Guid.Parse("81ad40ec-c89d-11ed-ac80-9acce9101761"), null))
                };
            }
        }

        [Theory]
        [MemberData(nameof(SerializeAsStringData))]
        public void SerializeAsString(string expectedResult, IRoomEvent? @event)
        {
            var serializer = new JsonRoomEventSerializer();

            var result = serializer.SerializeAsString(@event);

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
