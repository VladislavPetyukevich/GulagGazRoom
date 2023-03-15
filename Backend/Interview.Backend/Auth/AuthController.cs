using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Auth
{
    [ApiController]
    [Route("oauth")]
    public class AuthController : ControllerBase
    {
        private readonly OAuth2Service _oAuth2Dispatcher;

        public AuthController(OAuth2Service oAuth2Dispatcher)
        {
            _oAuth2Dispatcher = oAuth2Dispatcher;
        }

        [HttpGet("/login/{scheme}")]
        public IResult SignIn(string scheme, [FromQuery] string redirectUri)
        {
            if (!_oAuth2Dispatcher.HasAuthService(scheme))
            {
                return Results.BadRequest($"Not found service authorization with id ${scheme}");
            }

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = redirectUri,
            };

            var signIn = Results.Challenge(authenticationProperties, authenticationSchemes: new List<string> { scheme });

            return signIn;
        }
    }
}
