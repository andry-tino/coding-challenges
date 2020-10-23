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
            // Configure the promotion engine
            this.ConfigurePromotionEngine(services);

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

        /// <summary>
        /// Makes sure services are configured to add dependencies for using
        /// the promotion engine across components.
        /// </summary>
        private void ConfigurePromotionEngine(IServiceCollection services)
        {
            // Add dependency on the in-memory collection of carts
            IInMemoryCollection<CartsCollection.CartsCollectionEntry> cartsInMemoryCollection = new CartsCollection();
            services.AddSingleton<IInMemoryCollection<CartsCollection.CartsCollectionEntry>>(cartsInMemoryCollection);

            // Retrieve configuration and generate the pricelist and the pipeline out of those
            // 1. From the options, get the list of SKUs to load
            // 2. Create a cart factory and pass the pricelist to it
            // 3. From the options, get the list of rules to load
            // 4. Create the pipeline by using the list of SKUs and the list of rules
            PromotionEngineOptions promotionEngineOptions = this.Configuration.GetSection(PromotionEngineOptions.ConfigurationKeyName)
                .Get<PromotionEngineOptions>();

            IDictionary<Sku, decimal> skuPriceList = new SkuPriceListFactory(promotionEngineOptions.Skus).Create();

            ICartFactory cartFactory = new CartFactory(skuPriceList);

            IPromotionPipeline promotionPipeline = new PromotionPipelineFactory(promotionEngineOptions.Rules,
                cartFactory, skuPriceList.Keys).Create();

            // Add dependencies on cart factory, pricelist and pipeline
            services.AddSingleton<ICartFactory>(cartFactory);
            services.AddSingleton<IDictionary<Sku, decimal>>(skuPriceList);
            services.AddSingleton<IPromotionPipeline>(promotionPipeline);
        }
    }
}
