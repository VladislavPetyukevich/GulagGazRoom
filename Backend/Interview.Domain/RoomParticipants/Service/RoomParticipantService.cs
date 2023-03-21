using CSharpFunctionalExtensions;
using Interview.Domain.RoomParticipants.Records;
using Interview.Domain.RoomParticipants.Records.Request;
using Interview.Domain.RoomParticipants.Records.Response;
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
            UserType = participant.Type.Name,
        };
    }

    /// <summary>
    /// Adding a new member to a room.
    /// </summary>
    /// <param name="request">Data for adding a new participant to the room.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Data of the new room participant.</returns>
    public async Task<Result<RoomParticipantDetail?>> CreateParticipantAsync(
        RoomParticipantCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!RoomParticipantType.TryFromName(request.Type, out var participantType))
        {
            return Result.Failure<RoomParticipantDetail?>($"Invalid participant type");
        }

        var existingParticipant = await _roomParticipantRepository.IsExistsByRoomIdAndUserIdAsync(
            request.RoomId, request.UserId, cancellationToken);

        if (existingParticipant)
        {
            return Result.Failure<RoomParticipantDetail?>($"Participant already exists. " +
                                                          $"Room id = {request.RoomId} User id = {request.UserId}");
        }

        var room = await _roomRepository.FindByIdAsync(request.RoomId, cancellationToken);

        if (room == null)
        {
            return Result.Failure<RoomParticipantDetail?>($"Room not found with id = {request.RoomId}");
        }

        var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure<RoomParticipantDetail?>($"User not found with id = {request.UserId}");
        }

        var roomParticipant = new RoomParticipant(user, room, participantType);

        await _roomParticipantRepository.CreateAsync(roomParticipant, cancellationToken);

        return new RoomParticipantDetail
        {
            Id = roomParticipant.Id,
            RoomId = roomParticipant.Room.Id,
            UserId = roomParticipant.User.Id,
            UserType = roomParticipant.Type.Name,
        };
    }
}
