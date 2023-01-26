using Interview.Domain.Users;

namespace Interview.Domain.Reactions;

public record Reaction(User FromUser, User ToUser, ReactionType Type);