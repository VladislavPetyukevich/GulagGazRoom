namespace Interview.Domain.RoomParticipants.Records
{
    public class RoomParticipantDetail
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }

        public Guid UserId { get; set; }

        public RoomParticipantType UserType { get; set; }
    }
}
