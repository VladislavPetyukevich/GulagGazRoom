using Interview.Domain.ServiceResults.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Responses
{
    public static class ServiceErrorExt
    {
        public static ActionResult<T> ToActionResult<T>(this ServiceError self)
        {
            return self.ToActionResult();
        }

        public static ActionResult ToActionResult(this ServiceError self)
        {
            return self.Match<ActionResult>(
                appError => new BadRequestObjectResult(new MessageResponse
                {
                    Message = appError.Message,
                }),
                notFoundError => new NotFoundObjectResult(new MessageResponse
                {
                    Message = notFoundError.Message,
                }),
                accessDenied => new UnauthorizedObjectResult(new MessageResponse
                {
                    Message = accessDenied.Message,
                }));
        }
    }
}
