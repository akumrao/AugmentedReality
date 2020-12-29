using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scope.RemoteAR.Logging
{
    public interface IScopeLogger : IDisposable
    {
        Guid CurrentCallSessionId { set; }
        ILogger Bolt { get; }
        ILogger Unity { get; }
        ILogger Native { get; }
        void HandleNativeLog(string message);
        void Debug(string message);
        void Error(string message);
        void Info(string message);
        void Warn(string message);
    }

    public interface ILogger
    {
        string ToString();
    }

    public class ScopeLogger : IScopeLogger
    {
        public Guid CurrentCallSessionId { get; set; }

        public ILogger Bolt => (ILogger)new object();

        public ILogger Unity => (ILogger)new object();

        public ILogger Native => (ILogger)new object();

        public void Debug(string message)
        {
        }

        public void Dispose()
        {
        }

        public void Error(string message)
        {
        }

        public void HandleNativeLog(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Info(string message)
        {
        }

        public void Warn(string message)
        {
        }
    }
}
