using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayWorkEmailListener.Models;
using Microsoft.Exchange.WebServices.Data;
using StuffDelivery.Models;

namespace HolidayWorkEmailListener
{
    class Program
    {
        static ExchangeService Client = new ExchangeService(ExchangeVersion.Exchange2013);

        static void Main(string[] args)
        {
            CatchEmails();
        }

        private static void CatchEmails()
        {
            string login = ConfigurationManager.AppSettings["login"];
            string pass = ConfigurationManager.AppSettings["pass"];
            string mail = ConfigurationManager.AppSettings["mail"];

            Client.Credentials = new WebCredentials(login, pass);

            Client.UseDefaultCredentials = false;

            Client.AutodiscoverUrl(mail, RedirectionUrlValidationCallback);

            ItemView view = new ItemView(1000000000);//Чтобы ничего не пропустить берем миллиард записей сразу ))

            FindItemsResults<Item> findResults = Client.FindItems(WellKnownFolderName.Inbox, view);

            if (findResults.Any())
            {
                Client.LoadPropertiesForItems(findResults, PropertySet.FirstClassProperties);
                bool send = HolidayWork.CheckTodayIsPreHoliday().SendDelivery;

                foreach (Item item in findResults.Items)
                {
                    if (send)
                    {
                        if (item.Subject.Contains("Автоматический ответ:")) continue;
                        string fullName = item.LastModifiedName;
                        if (fullName.Equals("Microsoft Outlook")) continue;

                        Objects.ResponseMessage responseMessage;
                        bool complete = Confirmation.Save(fullName, out responseMessage);
                        //if (!complete) throw new Exception(responseMessage.ErrorMessage);

                        if (DateTime.Now.Hour < 16 || (DateTime.Now.Hour == 16 && DateTime.Now.Minute <= 3))
                        {
                            if (complete)
                            {
                                item.Delete(DeleteMode.SoftDelete);
                                Confirmation.SendNote(responseMessage.Email);
                            }
                            else
                            {
                                Confirmation.SendError(responseMessage.Email);
                            }
                        }
                        else if (DateTime.Now.Hour > 16)
                        {
                            item.Delete(DeleteMode.SoftDelete);
                            Confirmation.SendEndTime(responseMessage.Email);
                        }
                        else
                        {
                            Confirmation.SendError(responseMessage.Email);
                        }
                    }
                    else
                    {
                        item.Delete(DeleteMode.SoftDelete);
                    }
                }
            }
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }
    }
}
