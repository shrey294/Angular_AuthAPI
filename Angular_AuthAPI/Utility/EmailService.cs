using Angular_AuthAPI.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Angular_AuthAPI.Utility
{
	public class EmailService : IEmail
	{
		private readonly IConfiguration _config;
		public EmailService(IConfiguration configuration)
		{
			_config = configuration;
		}
		public void sendEmail(EmailModel emailModel)
		{
			var emailMessage = new MimeMessage();
			var From = _config["EmailSettings:From"];
			emailMessage.From.Add(new MailboxAddress("Demo", From));
			emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
			emailMessage.Subject = emailModel.Subject;
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = string.Format(emailModel.Content)
			};
			using (var Client = new SmtpClient())
			{
				try
				{
					Client.Connect(_config["EmailSettings:SmtpServer"],465,true);
					Client.Authenticate(_config["EmailSettings:From"], _config["EmailSettings:Password"]);
					Client.Send(emailMessage);
				}
				catch (Exception ex)
				{
					throw;
				}
				finally
				{
					Client.Disconnect(true);
					Client.Dispose();
				}
			}
		}
	}
}
