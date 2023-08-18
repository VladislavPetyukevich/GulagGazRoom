namespace Interview.Backend.Responses
{
    public class MessageResponse
    {
#pragma warning disable SA1206
        public required string Message { get; init; } = string.Empty;

        public required int Code { get; init; } = 0;
#pragma warning restore SA1206
    }
}
