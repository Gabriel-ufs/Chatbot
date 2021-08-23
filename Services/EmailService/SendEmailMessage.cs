using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CoreBot.Services.EmailService
{
    public class SendEmailMessage
    {
        public void SendEmail(string exception)
        {
            string to = "sergio.barbosa@zdoc.com.br";
            string from = "pedro.ailan@zdoc.com.br";

            MailMessage message = new MailMessage(from, to)
            {
                Subject = "Exceções Lançadas",
                Body = exception
            };

            List<string> ListEmails = new List<string>
            {
                "sergio.barbosa@zdoc.com.br",
                "pedro.ailan@zdoc.com.br",
                "Felipe.falcao@zdoc.com.br"
            };


            using (var client = new SmtpClient("smtp.gmail.com"))
            {
                client.EnableSsl = true; // SSL
                client.Port = 587;       // Porta SSL
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;


                client.Credentials = new NetworkCredential(from, "detran.teste");
                try
                {
                    foreach (string Email in ListEmails)
                    {
                        client.Send(from, Email, message.Subject, message.Body);
                    }
                }
                catch (Exception e)
                {

                    throw e;
                }


            };
        }
    }
}
