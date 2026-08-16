using Microsoft.AspNetCore.Mvc;

namespace Learning.Api.Features.OrderQuotes;

[ApiController]
[Route("api/order-quotes")]
[ServiceFilter<RequireTenantFilter>]
public sealed class OrderQuotesController(OrderQuoteService quoteService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<OrderQuote>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<OrderQuote> Create(CreateOrderQuoteRequest request)
    {
        // ApiController causes invalid DataAnnotations model state to return HTTP 400 before this
        // method executes. Business calculation therefore receives a structurally valid request.
        return Ok(quoteService.Create(request));
    }
}
