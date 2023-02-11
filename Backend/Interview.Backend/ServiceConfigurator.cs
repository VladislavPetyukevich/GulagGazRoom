using Interview.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Interview.Backend;

public class ServiceConfigurator
{
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public ServiceConfigurator(IHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
        
        var serviceOption = new DependencyInjectionAppServiceOption(optionsBuilder =>
        {
            if (_environment.IsDevelopment())
                optionsBuilder.UseSqlite(_configuration.GetConnectionString("sqlite"));
        });
        serviceCollection.AddAppServices(serviceOption);
    }
}