using CSharpFunctionalExtensions;
using Interview.Domain.Errors;
using Interview.Domain.ServiceResults;
using Interview.Domain.ServiceResults.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Responses
{
    public static class ResultExt
    {
        public static async Task<ActionResult<T>> ToResponseAsync<T>(this Task<Result<ServiceResult<T>, ServiceError>> self)
        {
            var result = await self;
            return result.ToResponse();
        }

        public static ActionResult<T> ToResponse<T>(this Result<ServiceResult<T>, ServiceError> self)
        {
            return self.Match(
                success => success.ToActionResult(),
                error => error.ToActionResult<T>());
        }
    }
}
