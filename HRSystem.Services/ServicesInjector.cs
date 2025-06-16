using Microsoft.Extensions.DependencyInjection;
using POSSystem.Services.MappingProfile;
using POSSystem.Services.Messaging.Emailing;
using POSSystem.Services.Messaging.Enventory;
using POSSystem.Services.Services;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace HRSystem.Services
{
    public static class ServicesInjector
    {
        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(ApplicationProfiler).Assembly);

            services.AddScoped<IItemsService, ItemsService>();
            services.AddScoped<IInvoiceService, InvoiceService>();

            services.AddHostedService<EmailConsumer>();
            services.AddHostedService<ItemStockConsumer>();

            return services;
        }
    }
}
