using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces
{
    public interface IDBGoogleWorkSpaceRepository
    {
        public Task<int> AddUserToGoogleSignIn(int userId);
        public Task<int> AcceptInvite(int userId);
        public Task<bool> IsInvited(int userId);
        public Task<GoogleSignIn> GetGoogleSignInDetails(int userId);
        public Task<AllowedDomains> AddDomain(AllowedDomains model);
        public Task<List<string>> GetAllowedDomains(int? orgId,int? appId);
        public Task<int> DeleteUserGoogleWorkSpace(int userid);
    }
}
