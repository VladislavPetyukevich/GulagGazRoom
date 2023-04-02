using Interview.Domain.ServiceResults;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Responses
{
    public static class ServiceResultExt
    {
        public static ActionResult<T> ToActionResult<T>(this ServiceResult<T> self)
        {
            return self.Match<ActionResult<T>>(
                ok => new OkObjectResult(ok.Value),
                create => new CreatedResult(string.Empty, create.Value));
        }
    }
}
