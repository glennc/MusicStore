using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MusicStore.Albums
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["Data:DefaultConnection:ConnectionString"];

            services.AddDbContext<AlbumContext>(options => {
                if(string.IsNullOrEmpty(connectionString))
                {
                    options.UseInMemoryDatabase();
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }
            });

            services.AddMvc()
                    //There are many self-referencing loops in the music store model, however we typically don't require the data
                    //in the queries that suffer from it, so ignoring the data appears fine. We will re-evaluate if we find a query
                    //where that isn't the case.
                    .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseMvc();

            if(string.IsNullOrEmpty(Configuration["Data:DefaultConnection:ConnectionString"]))
            {
                //Seed the InMemory database.
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var db = serviceScope.ServiceProvider.GetService<AlbumContext>();

                    if (db.Database.EnsureCreated())
                    {
                        db.Genres.AddRange(Models.SampleData.Genres.Select(g => g.Value).ToList());
                        db.Artists.AddRange(Models.SampleData.Artists.Select(a => a.Value).ToList());
                        db.Albums.AddRange(Models.SampleData.GetAlbums());
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
