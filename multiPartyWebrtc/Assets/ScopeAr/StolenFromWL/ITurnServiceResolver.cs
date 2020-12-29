using System.Collections.Generic;

namespace Scope.RemoteAR.NetworkServices
{
    public interface ITurnServiceResolver
    {
        List<ITurnConfiguration> GetAllTurnConfigurations();
    }

    public interface ITurnConfiguration
    {
        string Protocol { get; }
        string Address { get; }
        ushort Port { get; }
        string Username { get; }
        string Password { get; }
    }

    public class TurnConfiguration : ITurnConfiguration
    {
        public string Protocol { get; set; }
        public string Address { get; set; }
        public ushort Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}