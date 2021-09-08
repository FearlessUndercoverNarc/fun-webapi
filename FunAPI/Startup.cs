using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using FunAPI.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models.Attributes;
using Models.Misc;
using Newtonsoft.Json;
using Services;
using Services.External;

namespace FunAPI
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public string WWWRootPath { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRouting(options => options.LowercaseUrls = true);
            services
                .AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.Error =
                            (sender, args) => _logger.LogCritical(args.ErrorContext.Error.Message);
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                        // Use this to output UNIX seconds
                        // options.SerializerSettings.Converters.Insert(0, new UnixDateTimeConverter());
                        options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ";
                        options.SerializerSettings.Converters.Insert(0, new TimeSpanConverter());

                        // options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                        // options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                        // options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                        // options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    }
                );
            
            services
                .AddFunDependencies();
            
            WWWRootPath = Path.GetFullPath("../fun.io", _env.ContentRootPath);
            _env.WebRootPath = WWWRootPath;
            _env.WebRootFileProvider = new PhysicalFileProvider(WWWRootPath);
            
            services.AddSingleton(_ => new WWWRootPathHolder(WWWRootPath));
            
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(options => { options.RootPath = WWWRootPath; });
            
            services
                .AddSwaggerGen(swagger =>
                {
                    swagger.SwaggerDoc("v1", new OpenApiInfo() {Title = "FUN API Docs"});
                    swagger.AddSecurityDefinition("basic", new OpenApiSecurityScheme()
                    {
                        In = ParameterLocation.Header,
                        Description = "Please insert auth-token here",
                        Name = "auth-token",
                        Type = SecuritySchemeType.ApiKey
                    });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Startup>();
            
            if (!_env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionCatcherMiddleware>();
            app.UseMiddleware<RequestCounterMiddleware>();
            
            if (!_env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Akiana API Docs");
                    c.RoutePrefix = "docs";
                });
            }
            
            // app.UseHttpsRedirection();

            app.UseDefaultFiles(); // Serve index.html for route "/"
            
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = _env.WebRootFileProvider,
                ServeUnknownFileTypes = true
            };
            
            app.UseStaticFiles(staticFileOptions);
            
            if (!_env.IsDevelopment())
            {
                app.UseSpaStaticFiles(staticFileOptions);
            }

            app.UseRouting();
            
            app.UseCors(builder => builder
                .WithOrigins(
                    "http://localhost",
                    "http://localhost:4200",
                    "https://localhost",
                    "https://localhost:4200",
                    "http://memorize-cards.fun",
                    "https://memorize-cards.fun")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );

            app.UseAuthentication();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "shared_area",
                    areaName: "Shared",
                    pattern: "shared/{controller}/{action}");

                endpoints.MapFallback(FallbackDelegate);
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.DefaultPageStaticFileOptions = staticFileOptions;
            });
        }

        private async Task FallbackDelegate(HttpContext context)
        {
            var webClient = new WebClient();
            var remoteIp = context.Connection.RemoteIpAddress!.ToString().Split(':').Last();
            var ipLocation = await webClient.DownloadStringTaskAsync($"https://api.iplocation.net/?ip={remoteIp}");

            var deserializeObject = JsonConvert.DeserializeObject<dynamic>(ipLocation);
            var isp = deserializeObject.isp;
            var country_name = deserializeObject.country_name;

            // https://api.iplocation.net/?ip=XX.XX.XX.XX
            if (context.Request.Method.ToUpper() != "GET")
            {
                await TelegramAPI.Send($"Unknown endpoint Fallback!\n{context.Request.Path}\nMethod: {context.Request.Method}\nIP: {remoteIp}\nISP: {isp}\nCountry: {country_name}");
                await context.Response.WriteAsync("What are you doing bro?\nPlease use existing endpoints :)");
            }
            else
            {
                await context.Response.SendFileAsync(WWWRootPath + "/index.html");
            }
        }
    }
}