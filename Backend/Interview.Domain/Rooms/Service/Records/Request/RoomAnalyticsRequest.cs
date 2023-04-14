namespace Interview.Domain.Rooms.Service.Records.Request
{
    public class RoomAnalyticsRequest
    {
        public Guid RoomId { get; }

        public ICollection<Guid> SpecificUserIds { get; }

        public RoomAnalyticsRequest(Guid roomId)
            : this(roomId, Array.Empty<Guid>())
        {
        }

        public RoomAnalyticsRequest(Guid roomId, ICollection<Guid> specificUserIds)
        {
            RoomId = roomId;
            SpecificUserIds = specificUserIds;
        }
    }
}
