using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using SelectPdf;
using Stuff.Objects;
using StuffDelivery.Models;
using StuffDelivery.Objects;

namespace StuffDelivery
{
    class Program
    {
        private static MailAddress defaultMailFrom = new MailAddress("UN1T@un1t.group");
        private static readonly string stuffWebLeftPartUrl = "http://portal.unitgroup.ru";

        static void Main(string[] args)
        {
            if (!args.Any()) return;

            if (args[0] != null && args[0] == "today") ExecDeliveryBirthdayToday();
            if (args[0] != null && args[0] == "month") ExecDeliveryBirthdayMonth();
            if (args[0] != null && args[0] == "newemps") ExecDeliveryNewEmployees();
            if (args[0] != null && args[0] == "hldwrk") HolidayWorkDelivery();
            if (args[0] != null && args[0] == "hldwrklist") SendHolidayWorkConfirmList();
            if (args[0] != null && args[0] == "itbudget") SendItBudget();
        }

        public static void SendItBudget()
        {
            HtmlToPdf converter = new HtmlToPdf();
            string url = String.Format("{0}/Report/ItBudgetView", stuffWebLeftPartUrl);
            PdfDocument doc = converter.ConvertUrl(url);
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);

            var file = new AttachmentFile() { Data = stream.ToArray(), FileName = "it-budget.pdf", DataMimeType = MediaTypeNames.Application.Pdf };
            //var recipients = new MailAddress[] {new MailAddress("Ildar.Gimaltdinov@unitgroup.ru"), new MailAddress("Svetlana.Demysheva@unitgroup.ru"), new MailAddress("Larisa.Ganishina@unitgroup.ru") };
            var recipientsStr = ConfigurationManager.AppSettings["ItBudgetRecipients"].Split('|');
            //string monthName = new TranslitDate().GetMonthName(DateTime.Now.AddMonths(1).Month);
            var recipients = (from s in recipientsStr where !String.IsNullOrEmpty(s) select new MailAddress(s)).ToArray();
            string monthName = TranslitDate.GetMonthNameImenit(DateTime.Now.AddMonths(-1).Month);
            SendMailSmtp(String.Format("ИТ бюджет за {0} {1}", monthName, DateTime.Now.AddMonths(-1).Year), "Добрый день. ИТ бюджет во вложении", true, null, recipients, null, file);
        }

        public static void SendHolidayWorkConfirmList()
        {
            var wd = HolidayWork.CheckTodayIsPreHoliday();
            if (!wd.SendDelivery) return;
            string[] list = HolidayWork.GetConfirms();
            var emails = Employee.GetHolidayWorkDeliveryRecipientList();

            StringBuilder mailBody = new StringBuilder();
            mailBody.AppendLine("<div style='font-family: Calibri'>");
            mailBody.AppendLine("<table style='border-collapse: collapse; border: 1px solid black;'>");
            mailBody.AppendLine("<tr style='border: 1px solid black;'><th style='border: 1px solid black;padding: 5px;'>№</th><th style='border: 1px solid black;padding: 5px;'>ФИО сотрудника</th></tr>");
            int i = 0;
            foreach (string s in list)
            {
                i++;
                mailBody.AppendLine(
                    $"<tr style='border: 1px solid black;'><td style='border: 1px solid black;padding: 5px;'>{i}</td><td style='border: 1px solid black;padding: 5px;'>{s}</td></tr>");
            }
            mailBody.AppendLine("</table>");
            mailBody.AppendLine("</div>");
            var recipients = GetMailAddressesFromList(emails);
            SendMailSmtp("Список сотрудников (доступ в выходные)", mailBody.ToString(), true, null, recipients, null);
        }

        public static void HolidayWorkDelivery()
        {
            var wd = HolidayWork.CheckTodayIsPreHoliday();

            if (!wd.SendDelivery) return;

            var emails = Employee.GetFullRecipientList("EKB");

            StringBuilder mailBody = new StringBuilder();
            mailBody.AppendLine("<div style='font-family: Calibri'>");
            mailBody.AppendLine("<p>Уважаемые коллеги!</p>");
            if (wd.IsSundayOnly)
            {
                mailBody.AppendLine(
                    "<p>Кто планирует выйти на работу в воскресенье, запишитесь до 16:00, отправив ответ на это письмо.</p>");
            }
            else
            {
                mailBody.AppendLine("<p>Кто планирует выйти на работу в праздники, запишитесь до 16:00, отправив ответ на это письмо.</p>"
                    //String.Format("<p>Кто планирует выйти на работу в праздники с {0:dd.MM} по {1:dd.MM}, можете записаться отправив ответное письмо (можно пустое)!</p>", wd.DateStart, wd.DateEnd)
                    );
            }
            mailBody.AppendLine("</div>");
            var recipients = GetMailAddressesFromList(emails);
            var holidayWorkMailFrom = new MailAddress("holiday-work@unitgroup.ru");
            SendMailSmtp("Работа в выходные", mailBody.ToString(), true, null, recipients, holidayWorkMailFrom);
        }

        public static MailAddress[] GetMailAddressesFromList(IEnumerable<string> emails)
        {
            List<MailAddress> recipients = new List<MailAddress>();
            foreach (string email in emails)
            {
                recipients.Add(new MailAddress(email));
            }

            return recipients.ToArray();
        }

        private static void ExecDeliveryNewEmployees()
        {
            var emails = Employee.GetFullRecipientList();
            var newbie = Employee.GetNewbieList().ToArray();

            if (newbie.Any())
            {
                StringBuilder mailBody = new StringBuilder();
                mailBody.AppendLine("<div style='font-family: Calibri'>");
                mailBody.AppendLine("<p>Уважаемые коллеги!</p>");
                //mailBody.AppendLine("\r\n");
                mailBody.AppendLine(String.Format("<p>У нас новые сотрудники:</p>"));
                //mailBody.AppendLine("\r\n");
                string stuffUri = ConfigurationManager.AppSettings["stuffUrl"];
                //birthdays = birthdays.OrderBy(e => e.BirthDate.HasValue ? e.BirthDate.Value : new DateTime()).ToArray();
                mailBody.AppendLine("<table style='font-family: Calibri'>");
                foreach (Employee emp in newbie)
                {
                    mailBody.AppendLine(
                        String.Format(
                            "<tr><td><p><a href='{1}/Employee/Index/{2}'>{0}</a>&nbsp;&nbsp;</p></td><td>{3}</td><tr>",
                            emp.FullName, stuffUri, emp.Id, emp.Position.Name));
                }
                mailBody.AppendLine("</table>");

                var recipients = GetMailAddressesFromList(emails);
                //recipients = new [] {new MailAddress("anton.rehov@unitgroup.ru")};
                mailBody.AppendLine("</div>");
                SendMailSmtp(String.Format("Новые сотрудники"), mailBody.ToString(), true, null, recipients, defaultMailFrom);

                ResponseMessage responseMessage;
                //dep.Creator = new Employee(){AdSid = GetCurUser().Sid};
                bool complete = Employee.SetNewbieDeliverySend(out responseMessage, newbie);
                //if (!complete) throw new Exception(responseMessage.ErrorMessage);
            }
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
                string monthName = TranslitDate.GetMonthNamePredl(DateTime.Now.AddMonths(1).Month);
                mailBody.AppendLine(String.Format("<p>В {0} месяце свои дни рождения празднуют:</p>", monthName));
                //mailBody.AppendLine("\r\n");
                string stuffUri = ConfigurationManager.AppSettings["stuffUrl"];
                //birthdays = birthdays.OrderBy(e => e.BirthDate.HasValue ? e.BirthDate.Value : new DateTime()).ToArray();
                mailBody.AppendLine("<table style='font-family: Calibri'>");
                foreach (Employee emp in birthdays)
                {
                    //mailBody.AppendLine(String.Format("<tr><td><p>{0}&nbsp;&nbsp;</p></td><td>{1:dd.MM}</td><tr>", emp.FullName, emp.BirthDate.HasValue ? emp.BirthDate.Value : new DateTime()));
                    mailBody.AppendLine(String.Format("<tr><td><p><a href='{2}/Employee/Index/{3}'>{0}</a>&nbsp;&nbsp;</p></td><td>{1:dd.MM}</td><tr>", emp.FullName, emp.BirthDate.HasValue ? emp.BirthDate.Value : new DateTime(), stuffUri, emp.Id));
                }
                mailBody.AppendLine("</table>");
                List<MailAddress> recipients = new List<MailAddress>();
                foreach (string email in emails)
                {
                    recipients.Add(new MailAddress(email));
                    //recipients.Add(new MailAddress("anton.rehov@unitgroup.ru"));
                }
                mailBody.AppendLine("</div>");
                SendMailSmtp(String.Format("Дни рождения в {0}", monthName), mailBody.ToString(), true, null, recipients.ToArray(), defaultMailFrom);
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
                    mailBody.AppendLine(String.Format("<p><a href='{3}/Employee/Index/{4}'>{0}</a> - {1} | {2}</p>", emp.FullName, emp.Department.Name, emp.Position.Name, stuffUri, emp.Id));
                    //mailBody.AppendLine(String.Format("<p>{0}&nbsp;&nbsp;-&nbsp;&nbsp;{1} | {2}</p>", emp.FullName, emp.Department.Name, emp.Position.Name));
                }

                List<MailAddress> recipients = new List<MailAddress>();
                foreach (string email in emails)
                {
                    recipients.Add(new MailAddress(email));
                    //recipients.Add(new MailAddress("anton.rehov@unitgroup.ru"));
                }
                mailBody.AppendLine("</div>");
                SendMailSmtp("Дни рождения сегодня", mailBody.ToString(), true, null, recipients.ToArray(), defaultMailFrom);
            }
        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, MailAddress[] mailTo, MailAddress[] hiddenMailTo, MailAddress mailFrom, AttachmentFile file = null, bool isTest = false)
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

            if (file != null && file.Data.Length > 0)
            {
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    ms.Read(file.Data, 0, file.Data.Length);

                MemoryStream stream = new MemoryStream(file.Data);

                Attachment attachment = new Attachment(stream, file.FileName, file.DataMimeType);
                    mail.Attachments.Add(attachment);
                    
                    //client.Send(mail);
                //}

                //Attachment attachment = new Attachment(file.FileName, MediaTypeNames.Application.Octet);
                //ContentDisposition disposition = attachment.ContentDisposition;
                //disposition.CreationDate = File.GetCreationTime(file.FileName);
                //disposition.ModificationDate = File.GetLastWriteTime(file.FileName);
                //disposition.ReadDate = File.GetLastAccessTime(file.FileName);
                //disposition.FileName = Path.GetFileName(file.FileName);
                //disposition.Size = new FileInfo(file.FileName).Length;
                //disposition.DispositionType = DispositionTypeNames.Attachment;
                //mail.Attachments.Add(attachment);
            }
            //else
            //{
                client.Send(mail);
            //}


        }
    }
}
