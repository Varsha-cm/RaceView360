using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using Assignment.Service.Model;
using Assignment.Core.ThirdPartyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Threading.Channels;

namespace Assignment.Service.Services
{
    public class UserService
    {
        private readonly IDBUserRepository _userRepository;
        private readonly IDBOrganization _organizationRepository;
        private readonly Serilog.Core.Logger logger;
        private readonly IDBApplicationRepository _applicationRepository;
        private readonly OrganizationService _organizationService;
        private readonly AuthService _authService;
        private readonly RolesPermissionService _roleService;
        private readonly ApplicationsService _applicationsService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggingService _loggingService;
        private readonly INotificationService _notificationService;

        public UserService(IDBUserRepository userRepository, OrganizationService organizationService, INotificationService notificationService, AuthService authService, RolesPermissionService roleService, IHttpClientFactory httpClientFactory, ILoggingService loggingService, ApplicationsService applicationservice, IDBOrganization organizationRepository, IDBApplicationRepository applicationRepository, Serilog.Core.Logger logger)
        {
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            this.logger = logger;
            _applicationRepository = applicationRepository;
            _organizationService = organizationService;
            _authService = authService;
            _roleService = roleService;
            _applicationsService = applicationservice;
            _httpClientFactory = httpClientFactory;
            _loggingService = loggingService;
            _notificationService = notificationService;
        }

        public async Task<UserRS> CreateUserAsync(UserRQ userRequest, List<int> userRoles, string userEmail)
        {
            try
            {
                if (await IsEmailUnique(userRequest.EmailAddress))
                {
                    if (userRequest.roleId != 1 &&
                        ((string.IsNullOrEmpty(userRequest.orgcode) && string.IsNullOrEmpty(userRequest.appcode))
                        || (!string.IsNullOrEmpty(userRequest.orgcode) && !string.IsNullOrEmpty(userRequest.appcode))
                        ))
                    {
                        await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.EmailAddress, userRequest.EmailAddress, "Users Failure", userEmail, "something went wrong.");
                        logger.Error("please specify either orgcode or appcode");
                        throw new ArgumentException("please specify either orgcode or appcode");
                    }
                    if (!IsPasswordValid(userRequest.Password))
                    {
                        string check;
                        if (userRequest.orgcode == null)
                        {
                            check = userRequest.orgcode;
                        }
                        else
                        {
                            check = userRequest.appcode;
                        }
                        await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.EmailAddress, userRequest.EmailAddress, "Users Failure", userEmail, "Password does not meet the required criteria.");
                        logger.Error("Password does not meet the required criteria.");
                        throw new ArgumentException("Password does not meet the required criteria.");
                    }
                    // Creating user under organization
                    if (userRequest.roleId != 1)
                    {
                        if (userRequest.orgcode != null && await _organizationRepository.GetOrganizationByIdAsync(userRequest.orgcode) == null)
                        {
                            await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.orgcode, userRequest.EmailAddress, "Users Failure", userEmail, "org not found.");
                            logger.Error("org not found");
                            throw new ArgumentException("org not found");
                        }
                        if (userRequest.appcode != null && await _applicationRepository.GetApplicationDetailsAsync(userRequest.appcode) == null)
                        {
                            await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.EmailAddress, userRequest.appcode, "Users Failure", userEmail, "app not found.");
                            logger.Error("app not found");
                            throw new ArgumentException("app not found");
                        }
                        if ((!userRoles.Contains(2) && !userRoles.Contains(3)) && !userRoles.Contains(1))
                        {
                            await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.orgcode, userRequest.EmailAddress, "Users Failure", userEmail, "something went wrong.");
                            logger.Error("something went wrong");
                            throw new ArgumentException("something went wrong");
                        }
                    }
                    if (userRequest.roleId == 2 && !string.IsNullOrEmpty(userRequest.appcode))
                    {
                        await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.appcode, userRequest.appcode, "Users Failure", userEmail, "something went wrong.");
                        logger.Error("Something went wrong.");
                        throw new ArgumentException("something went wrong");
                    }

                    if (userRequest.roleId == 3 && !string.IsNullOrEmpty(userRequest.orgcode))
                    {
                        await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.orgcode, userRequest.orgcode, "Users Failure", userEmail, "something went wrong.");
                        logger.Error("Something went wrong.");
                        throw new ArgumentException("something went wrong");
                    }
                    if (userRequest.roleId == 1 && !userRoles.Contains(1))
                    {
                        await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.orgcode, userRequest.orgcode, "Users Failure", userEmail, "something went wrong.");
                        logger.Error("Something went wrong.");
                        throw new ArgumentException("something went wrong");
                    }

                    Users userModel = new Users();
                    userModel.IsActive = true;
                    userModel.Email = userRequest.EmailAddress;
                    userModel.FirstName = userRequest.FirstName;
                    userModel.LastName = userRequest.LastName;
                    userModel.Password = userRequest.Password;
                    var rs = new UserRS()
                    {
                        AppCode = userRequest.appcode,
                        EmailAddress = userRequest.EmailAddress,
                        FirstName = userRequest.FirstName,
                        LastName = userRequest.LastName,
                        OrgCode = userRequest.orgcode,
                        UserId = userModel.UserId
                    };
                    if (userRequest.roleId == 2 && string.IsNullOrEmpty(userRequest.appcode) && !string.IsNullOrEmpty(userRequest.orgcode) && (userRoles.Contains(2) || (userRoles.Contains(1))))
                    {
                        var output = await _userRepository.CreateUserAsync(userModel);
                        var orgInfo = await _organizationRepository.GetOrganizationByIdAsync(userRequest.orgcode);
                        await _userRepository.UpdateOrganizationUserAsync(output, orgInfo.OrganizationId, userRequest.orgcode);
                        await _userRepository.AddUserRoleAsync(output, userRequest.roleId, orgInfo.OrganizationId, null);
                        await _loggingService.LogEventAsync(userRequest, new AuditLogRQ
                        {
                            EventCode = "createorguser",
                            ObjectName = orgInfo.OrganizationName,
                            ObjectCode = orgInfo.OrgCode,
                            targets = new Dictionary<string, string> { { "Organization", orgInfo.OrgCode } },
                            ActionType = "Create OrgUser",
                            ExecutedBy = userEmail,
                            ExecutedOn = DateTime.Now,
                            LogData = rs
                        });
                    }
                    if (userRequest.roleId == 3 && string.IsNullOrEmpty(userRequest.orgcode))
                    {
                        var output = await _userRepository.CreateUserAsync(userModel);
                        var orgCode = await _applicationsService.GetOrgCodeByAppCodeAsync(userRequest.appcode);
                        var orgInfo = await _organizationService.GetOrganizationByIdAsync(orgCode);
                        var appInfo = await _applicationsService.GetAppIdByAppCodeAsync(userRequest.appcode);
                        var app = await _applicationsService.GetApplicationDetailsAsync(userRequest.appcode);
                        await _userRepository.UpdateApplicationUserAsync(output, orgInfo.OrganizationId, orgCode, userRequest.appcode, appInfo);
                        await _userRepository.AddUserRoleAsync(output, userRequest.roleId, orgInfo.OrganizationId, appInfo);
                        await _loggingService.LogEventAsync(rs, new AuditLogRQ
                        {
                            EventCode = "createappuser",
                            ObjectName = app.ApplicationName,
                            ObjectCode = rs.AppCode,
                            targets = new Dictionary<string, string> { { "Application", app.AppCode },{"Organization",orgInfo.OrgCode} },
                            ActionType = "Create AppUser",
                            ExecutedBy = userEmail,
                            ExecutedOn = DateTime.Now,
                            LogData = rs
                        });
                    }
                    if (userRequest.roleId == 1 && userRoles.Contains(1))
                    {
                        var output = await _userRepository.CreateUserAsync(userModel);
                        await _userRepository.AddUserRoleAsync(output, userRequest.roleId, null, null);
                        await _loggingService.LogEventAsync(rs, new AuditLogRQ
                        {
                            EventCode = "createsuperadmin",
                            ObjectName = "Raiden",
                            ObjectCode = "Raiden01",
                            targets = new Dictionary<string, string> { { "Application", "Raiden01" },{"Organization","STT01"} },
                            ActionType = "Create SuperAdmin",
                            ExecutedBy = userEmail,
                            ExecutedOn = DateTime.Now,
                            LogData = rs
                        });
                    }
                    var useroutput = _userRepository.GetUserByEmailAsync(userRequest.EmailAddress);
                    await _notificationService.UserNotification(userEmail, userModel, "CYRAX_USER_INVITE");
                    return rs;
                }
                else
                {
                    await _loggingService.LogEventHelperAsync(userRequest, "userfailures", userRequest.FirstName, userRequest.roleId.ToString(), userRequest.EmailAddress, "Users Failure", userEmail, "something went wrong.");
                    logger.Error("Something went wrong.");
                    throw new ArgumentException("something went wrong");
                }
            }
            catch (Exception ex)
            {
                // Log the error here
                throw;
            }
        }
        public async Task UpdateOrganizationUserAsync(int userId, int organizationId, string orgCode)
        {
            await _userRepository.UpdateOrganizationUserAsync(userId, organizationId, orgCode);
        }
        public async Task UpdateApplicationUserAsync(int userId, int organizationId, string orgCode, string appCode, int appId)
        {
            await _userRepository.UpdateApplicationUserAsync(userId, organizationId, orgCode, appCode, appId);
        }

        public async Task<IEnumerable<OrgUserRS>> GetOrganizationUserAsync(string orgcode)
        {
            var orguser = await _userRepository.GetOrganizationUserAsync(orgcode);
            if (!orguser.Any())
            {
                logger.Error("Organization not found or user does not exist");
                throw new KeyNotFoundException("Organization not found or user does not exist");
            }


            List<OrgUserRS> ret = new List<OrgUserRS>();
            ret = orguser.Where(e => e.IsActive == true).Select(org => new OrgUserRS
            {
                userId = org.UserId,
                OrgCode = org.Orgcode,
                FirstName = org.Users.FirstName,
                LastName = org.Users.LastName,
                EmailAddress = org.Users.Email,

            }).ToList();
            return ret;
        }
        public async Task<AppUserRS> CreateAppUserAsync(AppUserRQ userRequest, string appcode, string userEmail)
        {
            try
            {
                if (await IsEmailUnique(userRequest.EmailAddress))
                {
                    var apporgcode = await _applicationsService.GetOrgCodeByAppCodeAsync(appcode);
                    var organizationId = await _organizationRepository.GetOrganizationByIdAsync(apporgcode);

                    if (organizationId != null)
                    {
                        var userEntity = new Users
                        {
                            FirstName = userRequest.FirstName,
                            LastName = userRequest.LastName,
                            Email = userRequest.EmailAddress,
                            Password = userRequest.Password,
                        };

                        if (!IsPasswordValid(userRequest.Password))
                        {
                            logger.Error("Password doesnot meet the required criteria.");
                            throw new ArgumentException("Password doesnot meet the required criteria.");
                        }

                        int userId = await _userRepository.CreateAppUserAsync(userEntity);

                        var userResponse = new AppUserRS
                        {
                            UserId = userId,
                            OrgCode = apporgcode,
                            AppCode = appcode,
                            FirstName = userRequest.FirstName,
                            LastName = userRequest.LastName,
                            EmailAddress = userRequest.EmailAddress,
                            Password = userRequest.Password,
                        };

                        return userResponse;
                    }
                    else
                    {
                        logger.Error("Organization with the provided OrgCode does not exist.");
                        throw new Exception("Organization with the provided OrgCode does not exist.");
                    }
                }
                else
                {
                    logger.Error("Email Address is not unique.");
                    throw new Exception("Email Address is not unique.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> AddUserRoleAsync(int userId, int roleId, int? orgId, int? appId)
        {
            var rs = await _userRepository.AddUserRoleAsync(userId, roleId, orgId, appId);
            return rs;
        }

        public async Task<IEnumerable<AppUserDetailsRS>> GetApplicationUsersByAppCodeAsync(string appCode)
        {
            var output = await _applicationRepository.GetApplicationUsersByAppCodeAsync(appCode);
            if (!output.Any())
            {
                logger.Error("AppCode not found.");
                return null;
            }
            List<AppUserDetailsRS> rs = new List<AppUserDetailsRS>();
            foreach (var item in output)
            {
                if (item.IsActive)
                    rs.Add(new AppUserDetailsRS
                    {
                        userId = item.UserId,
                        AppCode = item.AppCode,
                        ApplicationName = item.Applications.ApplicationName,
                        Email = item.Users.Email,
                        FirstName = item.Users.FirstName,
                        LastName = item.Users.LastName,
                        OrgCode = item.Orgcode
                    });

            }
            return rs;
        }
        public async Task<int> GetUserIdByEmailAsync(string email)
        {
            var userId = await _userRepository.GetUserIdByEmailAsync(email);
            if (userId <= 0)
            {
                return 0;
            }
            return userId;
        }
        public async Task<IEnumerable<OrgUserRS>> GetOrganizationUsersAsync()
        {
            var output = await _userRepository.GetOrganizationUsersAsync();
            List<OrgUserRS> ret = new List<OrgUserRS>();
            ret = output.Where(org => org.IsActive == true)
                .Select(org => new OrgUserRS
                {
                    userId = org.UserId,
                    OrgCode = org.Orgcode,
                    FirstName = org.Users.FirstName,
                    LastName = org.Users.LastName,
                    EmailAddress = org.Users.Email,

                }).ToList();
            return ret;
        }
        public async Task<IEnumerable<OrgUserRS>> GetOrganizationUsersAsync(string OrgCode)
        {
            var output = await _userRepository.GetOrganizationUsersAsync(OrgCode);
            List<OrgUserRS> ret = new List<OrgUserRS>();
            ret = output.Select(org => new OrgUserRS
            {
                userId = org.UserId,
                OrgCode = org.Orgcode,
                FirstName = org.Users.FirstName,
                LastName = org.Users.LastName,
                EmailAddress = org.Users.Email,
            }).ToList();
            return ret.Distinct();
        }
        public async Task<IEnumerable<AppUserDetailsRS>> GetApplicationUsersAsync()
        {
            var output = await _userRepository.GetApplicationUsersAsync();
            List<AppUserDetailsRS> rs = new List<AppUserDetailsRS>();
            foreach (var item in output)
            {
                if (item.IsActive == true)
                {
                    rs.Add(new AppUserDetailsRS
                    {
                        AppCode = item.AppCode,
                        userId = item.UserId,
                        ApplicationName = item.Applications.ApplicationName,
                        Email = item.Users.Email,
                        FirstName = item.Users.FirstName,
                        LastName = item.Users.LastName,
                        OrgCode = item.Orgcode
                    });
                }
            }
            return rs;
        }
        public async Task DeactivateUserAsync(string orgcode, int UserId, string userEmail)
        {
            var users = await _userRepository.GetUserById(orgcode, UserId);
            if (users == null)
            {
                await _loggingService.LogEventAsync(users, new AuditLogRQ
                {
                    EventCode = "userfailures",
                    ObjectName = "userId is " + UserId,
                    ObjectCode = orgcode,
                    targets = new Dictionary<string, string> { { "Errors", orgcode } },
                    ActionType = "Users Failure",
                    ExecutedBy = userEmail,
                    ExecutedOn = DateTime.Now,
                    LogData = "User not found",
                });
                logger.Error("Invalid OrgCode or UserId.");
                throw new KeyNotFoundException("Something Went Wrong.");
            }
            users.IsActive = false;
            await _userRepository.DeactivateUserAsync(users);
            await _loggingService.LogEventAsync(users, new AuditLogRQ
            {
                EventCode = "deactivateorguser",
                ObjectName = users.Users.FirstName,
                ObjectCode = users.Orgcode,
                targets = new Dictionary<string, string> { { "Organization", orgcode } },
                ActionType = "deleteorguser",
                ExecutedBy = userEmail,
                ExecutedOn = DateTime.Now,
                LogData = users,
            });
            logger.Information("Deactivated user under organization by orgcode and userid.");
        }

        public async Task DeactivateAppUserAsync(string appcode, int userId, string userEmail)
        {
            var users = await _userRepository.GetAppUserById(appcode, userId);
            if (users == null)
            {
                await _loggingService.LogEventAsync(users, new AuditLogRQ
                {
                    EventCode = "userfailures",
                    ObjectName = "UserId is " + userId,
                    ObjectCode = appcode,
                    targets = new Dictionary<string, string> { { "Errors", appcode } },
                    ActionType = "Users Failure",
                    ExecutedBy = userEmail,
                    ExecutedOn = DateTime.Now,
                    LogData = "User not found",
                });
                logger.Error("Invalid AppCode or UserId.");
                throw new KeyNotFoundException("Something Went Wrong");
            }
            users.IsActive = false;
            await _userRepository.DeactivateAppUserAsync(users);
            await _loggingService.LogEventAsync(users, new AuditLogRQ
            {
                EventCode = "deactivateappuser",
                ObjectName = users.Users.Email,
                ObjectCode = users.Orgcode,
                targets = new Dictionary<string, string> { { "application", users.Users.Email } },
                ActionType = "deleteappuser",
                ExecutedBy = userEmail,
                ExecutedOn = DateTime.Now,
                LogData = users,
            });
            logger.Information("Deactivated user under application by appcode and userid.");
        }
        public async Task<int> SetUserRole(SetUserRoleRQ request, string loggedInUserEmail)
        {
            var userRole = await _roleService.GetRolesByEmail(loggedInUserEmail);
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(loggedInUserEmail);
            var userappcodes = await _applicationsService.GetAppCodeByEmailAsync(loggedInUserEmail);
            var userId = await GetUserIdByEmailAsync(loggedInUserEmail);
            var rquserId = await GetUserIdByEmailAsync(request.Email);
            if (request.RoleId == 1)
            {
                if (userRole.Contains(1))
                {
                    var rs = await _userRepository.AddUserRoleAsync(rquserId, 1, null, null);
                    await _loggingService.LogEventAsync(rs, new AuditLogRQ
                    {
                        EventCode = "Setrolesuperadmin",
                        ObjectName = "Raiden",
                        ObjectCode = "Raiden01",
                        targets = new Dictionary<string, string> { { "Application", "Raiden01" },{"Organization","STT01"} },
                        ActionType = "Setrole SuperAdmin",
                        ExecutedBy = loggedInUserEmail,
                        ExecutedOn = DateTime.Now,
                        LogData = request
                    });
                    return rs;
                }
                else
                {
                    await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, "SuperAdmin", request.Email, "Users Failure", loggedInUserEmail, "something went wrong.");

                    logger.Error("you do not have permission to add super admin");
                    throw new UnauthorizedAccessException("you do not have permission to add super admin");
                }
            }
            else if (request.RoleId == 2)
            {
                if (userRole.Contains(2) || userRole.Contains(1))
                {
                    if (!userRole.Contains(1) && userOrgCode.Contains(request.OrgCode))
                    {
                        var orgid = await _organizationService.GetOrganizationByIdAsync(request.OrgCode);
                        if (orgid != null)
                        {
                            await _userRepository.UpdateOrganizationUserAsync(rquserId, orgid.OrganizationId, request.OrgCode);
                            var rs = await _userRepository.AddUserRoleAsync(rquserId, 2, orgid.OrganizationId, null);
                            await _loggingService.LogEventAsync(rs, new AuditLogRQ
                            {
                                EventCode = "SetRoleforOrgUser",
                                ObjectName = orgid.OrganizationName,
                                ObjectCode = orgid.OrgCode,
                                targets = new Dictionary<string, string> { { "Organization", orgid.OrgCode } },
                                ActionType = "Setrole OrgUser",
                                ExecutedBy = loggedInUserEmail,
                                ExecutedOn = DateTime.Now,
                                LogData = request
                            });
                            return rs;
                        }
                        else
                        {
                            await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, request.Email, request.Email, "Users Failure", loggedInUserEmail, "you do not have permission.");

                            logger.Error("Invalid OrgCode");
                            throw new ArgumentException("Invalid Orgcode");
                        }
                    }
                    else if (userRole.Contains(1))
                    {
                        var orgid = await _organizationService.GetOrganizationByIdAsync(request.OrgCode);
                        if (orgid != null)
                        {
                            await _userRepository.UpdateOrganizationUserAsync(rquserId, orgid.OrganizationId, request.OrgCode);
                            var rs = await _userRepository.AddUserRoleAsync(rquserId, 2, orgid.OrganizationId, null);
                            await _loggingService.LogEventAsync(rs, new AuditLogRQ
                            {
                                EventCode = "SetRoleforOrgUser",
                                ObjectName = orgid.OrganizationName,
                                ObjectCode = orgid.OrgCode,
                                targets = new Dictionary<string, string> { { "Organization", orgid.OrgCode } },
                                ActionType = "Setrole OrgUser",
                                ExecutedBy = loggedInUserEmail,
                                ExecutedOn = DateTime.Now,
                                LogData = request
                            });
                            return rs;
                        }
                        else
                        {
                            await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, request.OrgCode, request.Email, "Users Failure", loggedInUserEmail, "you do not have permission.");

                            logger.Error("Invalid OrgCode");
                            throw new ArgumentException("Invalid Orgcode");
                        }
                    }
                    else
                    {
                        await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, request.AppCode, request.Email, "Users Failure", loggedInUserEmail, "you do not have permission.");

                        logger.Error("you do not have permission");
                        throw new UnauthorizedAccessException("you do not have permission");
                    }
                }
                else
                {
                    await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, rquserId.ToString(), request.Email, "Users Failure", loggedInUserEmail, "you do not have permission.");

                    logger.Error("You do not have permission");
                    throw new UnauthorizedAccessException("you do not have permission");
                }
            }
            else if (request.RoleId == 3)
            {
                if (userRole.Contains(2) || userRole.Contains(1) || userRole.Contains(3))
                {
                    var apporggcode = await _applicationsService.GetOrgCodeByAppCodeAsync(request.AppCode);
                    if (userRole.Contains(1) || userappcodes.Contains(request.AppCode) || userOrgCode.Contains(apporggcode))
                    {
                        var appid = await _applicationsService.GetAppIdByAppCodeAsync(request.AppCode);
                        if (appid != 0)
                        {
                            var appInfo = await _applicationsService.GetApplicationDetailsAsync(request.AppCode);
                            var orgid = await _organizationService.GetOrganizationByIdAsync(apporggcode);
                            await _userRepository.UpdateApplicationUserAsync(rquserId, orgid.OrganizationId, orgid.OrgCode, request.AppCode, appid);
                            var rs = await _userRepository.AddUserRoleAsync(rquserId, 3, orgid.OrganizationId, appid);
                            await _loggingService.LogEventAsync(rs, new AuditLogRQ
                            {
                                EventCode = "SetRoleforAppUser",
                                ObjectName = appInfo.ApplicationName,
                                ObjectCode = request.AppCode,
                                targets = new Dictionary<string, string> { { "Application", request.AppCode } ,{"Organization",orgid.OrgCode}},
                                ActionType = "Setrole AppUser",
                                ExecutedBy = loggedInUserEmail,
                                ExecutedOn = DateTime.Now,
                                LogData = request
                            });
                            return rs;
                        }
                        else
                        {
                            await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, request.AppCode, request.Email, "Users Failure", loggedInUserEmail, "you do not have permission.");

                            logger.Error("Invalid Appcode");
                            throw new ArgumentException("Invalid Appcode");
                        }
                    }
                    else
                    {
                        await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, request.AppCode, request.Email, "Users Failure", loggedInUserEmail, "you do not have permission.");

                        logger.Error("you do not have permission");
                        throw new UnauthorizedAccessException("you do not have permission");
                    }
                }
                else
                {
                    await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, request.AppCode, request.Email, "Users Failure", loggedInUserEmail, "you do not have permission.");

                    logger.Error("you do not have permission");
                    throw new UnauthorizedAccessException("you do not have permission");
                }
            }
            else
            {
                await _loggingService.LogEventHelperAsync(request, "userfailures", request.Email, request.AppCode, request.Email, "Users Failure", loggedInUserEmail, "you do not have permission.");

                logger.Error("Invalid role Id");
                throw new ArgumentException("Invalid role Id");

            }
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {

            var user = await _userRepository.GetUserByEmailAsync(email);
            return user;
        }
        //private
        private async Task<bool> IsEmailUnique(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user == null;
        }

        private bool IsPasswordValid(string password)
        {
            if (password.Length < 4 || password.Length > 12)
            {
                return false;
            }
            if (!password.Any(char.IsLower) || !password.Any(char.IsUpper) || !password.Any(char.IsDigit) || !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return false;
            }

            return true;
        }
    }

}

