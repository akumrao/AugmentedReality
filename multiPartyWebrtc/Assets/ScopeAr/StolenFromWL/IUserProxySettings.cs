namespace Scope.Core
{
    public interface IUserProxySettings
    {
        bool HasHttpProxySettings { get; }
        string HttpProxyAddress { get; set; }
        string HttpProxyUsername { get; set; }
        string HttpProxyPassword { get; set; }
    }
}