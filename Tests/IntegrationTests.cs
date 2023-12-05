using FluentAssertions;
using packetmail_api.Services;

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

        var mail4047 = await session.GetMailMessage(4047);
        mail4047.Should().NotBeNull();
        mail4047!.From.Should().Be("GM5AUG");
        mail4047!.To.Should().Be("M0LTE");
        mail4047!.Bid.Should().Be("623_GB7AUG");
        mail4047!.Body.Should().Be("Hey Tom\nJust checking the node is back up and running.\nHave a good day!\n73 M");
        mail4047!.Title.Should().Be("Testing");
        mail4047!.TypeStatus.Should().Be("PY");
        mail4047!.Routing.Should().HaveCount(2);

        string body = "this is a message\ntest";
        (int id, string bid, int size) = await session.SendMail(to: "m0lte", title: "this is the subject", body);
        id.Should().NotBe(0);
        bid.Should().Be($"{id}_GB7RDG");
        size.Should().Be(body.Length + 3);

        var mail = await session.GetMailMessage(4063);
        mail.Should().NotBeNull();
        mail!.From.Should().Be("M0LTE");
        mail!.To.Should().Be("M0LTE");
        mail!.Bid.Should().Be("4063_GB7RDG");
        mail!.Body.Should().Be("this is a message\ntest");
        mail!.Title.Should().Be("this is the subject");
        mail!.TypeStatus.Should().Be("PY");
    }
}
