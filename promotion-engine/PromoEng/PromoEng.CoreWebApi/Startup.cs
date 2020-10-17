using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PromoEng.CoreWebApi.Model;
using PromoEng.Engine;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Web core app startup class defining dependencies and configuring middlewares.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration being used.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Adds services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add configuration for the promotion engine
            services.Configure<PromotionEngineOptions>(
                this.Configuration.GetSection(PromotionEngineOptions.Position));

            // Add dependency on the in-memory collection of carts
            IInMemoryCollection<CartsCollection.CartsCollectionEntry> cartsInMemoryCollection = new CartsCollection();
            services.AddSingleton<IInMemoryCollection<CartsCollection.CartsCollectionEntry>>(cartsInMemoryCollection);

            // Add dependency on the pricelist and the cart factory
            IDictionary<Sku, decimal> priceList = new Dictionary<Sku, decimal>(); // TODO: Load the pricelist
            ICartFactory cartFactory = new CartFactory(priceList); // TODO: Load the pricelist
            services.AddSingleton<ICartFactory>(cartFactory);
            services.AddSingleton<IDictionary<Sku, decimal>>(priceList);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();

            services.AddControllers();
        }

        /// <summary>
        /// This method gets called by the runtime. Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui
            app.UseSwaggerUI(c =>
            {
                // Use the Swagger JSON endpoint
                c.SwaggerEndpoint(Constants.Routing.SwaggerUrl, "API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
