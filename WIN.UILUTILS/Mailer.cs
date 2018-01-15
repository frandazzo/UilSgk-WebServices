using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MailBee.Mime;
using MailBee.SmtpMail;

namespace WIN.UILUTILS
{
    public class Mailer  
    {
        private string _key = "MN800-834B65E74B234B0F4B3C80F1588C-9111";
        private string _pickupFolder = ConfigurationManager.AppSettings["mailbeePickFolder"];

        public Mailer()
        {
            Initialize ();
        }
        private void Initialize()
        {
            MailBee.Global.LicenseKey = _key;
        }

        public void SendMail(string from, string fromAsString, string to, string toAsString, string subject, string body, bool important, IList<string> attacments)
        {
            Smtp mailer = new Smtp();

            if (important)
            {
                mailer.Message.Priority = MailPriority.High;
                mailer.Message.Importance = MailPriority.High;
                mailer.Message.Sensitivity = MailSensitivity.Confidential;
            }

            if (!string.IsNullOrEmpty(fromAsString))
                mailer.Message.From.DisplayName = fromAsString;

            mailer.Message.From.Email = from;

            mailer.Message.To.Add(to, toAsString);


            mailer.Message.Subject = subject;
            mailer.Message.BodyHtmlText = body ;

            foreach (string item in attacments)
            {
                mailer.AddAttachment(item);
            }


            mailer.SubmitToPickupFolder(_pickupFolder, false);

        }


    }
}
