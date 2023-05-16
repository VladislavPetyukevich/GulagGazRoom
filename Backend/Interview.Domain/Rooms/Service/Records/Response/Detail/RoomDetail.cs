namespace Interview.Domain.Rooms.Service.Records.Response.Detail;

public class RoomDetail
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? TwitchChannel { get; set; }

    public List<RoomQuestionDetail>? Questions { get; set; }

    public List<RoomUserDetail>? Users { get; set; }

    public required EVRoomStatus RoomStatus { get; init; }

    public required bool EnableCodeEditor { get; set; }

    public required string? CodeEditorContent { get; set; }
}
