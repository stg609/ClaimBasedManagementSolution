using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using Common.Constants;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Common.Infra;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SampleMVCApp.Infra;
using SampleMVCApp.Services;
using SampleMVCApp.Domain;
using Common.Domain;
using System.Security.Claims;

namespace SampleMVCApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), op => op.MigrationsAssembly("SampleMVCApp.Infra")));

            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, ClaimAuthorizationHandler>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<HttpClient>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = "http://localhost:5000";
                    options.ClientId = Services.Constants.Identity;
                    options.ClientSecret = "secret";
                    options.RequireHttpsMetadata = false;
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken; //"code id_token";

                    options.GetClaimsFromUserInfoEndpoint = true;

                    //Given to the hybrid flow, we need to map custom claims to id token, otherwise only the common claims will be included in the id token.
                    options.ClaimActions.MapJsonKey(ClaimConstants.PermissionClaimType, ClaimConstants.PermissionClaimType);
                    options.ClaimActions.MapJsonKey(ClaimConstants.NicknameClaimType, ClaimConstants.NicknameClaimType);

                    options.SaveTokens = true;
                    options.Scope.Add("api1");
                    options.Scope.Add("offline_access");
                });

            //Return our container, reference our class that holds our registrations. 
            return new Container().WithDependencyInjectionAdapter(services).ConfigureServiceProvider<CompositionRoot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, HttpClient client)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            //IRouter routeCollection = null;
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //      name: "default",
            //      template: "{controller=Home}/{action=Index}/{id?}");

            //    routeCollection = routes.Build();
            //});
            //RouteData data = new RouteData();
            //data.Routers.Add(routeCollection);

            //ActionContext context = new ActionContext(new DefaultHttpContext(),data,
            //    new ActionDescriptor() { });
            ////context.RouteData.Routers = new List<IRouter> { routeCollection };
            //var url = urlHelper.GetUrlHelper(context);
            ////menuService.GenerateMenusByControllerAction();

            bool isSuccess = ClaimsAnalyzer.SendClaimToIdentityServer(client, "http://localhost:5000/api/claims", Services.Constants.Identity);
            if (!isSuccess)
            {
                //TODO log it
            }
        }
    }
}
