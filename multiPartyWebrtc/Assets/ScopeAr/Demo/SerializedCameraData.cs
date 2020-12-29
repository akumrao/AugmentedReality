using System;
using System.IO;
using Scope.RemoteAR;
using UnityEngine;

namespace Scope.RemoteAR
{
    public struct SerializedCameraData
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Matrix4x4 ProjectionMatrix { get; set; }
        public long Timestamp { get; set; }
        public bool SendProjectionMatrix { get; }

        public SerializedCameraData(Vector3 position, Quaternion rotation, Matrix4x4 projectionMatrix, long timestamp, bool sendProjectionMatrix)
        {
            Position = position;
            Rotation = rotation;
            ProjectionMatrix = projectionMatrix;
            Timestamp = timestamp;
            SendProjectionMatrix = sendProjectionMatrix;
        }

        internal byte[] Serialize()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteToStream(stream);

                return stream.GetBuffer();
            }
        }

        void WriteToStream(Stream where)
        {
            using (BinaryWriter writer = new BinaryWriter(where))
            {
                writer.Write(Timestamp);
                new Vector3Serializer(Position).WriteToStream(writer);
                new QuaternionSerializer(Rotation).WriteToStream(writer);

                if (!ProjectionMatrix.Equals(default(Matrix4x4)) || SendProjectionMatrix)
                {
                    writer.Write(true);
                    new Matrix4Serializer(ProjectionMatrix).WriteToStream(writer);
                }
                else
                {
                    writer.Write(false);
                }
            }
        }

        public static SerializedCameraData ReadFromStream(MemoryStream from)
        {
            BinaryReader reader = new BinaryReader(from);
            SerializedCameraData data = new SerializedCameraData();

            try
            {
                data.Timestamp = reader.ReadInt64();

                data.Position = new Vector3Serializer(data.Position).ReadFromStream(reader);
                data.Rotation = new QuaternionSerializer(data.Rotation).ReadFromStream(reader);

                var containsProjectionMatrix = reader.ReadBoolean();
                if (containsProjectionMatrix)
                {
                    data.ProjectionMatrix = new Matrix4Serializer(data.ProjectionMatrix).ReadFromStream(reader);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error Parsing SerializedCameraData: Packet Corrupt.");
                Debug.LogWarning(e.ToString());
            }

            return data;
        }
    }
}