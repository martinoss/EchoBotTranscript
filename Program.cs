// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace EchoBotTranscript
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .UseStartup<Startup>();


        private static void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder configBuilder)
        {
            // Build the config from sources we have
            var config = configBuilder.Build();

            // Create Managed Service Identity token provider
            var tokenProvider = new AzureServiceTokenProvider();

            // When acquiring a token from Visual Studio, it can take very long
            // because it first tries to get a token from a local URL (MSI authentication) which 
            // will time out.
            // Here are solution approaches to make the application start much faster.
            // The only one working at the time of writing was to set environment variable:
            // AzureServicesAuthConnectionString  => RunAs=Developer; DeveloperTool=VisualStudio
            // on the developer machine.
            // https://github.com/Azure/azure-sdk-for-net/issues/4645

            // Create the Key Vault client
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));
            // Add Key Vault to configuration pipeline
            configBuilder.AddAzureKeyVault(config["keyVault"], keyVaultClient, new DefaultKeyVaultSecretManager());
        }
    }
}
