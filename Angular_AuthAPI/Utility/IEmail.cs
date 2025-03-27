using Angular_AuthAPI.Models;

namespace Angular_AuthAPI.Utility
{
	public interface IEmail
	{
		void sendEmail(EmailModel emailModel);
	}
}
