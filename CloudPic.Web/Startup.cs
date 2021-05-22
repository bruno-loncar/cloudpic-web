using CloudPic.BAL.Interfaces;
using CloudPic.DAL.Concretes;
using CloudPic.DAL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Hosting;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CloudPic.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            LoggingServices.DefaultBackend = new PostSharp.Patterns.Diagnostics.Backends.NLog.NLogLoggingBackend();
            CachingServices.DefaultBackend = new PostSharp.Patterns.Caching.Backends.MemoryCachingBackend();

            services.AddAntiforgery(options => { options.Cookie.Expiration = TimeSpan.Zero; });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews()
                .AddSessionStateTempDataProvider();;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Authentication settings
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.LoginPath = "/account/login";
                        options.LogoutPath = "/account/logout";
                        options.SlidingExpiration = true;
                    })
                    .AddGitHub("Github", options =>
                    {
                        options.ClientId = Configuration["GitHub:ClientId"];
                        options.ClientSecret = Configuration["GitHub:ClientSecret"];
                        options.Scope.Add("user:email");
                        options.Events = new OAuthEvents { OnCreatingTicket = OnCreatingAuthTicket() };
                    })
                    .AddGoogle("Google", options =>
                    {
                        options.ClientId = Configuration["Google:ClientId"];
                        options.ClientSecret = Configuration["Google:ClientSecret"];
                        options.UserInformationEndpoint = Configuration["Google:UserInformationEndpoint"];
                        options.ClaimActions.Clear();
                        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                        options.Events = new OAuthEvents { OnCreatingTicket = OnCreatingAuthTicket() };
                    });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IUserRepo, UserRepo>();
            services.AddSingleton<IUserService, UserService>();

            services.AddSingleton<IPlanRepo, PlanRepo>();
            services.AddSingleton<IPlanService, PlanService>();

            services.AddSingleton<IFSRepo, FSRepo>();
            
            services.AddSingleton<IPhotoRepo, PhotoRepo>();
            services.AddSingleton<IPhotoService, PhotoService>();

            services.AddSingleton<ILogRepo, LogRepo>();
            services.AddSingleton<ILogService, LogService>();

        }


        private Func<OAuthCreatingTicketContext, Task> OnCreatingAuthTicket()
        {
            return async context =>
            {
                var email = context.Identity.FindFirst(ClaimTypes.Email).Value;
                var name = context.Identity.FindFirst(ClaimTypes.Name).Value;
                var loginProvider = context.Identity.AuthenticationType;


                await Task.FromResult(true);
            };
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseCookiePolicy();

            app.UseSession();

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
