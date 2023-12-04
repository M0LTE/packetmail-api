using Microsoft.AspNetCore.Mvc;
using packetmail_api.Services;

namespace packetmail_api.Controllers;

[Route("login")]
[ApiController]
public class LoginController(BpqSessionManager bpqSessionManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(string bpqTelnetUsername, string password)
    {
        var result = await bpqSessionManager.CreateSession(bpqTelnetUsername, password);
        
        if (!string.IsNullOrWhiteSpace(result))
        {
            return Ok(result);
        }

        return BadRequest();
    }
}