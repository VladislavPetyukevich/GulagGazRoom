namespace Interview.Domain.RoomQuestions.Records.Response
{
    public class RoomQuestionChangeActiveRequest
    {
        public Guid RoomId { get; set; }

        public Guid QuestionId { get; set; }
    }
}
