using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Core.ThirdPartyModels
{
    public class AuditLogRQ
    {
        public string EventCode { get; set; }
        public string ObjectName { get; set; }
        public string ObjectCode { get; set; }
        public Dictionary<string, string> targets { get; set; }
        public string ActionType { get; set; }
        public string ExecutedBy { get; set; }
        public DateTime ExecutedOn { get; set; }
        public string Channel { get; set; }
        public string ServerName { get; set; }
        public string ServerIpAddress { get; set; }
        public object LogData { get; set; }
     

    }
}
