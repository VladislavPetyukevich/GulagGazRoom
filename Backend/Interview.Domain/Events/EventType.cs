namespace Interview.Domain.Events;

/// <summary>
/// Event types.
/// </summary>
public enum EventType
{
    /// <summary>
    /// The event is triggered during a user reaction
    /// </summary>
    Reaction,

    /// <summary>
    /// The event is triggered when a new message appears in twitch
    /// </summary>
    ChatMessage,
}
