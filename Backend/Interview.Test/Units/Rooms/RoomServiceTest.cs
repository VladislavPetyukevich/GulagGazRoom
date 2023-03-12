using Interview.Domain.Questions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Response;
using Moq;

namespace Interview.Test.Units.Rooms
{
    public class RoomServiceTest
    {
        private readonly Mock<IRoomRepository> _roomRepository;

        private readonly Mock<IQuestionRepository> _questionRepository;

        private readonly RoomService _roomService;

        public RoomServiceTest()
        {
            _roomRepository = new Mock<IRoomRepository>();
            _questionRepository = new Mock<IQuestionRepository>();

            _roomService = new RoomService(_roomRepository.Object, _questionRepository.Object);
        }

        [Fact(DisplayName = "Patch update of room when request name is null")]
        public async void PatchUpdateRoomWhenRequestNameIsNull()
        {
            var roomPatchUpdateRequest = new RoomPatchUpdateRequest { Name = null };

            var roomPatchUpdate =
                await _roomService.PatchUpdate(Guid.NewGuid(), roomPatchUpdateRequest);

            Assert.True(roomPatchUpdate.IsFailure);
            Assert.Equal("Room name should not be empty", roomPatchUpdate.Error);
        }

        [Fact(DisplayName = "Patch update of room when room not found")]
        public async void PatchUpdateRoomWhenRoomNotFound()
        {
            var roomPatchUpdateRequest = new RoomPatchUpdateRequest { Name = "new_value_name_room" };
            var roomId = Guid.NewGuid();

            _roomRepository.Setup(repository => repository.FindByIdAsync(roomId, default))
                .ReturnsAsync((Room?)null);

            var roomPatchUpdate = await _roomService.PatchUpdate(roomId, roomPatchUpdateRequest);

            Assert.True(roomPatchUpdate.IsFailure);
            Assert.Equal($"Not found room with id [{roomId}]", roomPatchUpdate.Error);
        }

        [Fact(DisplayName = "Patch update of room when room id is null")]
        public async void PatchUpdateRoomWhenRoomIdIsNull()
        {
            var roomPatchUpdateRequest = new RoomPatchUpdateRequest { Name = "new_value_name_room" };

            var roomPatchUpdate = await _roomService.PatchUpdate(null, roomPatchUpdateRequest);

            Assert.True(roomPatchUpdate.IsFailure);
            Assert.Equal("Room id should not be null [id]", roomPatchUpdate.Error);
        }
    }
}
