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
        public static async Task<bool> SendEmail(string receiver, string message, Texture2D screenshot)
        {
            try
            {
                var mailData = LoadJsonData.Load("Secrets/EmailLogin");
                if (mailData == null)
                {
                    Debug.LogError("No valid MailData object returned!");
                    return false;
                }

                SmtpClient smtpClient = new SmtpClient(mailData.server, mailData.port);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(mailData.address);
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
                smtpClient.Credentials = new NetworkCredential(mailData.address, mailData.apiKey);
                await smtpClient.SendMailAsync(mail);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }

        public static async Task<bool> SendEmail(string receiver, string subject, string message, string attachmentPath)
        {
            try
            {
                var mailData = LoadJsonData.Load("Secrets/EmailLogin");
                if (mailData == null)
                {
                    Debug.LogError("No valid MailData object returned!");
                    return false;
                }

                SmtpClient smtpClient = new SmtpClient(mailData.server, mailData.port);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(mailData.address);
                mail.To.Add(new MailAddress(receiver));
                mail.Subject = subject;
                mail.Body = message;
            
                var attachment = new Attachment(attachmentPath, MediaTypeNames.Application.Octet);
                var disposition = attachment.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(attachmentPath);
                disposition.ModificationDate = File.GetLastWriteTime(attachmentPath);
                disposition.ReadDate = File.GetLastAccessTime(attachmentPath);
                mail.Attachments.Add(attachment);

                smtpClient.Timeout = 5000;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(mailData.address, mailData.apiKey);
                await smtpClient.SendMailAsync(mail);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }
    }
}
