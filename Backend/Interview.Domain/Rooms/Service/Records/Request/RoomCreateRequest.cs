namespace Interview.Domain.Rooms.Service.Records.Request
{
    public sealed class RoomCreateRequest
    {
        public string Name { get; set; } = string.Empty;

        public string TwitchChannel { get; set; } = string.Empty;

        public HashSet<Guid> Questions { get; set; } = new();

        public HashSet<Guid> Users { get; set; } = new();
    }
}
