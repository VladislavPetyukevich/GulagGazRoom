namespace Interview.Domain.RoomParticipants.Records.Response
{
    public class RoomParticipantDetail
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }

        public Guid UserId { get; set; }

        public string UserType { get; set; }
    }
}
