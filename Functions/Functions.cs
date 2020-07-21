using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Functions.Startup))]

namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder
                .Services
                .AddOptions<CosmosDB>()
                .Configure<IConfiguration>((cosmosDB, configuration) =>
                {
                    configuration.GetSection("CosmosDB").Bind(cosmosDB);
                });
        }
    }

    public class CosmosDB
    {
        public string AuthKey { get; set; }
        public string Endpoint { get; set; }
    }

    public class Functions
    {
        private readonly IConfiguration configuration;
        private readonly IOptions<CosmosDB> options;

        public Functions(IConfiguration configuration, IOptions<CosmosDB> options)
        {
            this.configuration = configuration;
            this.options = options;
        }

        [FunctionName("Functions")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // basic extraction of values from IConfiguration
            log.LogInformation(configuration.GetValue<string>("SecretKey"));
            log.LogInformation(configuration.GetValue<string>("CosmosDB:AuthKey"));

            // basic extraction of values from IConfiguration by section
            log.LogInformation(configuration.GetSection("CosmosDB").GetValue<string>("Endpoint"));

            // extraction of values using the options pattern
            log.LogInformation(options.Value.AuthKey);
            log.LogInformation(options.Value.Endpoint);

            return new OkResult();
        }
    }
}
