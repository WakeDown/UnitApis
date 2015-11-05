using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

            return String.Format("{{\"errorMessage\":\"{0}\"}}", ex.Message.Replace("\"", ""));
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

            SmtpClient client = new SmtpClient("smtp.office365.com",587);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("delivery@unitgroup.ru", "pRgvD7TL");

            mail.From = new MailAddress("delivery@unitgroup.ru", String.Empty, System.Text.Encoding.UTF8);

            //client.EnableSsl = false;

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
            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            

            if (file != null && file.Data.Length > 0)
            {
                MemoryStream stream = new MemoryStream(file.Data);
                Attachment attachment = new Attachment(stream, file.FileName, file.DataMimeType);
                mail.Attachments.Add(attachment);
            }

            try
            {
                client.SendAsync(mail, mail);
            }
            catch (Exception ex)
            {
                
                throw new Exception(String.Format("Сообщение не было отправлено. Текст ошибки - {0}", ex.Message));
            }
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the message we sent
            MailMessage msg = (MailMessage)e.UserState;

            if (e.Cancelled)
            {
                // prompt user with "send cancelled" message 
            }
            if (e.Error != null)
            {
                // prompt user with error message 
            }
            else
            {
                // prompt user with message sent!
                // as we have the message object we can also display who the message
                // was sent to etc 
            }

            // finally dispose of the message
            if (msg != null)
                msg.Dispose();
        }
    }
}