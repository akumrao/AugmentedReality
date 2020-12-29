// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.InteropServices;
using Scope.RemoteAR;
using UnityEngine;


using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace SpatialTranformHelper
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Float2 : IEquatable<Float2>
    {
        public float X;
        public float Y;

        public static readonly Float2 Zero = default(Float2);

        public static Float2 FromUnity(UnityEngine.Vector2 vector)
        {
            return new Float2
            {
                X = vector.x,
                Y = vector.y
            };
        }

        public bool Equals(Float2 other)
        {
            return this.X == other.X
                && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Float2)
            {
                return this.Equals((Float2)obj);
            }

            return false;
        }

        public static bool operator ==(Float2 a, Float2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Float2 a, Float2 b)
        {
            return !a.Equals(b);
        }

    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Float3 : IEquatable<Float3>
    {
        public float X;
        public float Y;
        public float Z;

        public static readonly Float3 Zero = default(Float3);

        public static Float3 FromUnity(UnityEngine.Vector3 vector)
        {
            return new Float3
            {
                X = vector.x,
                Y = vector.y,
                Z = vector.z
            };
        }

        public bool Equals(Float3 other)
        {
            return this.X == other.X
                && this.Y == other.Y
                && this.Z == other.Z;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Float3)
            {
                return this.Equals((Float3)obj);
            }

            return false;
        }

        public static bool operator ==(Float3 a, Float3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Float3 a, Float3 b)
        {
            return !a.Equals(b);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4x4 : IEquatable<Matrix4x4>
    {
        // row major
        public float M00;
        public float M01;
        public float M02;
        public float M03;
        public float M10;
        public float M11;
        public float M12;
        public float M13;
        public float M20;
        public float M21;
        public float M22;
        public float M23;
        public float M30;
        public float M31;
        public float M32;
        public float M33;

        public static readonly Matrix4x4 Zero = default(Matrix4x4);

        public static Matrix4x4 BaseProjectionMatrix = new Matrix4x4()
        {   // row major
            M00 = 2.0f,
            M01 = 0.0f,
            M02 = 0.0f,
            M03 = 0.0f,
            M10 = 0.0f,
            M11 = -2.0f,
            M12 = 0.0f,
            M13 = 0.0f,
            M20 = -1.0f,
            M21 = 1.0f,
            M22 = 1.0f,
            M23 = 1.0f,
            M30 = 0.0f,
            M31 = 0.0f,
            M32 = 0.0f,
            M33 = 0.0f
        };

        public static Matrix4x4 FromUnity(UnityEngine.Matrix4x4 unityMatrix)
        {
            return new Matrix4x4
            {   // to row major
                M00 = unityMatrix.m00,
                M10 = unityMatrix.m01,
                M20 = unityMatrix.m02,
                M30 = unityMatrix.m03,

                M01 = unityMatrix.m10,
                M11 = unityMatrix.m11,
                M21 = unityMatrix.m12,
                M31 = unityMatrix.m13,

                M02 = unityMatrix.m20,
                M12 = unityMatrix.m21,
                M22 = unityMatrix.m22,
                M32 = unityMatrix.m23,

                M03 = unityMatrix.m30,
                M13 = unityMatrix.m31,
                M23 = unityMatrix.m32,
                M33 = unityMatrix.m33,
            };
        }

        public static Matrix4x4 FromSystemNumerics(System.Numerics.Matrix4x4 dotNetMatrix)
        {
            return new Matrix4x4
            {   // to row major
                M00 = dotNetMatrix.M11,
                M10 = dotNetMatrix.M21,
                M20 = dotNetMatrix.M31,
                M30 = dotNetMatrix.M41,

                M01 = dotNetMatrix.M12,
                M11 = dotNetMatrix.M22,
                M21 = dotNetMatrix.M32,
                M31 = dotNetMatrix.M42,

                M02 = dotNetMatrix.M13,
                M12 = dotNetMatrix.M23,
                M22 = dotNetMatrix.M33,
                M32 = dotNetMatrix.M43,

                M03 = dotNetMatrix.M14,
                M13 = dotNetMatrix.M24,
                M23 = dotNetMatrix.M34,
                M33 = dotNetMatrix.M44,
            };
        }

        public bool Equals(Matrix4x4 other)
        {
            return this.M00 == other.M00
                && this.M01 == other.M01
                && this.M02 == other.M02
                && this.M03 == other.M03
                && this.M10 == other.M10
                && this.M11 == other.M11
                && this.M12 == other.M12
                && this.M13 == other.M13
                && this.M20 == other.M20
                && this.M21 == other.M21
                && this.M22 == other.M22
                && this.M23 == other.M23;
        }

        public override bool Equals(object obj)
        {
            if (obj is Matrix4x4)
            {
                return this.Equals((Matrix4x4)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Matrix4x4 a, Matrix4x4 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Matrix4x4 a, Matrix4x4 b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}\n{4} {5} {6} {7}\n{8} {9} {10} {11}\n{12} {13} {14} {15}\n"
                , this.M00, this.M01, this.M02, this.M03, this.M10, this.M11, this.M12, this.M13, this.M20, this.M21, this.M22, this.M23, this.M30, this.M31, this.M32, this.M33);
        }

        public Matrix4x4 Invert()
        {
            return FromUnity(this.ToUnityRaw().inverse);
        }
    }

    public struct CameraPose
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public CameraPose(Vector3 position, Quaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }
    }

    public static class Functions
    {
        /// <summary>
        /// Converts the input vector to a Unity vector2
        /// </summary>
        public static UnityEngine.Vector2 ToUnity(this Float2 rawVector)
        {
            return new UnityEngine.Vector2
            {
                x = rawVector.X,
                y = rawVector.Y
            };
        }

        /// <summary>
        /// Converts the input vector to a Unity vector3
        /// </summary>
        public static UnityEngine.Vector3 ToUnity(this Float3 rawVector)
        {
            return new UnityEngine.Vector3
            {
                x = rawVector.X,
                y = rawVector.Y,
                z = rawVector.Z
            };
        }


        /// <summary>
        /// Converts the input matrix to a Unity matrix and transposes it
        /// </summary>
        public static UnityEngine.Matrix4x4 ToUnityRaw(this Matrix4x4 rawMatrix)
        {
            return new UnityEngine.Matrix4x4
            {   // to column major
                m00 = rawMatrix.M00,
                m01 = rawMatrix.M10,
                m02 = rawMatrix.M20,
                m03 = rawMatrix.M30,

                m10 = rawMatrix.M01,
                m11 = rawMatrix.M11,
                m12 = rawMatrix.M21,
                m13 = rawMatrix.M31,

                m20 = rawMatrix.M02,
                m21 = rawMatrix.M12,
                m22 = rawMatrix.M22,
                m23 = rawMatrix.M32,

                m30 = rawMatrix.M03,
                m31 = rawMatrix.M13,
                m32 = rawMatrix.M23,
                m33 = rawMatrix.M33,
            };
        }

        /// <summary>
        /// Converts the input matrix to Unity transform matrix.
        /// </summary>
        public static UnityEngine.Matrix4x4 ToUnityTransform(this Matrix4x4 inputMatrix)
        {
            var zflip = UnityEngine.Matrix4x4.identity;
            zflip.SetColumn(2, -1 * zflip.GetColumn(2));

            return zflip * inputMatrix.ToUnityRaw() * zflip;
        }

        /// <summary>
        /// Converts Unity matrix to position and rotation values
        /// </summary>
        public static void GetTranslationRotation(this UnityEngine.Matrix4x4 matrix, out Vector3 position, out Quaternion rotation)
        {
            position = matrix.Translation();
            rotation = matrix.Rotation();
        }

        public static Vector3 Translation(this UnityEngine.Matrix4x4 trsMatrix)
        {
            return trsMatrix.GetColumn(3);
        }

        public static Quaternion Rotation(this UnityEngine.Matrix4x4 trsMatrix)
        {
            return Quaternion.LookRotation(trsMatrix.GetColumn(2), trsMatrix.GetColumn(1));
        }

        public static Vector3 Scale(this UnityEngine.Matrix4x4 trsMatrix)
        {
            var scale = new Vector3(
                trsMatrix.GetColumn(0).magnitude,
                trsMatrix.GetColumn(1).magnitude,
                trsMatrix.GetColumn(2).magnitude);
            return scale;
        }

        /// <summary>
        /// Converts the input projection matrix to a Unity matrix and overrides the near and far clip plane.
        /// </summary>
        public static UnityEngine.Matrix4x4 ToUnityProjection(this Matrix4x4 inputMatrix, float nearClipPlane, float farClipPlane)
        {
            // The following code enforces valid near/far clip plane values
            float epsilon = 0.01f;
            if (nearClipPlane < epsilon)
            {
                nearClipPlane = epsilon;
            }

            if (farClipPlane < nearClipPlane + epsilon)
            {
                farClipPlane = nearClipPlane + epsilon;
            }

            var unityMatrix = inputMatrix.ToUnityRaw();
            unityMatrix.m22 = -(farClipPlane + nearClipPlane) / (farClipPlane - nearClipPlane);
            unityMatrix.m23 = -(2.0f * farClipPlane * nearClipPlane) / (farClipPlane - nearClipPlane);

            return unityMatrix;
        }

        public static CameraPose? ConvertWorldViewMatrix(this Matrix4x4 cameraPose)
        {
            if (cameraPose == Matrix4x4.Zero)
            {
                return null;
            }

            Vector3 cameraPosition;
            Quaternion cameraRotation;
            cameraPose.ToUnityTransform().GetTranslationRotation(out cameraPosition, out cameraRotation);

            return new CameraPose(cameraPosition, cameraRotation);
        }

        public static UnityEngine.Matrix4x4? ConvertCameraProjectionMatrix(this Matrix4x4 cameraProjection, float nearClipPlane, float farClipPlane)
        {
            if (cameraProjection == Matrix4x4.Zero)
            {
                return null;
            }

            return cameraProjection.ToUnityProjection(nearClipPlane, farClipPlane);
        }

        public static UnityEngine.Matrix4x4? GetUnityMatrix(this Matrix4x4 matrix)
        {
            if (matrix == Matrix4x4.Zero)
            {
                return null;
            }

            return matrix.ToUnityRaw();
        }

        private static UnityEngine.Matrix4x4 ConvertMatrixFromSystemToUnity(System.Numerics.Matrix4x4 matrix)
        {
            UnityEngine.Matrix4x4 result = new UnityEngine.Matrix4x4();
            result.m00 = matrix.M11;
            result.m01 = matrix.M12;
            result.m02 = matrix.M13;
            result.m03 = matrix.M14;

            result.m10 = matrix.M21;
            result.m11 = matrix.M22;
            result.m12 = matrix.M23;
            result.m13 = matrix.M24;

            result.m20 = matrix.M31;
            result.m21 = matrix.M32;
            result.m22 = matrix.M33;
            result.m23 = matrix.M34;

            result.m30 = matrix.M41;
            result.m31 = matrix.M42;
            result.m32 = matrix.M43;
            result.m33 = matrix.M44;

            return result;
        }



        /* VULKAN METHODS*/

        // The TryGetProjectionMatrix code was taken from https://github.com/VulcanTechnologies/HoloLensCameraStream/blob/master/HoloLensCameraStream/Plugin%20Project/VideoCaptureSample.cs
        // Copyright (c) 2017 Vulcan, Inc. All rights reserved.  
        // Licensed under the Apache 2.0 license

        /// <summary>
        /// This returns the projection matrix at the time the photo was captured, if location data if available.
        /// If it's not, that is probably an indication that the HoloLens is not tracking and its location is not known.
        /// It could also mean the VideoCapture stream is not running.
        /// If location data is unavailable then the projecgtion matrix will be set to the identity matrix.
        /// </summary>
        /// <param name="matrix">The projection matrix used to match the true camera projection.
        /// The matrix will have to be converted to a Unity matrix before it can be used by methods in the UnityEngine namespace.
        /// See https://forum.unity3d.com/threads/locatable-camera-in-unity.398803/ for details.</param>
        public static bool TryGetProjectionMatrix(
            System.Numerics.Matrix4x4 projectionMatrix,
            float nearPlane,
            float farPlane,
            out UnityEngine.Matrix4x4 outMatrix)
        {
            // The following code enforces valid near/far clip plane values
            float epsilon = 0.01f;
            if (nearPlane < epsilon)
            {
                nearPlane = epsilon;
            }

            if (farPlane < nearPlane + epsilon)
            {
                farPlane = nearPlane + epsilon;
            }

            // Transpose matrix to match expected Unity format
            outMatrix = ConvertMatrixFromSystemToUnity(System.Numerics.Matrix4x4.Transpose(projectionMatrix));

            outMatrix.m22 = -(farPlane + nearPlane) / (farPlane - nearPlane);
            outMatrix.m23 = -(2.0f * farPlane * nearPlane) / (farPlane - nearPlane);

            return true;
        }
    }
}