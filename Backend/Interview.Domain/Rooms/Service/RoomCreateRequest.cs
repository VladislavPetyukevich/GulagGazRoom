namespace Interview.Domain.Rooms.Service
{
    public sealed class RoomCreateRequest
    {
        public string Name { get; set; } = string.Empty;

        public HashSet<Guid> Questions { get; set; } = new();
    }
}
