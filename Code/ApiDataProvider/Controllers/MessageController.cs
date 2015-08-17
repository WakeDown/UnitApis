using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace DataProvider.Controllers
{
    public class MessageController : ApiController
    {
        private MailAddress defaultMailFrom = new MailAddress("UN1T@un1t.group");

        public void SendMailSmtp(string subject, string body, bool isBodyHtml,  MailAddress[] mailTo, MailAddress mailFrom)
        {
            if (mailTo == null || !mailTo.Any()) throw new Exception("Не указаны получатели письма!");

            if (String.IsNullOrEmpty(mailFrom.Address)) mailFrom = defaultMailFrom;

            MailMessage mail = new MailMessage();

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            //if (!String.IsNullOrEmpty(settings.Login))
            //{
            //    client.Credentials = new NetworkCredential(settings.Login, settings.Password);
            //    mail.From = new MailAddress(settings.Login);
            //}
            //else
            //{
            mail.From = mailFrom;
            //}

            client.EnableSsl = false;//settings.EnableSsl;

            foreach (MailAddress mailAddress in mailTo)
            {
                mail.To.Add(mailAddress);
            }
            

            //Шлем копию письма если надо
            //if (!String.IsNullOrEmpty(settings.MailCopyTo))
            //{
            //    mail.CC.Add(new MailAddress(settings.MailCopyTo));
            //}
            //else
            //{
            //    mail.CC.Clear();
            //}

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;

            client.Host = "ums-1";

            client.Send(mail);
        }
    }
}
