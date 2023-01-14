using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Webhook : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<Webhook> _logger;
    private readonly MessageHandler _handler;

    public Webhook(IConfiguration config, ILogger<Webhook> logger, MessageHandler handler)
    {
        this._handler = handler;
        this._logger = logger;
        this._config = config;
    }

    [HttpGet]
    public ActionResult<string> ConnectWebhook([FromQuery(Name = "hub.mode")] string mode, [FromQuery(Name = "hub.verify_token")] string token, [FromQuery(Name = "hub.challenge")] int challenge)
    {

        if (mode == "subscribe" && token == _config["FBSecret"])
        {
            _logger.LogInformation("webhook verified");
            return Ok(challenge);
        }
        _logger.LogInformation("Not Real");
        return Unauthorized();
    }

    [HttpPost]
    public void MessageRecivedAsync([FromBody] MessageObject body)
    {
        // var message = JsonSerializer.Serialize(body);
        // _logger.LogInformation(body.ToString());
        // _logger.LogInformation(message);
        _handler.HandleMessage(body);
    }

}

