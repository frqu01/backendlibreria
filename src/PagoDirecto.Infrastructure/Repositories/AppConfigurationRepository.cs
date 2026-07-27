using PagoDirecto.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace PagoDirecto.Infrastructure.Repositories;

internal class AppConfigurationRepository : IAppConfiguration
{
    protected readonly IConfiguration _iConfiguration;

    public AppConfigurationRepository(IConfiguration iConfiguration)
    {
        _iConfiguration = iConfiguration;
    }

    public T GetAppSetting<T>(string key)
    {
        return _iConfiguration.GetValue<T>(key);
    }

    public string GetConnectionString(string connectionName)
    {
        return _iConfiguration.GetConnectionString(connectionName);
    }
}
