using Interview.Domain.Repository;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms.Service.Records.Response.Detail;

namespace Interview.Domain.Rooms.Service.Records.Response.RoomStates;

public class RoomState
{
    public static readonly Mapper<Room, RoomState> Mapper = new(room => new RoomState
    {
        Id = room.Id,
        Name = room.Name,
        ActiveQuestion = room.Questions.Select(q => new RoomStateQuestion
        {
            Id = q.Id,
            Value = q.Question!.Value,
            State = q.State!,
        }).FirstOrDefault(q => q.State == RoomQuestionState.Active),
    });

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public RoomStateQuestion? ActiveQuestion { get; set; }

    public int LikeCount { get; set; }

    public int DislikeCount { get; set; }
}
