using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Challenge.WebIntSorter.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Handles configuration operations to properly setup the server by
    /// injecting all the required dependencies.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration object, a list of key-value pairs.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add in memory collection of jobs
            var jobsCollection = new SortingJobCollection();
            services.AddSingleton(jobsCollection);

            // Add CORS capabilities
            services.AddCors(this.ConfigureCors);

            // Add response caching capabilities
            services.AddResponseCaching(this.ConfigureResponseCaching);

            // Add the controllers defined in this assembly
            services.AddControllers()
                .AddJsonOptions(this.ConfigureJsonSerialization);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseHttpsRedirection();
            }
            else
            {
                app.UseExceptionHandler("/error");

                app.UseHsts();
            }

            app.UseRouting();

            app.UseResponseCaching();

            app.UseCors(Constants.Service.CorsReactClientAllowSpecificOriginsPolicyName);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureCors(CorsOptions options)
        {
            options.AddPolicy(Constants.Service.CorsReactClientAllowSpecificOriginsPolicyName, builder =>
            {
                builder
                    .WithOrigins("http://localhost:3000") // Access-Control-Allow-Origin
                    .WithMethods("POST", "GET", "PUT") // Access-Control-Allow-Methods
                    .WithHeaders("Content-Type"); // Access-Control-Allow-Headers
            });
        }

        private void ConfigureJsonSerialization(JsonOptions options)
        {
        }

        private void ConfigureResponseCaching(ResponseCachingOptions options)
        {
            options.SizeLimit = 100 * 1000; // Cache size should not exceed 100MB
        }
    }
}
