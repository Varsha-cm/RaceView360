using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces
{
    public interface IDBProductsRepository
    {
        Task AddProductAsync(Products product);
        Task<bool> ToggleProductAsync(string AppCode, int productId, bool IsEnabled);
        public Task<bool> AddProductsApplication(int appId, int productId);
        public Task<List<string>> GetProductsAsync();
        public Task<List<ProductsApplication>> GetStatusOfProductAsync(string appCode);

        public Task<Products> GetModuleByIdAsync(int productId);
    }
}
