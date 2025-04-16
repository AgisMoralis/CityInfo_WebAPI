namespace CityInfo.API.Services
{
    public class LocalMailService
    {
        private string _emailTo = "admin@company.com";
        private string _emailFrom = "noreply@company.com";

        /// <summary>
        /// This function simulates the sending of an email.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public void Send(string subject, string message) 
        {
            Console.WriteLine($"Sending email from {_emailFrom} to {_emailTo} with the service {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Subject: {message}");
        }
    }
}
