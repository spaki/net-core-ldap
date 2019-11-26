using LdapPoc.Constants;
using LdapPoc.Ldap;
using LdapPoc.Settings;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LdapPoc
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => AppSettings = configuration.Get<AppSettings>();

        public AppSettings AppSettings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(AppSettings);
            services.AddSingleton<IAuthService, LdapService>();
            services
                .AddAuthentication()
                .AddCookie(
                    GlobalConstants.ApplicationSchema,
                    options =>
                    {
                        options.LoginPath = "/Auth/Login";
                        options.LogoutPath = "/Auth/Logout";
                        options.AccessDeniedPath = "/Home/AccessDenied";
                    }
                );
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
