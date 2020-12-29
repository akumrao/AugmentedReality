using System;

namespace Scope.Core.Logging
{
    public interface ICallLogProvider : IService
    {
        ICallLogger CallLogger { get; }
        Action<bool> PostScopeLogs(string type, string comment);
    }

    public interface ICallLogger
    {
        string LocalSDP { set; }
        string RemoteSDP { set; }
    }

    public class DumbCallLogger : ICallLogger
    {
        public string LocalSDP { get;  set; }
        public string RemoteSDP { get; set; }
    }

    public class DumbCallLogProvider : ICallLogProvider
    {
        public ICallLogger CallLogger => new DumbCallLogger();

        public Action<bool> PostScopeLogs(string type, string comment)
        {
            return (bool a) => { };
        }
    }
}
