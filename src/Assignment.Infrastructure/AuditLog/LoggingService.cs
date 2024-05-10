using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Assignment.Api.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using Amazon.Runtime;
using System.Net;
using Assignment.Core.ThirdPartyModels;
using Assignment.Infrastructure.Raiden;
using Amazon.CloudWatchLogs.Model.Internal.MarshallTransformations;

namespace Assignment.Infrastructure.AuditLog
{
    public class LoggingService : ILoggingService
    {
        //private readonly LoggingService loggingService;
        private readonly Serilog.Core.Logger logger;
        // private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly IRaidenService _raidenService;

        public LoggingService(IHttpClientFactory httpClientFactory, HttpClient httpClient, IRaidenService raidenService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
            _raidenService = raidenService;
        }
        public async Task LogEventAsync(object logData, AuditLogRQ audit)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())

                {
                    var eventCode = GetEventTypeId(audit.EventCode);
                    var logrequest = new AuditLogRQ
                    {
                        EventCode = eventCode,
                        ObjectName = audit.ObjectName,
                        ObjectCode = audit.ObjectCode,
                        targets = audit.targets,
                        ActionType = audit.ActionType,
                        ExecutedBy = audit.ExecutedBy,
                        ExecutedOn = audit.ExecutedOn,
                        Channel = audit.Channel,
                        ServerName = audit.ServerName,
                        ServerIpAddress = audit.ServerIpAddress,
                        LogData = audit.LogData

                    };




                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("KITANA_API_URL"));
                    request.Headers.Add("accept", "*/*");
                    string token = await _raidenService.ApplicationAuthentication();
                    request.Headers.Add("Authorization", token);
                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(logrequest);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Event successfully logged.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to log event. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while logging event: {ex.Message}");
            }
        }

        public async Task LogEventHelperAsync(object userRequest, string eventCode, string objectName, string objectCode, string errorTarget, string actionType, string executedBy, object logData)
        {
                await LogEventAsync(userRequest, new AuditLogRQ
                {
                    EventCode = eventCode,
                    ObjectName = objectName,
                    ObjectCode = objectCode,
                    targets = new Dictionary<string, string> { { "Errors", errorTarget } },
                    ActionType = actionType,
                    ExecutedBy = executedBy,
                    ExecutedOn = DateTime.Now,
                    LogData = logData,
                });
        }
        private string GetEventTypeId(string eventCode)
        {
            string createorg = Environment.GetEnvironmentVariable("CreateOrganization");
            string editorg = Environment.GetEnvironmentVariable("EditOrganization");
            string deactivateorg = Environment.GetEnvironmentVariable("DeactivateOrganization");
            string deactivateapp = Environment.GetEnvironmentVariable("DeactivateApplication");
            string createapp = Environment.GetEnvironmentVariable("CreateApplication");
            string editapp = Environment.GetEnvironmentVariable("EditApplication");
            string orgFailures = Environment.GetEnvironmentVariable("OrganizationFailure");
            string appFailures = Environment.GetEnvironmentVariable("ApplicationFailure");            
            string moduleEnable = Environment.GetEnvironmentVariable("moduleEnable");            
            string moduleDisable = Environment.GetEnvironmentVariable("moduleDisable");            
            string moduleActivityFailure = Environment.GetEnvironmentVariable("moduleActivityFailure");            
            string createsuperadmin = Environment.GetEnvironmentVariable("AddSuperAdmin");
            string setrolesuperadmin = Environment.GetEnvironmentVariable("Setrolesuperadmin");
            string createorguser = Environment.GetEnvironmentVariable("AddOrgUser");
            string deactivateorguser = Environment.GetEnvironmentVariable("DeactivateOrgUser");
            string setrolefororguser = Environment.GetEnvironmentVariable("SetRoleforOrgUser");
            string createappuser = Environment.GetEnvironmentVariable("AddAppUser");
            string deactivateappuser = Environment.GetEnvironmentVariable("DeactivateAppUser");
            string setroleforappuser = Environment.GetEnvironmentVariable("SetRoleforAppUser");
            string userfailures = Environment.GetEnvironmentVariable("UserFailures");
            switch (eventCode)
            {
                case "createorg":
                    return createorg;
                case "editorg":
                    return editorg;
                case "deactivateorg":
                    return deactivateorg;
                case "deactivateapp":
                    return deactivateapp;
                case "editapp":
                    return editapp;
                case "createapp":
                    return createapp;
                case "orgFailures":
                    return orgFailures;
                case "appFailures":
                    return appFailures;
                case "createsuperadmin":
                    return createsuperadmin;
                case "Setrolesuperadmin":
                    return setrolesuperadmin;
                case "createorguser":
                    return createorguser;
                case "deactivateorguser":
                    return deactivateorguser;
                case "SetRoleforOrgUser":
                    return setrolefororguser;
                case "createappuser":
                    return createappuser;
                case "deactivateappuser":
                    return deactivateappuser;
                case "SetRoleforAppUser":
                    return setroleforappuser;
                case "userfailures":
                    return userfailures;
                case "moduleEnable":
                    return moduleEnable;
                case "moduleDisable":
                    return moduleDisable;
                case "moduleActivityFailure":
                    return moduleActivityFailure;
                default:
                    return null;
            }
        }
    }


}
