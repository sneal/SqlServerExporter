namespace Sneal.SqlExporter.Core
{
    public interface IConnectionSettings
    {
        string ServerName { get; }
        bool UseIntegratedAuthentication { get; }
        string UserName { get; }
        string Password { get; }
    }
}