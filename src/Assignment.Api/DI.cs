using Amazon.CloudWatchLogs.Model;
using Microsoft.Extensions.Configuration;
using Assignment.Api.Controllers;
using Assignment.Api.Interfaces;
using Assignment.Core.Interfaces;
using Assignment.Infrastructure.AuditLog;
using Assignment.Infrastructure.Interfaces;
using Assignment.Infrastructure.Notification;
using Assignment.Infrastructure.Raiden;
using Assignment.Infrastructure.Repository;
using Assignment.Service.Services;
using Serilog.Core;
using System.ComponentModel;
using static Assignment.Api.Controllers.GoogleWorkSpaceController;
using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Infrastructure.Repository.RaceRepository;
using Assignment.Service.Services.RaceService;

namespace Assignment.Api
{
    internal static partial class DI
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var logger = Logger.CreateLogger(configuration);
            CS_AppServices(services, configuration, logger);
        }
        private static void CS_AppServices(IServiceCollection services, IConfiguration configuration, Serilog.Core.Logger logger)
        {
            services.AddHttpClient<LoggingService>();
            services.AddTransient(typeof(IDBGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient(typeof(IDBRolesPermissionsRepository), typeof(RolesPermissionsRepository));
            services.AddTransient(typeof(RolesPermissionService), typeof(RolesPermissionService));
            services.AddScoped(typeof(IDBOrganization), typeof(OrganizationRepository));
            services.AddTransient(typeof(AuthService), typeof(AuthService));
            services.AddTransient(typeof(IDBAuthRepository), typeof(AuthRepository));
            services.AddTransient(typeof(Interfaces.IDBUserRepository), typeof(UserRepository));
            services.AddTransient(typeof(UserService), typeof(UserService));
            services.AddScoped(typeof(ApplicationsService), typeof(ApplicationsService));
            services.AddScoped(typeof(IDBApplicationRepository), typeof(ApplicationRepository));
            services.AddScoped(typeof(AuthController), typeof(AuthController));
            services.AddScoped(typeof(GoogleWorkSpaceService), typeof(GoogleWorkSpaceService));
            services.AddScoped(typeof(IDBGoogleWorkSpaceRepository), typeof(GoogleWorkSpaceRepository));
            services.AddScoped(typeof(CustomAuthorizationService), typeof(CustomAuthorizationService));
            services.AddScoped(typeof(IDBProductsRepository), typeof(ProductsRepository));
            services.AddScoped(typeof(ProductsService), typeof(ProductsService));
            services.AddScoped(typeof(INotificationService), typeof(NotificationService));
            services.AddScoped(typeof(IRaidenService), typeof(RaidenService));
            services.AddHttpClient<GoogleWorkSpaceController>();
            services.AddHttpClient<NotificationService>();
            services.AddHttpClient<RaidenService>();
            services.AddScoped(typeof(OrganizationService), typeof(OrganizationService));
            services.AddScoped(typeof(ILoggingService), typeof(LoggingService));
            services.AddSingleton<Serilog.Core.Logger>(logger);

            services.AddScoped(typeof(CircuitService), typeof(CircuitService));
            services.AddScoped(typeof(IDBCircuitRepository), typeof(CircuitRepository));
            services.AddScoped(typeof(DriverService), typeof(DriverService));
            services.AddScoped(typeof(IDBDriverRepository), typeof(DriverRepository));
            services.AddScoped(typeof(TeamService), typeof(TeamService));
            services.AddScoped(typeof(IDBTeamRepository), typeof(TeamRepository));
            services.AddScoped(typeof(SeasonService), typeof(SeasonService));
            services.AddScoped(typeof(IDBSeasonRepository), typeof(SeasonRepository));
            services.AddScoped(typeof(RaceService), typeof(RaceService));
            services.AddScoped(typeof(IDBRaceRepository), typeof(RaceRepository));
            services.AddScoped(typeof(LiveTrackingService), typeof(LiveTrackingService));
            services.AddScoped(typeof(IDBLapsRepository), typeof(LapRepository));
            services.AddScoped(typeof(RaceResultService), typeof(RaceResultService));
            services.AddScoped(typeof(IDBRoundRepository), typeof(RoundRepository));
            services.AddScoped(typeof(IDBRaceStandingsRepository), typeof(RaceStandingsRepository));
            services.AddScoped(typeof(StandingService), typeof(StandingService));
            services.AddScoped(typeof(IDBRaceUserRepository), typeof(RaceUserRepository));
            services.AddScoped(typeof(RaceAuthService), typeof(RaceAuthService));
            services.AddScoped(typeof(IDBRaceResultRepository), typeof(RaceResultRepository));

        }
    }
}