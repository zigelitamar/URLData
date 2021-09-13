using System.Configuration;
using System.IO;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using URLdata.Data;
using URLdata.Models;
using IParser = URLdata.Data.IParser;
using IReader = URLdata.Data.IReader;


namespace URLdata
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // get path from app.config file.
            var directorPath = Path.Combine(Directory.GetCurrentDirectory(),
                $"{ConfigurationManager.AppSettings["resource directory"]}");

            services.AddControllers().AddFluentValidation();
            services.AddSingleton<IReader>( _ => new CSVReader(directorPath));
            services.AddSingleton<IParser,CsvDataParser>();
            services.AddHostedService<DataParsingService>();
            services.AddSingleton<DataStatusMiddleware>();
            services.AddTransient<IValidator<Url>, UrlRequestValidator>();
            services.AddTransient<IValidator<VisitorId>, VisitorIdRequestValidator>();
            services.AddSingleton<IDataHandler,DataHandler>();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Sessionizing", Version = "v1"}); });
        }

 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sessionizing v1"));
            }

            app.UseMiddleware<DataStatusMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    
        
        }
    }
}