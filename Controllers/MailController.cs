using Microsoft.AspNetCore.Mvc;
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
            return new NotFoundResult();
        }

        if (!await session.EnterBbs())
        {
            return StatusCode(500, "Unable to enter BBS");
        }

        return new ObjectResult(await session.GetMyMailSummary());
    }
}
