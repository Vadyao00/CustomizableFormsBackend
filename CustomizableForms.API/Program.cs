using CustomizableForms.API.Extensions;
using CustomizableForms.Controllers;
using CustomizableForms.Controllers.Filters;
using CustomizableForms.Domain.Requirements;
using CustomizableForms.LoggerService;
using CustomizableForms.Persistance.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomizableForms.API;

public class Program
{
    public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services,builder.Configuration);

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILoggerManager>();
            app.ConfigureExceptionHandler(logger);

            if (app.Environment.IsProduction())
            {
                app.UseHsts();
            }

            ConfigureApp(app);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.ConfigureSwagger();

            services.ConfigureCors();

            services.AddScoped<ValidationFilterAttribute>();

            services.ConfigureLoggerService();

            services.ConfigureRepositoryManager();
            services.AddHttpContextAccessor();
            services.AddSignalR();

            services.AddScoped<IAuthorizationHandler, NotBlockedUserRequirementHandler>();
            
            services.ConfigureServiceManager();

            services.ConfigureDbContext(configuration);

            services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
            }).AddXmlDataContractSerializerFormatters()
              .AddApplicationPart(typeof(AssemblyReference).Assembly);

            
            //services.AddMediatR(cfg =>
                //cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly));
            services.AddAutoMapper(typeof(Program));

            services.AddAuthentication();
            services.ConfigureJWT(configuration);
            services.AddJwtConfiguration(configuration);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("NotBlockedUserPolicy", policy =>
                    policy.Requirements.Add(new NotBlockedUserRequirement()));
            });
            services.AddRazorPages();
        }

        public static void ConfigureApp(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Cinema API v1");
            });
            
            app.UseHttpsRedirection();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
            });
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CommentsHub>("/hubs/comments");
            });
            
        }
}