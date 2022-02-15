// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MappingEdu.Service.Email
{
    public interface IEmailService
    {
        Task SendEmail(string from, string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        public async Task SendEmail(string from, string to, string subject, string body)
        {
            if (Configuration.Email.DisableSSLCertificateCheck)
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var mail = new MailMessage {From = new MailAddress(from)};
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            var client = new SmtpClient(Configuration.Email.ServerAddress, Configuration.Email.Port)
            {
                Credentials = new NetworkCredential(Configuration.Email.Username, Configuration.Email.Password),
                EnableSsl = Configuration.Email.EnableSSL,
                Port = Configuration.Email.Port
            };

            await client.SendMailAsync(mail);
        }
    }
}
