namespace UserGuard_API.Listeners
{
    public class SendWelcomeEmailHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly ResendEmailService _emailService;

        public SendWelcomeEmailHandler(ResendEmailService emailService)
        {
            _emailService = emailService;
        }


        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailAsync(notification.Email,notification.Subject,notification.HtmlContent);
        }
    }
}
