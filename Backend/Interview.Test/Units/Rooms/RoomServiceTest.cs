using Interview.Domain.Events;
using Interview.Domain.Questions;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Users;
using Moq;

namespace Interview.Test.Units.Rooms;

public class RoomServiceTest
{
    private readonly Mock<IRoomRepository> _roomRepository;

    private readonly RoomService _roomService;

    public RoomServiceTest()
    {
        _roomRepository = new Mock<IRoomRepository>();
        var questionRepository = new Mock<IQuestionRepository>();
        var roomQuestionRepository = new Mock<IRoomQuestionRepository>();
        var userRepository = new Mock<IUserRepository>();
        var eventDispatcher = new Mock<IRoomEventDispatcher>();
        var roomQuestionReactionRepository = new Mock<IRoomQuestionReactionRepository>();

        _roomService = new RoomService(_roomRepository.Object, roomQuestionRepository.Object, questionRepository.Object, userRepository.Object, eventDispatcher.Object, roomQuestionReactionRepository.Object);
    }

    [Fact(DisplayName = "Patch update of room when request name is null")]
    public async void PatchUpdateRoomWhenRequestNameIsNull()
    {
        var roomPatchUpdateRequest = new RoomUpdateRequest { Name = null };

        var roomPatchUpdate =
            await _roomService.UpdateAsync(Guid.NewGuid(), roomPatchUpdateRequest);

        Assert.True(roomPatchUpdate.IsFailure);
        Assert.Equal("Room name should not be empty", roomPatchUpdate.Error);
    }

    [Fact(DisplayName = "Patch update of room when room not found")]
    public async void PatchUpdateRoomWhenRoomNotFound()
    {
        var roomPatchUpdateRequest = new RoomUpdateRequest { Name = "new_value_name_room", TwitchChannel = "TwitchCH" };
        var roomId = Guid.NewGuid();

        _roomRepository.Setup(repository => repository.FindByIdAsync(roomId, default))
            .ReturnsAsync((Room?)null);

        var roomPatchUpdate = await _roomService.UpdateAsync(roomId, roomPatchUpdateRequest);

        Assert.True(roomPatchUpdate.IsFailure);
        Assert.Equal($"Not found room with id [{roomId}]", roomPatchUpdate.Error);
    }

    [Fact(DisplayName = "Patch update of room when room id is null")]
    public async void PatchUpdateRoomWhenRoomIdIsNull()
    {
        var roomPatchUpdateRequest = new RoomUpdateRequest { Name = "new_value_name_room", TwitchChannel = "TestCH" };

        var roomPatchUpdate = await _roomService.UpdateAsync(null, roomPatchUpdateRequest);

        Assert.True(roomPatchUpdate.IsFailure);
        Assert.Equal("Room id should not be null [id]", roomPatchUpdate.Error);
    }
}
