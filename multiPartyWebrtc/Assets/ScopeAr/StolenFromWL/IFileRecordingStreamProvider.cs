using System;

namespace Scope.Core
{
    public enum StreamType { VIDEO, VIDEOLOG, AUDIOIN, AUDIOOUT, ANNOTATIONLOG }

    public interface IFileRecordingStreamProvider
    {
        string GetPath(StreamType type);
    }

    public class FileRecordingStreamProvider : IFileRecordingStreamProvider
    {
        public string GetPath(StreamType type)
        {
            switch (type)
            {
                case StreamType.AUDIOIN:
                    return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "local.m4a");

                case StreamType.AUDIOOUT:
                    return System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "remote.m4a");

                default:
                    throw new NotImplementedException();
            }
        }
    }
}