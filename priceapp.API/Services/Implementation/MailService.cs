using System.Net;
using System.Net.Mail;
using System.Text;
using priceapp.API.Services.Interfaces;

namespace priceapp.API.Services.Implementation;

public class MailService : IMailService
{
    private const string FromEmail = "info@arxalex.co";
    private const string MailHost = "mail.arxalex.co";
    private const string Domain = $"https://{DomainPretty}";
    private const string DomainPretty = "priceapp.arxalex.co";
    private readonly ILogger<MailService> _logger;

    public MailService(ILogger<MailService> logger)
    {
        _logger = logger;
    }

    public async Task SendRegistrationConfirmEmailAsync(int userId, string email, string token)
    {
        var smtpClient = new SmtpClient(MailHost);
        var message = new MailMessage();
        var link = $"{Domain}/register/confirm_email?userid={userId}&token={token}";
        var credentials = new NetworkCredential(FromEmail, "nel8~VQ&BE1z");

        message.From = new MailAddress("info@arxalex.co", "priceapp");
        message.To.Add(new MailAddress(email));
        message.Body = @$"<h1>Вітаємо!</h1>
            <p>Нещодавно ви зареєструвались на сайті <a href='{Domain}'>{DomainPretty}</a></p>
            <p>Для завершення реєстрації необхідно підтвердити адресу електронної пошти. Для цього перейдіть за посиланням: <a href='{link}'>{link}</a></p>
            <p>Якщо цей лист надійшов до вас помилково, будь-ласка, напишіть на <a href='mailto:{FromEmail}'>{FromEmail}</a></p>
            <p>Дякуємо та бажаємо гарного дня!</p>
            <p>З повагою,<br>
            <a href='{Domain}'>PriceApp</a><br>
            <a href='{Domain}'><img src='{Domain}/public_resources/icons/priceapp_icon.png' width='100' height='100'/></a><br>
            <a href='mailto:{FromEmail}'>{FromEmail}</a></p>";
        message.IsBodyHtml = true;
        message.Subject = "Підтвердження реєстрації у PriceApp";
        message.BodyEncoding = Encoding.UTF8;
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = credentials;

        try
        {
            await smtpClient.SendMailAsync(message);
        }
        catch (Exception e)
        {
            message.Dispose();
            _logger.LogCritical(
                $"MailService: Something went wrong while while sending confirmation email for user {userId} with email {email}");
            throw new ApplicationException("Something went wrong while sending confirmation email");
        }
    }
}