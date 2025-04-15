using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(
       CertificateAuthenticationDefaults.AuthenticationScheme)
       .AddCertificate();
builder.Services.AddControllers(options =>
{
    // When a consumer asks for a specific type of content representation that the API does not support, the API should return a status code about that
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

// Example of defining inside the server error responses additional fields and detailed info messages
//builder.Services.AddProblemDetails(options =>
//{
//    options.CustomizeProblemDetails = ctx => 
//    {
//        ctx.ProblemDetails.Extensions.Add("additionalInfo", "Additional info added for the example");
//        ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
//    };
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use the FileExtensionContentTypeProvider servive to get the extension type of any file, to use for the content type of any returned FileContentResult
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline (middleware pipeline).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

// Latest .NET uses the top-level route registrations instead of "UseEndpoints"
//app.MapControllers();

app.Run();
