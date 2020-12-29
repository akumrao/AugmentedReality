using System;
using Scope.Core;

namespace Scope.RemoteAR
{
    public interface IApplicationLifeCycle : IService
    {
        event Action ApplicationQuitCallback;
    }
}
