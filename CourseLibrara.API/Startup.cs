using CourseLibrara.API.DbContexts;
using CourseLibrara.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace CourseLibrara.API
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
            services.AddHttpCacheHeaders((expirationModelOptions) => 
            {
                expirationModelOptions.MaxAge = 63;
                expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            },
            (validationModelOptions) =>
            {
                validationModelOptions.MustRevalidate = true;
            });

            // This ensures that the necessary services are registered on the container.
            services.AddResponseCaching();

            services.AddControllers(setupAction =>
            {
                // If this set to false, the API will return response in default supported format, if an unsupportive media
                // type is requested.
                // By default, it is false. 
                setupAction.ReturnHttpNotAcceptable = true;
                // By calling Add on it, we can add a new one.
                // Cache profiles is actually a dictionary, 
                setupAction.CacheProfiles.Add("240SecondsCacheProfile",
                                                new CacheProfile()
                                                {
                                                    Duration = 240
                                                });
            })
            // The default formatter
            // The default formatter is simply the one that was added first.
            .AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            // The preferred way of adding input and output formatters for XML
            .AddXmlDataContractSerializerFormatters()
            // Configure how the API controller Attribute should behave
            .ConfigureApiBehaviorOptions( setupAction =>
            {
                // This is what will be execued when the model state is invalid
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    // create problem detail object
                    var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    // This will thus translate the validation errors from the ModelState to the RFC format
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                    // add additional info not added by defalult
                    problemDetails.Detail = "See the errors field for details. Please check.";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    // find out which status code to use
                    // By default, all is returned as a 400 Bad Request
                    var actionExecutingContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // if there are modelstate errors & all arguments were correctly
                    // found/parsed we're dealing with validation errors
                    if((context.ModelState.ErrorCount > 0) && (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Type = "https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more validation error occurred. Please check.";

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json " }
                        };
                    }

                    // if one or the arguments wasn't correctly found/couldn't be parsed
                    // we're dealing with null/unparseable input
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more errors on input occurred. Please check.";

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            // By calling in to Configure on the ServiceCollection and passing through MvcOptions as the type, we can configure them again. 
            // We can configure options which we usuali configure in AddControllers, but Json and XML was not loadet yet, so we do it here. 
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if(newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                }
            });

            // register PropertyMappingService
            // Transient is a lifetime advised by the APS.NET Core team for lightweight stateless services
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            // register PropertyCheckerService
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            // This method allows us to input a set of assemblies.
            // It's these assemblies that will automatically get scanned for profiles tha contain mapping configurations.
            // By calling into AppDomain.CurrentDomain.GetAssemblies(), we are loading profiles from all assemblies in the current domain.
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("CourseLibraryForTest"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened! Try again later.");
                        // This is also where you typically want to log this fault
                    });
                });
            }

            app.UseResponseCaching();

            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
