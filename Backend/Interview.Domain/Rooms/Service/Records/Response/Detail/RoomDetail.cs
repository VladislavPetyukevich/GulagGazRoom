namespace Interview.Domain.Rooms.Service.Records.Response.Page
{
    public class RoomDetail
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<RoomQuestionDetail>? Questions { get; set; }

        public List<RoomUserDetail>? Users { get; set; }
    }
}
