using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Controllers;
using Catalog.API.Extensions;
using Catalog.API.ResponseModels;
using Catalog.Domain.Extensions;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RiskFirst.Hateoas;

namespace Catalog.API
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
            services
                .AddCatalogContext(Configuration.GetSection("DataSource:ConnectionString").Value)
                .AddScoped<IItemRepository, ItemRepository>()
                .AddScoped<IArtistRepository, ArtistRepository>()
                .AddScoped<IGenreRepository, GenreRepository>()
                .AddMappers()
                .AddServices()
                .AddControllers()
                .AddValidation();

            services.AddLinks(config =>
            {
                config.AddPolicy<ItemHateoasResponse>(policy =>
                {
                    policy
                        .RequireRoutedLink(nameof(ItemHateoasController.Get), nameof(ItemHateoasController.Get))
                        .RequireRoutedLink(nameof(ItemHateoasController.GetById),
                            nameof(ItemHateoasController.GetById), _ => new { id = _.Data.Id })
                        .RequireRoutedLink(nameof(ItemHateoasController.Post), nameof(ItemHateoasController.Post))
                        .RequireRoutedLink(nameof(ItemHateoasController.Put), nameof(ItemHateoasController.Put),
                            x => new { id = x.Data.Id })
                        .RequireRoutedLink(nameof(ItemHateoasController.Delete), nameof(ItemHateoasController.Delete),
                            x => new { id = x.Data.Id });
                });
            });
        }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
