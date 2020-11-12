using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Veneka.Indigo.Integration.Email
{
   public static class EmailSender
    {

        public static bool Send(string smptserver,int port, string fromaddress, string toaddress, string subject, string message, string username, string password,out string responsemessage)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(smptserver);

                mail.From = new MailAddress(fromaddress);
                mail.Bcc.Add(toaddress);
                mail.Subject = subject;
                mail.Body = message;
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                SmtpServer.Port = port;
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
                responsemessage = "Email sent successfully.";
                return true;
            }
            catch (Exception ex)
            {
                responsemessage = ex.Message;
                return false;

            }
        }
    }
}

