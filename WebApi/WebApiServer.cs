using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BMHRI.WCS.Server.WebApi
{
    public class WebApiServer
    {
        private WebApplication? WCSWebApplication;
        private WebApplicationBuilder? builder;
        private Task? WebAppTask;
        private static readonly Lazy<WebApiServer> lazy = new(() => new WebApiServer());
        public static WebApiServer Instance { get { return lazy.Value; } }
        public void Start()
        {
            builder = WebApplication.CreateBuilder();

            builder.WebHost.UseUrls("http://*:8090");

            // Add services to the container.
            builder.Services.AddControllers();
            //builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            WCSWebApplication = builder.Build();

            // Configure the HTTP request pipeline.
            //if (WCSWebApplication.Environment.IsDevelopment())
            //{
            //    WCSWebApplication.UseSwagger();
            //    WCSWebApplication.UseSwaggerUI();
            //}

            WCSWebApplication.UseHttpsRedirection();

            WCSWebApplication.UseAuthorization();

            WCSWebApplication.MapControllers();
            WebAppTask = WCSWebApplication.StartAsync();
        }
        //public void Start()
        //{
        //    CreateWebApiServer();
        //    if (WCSWebApplication != null)
        //        WebAppTask = WCSWebApplication.StartAsync();
        //}
        public void Stop()
        {
            if (WCSWebApplication != null)
                WCSWebApplication.StopAsync();
        }
    }
}
