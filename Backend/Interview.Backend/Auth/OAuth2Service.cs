using CSharpFunctionalExtensions;

namespace Interview.Backend.Auth
{
    public class OAuth2Service
    {
        private readonly IDictionary<string, AuthorizationService> _dictionaryService;

        public OAuth2Service(IConfiguration configuration)
        {
            var configurationSections = configuration.GetSection(nameof(OAuth2Service))
                .GetChildren()
                .ToList();

            _dictionaryService = configurationSections
                .Select<IConfigurationSection, AuthorizationService>(configurator =>
                {
                    var service = new AuthorizationService();
                    configurator.Bind(service);
                    return service;
                })
                .ToDictionary(service => service.Id, service => service);
        }

        public bool HasAuthService(string serviceId)
        {
            return _dictionaryService.ContainsKey(serviceId);
        }

        public AuthorizationService GetAuthService(string serviceId)
        {
            if (_dictionaryService.TryGetValue(serviceId, out var service))
            {
                return service;
            }

            throw new ArgumentException($"Authorization service not found by id ${serviceId}");
        }
    }
}
