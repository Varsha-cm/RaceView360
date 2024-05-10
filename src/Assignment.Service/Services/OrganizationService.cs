using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using Assignment.Core.ThirdPartyModels;
using Assignment.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services
{
    public class OrganizationService
    {
        private readonly IDBOrganization _organizationRepository;
        //private readonly ILogger<OrganizationService> _logger;
        private readonly Serilog.Core.Logger logger;
        private readonly INotificationService _notificationService;
        private readonly ILoggingService _loggingService;

        public OrganizationService(IDBOrganization organizationRepository, INotificationService notificationService, Serilog.Core.Logger logger, ILoggingService loggingService)
        {
            _organizationRepository = organizationRepository;
            _notificationService = notificationService;
            this.logger = logger;
            _loggingService = loggingService;
        }

        public async Task<IEnumerable<OrgDetailsRS>> GetOrganizationsAsync()
        {
            var output = await _organizationRepository.GetOrganizationsAsync();
            if (output == null)
            {
                logger.Error("Organization data not found");
                throw new InvalidOperationException("Organization data not found");

            }

            List<OrgDetailsRS> rs = new List<OrgDetailsRS>();
            rs = output.Select(org => new OrgDetailsRS
            {
                OrgCode = org.OrgCode.ToUpper(),
                OrganizationId = org.OrganizationId,
                OrganizationName = org.OrganizationName,
                OrganizationEmail = org.OrganizationEmail,
                OrganizationPhone = org.OrganizationPhone,
            }).ToList();
            return rs;
        }

        public async Task<OrgDetailsRS> GetOrganizationByIdAsync(string OrgCode)
        {
            var organization = await _organizationRepository.GetOrganizationByIdAsync(OrgCode);
            if (organization == null)
            {
                logger.Error("The requested organization was not found");
                throw new KeyNotFoundException("The requested organization was not found");
            }
            OrgDetailsRS rs = new OrgDetailsRS();
            rs.OrgCode = organization.OrgCode.ToUpper();
            rs.OrganizationId = organization.OrganizationId;
            rs.OrganizationName = organization.OrganizationName;
            rs.OrganizationEmail = organization.OrganizationEmail;
            rs.OrganizationPhone = organization.OrganizationPhone;
            return rs;

        }
        public async Task<OrgDetailsRS> EditOrganizationDetailsAsync(string OrgCode, UpdateOrganizationRQ model,string userEmail)
        {
            var organization = await _organizationRepository.GetOrganizationByIdAsync(OrgCode);
            
            var existingOrg = await _organizationRepository.GetOrganizationByEmailAsync(model.OrganizationEmail);
            if (existingOrg != null && existingOrg.OrganizationEmail != model.OrganizationEmail)
            {
                logger.Error("An organization with the same email already exists.");
                throw new ArgumentException("An organization with the same email already exists.");
            }

            if (organization == null)
            {
                logger.Error("Organization not found");
                throw new KeyNotFoundException("Organization not found");
            }
            var validatemodel = new OrganizationRQ()
            {
                orgcode = OrgCode,
                OrganizationName = model.OrganizationName,
                OrganizationEmail = model.OrganizationEmail,
                OrganizationPhone = model.OrganizationPhone,
            };

            var isvalid = ValidateOrganizationRequest(validatemodel);

            if (!isvalid)
            {
                await _loggingService.LogEventAsync(model, new AuditLogRQ
                {
                    EventCode = "orgFailures",
                    ObjectName = model.OrganizationName,
                    ObjectCode = OrgCode,
                    targets = new Dictionary<string, string> { { "Organization Errors", OrgCode} },
                    ActionType = "Update Organization",
                    ExecutedBy = userEmail,
                    ExecutedOn = DateTime.Now,
                    LogData = "Organization update validation checks failed"

                });
                logger.Error("validation error");
                throw new ArgumentException("Validation error");
            }

            organization.OrgCode = OrgCode.ToUpper();
            organization.OrganizationName = model.OrganizationName;
            organization.OrganizationEmail = model.OrganizationEmail;
            organization.OrganizationPhone = model.OrganizationPhone;
            await _notificationService.OrganizationNotification(userEmail, organization, "CYRAX_ORG_EDIT");
            await _organizationRepository.EditOrganizationDetailsAsync(organization);
            OrgDetailsRS rs = new OrgDetailsRS()
            {
                OrganizationId = organization.OrganizationId,
                OrgCode = organization.OrgCode.ToUpper(),
                OrganizationName = organization.OrganizationName,
                OrganizationEmail = organization.OrganizationEmail,
                OrganizationPhone = organization.OrganizationPhone
            };
            await _loggingService.LogEventAsync(rs, new AuditLogRQ
            {
                EventCode = "editorg",
                ObjectName = rs.OrganizationName,
                ObjectCode = rs.OrgCode,
                targets = new Dictionary<string, string> { { "Organization", OrgCode } },
                ActionType = "Edit Organization",
                ExecutedBy = userEmail,
                ExecutedOn = DateTime.Now,
                LogData = rs,

            });
            return rs;

        }
        public async Task<OrgDetailsRS> AddOrganizationAsync(OrganizationRQ organizationRequest,string userEmail)
        {
            var existingOrg = await _organizationRepository.GetOrganizationByOrgCodeAsync(organizationRequest.orgcode);
            if (existingOrg != null)
            {
                logger.Error("An organization with the same OrgCode already exists");
                throw new ArgumentException("An organization with the same OrgCode already exists.");
            }
           
            existingOrg = await _organizationRepository.GetOrganizationByEmailAsync(organizationRequest.OrganizationEmail);
            if (existingOrg != null)
            {
                logger.Error("An organization with the same email already exists.");
                throw new ArgumentException("An organization with the same email already exists.");
            }

            var isvalid = ValidateOrganizationRequest(organizationRequest);
            if (!isvalid)
            {
                await _loggingService.LogEventAsync(organizationRequest, new AuditLogRQ
                {
                    EventCode = "orgFailures",
                    ObjectName = organizationRequest.OrganizationName,
                    ObjectCode = organizationRequest.orgcode,
                    targets = new Dictionary<string, string> { { "Organization Errors", organizationRequest.orgcode } },
                    ActionType = "Create Organization",
                    ExecutedBy = userEmail,
                    ExecutedOn = DateTime.Now,
                    LogData = "Organization update validation checks failed"

                });
                logger.Error("validation error");
                throw new ArgumentException("Validation error");
            }

            var organizationEntity = new Organizations
            {
                OrgCode = organizationRequest.orgcode.ToUpper(),
                OrganizationName = organizationRequest.OrganizationName,
                OrganizationEmail = organizationRequest.OrganizationEmail,
                OrganizationPhone = organizationRequest.OrganizationPhone,
                CreatedTimestamp = DateTime.Now,
                ModifiedTimestamp = DateTime.Now
            };
            var output = await _organizationRepository.AddOrganizationAsync(organizationEntity);
            await _notificationService.OrganizationNotification(userEmail, organizationEntity, "CYRAX_ORG_CREATE");
            OrgDetailsRS rs = new OrgDetailsRS()
            {
                OrganizationId = output,
                OrgCode = organizationRequest.orgcode.ToUpper(),
                OrganizationName = organizationRequest.OrganizationName,
                OrganizationEmail = organizationRequest.OrganizationEmail,
                OrganizationPhone = organizationRequest.OrganizationPhone,

            };
            await _loggingService.LogEventAsync(rs, new AuditLogRQ
            {
                EventCode = "createorg",
                ObjectName = rs.OrganizationName,
                ObjectCode = rs.OrgCode,
                targets = new Dictionary<string, string> { { "organization", rs.OrgCode } },
                ActionType = "Create Organization",
                ExecutedBy = userEmail,
                ExecutedOn = DateTime.Now,
                LogData = rs

            });
            return rs;
        }

        public async Task DeactivateOrganizationAsync(string OrgCode,string userEmail)
        {
            var organization = await _organizationRepository.GetOrganizationByIdAsync(OrgCode);

            if (organization == null)
            {
                await _loggingService.LogEventAsync(organization, new AuditLogRQ
                {
                    EventCode = "orgFailures",
                    ObjectName = "Organization Error",
                    ObjectCode = OrgCode,
                    targets = new Dictionary<string, string> { { "organization", OrgCode} },
                    ActionType = "Organization Failure",
                    ExecutedBy = userEmail,
                    ExecutedOn = DateTime.Now,
                    LogData = "organization not found"

                });
                logger.Error("Organization not found.");
                throw new KeyNotFoundException("Organization not found.");
            }
            organization.IsActive = false;
            await _organizationRepository.DeactivateOrganizationAsync(organization);
            await _notificationService.OrganizationNotification(userEmail, organization, "CYRAX_ORG_DELETE");
            await _loggingService.LogEventAsync(organization, new AuditLogRQ
            {
                EventCode = "deactivateorg",
                ObjectName = organization.OrganizationName,
                ObjectCode = organization.OrgCode,
                targets = new Dictionary<string, string> { { "organization", organization.OrgCode } },
                ActionType = "Deactivate Organization",
                ExecutedBy = userEmail,
                ExecutedOn = DateTime.Now,
                LogData = organization

            });
            logger.Information("Organization with orgcode is deactivated");
        }
        public async Task<List<string>> GetOrgCodeByEmailAsync(string userEmail)
        {
            return await _organizationRepository.GetOrgCodeByEmailAsync(userEmail);
        }

        //private       
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length == 10 && phoneNumber.All(char.IsDigit);
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
        private bool IsValidOrganizationName(string name)
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

        private bool ValidateOrganizationRequest(OrganizationRQ organizationRequest)
        {

            if (string.IsNullOrWhiteSpace(organizationRequest.OrganizationName))
            {
                logger.Error("Organization name is required.");
                return false;
            }

            if (!IsValidPhoneNumber(organizationRequest.OrganizationPhone))
            {
                logger.Error("Invalid phone number. Phone number should have 10 digits.");
                return false;
            }

            if (!IsValidOrganizationName(organizationRequest.OrganizationName))
            {
                logger.Error("Organization name cannot contain special characters.");
                return false;
            }

            if (!IsValidEmail(organizationRequest.OrganizationEmail))
            {
                logger.Error("Invalid Email format");
                return false;

            }

            return true;
        }

    }
}


    

