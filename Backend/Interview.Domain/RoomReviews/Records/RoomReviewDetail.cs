namespace Interview.Domain.RoomReviews.Records
{
    public class RoomReviewDetail
    {
        public Guid? Id { get; init; }

        public Guid? UserId { get; init; }

        public Guid? RoomId { get; init; }

        public string? Review { get; init; } = string.Empty;

        public string? State { get; init; }
    }
}
