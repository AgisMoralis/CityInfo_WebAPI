namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private readonly string _emailTo = string.Empty;
        private readonly string _emailFrom = string.Empty;

        public LocalMailService(IConfiguration configuration)
        {
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
