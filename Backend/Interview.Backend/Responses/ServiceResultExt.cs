using Interview.Domain.ServiceResults;
using Interview.Domain.ServiceResults.Success;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Responses
{
    public static class ServiceResultExt
    {
        public static ActionResult<T> ToActionResult<T>(this ServiceResult<T> self)
        {
            return self.Match<ActionResult<T>>(
                ok => new OkObjectResult(ok.Value),
                create => new CreatedResult(string.Empty, create.Value),
                noContent => new NoContentResult());
        }

        public static ActionResult<T> ToActionResult<T>(this ServiceResult self) => self.ToActionResult();

        public static ActionResult ToActionResult(this ServiceResult self) => new OkObjectResult(string.Empty);
    }
}
