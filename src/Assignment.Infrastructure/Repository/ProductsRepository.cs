using Microsoft.EntityFrameworkCore;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using System.Reflection.Emit;

namespace Assignment.Infrastructure.Repository
{
    public class ProductsRepository : IDBProductsRepository
    {
        private readonly RaidenDBContext _dbContext;
        private readonly IDBApplicationRepository _applicationRepository;


        public ProductsRepository(RaidenDBContext dbContext, IDBApplicationRepository applicationRepository)
        {
            _dbContext = dbContext;
            _applicationRepository = applicationRepository;
        }

        public async Task AddProductAsync(Products product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetAppIdByAppCodeAsync(string appCode)
        {
            var application = await _applicationRepository.GetAppIdByAppCodeAsync(appCode);
            if (application == null)
            {
                return 0;
            }
            return application.ApplicationId;
        }
        public async Task<List<ProductsApplication>> GetStatusOfProductAsync(string appCode)
        {
            var application = await _applicationRepository.GetAppIdByAppCodeAsync(appCode);
            var rs = _dbContext.ProductsApplications
                     .Include(e => e.Products)
                     .Where(e => e.ApplicationId == application.ApplicationId).ToList();
            return rs;
        }
        public async Task<List<string>> GetProductsAsync()
        {
            return await _dbContext.Products.Select(e => e.ProductName).ToListAsync();
        }
        public async Task<bool> ToggleProductAsync(string AppCode, int productId, bool IsEnabled)
        {
            int appId = await GetAppIdByAppCodeAsync(AppCode);

            var productApplication = await _dbContext.ProductsApplications
                .Include(pa => pa.Products)
                .Include(pa => pa.Applications)

                .Where(pa => pa.Applications.AppCode == AppCode && pa.ProductId == productId)
                .FirstOrDefaultAsync();

            if (productApplication != null)
            {
                productApplication.ApplicationId = appId;
                productApplication.IsEnabled = IsEnabled;
                _dbContext.ProductsApplications.Update(productApplication);
                await _dbContext.SaveChangesAsync();
                return true;


            }
            return false;
        }
        public async Task<bool> AddProductsApplication(int appId, int productId)
        {
            ProductsApplication model = new ProductsApplication();
            model.ProductId = productId;
            model.ApplicationId = appId;
            model.IsEnabled = false;
            _dbContext.ProductsApplications.Add(model);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Products> GetModuleByIdAsync(int productId)
        {
            var productName = await _dbContext.Products.Where(a => a.ProductId == productId).FirstOrDefaultAsync();
            return productName;


        }
    }
}

