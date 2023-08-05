using FluentAssertions;
using Interview.Domain.Questions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Domain.Users.Service;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Interview.Test.Integrations;

public class AppDbContextTest
{
    [Fact(DisplayName = "AppDbContext should update the update date and the entity creation date when saving")]
    public async Task DbContext_Should_Update_Create_And_Update_Dates()
    {
        var clock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(clock);

        var room = new Room("Test room", "1234")
        {
            Questions = new List<RoomQuestion>
            {
                new RoomQuestion
                {
                    Question = new Question("Value 1"),
                    State = RoomQuestionState.Active,
                }
            }
        };
        appDbContext.Add(room);
        appDbContext.SaveChanges();
        
        room.Id.Should().NotBe(Guid.Empty);
        room.CreateDate.Should().Be(clock.UtcNow.DateTime);
        room.UpdateDate.Should().Be(clock.UtcNow.DateTime);
        
        room.Questions[0].Id.Should().NotBe(Guid.Empty);
        room.Questions[0].CreateDate.Should().Be(clock.UtcNow.DateTime);
        room.Questions[0].UpdateDate.Should().Be(clock.UtcNow.DateTime);
        
        room.Questions[0].Question!.Id.Should().NotBe(Guid.Empty);
        room.Questions[0].Question!.CreateDate.Should().Be(clock.UtcNow.DateTime);
        room.Questions[0].Question!.UpdateDate.Should().Be(clock.UtcNow.DateTime);
    }
}
