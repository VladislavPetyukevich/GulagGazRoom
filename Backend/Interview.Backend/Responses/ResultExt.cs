using CSharpFunctionalExtensions;
using Interview.Domain.Errors;
using Interview.Domain.ServiceResults;
using Interview.Domain.ServiceResults.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Responses
{
    public static class ResultExt
    {
        public static async Task<ActionResult<TObj>> ToResponseAsync<TObj>(this Task<Result<ServiceResult<TObj>, ServiceError>> self)
        {
            var result = await self;
            return result.ToResponse();
        }

        public static ActionResult<TObj> ToResponse<TObj>(this Result<ServiceResult<TObj>, ServiceError> self)
        {
            return self.Match(
                success =>
                {
                    return success.Match<ActionResult<TObj>>(
                        ok => new OkObjectResult(ok.Value),
                        create => new CreatedResult(string.Empty, create.Value));
                },
                error =>
                {
                    return error.Match<ActionResult<TObj>>(
                        appError => new BadRequestObjectResult(appError.Message),
                        notFoundError => new NotFoundObjectResult(notFoundError.Message));
                });
        }

        public static async Task<IActionResult> ToResponseAsync(this Task<Result<ServiceResult, ServiceError>> self)
        {
            var result = await self;
            return result.ToResponse();
        }

        public static IActionResult ToResponse(this Result<ServiceResult, ServiceError> self)
        {
            return self.Match(
                success => new OkObjectResult(string.Empty),
                error =>
                {
                    return error.Match<IActionResult>(
                        appError => new BadRequestObjectResult(appError.Message),
                        notFoundError => new NotFoundObjectResult(notFoundError.Message));
                });
        }

        public static async Task<ActionResult<T>> ToResponseAsync<T>(this Task<Result<ServiceResult, ServiceError>> self)
        {
            var result = await self;
            return result.ToResponse<T>();
        }

        public static ActionResult<T> ToResponse<T>(this Result<ServiceResult, ServiceError> self)
        {
            return self.Match(
                success => new OkObjectResult(string.Empty),
                error =>
                {
                    return error.Match<ActionResult<T>>(
                        appError => new BadRequestObjectResult(appError.Message),
                        notFoundError => new NotFoundObjectResult(notFoundError.Message));
                });
        }
    }
}
