namespace UserGuard_API.Events
{
    public class UserRegisteredEvent : INotification
    {
        public string Email { get; }
        public string Subject { get; }
        public string HtmlContent { get; }

        public UserRegisteredEvent(string email, string subject, string htmlContent)
        {
            Email = email?.ToLower();
            Subject = subject;
            HtmlContent = htmlContent;
        }
    }
}
