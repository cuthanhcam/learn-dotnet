using System.Net;
using System.Net.Http.Json;
using Learning.Api.Features.OrderQuotes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Learning.Api.Tests;

public sealed class OrderQuotesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrderQuotesControllerTests(WebApplicationFactory<Program> factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task Create_ValidRequestUsesScopedTenantAndBusinessRule()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/order-quotes")
        {
            Content = JsonContent.Create(new CreateOrderQuoteRequest
            {
                Sku = "desk-01",
                Quantity = 10,
                UnitPrice = 50m
            })
        };
        request.Headers.Add(RequireTenantFilter.HeaderName, "tenant-42");

        using HttpResponseMessage response = await _client.SendAsync(request);
        OrderQuote? quote = await response.Content.ReadFromJsonAsync<OrderQuote>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("tenant-42", quote!.TenantId);
        Assert.Equal("DESK-01", quote.Sku);
        Assert.Equal(50m, quote.Discount);
        Assert.Equal(450m, quote.Total);
    }

    [Fact]
    public async Task Create_MissingTenantIsShortCircuitedByFilter()
    {
        using HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/api/order-quotes",
            new CreateOrderQuoteRequest { Sku = "desk-01", Quantity = 1, UnitPrice = 50m });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidBodyUsesAutomaticApiControllerValidation()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/order-quotes")
        {
            Content = JsonContent.Create(new CreateOrderQuoteRequest
            {
                Sku = "invalid sku!",
                Quantity = 0,
                UnitPrice = -1m
            })
        };
        request.Headers.Add(RequireTenantFilter.HeaderName, "tenant-42");

        using HttpResponseMessage response = await _client.SendAsync(request);
        HttpValidationProblemDetails? problem =
            await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(nameof(CreateOrderQuoteRequest.Sku), problem!.Errors.Keys);
        Assert.Contains(nameof(CreateOrderQuoteRequest.Quantity), problem.Errors.Keys);
        Assert.Contains(nameof(CreateOrderQuoteRequest.UnitPrice), problem.Errors.Keys);
    }
}
