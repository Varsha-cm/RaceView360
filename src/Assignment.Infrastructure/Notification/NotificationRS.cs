using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Notification
{
    public class NotificationRS 
    {
        public string eventCode { get; set; }
        public List<EmailData> emailData { get; set; }
    }

    public class EmailData
    {
        public string recipientEmail { get; set; }
        public Dictionary<string, string> metadata { get; set; }
    }
}
