using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/v1/[controller]")]
    public abstract class BaseController : Controller
    {

        private readonly Serilog.Core.Logger logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        protected BaseController(Serilog.Core.Logger logger)
        {
            this.logger = logger;
        }

        // Common functionality, e.g., logging
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected void LogInformation(string message)
        {
            logger.Information(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected void LogError(string message, Exception exception)
        {
            logger.Error(exception, message);
        }
    }
}
