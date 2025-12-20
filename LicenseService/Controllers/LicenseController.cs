using MediatR;
using Microsoft.AspNetCore.Mvc;
using LicenseService.Commands;
using LicenseService.Queries;

namespace LicenseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LicenseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CreateLicenseResult>> CreateLicense([FromBody] CreateLicenseCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        

        [HttpGet("list")]
        public async Task<ActionResult> GetLicenses([FromQuery] int tenantId, [FromQuery] int? userId)
        {
            var query = new GetLicensesQuery 
            { 
                TenantId = tenantId,
                UserId = userId
            };
            
            var licenses = await _mediator.Send(query);
            return Ok(licenses);
        }

        [HttpPost("update-payment")]
        public async Task<ActionResult<UpdatePaymentResult>> UpdatePayment([FromBody] UpdateLicensePaymentCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



        [HttpGet("active")]
        public async Task<ActionResult> GetActiveLicense([FromQuery] int tenantId, [FromQuery] int userId)
        {
            var query = new GetActiveLicenseQuery 
            { 
                TenantId = tenantId,
                UserId = userId
            };
            
            var license = await _mediator.Send(query);
            
            if (license == null)
            {
                return NotFound(new { message = "No active license found" });
            }

            return Ok(license);
        }
    }
}