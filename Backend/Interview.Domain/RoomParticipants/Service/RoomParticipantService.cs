using CSharpFunctionalExtensions;
using Interview.Domain.RoomParticipants.Records;
using Interview.Domain.Rooms;
using Interview.Domain.Users;

namespace Interview.Domain.RoomParticipants.Service;

public class RoomParticipantService
{
    private readonly IRoomParticipantRepository _roomParticipantRepository;

    private readonly IRoomRepository _roomRepository;

    private readonly IUserRepository _userRepository;

    public RoomParticipantService(
        IRoomParticipantRepository roomParticipantRepository,
        IRoomRepository roomRepository,
        IUserRepository userRepository)
    {
        _roomParticipantRepository = roomParticipantRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<RoomParticipantDetail?>> ChangeParticipantStatusAsync(
        RoomParticipantChangeStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!RoomParticipantType.TryFromName(request.UserType, out var participantType))
        {
            return Result.Failure<RoomParticipantDetail?>($"Type user not valid");
        }

        var participant = await _roomParticipantRepository.FindByRoomIdAndUserId(request.RoomId, request.UserId, cancellationToken);

        if (participant == null)
        {
            return Result.Failure<RoomParticipantDetail?>($"The user not found in the room");
        }

        participant.Type = participantType;

        await _roomParticipantRepository.UpdateAsync(participant, cancellationToken);

        return new RoomParticipantDetail
        {
            Id = participant.Id,
            RoomId = participant.Room.Id,
            UserId = participant.User.Id,
            UserType = participant.Type,
        };
    }
}
