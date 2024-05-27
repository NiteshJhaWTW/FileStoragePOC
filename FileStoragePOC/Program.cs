using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using FileStoragePOC;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System.Text;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.ConfigureFunctionsApplicationInsights();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });



        var storageAccountName = Environment.GetEnvironmentVariable("StorageAccountName");
        DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
        {
            //ManagedIdentityClientId = "b7aef31a-135d-4aab-9eb4-e8e760025dc1",

            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = true
        };
        DefaultAzureCredential credential = new DefaultAzureCredential(options);


        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccountName}.blob.core.windows.net"), credential);




        services.AddSingleton(blobServiceClient);

        var rootContainerName = Environment.GetEnvironmentVariable("ContainerName");
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(rootContainerName);

        services.AddScoped<BlobContainerClient>(c => blobContainerClient);

        services.AddScoped<BlobStorageServices>();

        var retryPolicy = Policy
                        .Handle<RequestFailedException>()
                        .Or<IOException>()
                        .Or<TimeoutException>()
                        .WaitAndRetryAsync(
                                retryCount: 5,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                onRetryAsync: async (exception, timespan, retryAttempt, _) =>
                                {                                   
                                    Console.WriteLine($"Retrying attempt {retryAttempt} due to {exception.Message}." +
                                        $"Next retry in {timespan.TotalSeconds} seconds.");
                                });

        services.AddSingleton(retryPolicy);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    })
    .Build();

host.Run();
