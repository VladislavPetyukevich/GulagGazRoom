namespace Interview.Domain.Rooms.Service.Response
{
    public class RoomPageDetail
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<RoomQuestionPageDetail>? Questions { get; set; }

        public List<RoomUserPageDetail>? Users { get; set; }
    }
}
