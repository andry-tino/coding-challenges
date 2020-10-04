using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Challenge.WebIntSorter.Models;
using Microsoft.EntityFrameworkCore;
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
            services.AddDbContext<SortingJobContext>(options => options.UseInMemoryDatabase("JobsDb"));
            services.AddScoped<SortingJobContext>();
            services.AddCors(this.ConfigureCors);

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
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

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
                    .WithOrigins("http://localhost:3000")
                    .WithMethods("POST", "GET", "PUT")
                    .WithHeaders("Content-Type");
            });
        }

        private void ConfigureJsonSerialization(JsonOptions options)
        {
        }
    }
}
