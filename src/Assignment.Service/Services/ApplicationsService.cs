using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using Assignment.Core.ThirdPartyModels;
using Assignment.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Assignment.Service.Services
{
    public class ApplicationsService
    {
        private readonly IDBApplicationRepository _applicationRepository;
        private readonly IDBOrganization _organizationRepository;
        private readonly IDBProductsRepository _productRepository;
        private readonly ILoggingService _loggingService;
        private readonly INotificationService _notificationService;
        private readonly Serilog.Core.Logger logger;

        public ApplicationsService(IDBApplicationRepository applicationRepository, INotificationService notificationService, IDBProductsRepository productRepository, IDBOrganization organizationRepository, ILoggingService loggingService, Serilog.Core.Logger logger)
        {
            _applicationRepository = applicationRepository;
            _productRepository = productRepository;
            _organizationRepository = organizationRepository;
            _loggingService = loggingService;
            _notificationService = notificationService;
            this.logger = logger;

        }
        public async Task<AppDetailsRS> AddApplicationAsync(ApplicationsRQ applicationsRequest, string userEmail)
        {
            try
            {
                var isValidationSuccessful = ValidateApplicationRequest(applicationsRequest);

                if (!isValidationSuccessful)
                {

                    await _loggingService.LogEventAsync(applicationsRequest, new AuditLogRQ
                    {
                        EventCode = "appFailures",
                        ObjectName = applicationsRequest.ApplicationName,
                        ObjectCode = applicationsRequest.AppCode,
                        targets = new Dictionary<string, string> { { "Application Errors", applicationsRequest.AppCode} },
                        ActionType = "create Application",
                        ExecutedBy = userEmail,
                        ExecutedOn = DateTime.Now,
                        LogData = "Application creation validation checks failed",

                    });
                    logger.Error("Validation error");
                    throw new ArgumentException("Validation error");
                }

                var OrganzationId = await _organizationRepository.GetOrganizationByIdAsync(applicationsRequest.OrgCode);
                var appinfo = await _applicationRepository.GetApplicationDetailsAsync(applicationsRequest.AppCode);
                if (appinfo == null)
                {
                    var ClientId = GenerateClientId();
                    var ClientSecret = GenerateClientSecret();
                    var applicationEntiy = new Applications

                    {
                        OrganizationId = OrganzationId.OrganizationId,
                        ApplicationName = applicationsRequest.ApplicationName,
                        Description = applicationsRequest.Description,
                        FirstName = applicationsRequest.FirstName,
                        LastName = applicationsRequest.LastName,
                        AppCode = applicationsRequest.AppCode,
                        Phone = applicationsRequest.Phone,
                        IsActive = true,
                        ClientId = ClientId,
                        ClientSecret = ClientSecret,
                        ApplicationEmail = applicationsRequest.ApplicationEmail

                    };

                    var output = await _applicationRepository.AddApplicationAsync(applicationEntiy);
                    await _notificationService.ApplicationNotification(userEmail, applicationEntiy, OrganzationId, "CYRAX_APP_CREATE");
                    var output2 = await _productRepository.AddProductsApplication(output, 1);
                    var output3 = await _productRepository.AddProductsApplication(output, 2);
                    AppDetailsRS rs = new AppDetailsRS()
                    {
                        ApplicationId = output,
                        OrgCode = applicationsRequest.OrgCode.ToUpper(),
                        ApplicationName = applicationsRequest.ApplicationName,
                        Phone = applicationsRequest.Phone,
                        IsActive = true,
                        AppCode = applicationsRequest.AppCode,
                        Description = applicationsRequest.Description,
                        FirstName = applicationsRequest.FirstName,
                        LastName = applicationsRequest.LastName,
                        ClientId = ClientId,
                        ClientSecret = ClientSecret,
                        ApplicationEmail = applicationsRequest.ApplicationEmail
                    };


                    await _loggingService.LogEventAsync(rs, new AuditLogRQ
                    {
                        EventCode = "createapp",
                        ObjectName = rs.ApplicationName,
                        ObjectCode = rs.AppCode,
                        targets = new Dictionary<string, string> { { "Application", rs.AppCode }, { "Organization", rs.OrgCode } },
                        ActionType = "create application",
                        ExecutedBy = userEmail,
                        ExecutedOn = DateTime.Now,
                        LogData = rs,

                    });
                    return rs;
                }
                else
                {
                    throw new ArgumentException("Application creation failed");
                }
            }
            catch (Exception)
            {
                logger.Error("Unable to add new application");
                throw;
            }

        }
        public async Task<ApplicationsRQ> UpdateApplicationAsync(UpdateApplicationRQ applicationsRequest, string appcode, string appOrgCode, string userEmail)
        {
            try
            {
                int appId = await GetAppIdByAppCodeAsync(appcode);
                var application = await _applicationRepository.GetAppIdByAppCodeAsync(appcode);
                if (!application.IsActive)
                {
                    logger.Error("Application is inactive");
                    throw new ArgumentException("error while fetching application");
                }
                var validatemodel = new ApplicationsRQ()
                {
                    AppCode = appcode,
                    ApplicationName = applicationsRequest.ApplicationName,
                    Phone = applicationsRequest.Phone,
                    ApplicationEmail = applicationsRequest.ApplicationEmail,

                };
                var isValidationSuccessful = ValidateApplicationRequest(validatemodel);
                if (!isValidationSuccessful)
                {
                    await _loggingService.LogEventAsync(applicationsRequest, new AuditLogRQ
                    {
                        EventCode = "appFailures",
                        ObjectName = applicationsRequest.ApplicationName,
                        ObjectCode = validatemodel.AppCode,
                        targets = new Dictionary<string, string> { { "Application Errors", validatemodel.AppCode } },
                        ActionType = "Edit Application",
                        ExecutedBy = userEmail,
                        ExecutedOn = DateTime.Now,
                        LogData = "Application creation validation checks failed",

                    });
                    logger.Error("Validation error");
                    throw new ArgumentException("Validation error");
                }

                var OrganzationId = await _organizationRepository.GetOrganizationByIdAsync(appOrgCode);
                var updateApplicationRQ = new Applications
                {
                    ApplicationId = appId,
                    OrganizationId = OrganzationId.OrganizationId,
                    ApplicationName = applicationsRequest.ApplicationName,
                    Description = applicationsRequest.Description,
                    FirstName = applicationsRequest.FirstName,
                    LastName = applicationsRequest.LastName,
                    AppCode = appcode,
                    Phone = applicationsRequest.Phone,
                    ApplicationEmail = applicationsRequest.ApplicationEmail
                };

                var output = await _applicationRepository.UpdateApplicationAsync(updateApplicationRQ, appOrgCode);
                await _notificationService.ApplicationNotification(userEmail, updateApplicationRQ, OrganzationId, "CYRAX_APP_EDIT");
                var appdetails = await _applicationRepository.GetApplicationDetailsAsync(appcode);
                if (output == 1)
                {
                    ApplicationsRQ rs = new ApplicationsRQ()
                    {
                        OrgCode = appdetails.Organizations.OrgCode.ToUpper(),
                        ApplicationName = appdetails.ApplicationName,
                        Phone = appdetails.Phone,
                        AppCode = appcode,
                        Description = appdetails.Description,
                        FirstName = appdetails.FirstName,
                        LastName = appdetails.LastName,
                        ApplicationEmail = appdetails.ApplicationEmail

                    };
                    await _loggingService.LogEventAsync(rs, new AuditLogRQ
                    {
                        EventCode = "editapp",
                        ObjectName = rs.ApplicationName,
                        ObjectCode = rs.AppCode,
                        targets = new Dictionary<string, string> { { "Application", rs.AppCode }, { "Organization", rs.OrgCode } },
                        ActionType = "Edit application",
                        ExecutedBy = userEmail,
                        ExecutedOn = DateTime.Now,
                        LogData = rs,

                    });
                    return rs;
                }
                else
                {
                    logger.Error("Unable to update application");
                    throw new ApplicationException("Error while updating application");
                }
            }
            catch (Exception)
            {
                logger.Error("Unable to update application details..");
                throw;
            }
        }
        public async Task<IEnumerable<AppDetailsRS>> GetApplicationListRepositoryAsync(string orgcode)
        {
            var orgDetail = await _organizationRepository.GetOrganizationByIdAsync(orgcode);
            if (orgDetail == null)
            {
                logger.Error("Organization is deactivated or no organization found");
                throw new ArgumentException("error while fetching OrgDetails.");
            }
            var output = await _applicationRepository.GetApplicationListAsync(orgcode);
            if (!output.Any())
            {
                logger.Error("No applications were found");
                return null;
            }
            List<AppDetailsRS> rs = new List<AppDetailsRS>();
            foreach (var app in output)
            {

                rs.Add(new AppDetailsRS()
                {
                    ApplicationId = app.ApplicationId,
                    ApplicationName = app.ApplicationName,
                    AppCode = app.AppCode,
                    Description = app.Description,
                    Phone = app.Phone,
                    FirstName = app.FirstName,
                    LastName = app.LastName,
                    OrgCode = orgcode,
                    ClientId = app.ClientId,
                    ClientSecret = app.ClientSecret,
                    IsActive = true,
                    ApplicationEmail = app.ApplicationEmail
                });

            }
            return rs;
        }

        public async Task<IEnumerable<AppDetailsRS>> GetApplicationListAsync(string orgcode)
        {
            var orgDetail = await _organizationRepository.GetOrganizationByIdAsync(orgcode);
            if (orgDetail == null)
            {
                logger.Error("Organization is deactivated or no organization found");
                throw new ArgumentException("error while fetching OrgDetails.");
            }
            var output = await _applicationRepository.GetApplicationListAsync(orgcode);
            if (!output.Any())
            {
                logger.Error("No applications were found");
                throw new ArgumentException("Applications are not created");
            }
            List<AppDetailsRS> rs = new List<AppDetailsRS>();
            foreach (var app in output)
            {

                rs.Add(new AppDetailsRS()
                {
                    ApplicationId = app.ApplicationId,
                    ApplicationName = app.ApplicationName,
                    AppCode = app.AppCode,
                    Description = app.Description,
                    Phone = app.Phone,
                    FirstName = app.FirstName,
                    LastName = app.LastName,
                    OrgCode = orgcode,
                    ClientId = app.ClientId,
                    ClientSecret = app.ClientSecret,
                    IsActive = true,
                    ApplicationEmail = app.ApplicationEmail
                });

            }
            return rs;
        }
        public async Task<AppDetailsRS> GetApplicationDetailsAsync(string appcode)
        {
            var appOrgCode = await _applicationRepository.GetOrgCodeByAppCodeAsync(appcode);
            var orgDetail = await _organizationRepository.GetOrganizationByIdAsync(appOrgCode);
            if (orgDetail == null)
            {
                logger.Error("Organization is deactivated or no organization found");
                throw new ArgumentException("error while fetching OrgDetails.");
            }
            var output = await _applicationRepository.GetApplicationDetailsAsync(appcode);
            if (output == null)
            {
                logger.Error("No applications were found");
                throw new ArgumentException("Applications are not created");
            }

            if (output.IsActive)
            {
                AppDetailsRS rs = new AppDetailsRS()
                {
                    ApplicationId = output.ApplicationId,
                    ApplicationName = output.ApplicationName,
                    AppCode = output.AppCode,
                    Description = output.Description,
                    Phone = output.Phone,
                    FirstName = output.FirstName,
                    LastName = output.LastName,
                    OrgCode = output.Organizations.OrgCode,
                    ClientId = output.ClientId,
                    ClientSecret = output.ClientSecret,
                    IsActive = true,
                    ApplicationEmail = output.ApplicationEmail
                };
                return rs;
            }
            else
            {
                logger.Error("Application is deactivated");
                throw new ArgumentException("Application not found");
            }
        }
        public async Task<List<string>> GetAppCodeByEmailAsync(string userEmail)
        {
            var rs = await _applicationRepository.GetAppCodeByEmailAsync(userEmail);
            return rs;
        }
        public async Task<List<string>> GetAppCodeByOrgCodeAsync(string orgcode)
        {
            var rs = await _applicationRepository.GetAppCodeByOrgCodeAsync(orgcode);
            return rs;
        }

        public async Task<List<string>> GetOrgCodeByEmailAsync(string userEmail)
        {
            return await _applicationRepository.GetOrgCodeByEmailAsync(userEmail);
        }
        public async Task<string> GetOrgCodeByAppCodeAsync(string appcode)
        {
            var rs = await _applicationRepository.GetOrgCodeByAppCodeAsync(appcode);
            return rs;
        }
        public async Task<int> GetAppIdByAppCodeAsync(string appCode)
        {
            var application = await _applicationRepository.GetAppIdByAppCodeAsync(appCode);
            if (application == null)
            {
                return 0;
            }
            return application.ApplicationId;
        }

        public async Task DeactivateApplicationAsync(string appcode, string userEmail)
        {
            var application = await _applicationRepository.GetAppIdByAppCodeAsync(appcode);

            if (application == null)
            {
                await _loggingService.LogEventAsync(application, new AuditLogRQ
                {
                    EventCode = "appFailures",
                    ObjectName = "appcode not found",
                    ObjectCode = appcode,
                    targets = new Dictionary<string, string> { { "Application Errors", appcode } },
                    ActionType = "Application Failure",
                    ExecutedBy = userEmail,
                    ExecutedOn = DateTime.Now,
                    LogData = "Application creation validation checks failed",

                });
                logger.Error("Application not found.");
                throw new KeyNotFoundException("Application not found.");

            }
            application.IsActive = false;
            var orgcode = await _applicationRepository.GetOrgCodeByAppCodeAsync(appcode);
            var org = await _organizationRepository.GetOrganizationByIdAsync(orgcode);
            await _applicationRepository.DeactivateApplicationAsync(application);
            await _notificationService.ApplicationNotification(userEmail, application, org, "CYRAX_APP_DELETE");
            await _loggingService.LogEventAsync(application, new AuditLogRQ
            {
                EventCode = "deactivateapp",
                ObjectName = application.ApplicationName,
                ObjectCode = application.AppCode,
                targets = new Dictionary<string, string> { { "Application", application.AppCode }, { "Organization", org.OrgCode } },
                ActionType = "deactivate application",
                ExecutedBy = userEmail,
                ExecutedOn = DateTime.Now,
                LogData = application,

            });
            logger.Information("Application with AppCode is Deactivated");
        }
        public async Task<string> GetAppCodeByClientIdAsync(string clientId)
        {
            try
            {
                var appCode = await _applicationRepository.GetAppCodeByClientIdAsync(clientId);

                if (string.IsNullOrWhiteSpace(appCode))
                {
                    logger.Error($"No application found for client ID: {clientId}");
                    throw new KeyNotFoundException($"No application found for client ID: {clientId}");
                }

                return appCode;
            }
            catch (Exception ex)
            {
                logger.Error($"Error while retrieving app code by client ID: {ex.Message}");
                throw;
            }
        }

        //privates
        private string GenerateClientId()
        {
            // Generate a unique client ID using a GUID
            Guid uniqueGuid = Guid.NewGuid();
            return uniqueGuid.ToString("N");
        }

        private string GenerateClientSecret()
        {
            // Generate a unique client secret using a cryptographic library
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] secretBytes = new byte[32];
                rng.GetBytes(secretBytes);

                return Convert.ToBase64String(secretBytes);
            }
        }
        private bool ValidateApplicationRequest(ApplicationsRQ applicationsRequest)
        {
            if (string.IsNullOrWhiteSpace(applicationsRequest.ApplicationName))
            {
                logger.Error("application name is required.");
                return false;
            }
            if (!IsValidPhoneNumber(applicationsRequest.Phone))
            {
                logger.Error("Invalid phone number. Phone number should have 10 digits.");
                return false;
            }
            if (!IsValidApplicationName(applicationsRequest.ApplicationName))
            {
                logger.Error("Organization name cannot contain special characters.");
                return false;

            }
            if (!IsValidEmail(applicationsRequest.ApplicationEmail))
            {
                logger.Error("Invalid email format.");
                return false;
            }
            return true;
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length == 10 && phoneNumber.All(char.IsDigit);
        }
        private bool IsValidApplicationName(string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            string allowedSpecialCharacters = "-_ ";

            foreach (char character in name)
            {
                if (!char.IsLetterOrDigit(character) && !allowedSpecialCharacters.Contains(character))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var emailAddress = new System.Net.Mail.MailAddress(email);
                var domainParts = emailAddress.Host.Split('.');
                if (domainParts.Length >= 2 && domainParts.Length <= 3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
