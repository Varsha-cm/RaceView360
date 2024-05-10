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
    public class OrganizationRepository : IDBOrganization
    {
        private readonly RaidenDBContext _dbContext;

        public OrganizationRepository(RaidenDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Organizations>> GetOrganizationsAsync()
        {
                return await _dbContext.Organizations
                                        .Where(org => org.IsActive)
                                        .ToListAsync();
        }

        public async Task<Organizations> GetOrganizationByIdAsync(string OrgCode)
        {
              return await _dbContext.Organizations.FirstOrDefaultAsync(o => o.IsActive && o.OrgCode == OrgCode);
            
        }

        public async Task<int> EditOrganizationDetailsAsync(Organizations organization)
        {
            _dbContext.Update(organization);
            var rs = await _dbContext.SaveChangesAsync();
            return rs;
        }
        public async Task<int> AddOrganizationAsync(Organizations organization)
        {
            _dbContext.Organizations.Add(organization);
            await _dbContext.SaveChangesAsync();
            return organization.OrganizationId;
        }
        public async Task DeactivateOrganizationAsync(Organizations organization)
        {
            _dbContext.Update(organization);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Organizations> GetOrganizationByEmailAsync(string organizationEmail)
        {
            return await _dbContext.Organizations
                .FirstOrDefaultAsync(org => org.OrganizationEmail == organizationEmail);
        }

        public async Task<Organizations> GetOrganizationByOrgCodeAsync(string orgCode)
        {
            return await _dbContext.Organizations
                .FirstOrDefaultAsync(org => org.OrgCode == orgCode);
        }
        public async Task<List<string>> GetOrgCodeByEmailAsync(string userEmail)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user != null)
            {
                var organizationUsers = _dbContext.OrganizationUsers
                    .Include(ou => ou.Organizations)
                    .Where(ou => ou.UserId == user.UserId)
                    .ToList();

                if (organizationUsers != null)
                {
                    var orgCodes = organizationUsers.Select(ou => ou.Organizations.OrgCode).ToList();
                    return orgCodes;
                }
            }

            return new List<string>();
        }
    }
}
