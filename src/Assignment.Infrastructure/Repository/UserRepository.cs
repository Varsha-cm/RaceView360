using Microsoft.EntityFrameworkCore;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Repository
{
    public class UserRepository : IDBUserRepository
    {
        private readonly RaidenDBContext _dbContext;

        public UserRepository(RaidenDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> CreateUserAsync(Users user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user.UserId;
        }
        public async Task UpdateOrganizationUserAsync(int userId, int organizationId, string orgCode)
        {
            var orgUser = await _dbContext.OrganizationUsers
                .SingleOrDefaultAsync(ou => ou.UserId == userId && ou.OrganizationId == organizationId);

            if (orgUser == null)
            {

                orgUser = new OrganizationUsers
                {
                    UserId = userId,
                    OrganizationId = organizationId,
                    Orgcode = orgCode,
                    CreatedTimestamp = DateTime.Now,
                    ModifiedTimestamp = DateTime.Now
                };

                _dbContext.OrganizationUsers.Add(orgUser);
            }
            else
            {
                orgUser.Orgcode = orgCode;
                orgUser.ModifiedTimestamp = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task<Users> GetUserByEmailAsync(string email)
        {

            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }        


        public async Task<IEnumerable<OrganizationUsers>> GetOrganizationUserAsync(string orgcode)
        {
            return await _dbContext.OrganizationUsers
                .Include(t => t.Users)
                .Where(t => t.Orgcode == orgcode).ToListAsync();
        }       
        public async Task DeactivateUserAsync(OrganizationUsers users)
        {
            users.IsActive = false;
            _dbContext.Update(users);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CreateAppUserAsync(Users user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user.UserId;
        }
        public async Task<int> AddUserRoleAsync(int userId, int roleId, int? orgId, int? appId)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                OrganizationId = orgId,
                ApplicationId = appId
            };

            _dbContext.UserRoles.Add(userRole);
            await _dbContext.SaveChangesAsync();

            return userRole.UserRoleId;
        }
    
    public async Task UpdateApplicationUserAsync(int userId, int organizationId, string orgCode, string appCode, int appId)
        {
            var appUser = await _dbContext.ApplicationUsers
                .SingleOrDefaultAsync(au => au.UserId == userId && au.OrganizationId == organizationId && au.AppCode == appCode);

            if (appUser == null)
            {
                appUser = new ApplicationUsers
                {
                    UserId = userId,
                    OrganizationId = organizationId,
                    ApplicationId = appId,
                    Orgcode = orgCode,
                    AppCode = appCode,
                    CreatedTimestamp = DateTime.Now,
                    ModifiedTimestamp = DateTime.Now
                };

                _dbContext.ApplicationUsers.Add(appUser);
            }
            else
            {
                appUser.AppCode = appCode;
                appUser.ModifiedTimestamp = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task<int> GetUserIdByEmailAsync(string email)
        {
            var user = _dbContext.Users
                .Where(au => au.Email == email).FirstOrDefault();
            if (user != null)
            {
                return user.UserId;
            }
            else
            {
                return 0;
            }

        }

        public async Task<IEnumerable<OrganizationUsers>> GetOrganizationUsersAsync()
        {
            // Implement the query to retrieve organization users
            return await _dbContext.OrganizationUsers.Include(u => u.Organizations).Include(a=>a.Users)
                .ToListAsync();
        }
        public async Task<IEnumerable<OrganizationUsers>> GetOrganizationUsersAsync(string OrgCode)
        {
            // Implement the query to retrieve organization users
            return await _dbContext.OrganizationUsers.Include(u => u.Organizations).Include(a => a.Users).Where(e=>e.Orgcode==OrgCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUsers>> GetApplicationUsersAsync()
        {
            // Implement the query to retrieve application users
            return await _dbContext.ApplicationUsers.Include(u => u.Organizations).Include(u => u.Applications).Include(a=>a.Users)
                .ToListAsync();
        }
        public async Task<IEnumerable<ApplicationUsers>> GetApplicationUsersAsync(String AppCode)
        {
            // Implement the query to retrieve application users
            return await _dbContext.ApplicationUsers.Include(u => u.Organizations).Include(u => u.Applications).Include(a => a.Users).Where(e=>e.AppCode == AppCode)
                .ToListAsync();
        }



        public async Task<OrganizationUsers> GetUserById(string orgcode,int userId)
        {
            return  _dbContext.OrganizationUsers.Include(e => e.Users)
                .SingleOrDefault(u => u.IsActive && u.Orgcode == orgcode && u.UserId == userId);
        }

        public async Task<ApplicationUsers> GetAppUserById(string appcode, int userId)
        {
            return  _dbContext.ApplicationUsers.Include(e=>e.Users)
                .SingleOrDefault(t => t.IsActive && t.AppCode == appcode && t.UserId == userId);
        }

        public async Task DeactivateAppUserAsync(ApplicationUsers users)
        {
            users.IsActive = false;
            _dbContext.Update(users);
            await _dbContext.SaveChangesAsync();
        }
    }

}
