using MimeKit;
using MyHTTPServer.config;
using MyHTTPServer.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace MyHTTPServer.services
{
    public class EmailSenderService:IEmailSenderService
    {
        public EmailSenderService() { }
        public async Task SendEmailAsync(Form objForm)
        {
            var appSettings = HttpServer.GetAppSettings(); 
            //MailAddress from = new MailAddress(appSettings.EmailAdress, "Klim", System.Text.Encoding.UTF8);
            //MailAddress to = new MailAddress("mklim04@mail.ru");
            //MailMessage m = new MailMessage(from, to);
            ////m.Attachments.Add(new Attachment(""));
            //m.Subject = "Homework Form";
            //m.SubjectEncoding = System.Text.Encoding.UTF8;
            //m.Body = form.ToString();
            //m.BodyEncoding = System.Text.Encoding.UTF8;
            //Console.WriteLine("readen form");
            //SmtpClient smtp = new SmtpClient("smtp.mail.ru", 465);
            //smtp.Credentials = new NetworkCredential(appSettings.EmailAdress, appSettings.EmailPassword);
            //smtp.EnableSsl = true;
            //Console.WriteLine("Trying to send Email");
            //await smtp.SendMailAsync(m);

            try
            {
                //FormAnswer form = (FormAnswer)objForm;
                Console.WriteLine("Trying to send Email");
                using var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Matveev", appSettings.EmailAdress));
                emailMessage.To.Add(new MailboxAddress("", objForm.NWLink.Trim()));//Почта на которую будет отправленно письмо
                emailMessage.Subject = "Homework form";

                BodyBuilder builder = new BodyBuilder();
                builder.TextBody = objForm.ToString();
                // We may also want to attach a calendar event for Monica's party...
                builder.Attachments.Add(@"C:\Users\nazig\source\repos\3семестр\ConsoleApp1.zip");
                emailMessage.Body = builder.ToMessageBody();
                Console.WriteLine("Created email");
                //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                //{
                //    Text = form.ToString()
                //};


                Console.WriteLine("Sending Email");
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.mail.ru", 465, true);
                    await client.AuthenticateAsync(appSettings.EmailAdress, appSettings.EmailPassword);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                    Console.WriteLine("Connected smtp");
                }

                Console.WriteLine("Email Sended");
            }
            catch
            {
                //throw new Exception("Sending email caused error");
            }
        }
    }
}
