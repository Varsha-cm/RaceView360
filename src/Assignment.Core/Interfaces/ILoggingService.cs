using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment.Infrastructure.Interfaces;
using Assignment.Core.ThirdPartyModels;

namespace Assignment.Api.Interfaces
{
    public interface ILoggingService
    {
        public Task LogEventAsync(object logData, AuditLogRQ audit);
        public Task LogEventHelperAsync(object userRequest, string eventCode, string objectName, string objectCode, string errorTarget, string actionType, string executedBy, object logData);
    }

}
