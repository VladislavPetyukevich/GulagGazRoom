using System.Security.Claims;
using AspNet.Security.OAuth.Twitch;
using CSharpFunctionalExtensions;

namespace Interview.Backend.Auth
{
    public class UserClaimService
    {
        /// <summary>
        /// Parse claims to user object of type <see cref="UserClaim"/>.
        /// </summary>
        /// <param name="claims">List with claim is a statement about an entity by an Issuer.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns><see cref="Task{TResult}"/> with the result of user data or an error message.</returns>
        public Task<Result<UserClaim?>> ParseClaimsAsync(
            IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default)
        {
            var claimList = claims.ToList();

            var externalId = claimList.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier);
            var nickname = claimList.FirstOrDefault(e => e.Type == ClaimTypes.Name);

            var avatar = claimList
                .FirstOrDefault(e => e.Type == TwitchAuthenticationConstants.Claims.ProfileImageUrl);

            if (externalId == null || nickname == null || avatar == null)
            {
                return Task.FromResult(Result.Failure<UserClaim?>($"Not found users fields"));
            }

            var id = claimList.FirstOrDefault(e => e.Type == UserClaimConstants.UserId);

            if (id == null)
            {
                return Task.FromResult(Result.Failure<UserClaim?>($"User id not found"));
            }

            if (!Guid.TryParse(id.Value, out var typedId))
            {
                return Task.FromResult(Result.Failure<UserClaim?>($"User id is invalid"));
            }

            return Task.FromResult<Result<UserClaim?>>(new UserClaim
            {
                Identity = typedId,
                Nickname = nickname.Value,
                Avatar = avatar.Value,
                Roles = claimList.Where(claim => claim.Type == ClaimTypes.Role)
                    .Select(claim => claim.Value)
                    .ToList(),
                TwitchIdentity = externalId.Value,
            })
                .WaitAsync(cancellationToken);
        }
    }
}
