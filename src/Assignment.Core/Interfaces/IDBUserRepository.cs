using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces
{

    public interface IDBUserRepository
    {
        Task<int> CreateUserAsync(Users user);
        Task UpdateOrganizationUserAsync(int userId, int organizationId, string orgCode);
        Task<Users> GetUserByEmailAsync(string email);
        Task<IEnumerable<OrganizationUsers>> GetOrganizationUserAsync(string orgcode);

        Task<int> CreateAppUserAsync(Users user);
        Task UpdateApplicationUserAsync(int userId, int organizationId, string orgCode, string appCode, int appId);
        
        Task<IEnumerable<OrganizationUsers>> GetOrganizationUsersAsync();
        Task<IEnumerable<OrganizationUsers>> GetOrganizationUsersAsync(string OrgCode);

        public Task<IEnumerable<ApplicationUsers>> GetApplicationUsersAsync();
        public Task<IEnumerable<ApplicationUsers>> GetApplicationUsersAsync(string AppCode);

        public Task DeactivateUserAsync(OrganizationUsers users);
        public Task<OrganizationUsers> GetUserById(string orgcode, int userId);

        public Task<ApplicationUsers> GetAppUserById(string appcode, int userId);

        public Task DeactivateAppUserAsync(ApplicationUsers users);
        public Task<int> AddUserRoleAsync(int userId, int roleId, int? orgId, int? appId);
        public Task<int> GetUserIdByEmailAsync(string email);
    }

}
