using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces
{
    public interface IDBOrganization
    {
        Task<IEnumerable<Organizations>> GetOrganizationsAsync();
        Task<Organizations> GetOrganizationByIdAsync(string OrgCode);
        Task<int> EditOrganizationDetailsAsync(Organizations organization);
        Task<int> AddOrganizationAsync(Organizations organization);
        Task DeactivateOrganizationAsync(Organizations organization);
        Task<Organizations> GetOrganizationByOrgCodeAsync(string orgCode);
        Task<Organizations> GetOrganizationByEmailAsync(string organizationEmail);
        public Task<List<string>> GetOrgCodeByEmailAsync(string userEmail);
    }
}
