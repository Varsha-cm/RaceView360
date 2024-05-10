using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces
{
    public interface IDBApplicationRepository
    {
        public Task<int> AddApplicationAsync(Applications application);
        Task<Users> GetUserByEmailAsync(string email);
        public Task<List<string>> GetAppCodeByEmailAsync(string userEmail);
        Task<IEnumerable<ApplicationUsers>> GetApplicationUsersByAppCodeAsync(string appCode);
        public Task<Applications> GetAppIdByAppCodeAsync(string appCode);
        Task<int> UpdateApplicationAsync(Applications applicationDM,string orgcode);
        public Task<List<string>> GetOrgCodeByEmailAsync(string userEmail);
        public Task<IEnumerable<Applications>> GetApplicationListAsync(string orgcode);
        public Task<Applications> GetApplicationDetailsAsync(string appcode);
        public Task<string> GetOrgCodeByAppCodeAsync(string appcode);
        Task DeactivateApplicationAsync(Applications applications);
        public Task<List<string>> GetAppCodeByOrgCodeAsync(string orgcode);


        Task<string> GetEmailByAppCodeAsync(string appCode);

        Task<string> GetAppCodeByClientIdAsync(string clientId);

    }
}

