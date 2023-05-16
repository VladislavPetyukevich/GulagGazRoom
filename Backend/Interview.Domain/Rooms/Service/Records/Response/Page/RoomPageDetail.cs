using Interview.Domain.Rooms.Service.Records.Response.Detail;

namespace Interview.Domain.Rooms.Service.Records.Response.Page;

public class RoomPageDetail
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? TwitchChannel { get; set; }

    public List<RoomQuestionDetail>? Questions { get; set; }

    public List<RoomUserDetail>? Users { get; set; }
}
