using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tusdotnet;
using tusdotnet.Helpers;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;

namespace LargeFileUpload
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
            services.AddRazorPages();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use((context, next) =>
            {
                // Default limit was changed some time ago. Should work by setting MaxRequestBodySize to null using ConfigureKestrel but this does not seem to work for IISExpress.
                // Source: https://github.com/aspnet/Announcements/issues/267
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null;
                return next.Invoke();
            });
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseCors(builder => builder
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowAnyOrigin()
                   .WithExposedHeaders(CorsHelper.GetExposedHeaders()));
            app.UseTus(httpContext => new DefaultTusConfiguration
            {
                Store = new TusDiskStore(@"C:\tusfiles\"),
                UrlPath = "/files",
                Events = new Events
                {
                    OnFileCompleteAsync = async eventContext =>
                    {
                        // eventContext.FileId is the id of the file that was uploaded.
                        // eventContext.Store is the data store that was used (in this case an instance of the TusDiskStore)

                        // A normal use case here would be to read the file and do some processing on it.
                        ITusFile file = await eventContext.GetFileAsync();
                        var result = await DoSomeProcessing(file, eventContext.CancellationToken).ConfigureAwait(false);

                        if (!result)
                        {
                            //throw new MyProcessingException("Something went wrong during processing");
                        }
                    }
                }
            });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        private Task<bool> DoSomeProcessing(ITusFile file, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
