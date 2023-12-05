using Microsoft.AspNetCore.Mvc;
using packetmail_api.Models;
using packetmail_api.Services;

namespace packetmail_api.Controllers;

[Route("mail")]
[ApiController]
public class MailController(BpqSessionManager bpqSessionManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromHeader] string sessionToken)
    {
        var session = await bpqSessionManager.RetrieveSessionAsync(sessionToken);

        if (session == null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized, "Session token not / no longer valid");
        }

        if (!await session.EnterBbs())
        {
            return StatusCode(500, "Failed to enter BBS");
        }

        return new ObjectResult(await session.GetMyMailSummary());
    }

    [HttpGet("{messageId}")]
    public async Task<IActionResult> GetMessage([FromHeader] string sessionToken, int messageId)
    {
        var session = await bpqSessionManager.RetrieveSessionAsync(sessionToken);

        if (session == null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized, "Session token not / no longer valid");
        }

        if (!await session.EnterBbs())
        {
            return StatusCode(500, "Failed to enter BBS");
        }

        return new ObjectResult(await session.GetMailMessage(messageId));
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromHeader] string sessionToken, [FromBody] SendMailRequest request)
    {
        var session = await bpqSessionManager.RetrieveSessionAsync(sessionToken);

        if (session == null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized, "Session token not / no longer valid");
        }

        if (!await session.EnterBbs())
        {
            return StatusCode(500, "Failed to enter BBS");
        }

        var (id, bid, size) = await session.SendMail(request.To, request.Title, request.Body);

        return new ObjectResult(new SendMailResponse(bid, id, size));
    }
}
