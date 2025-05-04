using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using priceapp.Services.Interfaces;

namespace priceapp.Services.Implementation;

public class MailService : IMailService
{
    private readonly ILogger<MailService> _logger;
    private readonly string Domain;
    private readonly string DomainPretty;
    private readonly string MailFrom;
    private readonly string MailHost;
    private readonly string MailLogin;
    private readonly string MailName;
    private readonly string MailPassword;
    private readonly int MailPort;

    public MailService(ILogger<MailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        MailFrom = configuration["Mail:From"];
        MailLogin = configuration["Mail:Login"];
        MailPassword = configuration["Mail:Password"];
        MailHost = configuration["Mail:Host"];
        MailPort = Convert.ToInt32(configuration["Mail:Port"]);
        MailName = configuration["Mail:Name"];
        Domain = configuration["Domain:Host"];
        DomainPretty = configuration["Domain:Pretty"];
    }

    public async Task SendRegistrationConfirmEmailAsync(int userId, string email, string token)
    {
        var link = $"{Domain}/confirm_email?userid={userId}&token={token}";
        var body = @$"<h1>Вітаємо!</h1>
            <p>Нещодавно ви зареєструвались на сайті <a href='{Domain}'>{DomainPretty}</a></p>
            <p>Для завершення реєстрації необхідно підтвердити адресу електронної пошти. Для цього перейдіть за посиланням: <a href='{link}'>{link}</a></p>
            <p>Якщо цей лист надійшов до вас помилково, будь-ласка, напишіть на <a href='mailto:{MailFrom}'>{MailFrom}</a></p>
            <p>Дякуємо та бажаємо гарного дня!</p>
            <p>З повагою,<br>
            <a href='{Domain}'>PriceApp</a><br>
            <a href='{Domain}'><img src='{Domain}/public_resources/icons/priceapp_icon.png' width='100' height='100'/></a><br>
            <a href='mailto:{MailFrom}'>{MailFrom}</a></p>";
        var subject = $"Підтвердження реєстрації у {MailName}";

        try
        {
            await SendEmailAsync(subject, body, email);
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"MailService: Something went wrong while while sending confirmation email for user {userId} with email {email}");
            throw new ApplicationException("Something went wrong while sending confirmation email");
        }
    }
    
    
    public async Task SendEmailAsync(string subject, string body, string to)
    {
        var smtpClient = new SmtpClient(MailHost, MailPort);
        var message = new MailMessage();
        var credentials = new NetworkCredential(MailLogin, MailPassword);

        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = credentials;

        message.From = new MailAddress(MailFrom, MailName);
        message.To.Add(new MailAddress(to));
        message.IsBodyHtml = true;
        message.BodyEncoding = Encoding.UTF8;
        message.Body = body;
        message.Subject = subject;

        try
        {
            await smtpClient.SendMailAsync(message);
        }
        catch (Exception e)
        {
            message.Dispose();
            _logger.LogCritical(
                $"MailService: Something went wrong while while sending email to {to}");
            throw new ApplicationException("Something went wrong while sending email");
        }

        smtpClient.Dispose();
    }
}