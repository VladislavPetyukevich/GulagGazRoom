using Interview.Domain.Repository;
using Interview.Domain.Rooms;
using Interview.Domain.Users;

namespace Interview.Domain.RoomReviews;

public class RoomReview : Entity
{
    public RoomReview(User user, Room room, SERoomReviewState state)
    {
        User = user;
        Room = room;
        SeRoomReviewState = state;
    }

    private RoomReview()
    {
    }

    public User User { get; init; } = null!;

    public Room Room { get; init; } = null!;

    public string? Review { get; set; } = string.Empty;

    public SERoomReviewState SeRoomReviewState { get; set; } = null!;
}
