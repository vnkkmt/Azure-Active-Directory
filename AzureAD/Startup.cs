using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureADWeb
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
            //Application/Client ID : 5e2fd127-f769-49fa-9bb4-f27873e7aa5e
            //Auth endpoint : https://login.microsoftonline.com/444b27d6-b3d5-4305-b710-469e4114d7a9/oauth2/v2.0/authorize

            services.AddControllersWithViews();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; //sets the default authentication scheme to use cookies
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // and default challenge scheme to use OpenId connect authentication scheme defined in line 35 in AddOpenIdConnect() method
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //adds cokkie authentication to middleware

            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => // adds openid connect middleware to pipeline, it specifies the authentication scheme to use etc
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "https://login.microsoftonline.com/444b27d6-b3d5-4305-b710-469e4114d7a9/v2.0"; // provider who authenticates, here it is Azure AD
                options.ClientId = "123";                                   // Id of registered application in AD
                options.ResponseType = "code";                             // expected response type which will be excahnged for access tokens
                options.SaveTokens = true;                                 // save the access and refresh tokens received form OpenID provider
                options.ClientSecret = "xyz"; //

            })
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
