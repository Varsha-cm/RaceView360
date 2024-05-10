using Newtonsoft.Json;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IRaidenService _raidenService;

        public NotificationService(IRaidenService raidenService)
        {
            _raidenService = raidenService;
        }
        public async Task<string> SendNotification(string model)
        {
            var client = new HttpClient();
            string token = await _raidenService.ApplicationAuthentication();
            var request = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("CYRAX_API_URL"));
            request.Headers.Add("accept", "text/plain");
            request.Headers.Add($"Authorization", $"Bearer {token}");
            var content = new StringContent(model, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> OrganizationNotification(string userEmail, Organizations rs,string eventName)
        {
            NotificationRS modelRq = new NotificationRS();
            modelRq.emailData = new List<EmailData>();
            EmailData emailData1 = new EmailData();
            EmailData emailData2 = new EmailData();
            Dictionary<string, string> meta = new Dictionary<string, string>();
            meta["orgcode"] = rs.OrgCode;
            meta["useremail"] = userEmail;
            meta["orgemail"] = rs.OrganizationEmail;
            meta["orgname"] = rs.OrganizationName;
            meta["orgphone"] = rs.OrganizationPhone;
            modelRq.eventCode = GetCyraxEvent(eventName);
            if (rs.OrganizationEmail != null)
            {
                emailData1.recipientEmail = rs.OrganizationEmail;
                emailData1.metadata = (meta);
                modelRq.emailData.Add(emailData1);
            }
            emailData2.recipientEmail = userEmail;
            emailData2.metadata = (meta);
            modelRq.emailData.Add(emailData2);
            var notificationRq = JsonConvert.SerializeObject(modelRq);
            var notificationRs = await SendNotification(notificationRq);
            return notificationRs;
        }

        public async Task <string> ApplicationNotification(string userEmail, Applications rs, Organizations org, string eventName)
        {
            NotificationRS modelRq = new NotificationRS();
            modelRq.emailData = new List<EmailData>();
            EmailData emailData1 = new EmailData();
            EmailData emailData2 = new EmailData();
            EmailData emailData3 = new EmailData();
            Dictionary<string, string> meta = new Dictionary<string, string>();
            meta["appname"] = rs.ApplicationName;
            meta["appemail"] = rs.ApplicationEmail;
            meta["orgcode"] = org.OrgCode;
            meta["appphone"] = rs.Phone;
            meta["appcode"] = rs.AppCode;
            meta["useremail"] = userEmail;
            modelRq.eventCode = GetCyraxEvent(eventName);
            if (rs.ApplicationEmail != null)
            {
                emailData1.recipientEmail = rs.ApplicationEmail;
                emailData1.metadata = (meta);
                modelRq.emailData.Add(emailData1);
            }
            if(org.OrganizationEmail != null)
            {
                emailData3.recipientEmail = org.OrganizationEmail;
                emailData3.metadata = (meta);
                modelRq.emailData.Add(emailData3);
            }
            emailData2.recipientEmail = userEmail;
            emailData2.metadata = (meta);
            modelRq.emailData.Add(emailData2);
            var notificationRq = JsonConvert.SerializeObject(modelRq);
            var notificationRs = await SendNotification(notificationRq);
            return notificationRs;
        }
        public async Task<string> SendGoogleInviteAsync(string email, string eventName)
        {
            NotificationRS modelRq = new NotificationRS();
            modelRq.emailData = new List<EmailData>();
            EmailData emailData1 = new EmailData();
            EmailData emailData2 = new EmailData();
            Dictionary<string, string> meta = new Dictionary<string, string>();
            meta["heading"] = "You Have Invite From Raiden!!!";
            meta["paragraph"] = "click on the link and get authorized to add google signin";
            meta["url"] = Environment.GetEnvironmentVariable("GOOGLE_REDIR_URL");
            meta["aTagText"] = "Click here for redirection";
            modelRq.eventCode = GetCyraxEvent(eventName);

            emailData2.recipientEmail = email;
            emailData2.metadata = (meta);
            modelRq.emailData.Add(emailData2);
            var notificationRq = JsonConvert.SerializeObject(modelRq);
            var notificationRs = await SendNotification(notificationRq);
            return notificationRs;
        }

        public async Task<string> ModuleNotificationAsync(string moduleName, Applications app,string userEmail,string status, string eventName)
        {
            NotificationRS modelRq = new NotificationRS();
            modelRq.emailData = new List<EmailData>();
            EmailData emailData1 = new EmailData();
            EmailData emailData2 = new EmailData();
            Dictionary<string, string> meta = new Dictionary<string, string>();
            meta["moduleName"] = moduleName;
            meta["appcode"] = app.AppCode;
            meta["status"] = status;
            meta["useremail"] = userEmail;
            modelRq.eventCode = GetCyraxEvent(eventName);
            if (app.ApplicationEmail != null)
            {
                emailData1.recipientEmail = app.ApplicationEmail;
                emailData1.metadata = (meta);
                modelRq.emailData.Add(emailData1);
            }           

            emailData2.recipientEmail = userEmail;
            emailData2.metadata = (meta);
            modelRq.emailData.Add(emailData2);
            var notificationRq = JsonConvert.SerializeObject(modelRq);
            var notificationRs = await SendNotification(notificationRq);
            return notificationRs;
        }
        public async Task<string> UserNotification(string userEmail, Users user, string eventName)
        {
            NotificationRS modelRq = new NotificationRS();
            modelRq.emailData = new List<EmailData>();
            EmailData emailData1 = new EmailData();
            EmailData emailData2 = new EmailData();
            Dictionary<string, string> meta = new Dictionary<string, string>();

            meta["FirstName"] = user.FirstName;
            meta["LastName"] = user.LastName;
            meta["email"] = user.Email;
            meta["password"] = user.Password;

            modelRq.eventCode = GetCyraxEvent(eventName);

            if (user.Email != null)
            {
                emailData1.recipientEmail = user.Email;
                emailData1.metadata = meta;
                modelRq.emailData.Add(emailData1);
            }
            emailData2.recipientEmail = userEmail;
            emailData2.metadata = meta;
            modelRq.emailData.Add(emailData2);

            var notificationRq = JsonConvert.SerializeObject(modelRq);
            var notificationRs = await SendNotification(notificationRq);

            return notificationRs;
        }
        public static string GetCyraxEvent(string eventName)
        {
            string createOrgEvent = Environment.GetEnvironmentVariable("CYRAX_ORG_CREATE");
            string deleteOrgEvent = Environment.GetEnvironmentVariable("CYRAX_ORG_DELETE");
            string editOrgEvent = Environment.GetEnvironmentVariable("CYRAX_ORG_EDIT");
            string googleInviteEvent = Environment.GetEnvironmentVariable("CYRAX_GOOGLE_INVITE");
            string cyraxModule = Environment.GetEnvironmentVariable("CYRAX_MODULE");
            string adduser = Environment.GetEnvironmentVariable("CYRAX_USER_INVITE");
            string createAppEvent = Environment.GetEnvironmentVariable("CYRAX_APP_CREATE");
            string editAppEvent = Environment.GetEnvironmentVariable("CYRAX_APP_EDIT");
            string deleteAppEvent = Environment.GetEnvironmentVariable("CYRAX_APP_DELETE");
            switch (eventName)
            {
                case "CYRAX_ORG_CREATE":
                                    return createOrgEvent;
                case "CYRAX_ORG_DELETE":
                                    return deleteOrgEvent;
                case "CYRAX_ORG_EDIT":
                                    return editOrgEvent;
                case "CYRAX_GOOGLE_INVITE":
                                    return googleInviteEvent;
                case "CYRAX_MODULE":
                                    return cyraxModule;
                case "CYRAX_USER_INVITE":
                                    return adduser;
                case "CYRAX_APP_CREATE":
                                    return createAppEvent;
                case "CYRAX_APP_EDIT":
                                    return editAppEvent;
                case "CYRAX_APP_DELETE":
                                   return deleteAppEvent;
                default: 
                                    return "NO_EVENT_FOUND";
            }
        }
    }
}
