using System;

namespace Scope.RemoteAR
{
    public class AudioSDPToken
    {
        public string Type = "";
        public string SDP = "";
        public Guid MessageId = Guid.NewGuid();
    }

    public class AudioIceCandidateToken
    {
        public string Candidate = "";
        public int SdpMlineIndex;
        public string SdpMid = "";
        public Guid MessageId = Guid.NewGuid();
    }

}
