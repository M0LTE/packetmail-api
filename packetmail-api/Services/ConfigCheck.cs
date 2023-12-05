using Microsoft.Extensions.Options;
using packetmail_api.Configuration;

namespace packetmail_api.Services;

public class ConfigCheck(IOptions<ServiceConfig> serviceConfig) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(serviceConfig.Value.NodeWelcomeText))
        {
            throw new Exception($"{nameof(serviceConfig.Value.NODE_TELNET_CTEXT)} not specified");
        }

        if (string.IsNullOrWhiteSpace(serviceConfig.Value.BpqHost))
        {
            throw new Exception($"{nameof(serviceConfig.Value.BPQ_HOST)} not specified");
        }

        if (serviceConfig.Value.BpqTelnetPort == 0)
        {
            throw new Exception($"{nameof(serviceConfig.Value.BPQ_TELNET_PORT)} not specified");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
