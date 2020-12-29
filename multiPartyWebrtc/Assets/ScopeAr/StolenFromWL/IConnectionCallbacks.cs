using System;

namespace Scope.Core
{
    public enum CallSessionTermination
    {
        Accepted = 0,
        Rejected = 1,
        Busy = 2,
        Disconnected = 3,
        NegotiationTimeout = 4,
        ConnectionError = 5,
        ConnectionTimeout = 6,
        FailedToConnect = 7,
        Cancelled = 8,
        Incompatible = 9
    }

    public interface IConnectionCallbacks
    {
        event Action OnConnected;
        event Action OnDisconnected;
    }
}

