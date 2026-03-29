using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevForge.API.Controllers
{
    /// <summary>
    /// Health and monitoring endpoints
    /// </summary>
    [ApiController]
    [Route("api/v1/health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        /// <summary>
        /// Initializes a new instance of the HealthController
        /// </summary>
        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Simple ping endpoint to check if API is alive
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/health/ping
        ///     
        /// Returns simple pong response to verify API is running.
        /// This endpoint does not check database or other dependencies.
        /// </remarks>
        /// <returns>Pong response with timestamp</returns>
        /// <response code="200">API is alive</response>
        [HttpGet("ping")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok(new
            {
                status = "healthy",
                message = "pong",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        }

        /// <summary>
        /// Get API information
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/health/info
        ///     
        /// Returns basic API information including version, uptime, etc.
        /// </remarks>
        /// <returns>API information</returns>
        /// <response code="200">Successfully retrieved API info</response>
        [HttpGet("info")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetInfo()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "1.0.0";
            var processStartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime;
            var uptime = DateTime.Now - processStartTime;

            return Ok(new
            {
                application = "DevForge Authentication API",
                version = version,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                framework = Environment.Version.ToString(),
                uptime = new
                {
                    days = uptime.Days,
                    hours = uptime.Hours,
                    minutes = uptime.Minutes,
                    seconds = uptime.Seconds,
                    totalSeconds = uptime.TotalSeconds
                },
                machineName = Environment.MachineName,
                processId = Environment.ProcessId,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
