using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceStatus.Application.DTO;
using ServiceStatus.Application.Interfaces;

namespace ServiceStatus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceStatusController : ControllerBase
    {
        private readonly IMonitoredServiceService _service;

        public ServiceStatusController(IMonitoredServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MonitoredServiceDTO>>> GetAllServices()
        {
            var services = await _service.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MonitoredServiceDTO>> GetService(Guid id)
        {
            var service = await _service.GetServiceStatusAsync(id);
            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpPost]
        public async Task<ActionResult<MonitoredServiceDTO>> AddService(CreateServiceStatusRequest request)
        {
            try
            {
                Console.WriteLine("adding service");
                var service = await _service.AddServiceAsync(request);
                return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var deleted = await _service.DeleteServiceAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id:guid}/check")]
        public async Task<ActionResult> CheckServiceStatus(Guid id)
        {
            var serviceExists = await _service.GetServiceStatusAsync(id);
            if (serviceExists == null)
                return NotFound();

            var isHealthy = await _service.CheckServiceStatusAsync(id);
            return Ok(new { isHealthy });
        }

        [HttpPost("check-all")]
        public async Task<ActionResult> CheckAllServices()
        {
            var services = await _service.GetAllServicesAsync();
            var checkTasks = services.Select(s => _service.CheckServiceStatusAsync(s.Id));
            var results = await Task.WhenAll(checkTasks);

            var healthyCount = results.Count(r => r);
            return Ok(new
            {
                totalServices = services.Count(),
                healthyServices = healthyCount,
                unhealthyServices = services.Count() - healthyCount
            });
        }
    }
}
