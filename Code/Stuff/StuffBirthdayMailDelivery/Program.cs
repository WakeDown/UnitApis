using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using StuffBirthdayMailDelivery.Models;

namespace StuffBirthdayMailDelivery
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any()) return;

            if (args[0] != null && args[0] == "today")ExecDeliveryBirthdayToday();
            if (args[0] != null && args[0] == "month") ExecDeliveryBirthdayMonth();
        }

        private static void ExecDeliveryBirthdayMonth()
        {
            var emails = Employee.GetFullRecipientList().ToList();
            var birthdays = Employee.GetNextMonthBirthdayList().ToArray();

            if (birthdays.Any())
            {
                StringBuilder mailBody = new StringBuilder();
                mailBody.AppendLine("<div style='font-family: Calibri'>");
                mailBody.AppendLine("<p>Уважаемые коллеги!</p>");
                //mailBody.AppendLine("\r\n");
                string monthName =  new TranslitDate().GetMonthName(DateTime.Now.AddMonths(1).Month);
                mailBody.AppendLine(String.Format("<p>В {0} месяце свои дни рождения празднуют:</p>",monthName));
                //mailBody.AppendLine("\r\n");
                string stuffUri = ConfigurationManager.AppSettings["stuffUrl"];
                //birthdays = birthdays.OrderBy(e => e.BirthDate.HasValue ? e.BirthDate.Value : new DateTime()).ToArray();
                mailBody.AppendLine("<table style='font-family: Calibri'>");
                foreach (Employee emp in birthdays)
                {
                    mailBody.AppendLine(String.Format("<tr><td><p>{0}&nbsp;&nbsp;</p></td><td>{1:dd.MM}</td><tr>", emp.FullName, emp.BirthDate.HasValue ? emp.BirthDate.Value : new DateTime()));
                    //mailBody.AppendLine(String.Format("<tr><td><p><a href='{2}/Employee/Index/{3}'>{0}</a>&nbsp;&nbsp;</p></td><td>{1:dd.MM}</td><tr>", emp.FullName, emp.BirthDate.HasValue ? emp.BirthDate.Value : new DateTime(), stuffUri, emp.Id));
                }
                mailBody.AppendLine("</table>");
                List<MailAddress> recipients = new List<MailAddress>();
                foreach (string email in emails)
                {
                    //recipients.Add(new MailAddress(email));
                    recipients.Add(new MailAddress("anton.rehov@unitgroup.ru"));
                }
                mailBody.AppendLine("</div>");
                SendMailSmtp(String.Format("Дни рождения в {0}", monthName), mailBody.ToString(), true, recipients.ToArray(), defaultMailFrom);
            }
        }

        private static void ExecDeliveryBirthdayToday()
        {
            var emails = Employee.GetFullRecipientList().ToList();
            var birthdays = Employee.GetTodayBirthdayList().ToArray();

            if (birthdays.Any())
            {
                foreach (var emp in birthdays)
                {
                    emails.RemoveAll(e => e.Equals(emp.Email));
                }

                StringBuilder mailBody = new StringBuilder();
                mailBody.AppendLine("<div style='font-family: Calibri'>");
                mailBody.AppendLine("<p>Доброе утро!</p>");
                //mailBody.AppendLine("\r\n");
                mailBody.AppendLine("<p>Уважаемые коллеги, сегодня день рождения у следующих сотрудников:</p>");
                //mailBody.AppendLine("\r\n");
                string stuffUri = ConfigurationManager.AppSettings["stuffUrl"];
                birthdays = birthdays.OrderBy(e => e.FullName).ToArray();
                
                foreach (Employee emp in birthdays)
                {
                    //mailBody.AppendLine(String.Format("<p><a href='{3}/Employee/Index/{4}'>{0}</a> - {1} | {2}</p>", emp.FullName, emp.Department.Name, emp.Position.Name, stuffUri, emp.Id));
                    mailBody.AppendLine(String.Format("<p>{0}&nbsp;&nbsp;-&nbsp;&nbsp;{1} | {2}</p>", emp.FullName, emp.Department.Name, emp.Position.Name));
                }
               
                List<MailAddress> recipients = new List<MailAddress>();
                foreach (string email in emails)
                {
                    recipients.Add(new MailAddress(email));
                }
                mailBody.AppendLine("</div>");
                SendMailSmtp("Дни рождения сегодня", mailBody.ToString(), true, recipients.ToArray(), defaultMailFrom);
            }
        }
        private static MailAddress defaultMailFrom = new MailAddress("UN1T@un1t.group");
        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, MailAddress[] mailTo, MailAddress mailFrom)
        {
            if (!mailTo.Any()) throw new Exception("Не указаны получатели письма!");

            if (String.IsNullOrEmpty(mailFrom.Address)) mailFrom = defaultMailFrom;

            MailMessage mail = new MailMessage();

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            mail.From = mailFrom;

            client.EnableSsl = false;

            foreach (MailAddress mailAddress in mailTo)
            {
                mail.To.Add(mailAddress);
            }
            //mail.To.Clear();
            //mail.To.Add(new MailAddress("anton.rehov@unitgroup.ru"));
            //mail.To.Add(new MailAddress("alexander.medvedevskikh@unitgroup.ru"));

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;

            client.Host = "ums-1";

            client.Send(mail);
        }
    }
}
