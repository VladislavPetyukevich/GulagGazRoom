using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Auth;

[ApiController]
[Route("oauth")]
public class AuthController : ControllerBase
{
    private readonly OAuthServiceDispatcher _oAuthDispatcher;

    public AuthController(OAuthServiceDispatcher oAuthDispatcher)
    {
        _oAuthDispatcher = oAuthDispatcher;
    }

    [HttpGet("/login/{scheme}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(string), 400)]
    public IResult SignIn(string scheme, [FromQuery] string redirectUri)
    {
        if (!_oAuthDispatcher.HasAuthService(scheme))
        {
            return Results.BadRequest($"Not found service authorization with id ${scheme}");
        }

        var authorizationService = _oAuthDispatcher.GetAuthService(scheme);

        if (!authorizationService.AvailableLoginRedirects.Contains(redirectUri))
        {
            return Results.BadRequest($"Redirect link {redirectUri} is not available");
        }

        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
        };

        var signIn = Results.Challenge(authenticationProperties, authenticationSchemes: new List<string> { scheme });

        return signIn;
    }
}
