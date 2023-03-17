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

        public async Task<IPagedList<ReactionDetail>> GetPageAsync(
            int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var mapper = new Mapper<Reaction, ReactionDetail>(
                reaction => new ReactionDetail { Id = reaction.Id, Type = reaction.Type, });

            return await _reactionRepository.GetPageDetailedAsync(mapper, pageNumber, pageSize, cancellationToken);
        }
    }
}
