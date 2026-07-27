namespace Furaqui.Application.Interfaces;

public interface IAppConfiguration
{
    T GetAppSetting<T>(string key);
    string GetConnectionString(string connectionName);
}