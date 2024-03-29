using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PiHealth.DataModel;
using PiHealth.Service.UserAccounts;
using PiHealth.Services;
using PiHealth.Services.AccessServices;
using PiHealth.Services.DBContext;
using PiHealth.Services.Master;
using PiHealth.Services.PatientProfileService;
using PiHealth.Services.PiHealthPatients;
using PiHealth.Services.UserAccounts;
using PiHealth.Web.Extention;
using PiHealth.Web.Filter;
using PiHealth.Web.Middleware;
using PiHealth.Web.Model.Prefix;
using PiHealth.WebInfra;
using PiHealth.Extention;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

namespace PiHealth
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "CorsOrigins";
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        private readonly IWebHostEnvironment _webHostEnvironment;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            GlobalConfiguration.WebRootPath = _webHostEnvironment.WebRootPath;
            GlobalConfiguration.ContentRootPath = _webHostEnvironment.ContentRootPath;

            services.AddMemoryCache();
            if (!_webHostEnvironment.IsDevelopment())
            {
                services.AddControllers().AddNewtonsoftJson(setup =>
                {
                    setup.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    setup.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            }
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<ITokenValidatorService, TokenValidatorService>();
            services.AddScoped<ITokenService, TokenService>();


            services.AddCustomizedAuthentication(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IDbInitializerService, DbInitializerService>();
            services.AddScoped<SecurityService>();
            services.AddScoped<AppointmentService>();
            services.AddScoped<DepartmentService>();
            services.AddScoped<PatientService>();
            services.AddScoped<PrescriptionMasterService>();
            services.AddScoped<DoctorMasterService>();
            services.AddScoped<VitalsReportService>();
            services.AddScoped<DoctorDataServices>();
            services.AddScoped<PatientProfileDataService>();
            services.AddScoped<TemplateMasterDataMapService>();
            services.AddScoped<PatientProfileDataMapService>();
            services.AddScoped<ReportService>();

            services.AddScoped<DiagnosisMasterService>();
            services.AddScoped<TestMasterService>();
            services.AddScoped<TemplateMasterService>();
            services.AddScoped<PatientProfileService>();
            services.AddScoped<PatientProcedureService>();
            services.AddScoped<AuditLogServices>();
            services.AddScoped<AccessModuleService>();
            services.AddScoped<AccessFunctionService>();
            services.AddScoped<ProcedureMasterService>();
            services.AddScoped<AccessRoleFunctionService>();
            services.Configure<BearerTokensOptions>(options => Configuration.GetSection("BearerTokens").Bind(options));
            services.Configure<PrefixOption>(options => Configuration.GetSection("PrefixOption").Bind(options));
            services.AddCustomizedDataStore(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder
                            .WithOrigins(Configuration.GetSection(MyAllowSpecificOrigins).Value
                                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                            .Select(o => o)
                                            .ToArray())
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            services.AddMvc(option =>
            {
                option.Filters.Add(new ExceptionLogFilter());
                option.Filters.Add(typeof(ExceptionHandlingFilter)); // by type                
            }).AddJsonOptions(
                options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                ).AddNewtonsoftJson(
          options => {
              options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
          }); ;

            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(x => x.First());
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PIHealth API", Version = "v1" });
            });

            services.AddMemoryCache();
            services.AddEntityFrameworkSqlServer();
            services.AddMiniProfiler(options => 
            { 
                options.RouteBasePath = "/profiler";
            }).AddEntityFramework();
            services.AddControllers();

            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "localhost:6379";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
               
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PIHealth API V1");
                //options.IndexStream = () => Assembly.GetExecutingAssembly()
                //    .GetManifestResourceStream("PiHealthAPI.wwwroot.swagger.ui.index.html");

            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseFileServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    if (error != null && error.Error is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            State = 401,
                            Msg = "token expired"
                        }));
                    }
                    else if (error != null && error.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            State = 500,
                            Msg = error.Error.Message
                        }));
                    }
                    else
                    {
                        await next();
                    }
                });
            });

            app.UseCustomizedIdentity();



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseDeveloperExceptionPage();
            }



            //app.UseSwagger();

            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            //});

            //loggerFactory.AddNLog();
            //app.AddNLogWeb();
            //env.ConfigureNLog("nlog.config");


            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetService<IDbInitializerService>();
                dbInitializer.Initialize();
                dbInitializer.SeedData();
            }
            app.UseMiddleware<ErrorHandlingMiddleware>();
            //app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMiniProfiler();
        }
    }
}
