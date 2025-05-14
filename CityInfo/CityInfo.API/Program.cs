using Asp.Versioning.ApiExplorer;
using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfoapi.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// This would clean all the configurations applied from the service provider into the logging service
// and we could add right below a custom configuration to it (the Console one)
// This solution in combination with the configuration file "appsettings.Development.json" gives flexibility on how to use the logging service
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();

// Use the extension method "UseSerilog()" of Serilog that adds Serilog to the Services Collection
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddAuthentication(
       CertificateAuthenticationDefaults.AuthenticationScheme)
       .AddCertificate();

builder.Services.AddControllers(options =>
{
    // When a consumer asks for a specific type of content representation that the API does not support, the API should return a status code about that
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

// By adding the "AddProblemDetails" service, the "ExceptionHandler" added in the middleware
// is able to generate nice formatted error problem/messages in the response that will send to the
// consumer, in case of e.g. unexpected exception/error that leads to the crash of the app
builder.Services.AddProblemDetails();

// Example of defining inside the server error responses additional fields and detailed info messages
// builder.Services.AddProblemDetails(options =>
// {
//     options.CustomizeProblemDetails = ctx => 
//     {
//         ctx.ProblemDetails.Extensions.Add("additionalInfo", "Additional info added for the example");
//         ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
//     };
// });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Use the FileExtensionContentTypeProvider servive to get the extension type of any file, to use for the content type of any returned FileContentResult
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

// Add the 'LocalMailService' into the service container as transient to be the implemtation of the 'IMailService' (in Debug mode)
#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
// Add the 'CloudMailService' into the service container as transient to be the implemtation of the 'IMailService' (in Release mode for example)
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddSingleton<CitiesDatastore>();

// Register the 'CityInfoContext' in the service container
// as a DbContext with a 'scoped' lifetime.
builder.Services.AddDbContext<CityInfoContext>(dbContextOptions => dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

// This ensures the the assembly "CityInfo.API" will be added in the AutoMapper
// and the AutoMapper will be able to scan this assembly for profiles (that make
// mappings between DTO and Entity model classes)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Set up the 'Bearer' scheme token-authentication middleware for all API controllers
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]))
        };
    });

// Create a new policy that can be used during authorization at the controller or action level
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromAthens", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Athens");
    });
});

builder.Services.AddApiVersioning(setupAction =>
{
    // API response header shall contain information about the versioning (what is supported etc)
    setupAction.ReportApiVersions = true;
    // Default API version configuration (v1.0 by default used if not defined)
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
}).AddMvc()
.AddApiExplorer(setupAction =>
{
    // The API version shall be substitued in route template (as part of the routing and not as a parameter of the HTTP request body)
    setupAction.SubstituteApiVersionInUrl = true;
});

// Add the ApiVersionDescriptionProvider after the ApiExplorer service has been added to the services
var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
builder.Services.AddSwaggerGen(setupAction =>
{
    // Add all different API version pages in the Swagger UI documentation
    foreach (var description in provider.ApiVersionDescriptions)
    {
        setupAction.SwaggerDoc(
            description.GroupName,
            new()
            {
                Title = $"CityInfo API",
                Version = description.ApiVersion.ToString(),
                Description = "With this API you can access any city and their points of interests"
            });
    }

    // Get the XML comments extracted by the C# code of the CityInfo.API project documentation and use them in Swagger UI documentation
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setupAction.IncludeXmlComments(xmlCommentsFullPath);

    // Add here a new definition in Swagger documentation, to document the API authentication
    setupAction.AddSecurityDefinition("CityInfoApiBearerAuthentication",
        new()
        {
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            Description = "Input a valid token to access this API"
        });

    // Need to refer to the OpenAPI security API scheme used when the security definition was added
    // This will use the API key used through the Swagger UI on each request, once it is defined through the "Authorize" UI option
    setupAction.AddSecurityRequirement(
        new()
        {
            {
                new()
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    { 
                        Type= Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "CityInfoApiBearerAuthentication"
                    }
                },
                new List<string>()
            }
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline (middleware pipeline).

// By adding the ExceptionHandler at the beginning of the middleware,
// ensures that exceptions raised by other parts of middleware added after this part, are managed as well
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        // Configure the Swagger UI
        foreach (var description in app.DescribeApiVersions())
        {
            setupAction.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Latest .NET uses the top-level route registrations instead of "UseEndpoints"
//app.MapControllers();

app.Run();
