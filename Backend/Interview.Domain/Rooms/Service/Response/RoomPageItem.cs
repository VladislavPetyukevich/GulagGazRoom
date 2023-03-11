namespace Interview.Domain.Rooms.Service
{
    public class RoomPageItem
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public List<RoomQuestionPageItem>? Questions { get; set; }

        public List<RoomUserPageItem>? Users { get; set; }
    }
}
