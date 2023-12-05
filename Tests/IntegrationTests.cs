using FluentAssertions;
using packetmail_api.Services;
using System.Diagnostics;

namespace Tests;

public class IntegrationTests
{
    [Fact]
    public async Task Test1()
    {
        var session = new BpqSession();
        var loginResult = await session.LoginToNode("localhost", 8010, "user", "password", "Welcome to GB7AAA Telnet Server\\n Enter ? for list of commands\\n\\n");
        loginResult.Should().BeTrue();
        var enterBbsResult = await session.EnterBbs();
        enterBbsResult.Should().BeTrue();
        var mailList = await session.GetMyMailSummary();
        mailList.Should().NotBeNullOrEmpty();

        var mail = await session.GetMailMessage(mailList[0].Id);
        mail.Should().NotBeNull();
        Debugger.Break();
    }
}
