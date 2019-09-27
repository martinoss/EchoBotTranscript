// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using EchoBotTranscript.Bots;
using Microsoft.Extensions.Options;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;

namespace EchoBotTranscript
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOptions();
            services.Configure<BlobStorageOptions>(Configuration.GetSection(nameof(BlobStorageOptions)));

            services.AddSingleton<ITranscriptStore>(serviceProvider => {
                var options = serviceProvider.GetRequiredService<IOptions<BlobStorageOptions>>().Value;
                return new AzureBlobTranscriptStore(options.ConnectionString, options.TranscriptsContainer);
            });

            // Middleware: Transcript logger
            services.AddSingleton(serviceProvider =>
            {
                var transcriptStore = serviceProvider.GetRequiredService<ITranscriptStore>();
                return new TranscriptLoggerMiddleware(transcriptStore);
            });

            // Create the Bot Framework Adapter with error handling and middleware enabled. 
            services.AddScoped<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>(serviceProvider =>
            {
                var adapter = new AdapterWithErrorHandler(
                    Configuration,
                    serviceProvider.GetRequiredService<ILogger<BotFrameworkHttpAdapter>>());

                adapter
                    // Add middleware that stores transcripts of conversations
                    .Use(serviceProvider.GetRequiredService<TranscriptLoggerMiddleware>());

                return adapter;
            });

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, EchoBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
