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

    /// <summary>
    /// The event is triggered when the admin turns on the gas in the room
    /// </summary>
    GasOn,

    /// <summary>
    /// The event is triggered when the admin turns off the gas in the room
    /// </summary>
    GasOff,

    /// <summary>
    /// The event is triggered when question is changed
    /// </summary>
    ChangeQuestion,

    /// <summary>
    /// The event is triggered when room question state is changed
    /// </summary>
    ChangeRoomQuestionState,

    /// <summary>
    /// The event is triggered when room question is created
    /// </summary>
    AddRoomQuestion,
}
