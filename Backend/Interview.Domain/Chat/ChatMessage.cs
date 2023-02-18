using Interview.Domain.Users;

namespace Interview.Domain.Chat;

public record ChatMessage(User User, string Message);
