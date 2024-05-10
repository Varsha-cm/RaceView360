using Microsoft.EntityFrameworkCore;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Assignment.Infrastructure.Repository
{
    public class ApplicationRepository : IDBApplicationRepository
    {
        private readonly RaidenDBContext _dbContext;
        public ApplicationRepository(RaidenDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddApplicationAsync(Applications application)
        {
            _dbContext.Applications.Add(application);
            await _dbContext.SaveChangesAsync();
            return application.ApplicationId;
        }

        public async Task<int> UpdateApplicationAsync(Applications applicationDM, string orgcode)
        {
            var existingApplication = _dbContext.Applications.Find(applicationDM.ApplicationId);

            if (existingApplication != null)
            {

                if (applicationDM.Description != null)
                {
                    existingApplication.Description = applicationDM.Description;
                }
                if (applicationDM.Phone != null)
                {
                    existingApplication.Phone = applicationDM.Phone;
                }
                if (applicationDM.FirstName != null)
                {
                    existingApplication.FirstName = applicationDM.FirstName;
                }
                if (applicationDM.LastName != null)
                {
                    existingApplication.LastName = applicationDM.LastName;
                }
                if (applicationDM.ApplicationEmail != null)
                {
                    existingApplication.ApplicationEmail = applicationDM.ApplicationEmail;
                }
                existingApplication.AppCode = applicationDM.AppCode;
                existingApplication.ApplicationId = applicationDM.ApplicationId;
                existingApplication.ApplicationName = applicationDM.ApplicationName;
                existingApplication.OrganizationId = applicationDM.OrganizationId;

                var existingAppUsers = _dbContext.ApplicationUsers.Where(au => au.ApplicationId == applicationDM.ApplicationId).ToList();
                if (existingAppUsers != null)
                {

                    foreach (var existingAppUser in existingAppUsers)
                    {
                        existingAppUser.AppCode = applicationDM.AppCode;
                        existingAppUser.OrganizationId = applicationDM.OrganizationId;
                        existingAppUser.Orgcode = orgcode;
                        _dbContext.Entry(existingAppUser).State = EntityState.Modified;
                    }

                }
                _dbContext.Entry(existingApplication).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return 1;
            }
            return 0;
        }

        public async Task<List<string>> GetOrgCodeByEmailAsync(string userEmail)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
           
            if (user != null)
            {
                var appUsers = _dbContext.ApplicationUsers
                    .Include(au => au.Organizations)
                    .Where(au => au.UserId == user.UserId)
                    .ToList();

                if (appUsers != null && appUsers.Any())
                {
                    var orgCodes = appUsers.Select(au => au.Organizations.OrgCode).ToList();
                    return orgCodes;
                }
            }

            return new List<string>();
        }

        public async Task<IEnumerable<Applications>> GetApplicationListAsync(string orgcode)
        {
            return await _dbContext.Applications
                .Where(app => app.Organizations.OrgCode == orgcode)
                .ToListAsync();
        }
        public async Task<Applications> GetApplicationDetailsAsync(string appcode)
        {
            return await _dbContext.Applications
                .Include(ou => ou.Organizations)
                .FirstOrDefaultAsync(app => app.AppCode == appcode);
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<List<string>> GetAppCodeByEmailAsync(string userEmail)
        {            
            var appCodes = await _dbContext.ApplicationUsers
        .Where(au => au.Users.Email == userEmail)
        .Select(au => au.AppCode)
        .Distinct()
        .ToListAsync();

            return appCodes;
        }
        public async Task<Applications> GetAppIdByAppCodeAsync(string appCode)
        {
            return await _dbContext.Applications
                .FirstOrDefaultAsync(app => app.AppCode == appCode);
        }
        public async Task<List<string>> GetAppCodeByOrgCodeAsync(string orgcode)
        {
            return await _dbContext.Applications.Include(a => a.Organizations).Where(e => e.Organizations.OrgCode == orgcode).Select(e => e.AppCode).ToListAsync();
        }
        public async Task<string> GetOrgCodeByAppCodeAsync(string appcode)
        {
            var orgCode = await _dbContext.Applications
                .Where(au => au.AppCode == appcode)
                .Select(au => au.Organizations.OrgCode)
                .FirstOrDefaultAsync();
            return orgCode;
        }
        public async Task<IEnumerable<ApplicationUsers>> GetApplicationUsersByAppCodeAsync(string appCode)
        {
            return await _dbContext.ApplicationUsers
                .Where(au => au.AppCode == appCode).Include(au => au.Users).Include(a => a.Applications).Where(e => e.AppCode == appCode)
                .ToListAsync();
        }
        public async Task DeactivateApplicationAsync(Applications applications)
        {
            _dbContext.Update(applications);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GetEmailByAppCodeAsync(string appCode)
        {
            var appEmail = await _dbContext.Applications
                .Where(au => au.AppCode == appCode)
                .Select(au => au.ApplicationEmail)
                .FirstOrDefaultAsync();
            return appEmail;
        }
        public async Task<string> GetAppCodeByClientIdAsync(string clientId)
        {
            var application = await _dbContext.Applications
                .Where(app => app.ClientId == clientId)
                .FirstOrDefaultAsync();

            return application?.AppCode;
        }
    }
}
