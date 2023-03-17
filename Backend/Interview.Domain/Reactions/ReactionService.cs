using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Interview.Domain.Reactions.Records;
using Interview.Domain.Repository;
using X.PagedList;

namespace Interview.Domain.Reactions
{
    public class ReactionService
    {
        private readonly IReactionRepository _reactionRepository;

        public ReactionService(IReactionRepository reactionRepository)
        {
            _reactionRepository = reactionRepository;
        }

        public async Task<IPagedList<ReactionDetail>> GetPageAsync(int pageNumber, int pageSize)
        {
            return await _reactionRepository.GetPageDetailedAsync(
                new Mapper<Reaction, ReactionDetail>(
                    reaction => new ReactionDetail { Id = reaction.Id, Type = reaction.Type, }),
                pageNumber,
                pageSize);
        }
    }
}
