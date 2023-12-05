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
}