using FCG.Functions.Helpers;
using FCG.Functions.Interfaces;
using FCG.Functions.Options;
using FCG.Functions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

var host = new HostBuilder()
    .ConfigureAppConfiguration(cfg =>
    {
        cfg.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        cfg.AddEnvironmentVariables();
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.Configure<ApiOptions>(context.Configuration.GetSection("FunctionOptions"));

        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

        services.AddHttpClient<IPaymentClient, PaymentClient>((sp, http) =>
        {
            var opt = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiOptions>>().Value;

            if (string.IsNullOrWhiteSpace(opt.BaseUrl))
                throw new InvalidOperationException("Configurações API: BaseUrl não configurado.");

            if(string.IsNullOrWhiteSpace(opt.ApimSubscriptionKey))
                throw new InvalidOperationException("Configurações API: APIM Subscription Key não configurado.");

            Console.WriteLine($"BaseUrl carregada: {opt.BaseUrl}");
            Console.WriteLine($"APIM Subscription Key carregada: {opt.ApimSubscriptionKey}");

            http.BaseAddress = new Uri(opt.BaseUrl.TrimEnd('/') + "/");
            http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", opt.ApimSubscriptionKey);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
    })
    .Build();

host.Run();