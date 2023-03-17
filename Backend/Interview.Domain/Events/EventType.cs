namespace Interview.Domain.Events;

/// <summary>
/// Event types.
/// </summary>
public enum EventType
{
    /// <summary>
    /// Unknown value
    /// </summary>
    Unknown,

    /// <summary>
    /// The event is triggered during a user reaction (like)
    /// </summary>
    ReactionLike,

    /// <summary>
    /// The event is triggered during a user reaction (dislike)
    /// </summary>
    ReactionDislike,

    /// <summary>
    /// The event is triggered when a new message appears in twitch
    /// </summary>
    ChatMessage,
}
