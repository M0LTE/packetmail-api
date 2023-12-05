using FluentAssertions;
using packetmail_api.Models;

namespace Tests;

public class BpqMailSummaryModelTests
{ 
    [Fact]
    public void TestParsingFull()
    {
        MailSummary.TryParse("404312 02-Nov PY 1111184 M0LTEA @GB7AAA GM5AUG Test of HF forwarding", out var summary).Should().BeTrue();
        summary.Id.Should().Be(404312);
        summary.Date.Should().Be("02-Nov");
        summary.Type.Should().Be("PY");
        summary.Length.Should().Be(1111184);
        summary.DestinationCall.Should().Be("M0LTEA");
        summary.DestinationBbs.Should().Be("@GB7AAA");
        summary.SourceCall.Should().Be("GM5AUG");
        summary.Subject.Should().Be("Test of HF forwarding");
    }

    [Fact]
    public void TestParsingShort()
    {
        MailSummary.TryParse("4043   02-Nov PY     184 M0LTE  @GB7RD  GM5AU  Test of HF forwarding", out var summary).Should().BeTrue();
        summary.Id.Should().Be(4043);
        summary.Date.Should().Be("02-Nov");
        summary.Type.Should().Be("PY");
        summary.Length.Should().Be(184);
        summary.DestinationCall.Should().Be("M0LTE");
        summary.DestinationBbs.Should().Be("@GB7RD");
        summary.SourceCall.Should().Be("GM5AU");
        summary.Subject.Should().Be("Test of HF forwarding");
    }

    [Fact]
    public void TestParsingLocalMail()
    {
        var lines = new[] {
            "From: fromcall",
"To: M0LTE",
"Type/Status: PN",
"Date/Time: 05-Dec 21:32Z",
"Bid: 4062_GB7RDG",
"Title: this is the subject",
"",
"this is a message",
"test",
"",
"",
"[End of Message #4062 from M0LTE]",
"de GB7RDG>",
""
        };

        Mail.TryParse(4062, lines, out var mail).Should().BeTrue();

        mail.From.Should().Be("fromcall");
        mail.To.Should().Be("M0LTE");
        mail.TypeStatus.Should().Be("PN");
        mail.DateTime.Should().Be("05-Dec 21:32Z");
        mail.Bid.Should().Be("4062_GB7RDG");
        mail.Title.Should().Be("this is the subject");
        mail.Body.Should().Be("this is a message\ntest");
        mail.Routing.Should().BeEmpty();
    }

    [Fact]
    public void TestParsingRemoteMail()
    {
        var lines = new[] {
            "From: fromcall",
"To: M0LTE",
"Type/Status: PN",
"Date/Time: 05-Dec 21:32Z",
"Bid: 4062_GB7RDG",
"Title: this is the subject",
"",
"R:231116/1228Z 1892@GB7IOW.GB7IOW.#48.GBR.EURO LinBPQ6.0.24",
"R:231116/1226Z 623@GB7AUG.#78.GBR.EURO LinBPQ6.0.24",
"",
"this is a message",
"",
"",
"[End of Message #4062 from M0LTE]",
"de GB7RDG>",
""
        };

        Mail.TryParse(4062, lines, out var mail).Should().BeTrue();

        mail.From.Should().Be("fromcall");
        mail.To.Should().Be("M0LTE");
        mail.TypeStatus.Should().Be("PN");
        mail.DateTime.Should().Be("05-Dec 21:32Z");
        mail.Bid.Should().Be("4062_GB7RDG");
        mail.Title.Should().Be("this is the subject");
        mail.Body.Should().Be("this is a message");
        mail.Routing.Should().Contain("R:231116/1228Z 1892@GB7IOW.GB7IOW.#48.GBR.EURO LinBPQ6.0.24");
        mail.Routing.Should().Contain("R:231116/1226Z 623@GB7AUG.#78.GBR.EURO LinBPQ6.0.24");
    }
}