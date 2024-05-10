using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Assignment.Infrastructure.Repository
{
    public class GoogleWorkSpaceRepository : IDBGoogleWorkSpaceRepository
    {
        private readonly RaidenDBContext _dbContext;

        public GoogleWorkSpaceRepository(RaidenDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> AddUserToGoogleSignIn(int userId)
        {
            GoogleSignIn model = new GoogleSignIn() { UserId = userId};
            model.IsEnabled=true;
            model.HasAcceptedInvite = false;
           _dbContext.GoogleSignIn.Add(model);
           var rs = await _dbContext.SaveChangesAsync();
            return rs;
        }
        public async Task<int> AcceptInvite(int userId)
        {
            var info = _dbContext.GoogleSignIn.FirstOrDefault(e=>e.UserId==userId);
            if(info!=null){
            GoogleSignIn model = new GoogleSignIn() { 
                GoogleSignInId= info.GoogleSignInId,
                UserId = userId,
            HasAcceptedInvite=true,
            IsEnabled=true};
            _dbContext.GoogleSignIn.Update(model);
            var rs = await _dbContext.SaveChangesAsync();
            return rs;
            }
            else{
                return 0;
            }
        }
        public async Task<GoogleSignIn> GetGoogleSignInDetails(int userId)
        {
            var rs = _dbContext.GoogleSignIn.FirstOrDefault(e=>e.UserId==userId);
            return rs;
        }
        public async Task<AllowedDomains> AddDomain(AllowedDomains model)
        {
            _dbContext.AllowedDomains.Add(model);
            await _dbContext.SaveChangesAsync();
            return model;
        }
        public async Task<bool> IsInvited(int userId)
        {
            var output = _dbContext.GoogleSignIn.FirstOrDefault(e=>e.UserId==userId);
            if (output != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<string>> GetAllowedDomains(int? orgId, int? appId)
        {
            var domains = await _dbContext.AllowedDomains
            .Where(ad => ad.OrganizationId == orgId && ad.ApplicationId == appId && ad.IsActive)
            .Select(ad => ad.Domain)
            .ToListAsync();
            return domains;
        }
        public async Task<int> DeleteUserGoogleWorkSpace(int userid)
        {
            try
            {
                var userToDelete = await _dbContext.GoogleSignIn
                                             .FirstOrDefaultAsync(e => e.UserId == userid);

                if (userToDelete != null)
                {
                    _dbContext.GoogleSignIn.Remove(userToDelete);
                    await _dbContext.SaveChangesAsync();
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Somthing went wrong");
            }
        }
    }
}
