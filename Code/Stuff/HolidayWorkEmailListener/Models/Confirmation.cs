using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using HolidayWorkEmailListener.Objects;
using Newtonsoft.Json;

namespace HolidayWorkEmailListener.Models
{
    public class Confirmation:DbModel
    {
        public static bool Save(string fullName, out ResponseMessage responseMessage)
        {
            Uri uri = new Uri(String.Format("{0}/HolidayWork/SaveConfirm?fullName={1}", OdataServiceUri, fullName));
            string json = String.Empty;//JsonConvert.SerializeObject(fullName);
            bool result = PostJson(uri, json, out responseMessage);
            return result;
        }

        public static void SendEndTime(string email)
        {
            SendMailSmtp("ВРЕМЯ ВЫШЛО заявка на доступ в выходные не принята!", "К сожалению вы не успели вовремя отправить сообщение на добавление заявки на доступ в выходные. Обратитесь в отдел по работе с персоналом, может быть вы еще успеете!",
                false, new[] { new MailAddress(email) }, null, null);
        }

        public static void SendError(string email)
        {
            SendMailSmtp("ОШИБКА при принятии заявки на доступ в выходные!", "При добавлении вашей заявки на доступ в выходные произошла ошибка. Обратитесь в Техподдержку или в отдел по работе с персоналом!",
                false, new[] { new MailAddress(email) }, null, null);
        }

        public static void SendNote(string email)
        {
            SendMailSmtp("Заявка на доступ в выходные успешно принята!", "Заявка на доступ в выходные успешно принята!",
                false, new[] { new MailAddress(email) }, null, null);
            //Ваша заявка на работу в выходные успешно добавлена
        }

        private static MailAddress defaultMailFrom = new MailAddress("UN1T@un1t.group");

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, MailAddress[] mailTo, MailAddress[] hiddenMailTo, MailAddress mailFrom, bool isTest = false)
        {

            if ((mailTo == null || !mailTo.Any()) && (hiddenMailTo == null || !hiddenMailTo.Any())) throw new Exception("Не указаны получатели письма!");

            if (mailFrom == null || String.IsNullOrEmpty(mailFrom.Address)) mailFrom = defaultMailFrom;

            MailMessage mail = new MailMessage();

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            mail.From = mailFrom;

            client.EnableSsl = false;

            if (mailTo != null)
            {
                foreach (MailAddress mailAddress in mailTo)
                {
                    mail.To.Add(mailAddress);
                }
            }
            if (hiddenMailTo != null)
            {
                foreach (MailAddress mailAddress in hiddenMailTo)
                {
                    mail.Bcc.Add(mailAddress);
                }
            }

            if (isTest)
            {
                mail.To.Clear();
                mail.CC.Clear();
                mail.Bcc.Clear();
                mail.Bcc.Add(new MailAddress("anton.rehov@unitgroup.ru"));
                //mail.Bcc.Add(new MailAddress("alexander.medvedevskikh@unitgroup.ru"));
            }

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;
            client.Host = "ums-1";
            client.Send(mail);
        }
    }
}
