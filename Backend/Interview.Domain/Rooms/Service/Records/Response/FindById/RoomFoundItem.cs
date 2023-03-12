namespace Interview.Domain.Rooms.Service.Records.Response.FindById
{
    public class RoomFoundItem
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<RoomQuestionFoundItem>? Questions { get; set; }

        public List<RoomUserFoundItem>? Users { get; set; }
    }
}
