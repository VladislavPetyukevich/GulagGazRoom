using Ardalis.SmartEnum;

namespace Interview.Domain.RoomReviews
{
    public class SERoomReviewState : SmartEnum<SERoomReviewState>
    {
        public static readonly SERoomReviewState Open = new("Open", 0);
        public static readonly SERoomReviewState Closed = new("Closed", 0);

        public SERoomReviewState(string name, int value)
            : base(name, value)
        {
        }
    }
}
