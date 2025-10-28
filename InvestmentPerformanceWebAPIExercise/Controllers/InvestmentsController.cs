using InvestmentPerformanceWebAPIExercise.Data;
using Microsoft.AspNetCore.Mvc;
using static InvestmentPerformanceWebAPIExercise.Common.Attributes;

namespace InvestmentPerformanceWebAPIExercise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class InvestmentsController : ControllerBase
    {
        private readonly InvestmentDbContext _context;
        private readonly ILogger<InvestmentsController> _logger;

        public InvestmentsController(InvestmentDbContext context, ILogger<InvestmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all investments for a user.
        /// </summary>
        [HttpGet("user")]
        public IActionResult GetUserInvestments([FromQuery] Guid userId)
        {
            _logger.LogInformation($"*** {ControllerContext.ActionDescriptor.ControllerName} - {ControllerContext.ActionDescriptor.ActionName} called ***");

            try
            {
                var investments = _context.Investments
                    .Where(i => i.UserId == userId)
                    .Select(i => new { i.InvestmentId, i.Name })
                    .ToList();

                if (!investments.Any())
                {
                }

                return Ok(investments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ControllerContext.ActionDescriptor.ControllerName} - {ControllerContext.ActionDescriptor.ActionName}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get detailed performance of a user's investment.
        /// </summary>
        [HttpGet("user/{userId}/investment/{investmentId}")]
        public IActionResult GetInvestmentDetails(Guid userId, Guid investmentId)
        {
            _logger.LogInformation($"*** {ControllerContext.ActionDescriptor.ControllerName} - {ControllerContext.ActionDescriptor.ActionName} called ***");

            try
            {
                var investment = _context.Investments
                    .FirstOrDefault(i => i.UserId == userId && i.InvestmentId == investmentId);

                if (investment == null)
                    return NotFound();

                var dto = new InvestmentDetailDto
                {
                    Shares = investment.Shares,
                    CostBasisPerShare = investment.CostBasisPerShare,
                    CurrentPrice = investment.CurrentPrice,
                    Term = (DateTime.UtcNow - investment.PurchaseDate).TotalDays <= 365 ? "Short Term" : "Long Term",
                    TotalGainLoss = (investment.Shares * investment.CurrentPrice) - (investment.Shares * investment.CostBasisPerShare)
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ControllerContext.ActionDescriptor.ControllerName} - {ControllerContext.ActionDescriptor.ActionName}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
