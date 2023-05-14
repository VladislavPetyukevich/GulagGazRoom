namespace Interview.Domain.RoomReviews.Records;

public class RoomReviewCreateRequest
{
    public Guid RoomId { get; set; }

    public string? Review { get; set; }
}
