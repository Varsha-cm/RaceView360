using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces
{
    public interface INotificationService
    {
        public Task<string> SendNotification(string model);
        public Task<string> OrganizationNotification(string userEmail, Organizations rs,string eventName);
        public Task<string> ApplicationNotification(string userEmail, Applications rs, Organizations org, string eventName);
        public Task<string> SendGoogleInviteAsync(string email, string eventName);
        public Task<string> ModuleNotificationAsync(string moduleName, Applications app, string userEmail, string status, string eventName);

        public Task<string> UserNotification(string userEmail, Users user, string eventName);
    }
}
