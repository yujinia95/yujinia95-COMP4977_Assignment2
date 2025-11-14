using System;
using IosAssignment2Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace IosAssignment2Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketMasterController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly TicketMasterApiSettings _settings;

    public TicketMasterController(
        IHttpClientFactory httpClientFactory,
        IOptions<TicketMasterApiSettings> settings)
    {
        _httpClient = httpClientFactory.CreateClient("TicketMasterClient");
        _settings = settings.Value;
    }

    // Example: GET api/ticketmaster/events?dmaId=528
    [HttpGet("events")]
    public async Task<IActionResult> GetEvents([FromQuery] int dmaId)
    {
        if (dmaId < 500 || dmaId > 531)
        {
            return BadRequest("Invalid DMA ID for Canadian cities.");
        }

        try
        {
            string url = $"{_settings.BaseUrl}events.json" +
                            $"?classificationName=music" +
                            $"&dmaId={dmaId}" +
                            $"&apikey={_settings.ApiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode,
                    $"TicketMaster error: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                DmaId = dmaId,
                RawJson = json
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Server error: {ex.Message}");
        }
    }
}
