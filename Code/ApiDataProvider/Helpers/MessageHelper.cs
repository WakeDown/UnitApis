using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using DataProvider.Models.SpeCalc;
using DataProvider.Objects;

namespace DataProvider.Helpers
{
    public class MessageHelper
    {
        private static MailAddress defaultMailFrom = new MailAddress("UN1T@un1t.group");
        public static string ConfigureExceptionMessage(Exception ex)
        {
            var st = new StackTrace(ex, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();

            return String.Format("{{\"errorMessage\":\"{0} {2}: {1}\"}}", ex.Source, ex.Message.Replace("\"", ""), line);
        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, string mailTo, string hiddenMailTo = null,
            string mailFrom = null, AttachmentFile file = null, bool isTest = false)
        {
            SendMailSmtp(subject, body, isBodyHtml, new[] { mailTo }, new [] { hiddenMailTo }, mailFrom, file, isTest: isTest);
        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, IEnumerable<string> mailTo, IEnumerable<string> hiddenMailTo = null,
            string mailFrom = null, AttachmentFile file = null, bool isTest = false)
        {
            var recipients = new List<MailAddress>();
            if (mailTo != null)
            {
                foreach (var email in mailTo)
                {
                    if (String.IsNullOrEmpty(email)) continue;
                    recipients.Add(new MailAddress(email));
                }
            }

            var recHidden = new List<MailAddress>();
            if (hiddenMailTo != null)
            {
                foreach (var email in hiddenMailTo)
                {
                    if (String.IsNullOrEmpty(email)) continue;
                    recHidden.Add(new MailAddress(email));
                }
            }

            if (String.IsNullOrEmpty(mailFrom)) mailFrom = defaultMailFrom.Address;
            SendMailSmtp(subject, body, isBodyHtml, recipients.ToArray(),recHidden.ToArray(), new MailAddress(mailFrom), file, isTest: isTest);
        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, MailAddress[] mailTo, MailAddress[] hiddenMailTo = null, MailAddress mailFrom = null, AttachmentFile file = null, bool isTest = false)
        {
            if (!mailTo.Any() && (hiddenMailTo == null || !hiddenMailTo.Any())) throw new Exception("Не указаны получатели письма!");

            if (mailFrom == null || String.IsNullOrEmpty(mailFrom.Address)) mailFrom = defaultMailFrom;

            MailMessage mail = new MailMessage();

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            mail.From = mailFrom;

            client.EnableSsl = false;

            if (ConfigurationManager.AppSettings["Environment"].Equals("Production") && !isTest)
            {
                if (mailTo != null)
                {
                    foreach (MailAddress mailAddress in mailTo)
                    {
                        if (String.IsNullOrEmpty(mailAddress.Address)) continue;
                        mail.To.Add(mailAddress);
                    }
                }
                if (hiddenMailTo != null)
                {
                    foreach (MailAddress mailAddress in hiddenMailTo)
                    {
                        if (String.IsNullOrEmpty(mailAddress.Address)) continue;
                        mail.CC.Add(mailAddress);
                    }
                }
            }
            else
            {
                string[] testMails = ConfigurationManager.AppSettings["Emails4Test"].Split('|');
                foreach (var email in testMails)
                {
                    if (String.IsNullOrEmpty(email)) continue;
                    mail.To.Add(email);
                }
                
                body += "\r\n";
                if (mailTo != null)
                {
                    foreach (var mailAddress in mailTo)
                    {
                        body += "\r\n" + mailAddress.Address;
                    }
                }
                //Hidden recipients
                if (hiddenMailTo != null)
                {
                    foreach (var mailAddress in hiddenMailTo)
                    {
                        body += "\r\n" + mailAddress.Address;
                    }
                }
            }

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;

            client.Host = "ums-1";

            if (file != null && file.Data.Length > 0)
            {
                MemoryStream stream = new MemoryStream(file.Data);
                Attachment attachment = new Attachment(stream, file.FileName, file.DataMimeType);
                mail.Attachments.Add(attachment);
            }

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                
                throw new Exception(String.Format("Сообщение не было отправлено. Текст ошибки - {0}", ex.Message));
            }
        }
    }
}