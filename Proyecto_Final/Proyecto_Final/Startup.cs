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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Proyecto_Final.Models;
using Proyecto_Final.Services;

namespace Proyecto_Final
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
            services.Configure<BookstoreDatabaseSettings>(
            Configuration.GetSection(nameof(BookstoreDatabaseSettings)));

            services.AddSingleton<IBookstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);

            services.AddSingleton<BookService>();

            ////////////////////////////////////////////////////////////
            services.Configure<UsersDatabaseSettings>(
            Configuration.GetSection(nameof(UsersDatabaseSettings)));

            services.AddSingleton<IUsersDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<UsersDatabaseSettings>>().Value);

            services.AddSingleton<UserService>();

            ////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////
            services.Configure<FriendsDatabaseSettings>(
            Configuration.GetSection(nameof(FriendsDatabaseSettings)));

            services.AddSingleton<IFriendsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<FriendsDatabaseSettings>>().Value);

            services.AddSingleton<FriendService>();

            ////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////
            services.Configure<MessagesDatabaseSettings>(
            Configuration.GetSection(nameof(MessagesDatabaseSettings)));

            services.AddSingleton<IMessagesDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<MessagesDatabaseSettings>>().Value);

            services.AddSingleton<FriendService>();

            ////////////////////////////////////////////////////////////
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
