namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private readonly string _emailTo = string.Empty;
        private readonly string _emailFrom = string.Empty;

        public LocalMailService(IConfiguration configuration)
        {
            // Any file "appsettings.{Environment}.json" is loaded after the "appsettings.json" and overwrites the common variables.
            // Likeso, When the application is initiated in "Development" environment, the "mailToAddress" variable from "appsettings.Development.json" is loaded.
            // When the application is initiated in "Production" environment, the "mailToAddress" variable from "appsettings.Production.json" is loaded.
            // The "mailToAddress" variable is defined in the "appsettings.json" with the value "default@company.com", but because it is also defined in both
            // environment configuration files, it is overwritten by the value "developer@company.com" or "admin@company.com" respectively.

            // NOTE: To change between "Development" and "Production" environment modify the "ASPNETCORE_ENVIRONMENT" from the "launchSettings.json"

            _emailTo = configuration["mailSettings:mailToAddress"];
            _emailFrom = configuration["mailSettings:mailFromAddress"];
        }

        /// <inheritdoc/>
        public void Send(string subject, string message)
        {
            Console.WriteLine($"Sending email from {_emailFrom} to {_emailTo} with the service {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Subject: {message}");
        }
    }
}
