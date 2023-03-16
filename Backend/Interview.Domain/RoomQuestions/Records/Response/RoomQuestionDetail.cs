namespace Interview.Domain.RoomQuestions.Records.Response.Response
{
    public class RoomQuestionDetail
    {
        public Guid RoomId { get; set; }

        public Guid QuestionId { get; set; }

        public RoomQuestionState State { get; set; }
    }
}
