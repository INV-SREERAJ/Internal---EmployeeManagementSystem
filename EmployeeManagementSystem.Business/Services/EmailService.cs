using EmployeeManagementSystem.Business.Configuration;
using EmployeeManagementSystem.Business.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace EmployeeManagementSystem.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task WelcomeEmailAsync(string email, string fullName, string tempPassword)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _emailSettings.FromName,
                _emailSettings.FromEmail));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Welcome to Employee Management System";

            message.Body = new TextPart("html")
            {
                Text = $@"
                <h2>Welcome, {fullName}!</h2>

                <p>Your account has been created successfully.</p>

                <p><strong>Email:</strong> {email}</p>

                <p><strong>Temporary Password:</strong> {tempPassword}</p>

                <p>Please login and change your password immediately.</p>

                <br/>

                <p>Regards,</p>
                <p>Employee Management System</p>"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _emailSettings.Host,
                _emailSettings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _emailSettings.Username,
                _emailSettings.Password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}
