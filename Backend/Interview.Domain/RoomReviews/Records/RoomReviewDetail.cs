namespace Interview.Domain.RoomReviews.Records
{
    public class RoomReviewDetail
    {
        public required Guid? Id { get; init; }

        public required Guid UserId { get; init; }

        public required Guid RoomId { get; init; }

        public required string Review { get; init; } = string.Empty;

        public required EVRoomReviewState State { get; init; }
    }
}
