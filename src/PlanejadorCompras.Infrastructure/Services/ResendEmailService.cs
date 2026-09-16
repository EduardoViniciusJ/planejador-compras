using Microsoft.Extensions.Configuration;
using PlanejadorCompras.Application.Services.Interfaces;
using Resend;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly string _fromEmail;

    public ResendEmailService(IConfiguration configuration)
    {
        var apiKey = configuration["Resend:ApiKey"];
        var fromEmail = configuration["Resend:FromEmail"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Missing configuration 'Resend:ApiKey'.");
        }

        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException("Missing configuration 'Resend:FromEmail'.");
        }

        _resend = ResendClient.Create(apiKey);
        _fromEmail = fromEmail;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage
        {
            From = _fromEmail,
            To = to,
            Subject = subject,
            HtmlBody = body
        };

        await _resend.EmailSendAsync(message, cancellationToken);
    }
}
