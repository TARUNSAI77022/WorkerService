//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.Extensions.Configuration;

//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using MimeKit;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//namespace WorkerService1
//{
//    public class EmailSender : BackgroundService
//    {
//        private readonly IConfiguration _config;

//        public EmailSender(IConfiguration config)
//        {
//            _config = config;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                // Do some work here

//                // Send an email
//                var message = new MimeMessage();
//                message.From.Add(new MailboxAddress("erbor", "notifications@earbor.com"));
//                message.To.Add(new MailboxAddress("srikanth", "kandesrikanthcoe@gmail.com"));
//                message.Subject = "Test Email";
//                message.Body = new TextPart("plain")
//                {
//                    Text = "Hello, this is a test email."
//                };

//                using (var client = new SmtpClient())
//                {
//                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
//                    await client.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
//                    await client.SendAsync(message);
//                    await client.DisconnectAsync(true);
//                }

//                // Wait for some time before sending the next email
//                await Task.Delay(6000, stoppingToken);
//            }
//        }
//    }
//}
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using OfficeOpenXml;

namespace EmailWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly IHostApplicationLifetime _appLifetime;

        public Worker(ILogger<Worker> logger, IConfiguration config, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _config = config;
            _appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get email settings from configuration
                var displayName = _config["MailSettings:DisplayName"];
                var fromEmail = _config["MailSettings:From"];
                var host = _config["MailSettings:Host"];
                var port = int.Parse(_config["MailSettings:Port"]);
                var userName = _config["MailSettings:UserName"];
                var password = _config["MailSettings:Password"];
                var useSSL = bool.Parse(_config["MailSettings:UseSSL"]);
                var useStartTls = bool.Parse(_config["MailSettings:UseStartTls"]);

                try
                {
                    //// Generate sample Excel data
                    //var fileName = "SampleData.xlsx";
                    //var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                    // Generate sample Excel data
                    var fileName = "SampleData.xlsx";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Sample Worksheet");
                        worksheet.Cells["A1"].Value = "Sample Data";
                        worksheet.Cells["A2"].Value = "123";
                        worksheet.Cells["A3"].Value = "456";
                        worksheet.Cells["A4"].Value = "789";
                        worksheet.Cells["B2"].Value = "abc";
                        worksheet.Cells["B3"].Value = "def";
                        worksheet.Cells["B4"].Value = "ghi";
                        package.SaveAs(new FileInfo(filePath));
                    }


                    // Create email message
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(displayName, fromEmail));
                    message.To.Add(new MailboxAddress("Recipient Name", "kandesrikanthcoe@gmail.com"));
                    message.Subject = "hii  srikanth";
                    //message.Body = new TextPart("plain") { Text = "This is a test email message." };
                    // builder = new BodyBuilder();
                    //builder.TextBody = "Please find attached sample data in Excel format.";
                    message.Body = new TextPart("plain") { Text = "This is a test email message with an Excel attachment." };

                    var attachment = new MimePart("application", "vnd.ms-excel")
                    {
                        Content = new MimeContent(File.OpenRead(filePath), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        FileName = "SampleData.xlsx"
                    };
                    message.Body = new Multipart("mixed") { message.Body, attachment };

                    // Send email using MailKit SMTP client
                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(host, port, useSSL);
                        await client.AuthenticateAsync(userName, password);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }

                    _logger.LogInformation("Email sent successfully.");

                    await Task.Delay(60000, stoppingToken); // Wait for 1 minute before sending the next email
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email.");
                }
            }
        }
    }
}


