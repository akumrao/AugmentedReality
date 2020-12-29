using System;

namespace Scope.Core.Audio
{
    public interface IAudioManager : IRecordingManagement, IService
    {
        event EventHandler<EventArgs> AudioDisconnected;
        event EventHandler<EventArgs> AudioConnected;
        event EventHandler<EventArgs> AudioConnectionError;
        bool Muted { get; set; }
    }
}
