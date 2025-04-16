namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private string _emailTo = "admin@company.com";
        private string _emailFrom = "noreply@company.com";

        /// <inheritdoc/>
        public void Send(string subject, string message)
        {
            Console.WriteLine($"Sending email from {_emailFrom} to {_emailTo} with the service {nameof(CloudMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Subject: {message}");
        }
    }
}
