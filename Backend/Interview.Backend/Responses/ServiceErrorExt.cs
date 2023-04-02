using Interview.Domain.ServiceResults.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Responses
{
    public static class ServiceErrorExt
    {
        public static ActionResult<T> ToActionResult<T>(this ServiceError self)
        {
            return self.Match<ActionResult<T>>(
                appError => new BadRequestObjectResult(new MessageResponse
                {
                    Message = appError.Message,
                }),
                notFoundError => new NotFoundObjectResult(new MessageResponse
                {
                    Message = notFoundError.Message,
                }));
        }
    }
}
