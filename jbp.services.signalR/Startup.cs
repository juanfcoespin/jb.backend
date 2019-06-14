using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using jbp.services.signalR.Hubs;


namespace jbp.services.signalR
{
    public class Startup
    {
        readonly string CorsPolicyName = "CorsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Para soportar Signal R
            services.AddCors(o =>
            {
                o.AddPolicy(CorsPolicyName,
                builder =>
                    builder.WithOrigins("http://localhost:4200",
                        "http://app.jbp.com.ec",
                        "http://app.jamesbrownpharma.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Para soportar SignarR
            app.UseCors(CorsPolicyName);
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotifyHub>("/notify");
                routes.MapHub<CheckOrdersToPromotickBusinessService>("/checkOrdersToPromotickBusinessService");
            });
            app.UseMvc();
        }
    }
}
