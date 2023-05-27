namespace Interview.Domain.Rooms.Service.Records.Request
{
    public class RoomAnalyticsRequest
    {
        public Guid RoomId { get; }

        public RoomAnalyticsRequest(Guid roomId)
        {
            RoomId = roomId;
        }
    }
}
