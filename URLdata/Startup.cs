using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using URLdata.Data;

namespace URLdata
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
            var directorPath = "";
            try
            {
                directorPath = Path.Combine(Directory.GetCurrentDirectory(),
                    $"{ConfigurationManager.AppSettings["resource directory"]}");
            }
            catch (Exception e)
            {
                throw new FileNotFoundException("repository dose not exist!");
            }
    
            services.AddControllers();
            services.AddSingleton<IReader>( reader => new CSVReader(directorPath));
            services.AddSingleton<IParser,CsvDataParser>();
            services.AddHostedService<DataParsingService>();
            services.AddSingleton<DataStatusMiddleware>();
            services.AddSingleton<IDataHandler,DataHandler>();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "URLdata", Version = "v1"}); });
        }

 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "URLdata v1"));
            }

            app.UseMiddleware<DataStatusMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    
        
        }
    }
}