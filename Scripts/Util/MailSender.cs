using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using UnityEngine;

namespace cpvrlab_vr_suite.Scripts.Util
{
    public static class MailSender
    {
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SenderEmail = "cpvr.architects@gmail.com";
        private const string Password = "qqkspvmhdslrizea";

        public static async Task<bool> SendEmail(string receiver, string message, Texture2D screenshot)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(SmtpHost, SmtpPort);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(SenderEmail);
                mail.To.Add(new MailAddress(receiver));
                mail.Subject = "VR4Architects Screenshot";
                mail.Body = message;
            
                var memoryStream = new MemoryStream(screenshot.EncodeToPNG());
                memoryStream.Seek(0, SeekOrigin.Begin);
                var data = new Attachment(memoryStream, "Screenshot");
                var disposition = data.ContentDisposition;
                disposition.CreationDate = DateTime.Now;
                disposition.ModificationDate = DateTime.Now;
                disposition.ReadDate = DateTime.Now;
                data.ContentType = new ContentType(MediaTypeNames.Image.Jpeg);
                mail.Attachments.Add(data);

                smtpClient.Timeout = 5000;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(SenderEmail, Password);
                await smtpClient.SendMailAsync(mail);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}
