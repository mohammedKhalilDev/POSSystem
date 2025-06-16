using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POSSystem.Core.Repository;
using POSSystem.Infrastructure.ApplicationContext;
using POSSystem.Infrastructure.Email;
using POSSystem.Infrastructure.RabbitMQ.Configuration;
using POSSystem.Infrastructure.RabbitMQ.Interfaces;
using POSSystem.Infrastructure.RabbitMQ.Service;
using POSSystem.Infrastructure.Repository;
using System.Collections;

namespace HRSystem.Infrastructure
{
    public static class InfrastructureInjector
    {
        public static IServiceCollection InjectInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    config.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(config.GetConnectionString("DefaultConnection"))
                ));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            var rabbitConfig = config.GetSection("RabbitMQ").Get<RabbitMQConfiguration>();
            services.AddSingleton(rabbitConfig);

            services.AddScoped<IMessagePublisher, RabbitMQPublisher>();
            services.AddScoped<IEmailSender, EmailSender>();
            //services.AddScoped<IEmailSPublisherService, EmailPublisherService>();

            //services.AddHostedService<EmailSendingBackgroundService>();
            // Or with IOptions
            //services.Configure<RabbitMQConfiguration>(config.GetSection("RabbitMQ"));
            return services;
        }
    }
}
