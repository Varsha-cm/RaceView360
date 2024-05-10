using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using Assignment.Core.ThirdPartyModels;
using Assignment.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services
{
    public class ProductsService
    {
        private readonly IDBProductsRepository _productsRepository;
        private readonly INotificationService _notificationService;
        private readonly IDBApplicationRepository _applicationrepository;
        private readonly IDBOrganization _organizationrepository;
        private readonly ILoggingService _loggingService;

        public ProductsService(IDBProductsRepository productsRepository,IDBOrganization oraganizationrepository, ILoggingService loggingService, IDBApplicationRepository applicationRepository, INotificationService notificationService)
        {
            _productsRepository = productsRepository;
            _notificationService = notificationService;
            _applicationrepository = applicationRepository;
            _loggingService = loggingService;
            _organizationrepository = oraganizationrepository;
        }

        public async Task AddProductAsync(ProductsRQ productDto)
        {
            var newProduct = new Products
            {
                ProductId = productDto.ProductId
            };

            await _productsRepository.AddProductAsync(newProduct);
        }

        public async Task<bool> ToggleProductAsync(ProductsRQ requestModel, string userEmail)
        {

            var rs = await _productsRepository.ToggleProductAsync(requestModel.AppCode, requestModel.ProductId, requestModel.IsEnabled);
            var app = await _applicationrepository.GetApplicationDetailsAsync(requestModel.AppCode);
            var moduleName = await _productsRepository.GetModuleByIdAsync(requestModel.ProductId);
            

            var status = (requestModel.IsEnabled) ? "Enabled" : "Disabled";
            if (rs && moduleName != null)
            {
                var orgCode = await _applicationrepository.GetOrgCodeByAppCodeAsync(requestModel.AppCode);
                var appModel = new Applications()
                {
                    AppCode = app.AppCode,
                    ApplicationEmail = app.ApplicationEmail,
                    ApplicationName = app.ApplicationName,
                   
                };
                await _notificationService.ModuleNotificationAsync(moduleName.ProductName, appModel, userEmail, status, "CYRAX_MODULE");

                var eventCode = requestModel.IsEnabled ? "moduleEnable" : "moduleDisable";
                await LogModuleEnableDisableAsync(rs, appModel, moduleName.ProductName, userEmail, status, orgCode, eventCode);   

            }
            else
            {
                await _loggingService.LogEventAsync(rs, new AuditLogRQ
                {
                    EventCode = "moduleActivityFailure",
                    ObjectName = "ModuleToggleFailed",
                    ObjectCode = requestModel.AppCode,
                    ActionType = "Module Enable/Disable",
                    targets = new Dictionary<string, string> { { "modules", "Module Id is " + requestModel.ProductId.ToString() } },
                    ExecutedBy = userEmail,
                    ExecutedOn = DateTime.Now,
                    LogData = new { ErrorMessage = "unable to perform the action" },
                });
            }
            return rs;
        }

        public async Task LogModuleEnableDisableAsync(bool isSuccess, Applications appModel, string productName, string userEmail, string status, string orgCode, string eventCode)
        {
            var logEvent = new AuditLogRQ
            {
                EventCode =  eventCode,
                ObjectName = appModel.ApplicationName,
                ObjectCode = appModel.AppCode,
                ActionType = $"Module {status}",
                targets = new Dictionary<string, string> { { "Application", appModel.AppCode}, { "Organization", orgCode} },
                ExecutedBy = userEmail,
                ExecutedOn = DateTime.Now,
                LogData = new { ModuleName = productName , Action = status },
            };

            await _loggingService.LogEventAsync(isSuccess, logEvent);
        }
    }

}
