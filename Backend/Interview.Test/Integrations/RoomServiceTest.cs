using FluentAssertions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Infrastructure.Questions;
using Interview.Infrastructure.RoomQuestionReactions;
using Interview.Infrastructure.Rooms;
using Interview.Infrastructure.Users;

namespace Interview.Test.Integrations;

public class RoomServiceTest
{
    private const string DefaultRoomName = "Test_Room";

    [Fact(DisplayName = "Patch update room with request name not null")]
    public async Task PatchUpdateRoomWithRequestNameIsNotNull()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        var savedRoom = new Room(DefaultRoomName, DefaultRoomName);

        appDbContext.Rooms.Add(savedRoom);

        await appDbContext.SaveChangesAsync();

        var roomRepository = new RoomRepository(appDbContext);
        var roomService = new RoomService(roomRepository, new QuestionRepository(appDbContext), new UserRepository(appDbContext), new EmptyRoomEventDispatcher(), new RoomQuestionReactionRepository(appDbContext));

        var roomPatchUpdateRequest = new RoomPatchUpdateRequest { Name = "New_Value_Name_Room", TwitchChannel = "TwitchCH" };

        var patchUpdate = await roomService.PatchUpdate(savedRoom.Id, roomPatchUpdateRequest);

        Assert.True(patchUpdate.IsSuccess);

        var foundedRoom = await roomRepository.FindByIdAsync(savedRoom.Id);

        foundedRoom?.Name.Should().BeEquivalentTo(roomPatchUpdateRequest.Name);
    }
}
