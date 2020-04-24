using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using DSPCHR.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Hangfire.PostgreSql;
using DSPCHR.Data.PostgreSql;
using DSPCHR.Data.SqlServer;
using Microsoft.AspNetCore.Http;
using DSPCHR.Models;

namespace DSPCHR
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
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Use PostgreSQL for the database if appropriate environemnt variable is set
            // default to SQL Server otherwise
            //string dbms = Environment.GetEnvironmentVariable("DBMS") ?? "";
            string dbms = Configuration["DBMS"] ?? "";
            if (dbms.ToLower().Contains("postgres"))
            {
                services.AddDbContext<ApplicationDbContext, PostgreSqlContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("PostgreSqlConnection")));

                services.AddHangfire(config => config.UsePostgreSqlStorage(Configuration.GetConnectionString("PostgreSqlConnection")));
            }
            else
            {
                //services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlServer(
                //    Configuration.GetConnectionString("SqlServerConnection"), builder => builder.UseRowNumberForPaging(true)));

                services.AddDbContext<ApplicationDbContext, SqlServerContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SqlServerConnection")));

                services.AddHangfire(config => config.UseSqlServerStorage(Configuration.GetConnectionString("SqlServerConnection")));
            }

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            // Use Hangfire to run background jobs
            services.AddHangfireServer();

            services.AddHttpClient<Gateway.Client, Gateway.Client>();

            services.AddTransient<Jobs.Subscriptions, Jobs.Subscriptions>();
            services.AddTransient<Jobs.Messages, Jobs.Messages>();

            services.AddTransient<Authorisation.Resources>();

            // For pagination
            services.AddCloudscribePagination();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            string basePath = Configuration["ASPNETCORE_BASEPATH"];
            if (!string.IsNullOrEmpty(basePath))
            {
                app.Use(async (context, next) =>
                {
                    context.Request.PathBase = new PathString(basePath);
                    await next.Invoke();
                });
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseHangfireDashboard();
        }
    }
}
