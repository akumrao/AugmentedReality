using System;

namespace Scope.Core.NetworkServices
{
    public interface IConnectionICEMessageSender
    {
        void SendSDP(string type, string sdp);
        void SendIceCandidate(string candidate, int sdpMlineIndex, string sdpMid);
    }
}