using Microsoft.EntityFrameworkCore;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using Assignment.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services
{
    public class GoogleWorkSpaceService
    {
        private readonly IDBGoogleWorkSpaceRepository _googleWorkSpaceRepository;
        private readonly UserService _userService;
        private readonly OrganizationService _organizationService;
        private readonly ApplicationsService _applicationService;
        private readonly RolesPermissionService _rolePermissionService;
        private readonly Serilog.Core.Logger logger;

        public GoogleWorkSpaceService(IDBGoogleWorkSpaceRepository googleWorkSpaceRepository,RolesPermissionService rolesPermissionService, UserService userService, OrganizationService organizationService, ApplicationsService applicationsService, Serilog.Core.Logger logger)
        {
            _googleWorkSpaceRepository = googleWorkSpaceRepository;
            _userService = userService;
            _organizationService = organizationService;
            _applicationService = applicationsService;
            _rolePermissionService = rolesPermissionService;
            this.logger = logger;
        }
        public async Task<int> AddUserToGoogleSignIn(int userId)
        {
            return await _googleWorkSpaceRepository.AddUserToGoogleSignIn(userId);
        }
        public async Task<int> AcceptInvite(int userId)
        {
            return await _googleWorkSpaceRepository.AcceptInvite(userId);
        }
        public async Task<bool> IsInvited(int userId)
        {
            return await _googleWorkSpaceRepository.IsInvited(userId);
        }
        public async Task<int> DeleteUserGoogleWorkSpace(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            var rs = await _googleWorkSpaceRepository.DeleteUserGoogleWorkSpace(user.UserId);
            return rs;
        }
        public async Task<GoogleSignIn> GetGoogleSignInDetails(int userId)
        {
            var rs = await _googleWorkSpaceRepository.GetGoogleSignInDetails(userId);
            if (rs != null)
            {
                return rs;
            }
            else
            {
                throw new Exception("User is not enabled google Sign In");
            }
        }
        public async Task<List<string>> GetAllowedDomains(string orgCode,string appCode)
        {
            int? orgid = null;
            int? appid = null;
            if (!string.IsNullOrEmpty(orgCode)) {
            var info= await _organizationService.GetOrganizationByIdAsync(orgCode);
                orgid = info.OrganizationId;
            }
            if (!string.IsNullOrEmpty(appCode))
            {
                appid = await _applicationService.GetAppIdByAppCodeAsync(appCode);
            }
            return await _googleWorkSpaceRepository.GetAllowedDomains(orgid,appid);
        }
        public async Task<AddDomainRQ> AddDomain(AddDomainRQ model)
        {
            var newModel = new AllowedDomains();
            int? appId = null;
            int? orgId = null;
            if (model.AppCode != null && model.OrgCode != null)
            {
                throw new ArgumentException("Please specify Either orgcode or appcode. Both are not allowed");
            }
            if (model.AppCode != null)
            {
                appId = await _applicationService.GetAppIdByAppCodeAsync(model.AppCode);
            }
            if(model.OrgCode != null)
            {
                var orgInfo = await _organizationService.GetOrganizationByIdAsync(model.OrgCode);
                orgId = orgInfo.OrganizationId;
            }
            newModel.Domain = model.Domain;
            newModel.ApplicationId = appId;
            newModel.OrganizationId = orgId;
            newModel.IsActive = true;
            await _googleWorkSpaceRepository.AddDomain(newModel);
            return model;
        }
        public async Task<bool> IdDomainAllowed(string email)
        {
            var userData = await _userService.GetUserByEmailAsync(email);
            var userRole = await _rolePermissionService.GetRolesByEmail(email);
            var userDomain = GetDomainFromEmail(email);
            var allowedDomain = new List<string>();
            if (userData != null && userRole != null)
            {
                List<List<string>> domains = new List<List<string>>();
                if (userRole.Contains(1))
                {
                    allowedDomain = await GetAllowedDomains(null, null);
                }
                if (userRole.Contains(2))
                {
                    var orgcodes = await _organizationService.GetOrgCodeByEmailAsync(email);

                    foreach (var org in orgcodes)
                    {
                        domains.Add(await GetAllowedDomains(org, null));
                    }
                }
                if (userRole.Contains(3))
                {
                    var orgcodes = await _applicationService.GetOrgCodeByEmailAsync(email);
                    var appcodes = await _applicationService.GetAppCodeByEmailAsync(email);
                    foreach (var org in orgcodes)
                    {
                        domains.Add(await GetAllowedDomains(org, null));
                    }
                    foreach (var app in appcodes)
                    {
                        domains.Add(await GetAllowedDomains(null, app));
                    }
                }
                bool userdomainallowed = false;
                if (!userRole.Contains(1))
                {
                    foreach (var item in domains)
                    {
                        if (item.Contains(userDomain))
                        {
                            userdomainallowed = true;
                        }
                    }
                }
                else
                {
                    if (allowedDomain.Contains(userDomain))
                    {
                        userdomainallowed = true;
                    }
                    else
                    {
                        throw new ArgumentException("Domain not allowed");
                    }
                }
                return userdomainallowed;
            }
            else
            {
                throw new ArgumentException("Error while fetching the user");
            }
        }
        public string GetDomainFromEmail(string email)
        {
            try
            {
                string[] parts = email.Split('@');
                if (parts.Length == 2)
                {
                    return parts[1];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting domain: {ex.Message}");
            }
            return null;
        }
    }
}
