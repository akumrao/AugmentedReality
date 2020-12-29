﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// native impl: 
// https://chromium.googlesource.com/external/webrtc/+/51e2046dbcbbb0375c383594aa4f77aa8ed67b06/examples/unityplugin/simple_peer_connection.cc
// https://chromium.googlesource.com/external/webrtc/+/51e2046dbcbbb0375c383594aa4f77aa8ed67b06/examples/unityplugin/unity_plugin_apis.cc

namespace SimplePeerConnectionM
{
   
      
    // A managed wrapper up class for the native c style peer connection APIs.
    public class PeerConnectionM
    {
        //private const string dllPath = "webrtc_unity_plugin";
#if UNITY_ANDROID
        const string dllPath = "libjingle_peerconnection_so";
#else
        const string dllPath = "webrtc_unity_plugin";
#endif

        //create a peerconnection with turn servers
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int CreatePeerConnection( string signalServerIP, int port, string roomid, string[] turnUrls, int noOfUrls,
            string username, string credential);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ClosePeerConnection(int peerConnectionId);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool AddStream(int peerConnectionId, bool audioOnly);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool AddDataChannel(int peerConnectionId);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CreateOffer(int peerConnectionId);


        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SendDataViaDataChannel(int peerConnectionId, string data);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SetAudioControl(int peerConnectionId, bool isMute, bool isRecord);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void  I420_PushFrame(int peerConnectionId,
            IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA,
            int strideY, int strideU, int strideV, int strideA,
            uint width, uint height);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void I420_PushFrameRGBA(int peerConnectionId,
        IntPtr rgbBuf, int bufLen, IntPtr augBuf, int augLen, uint width, uint height);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void LocalDataChannelReadyInternalDelegate();
        public delegate void LocalDataChannelReadyDelegate(int id);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool RegisterOnLocalDataChannelReady(
            int peerConnectionId, LocalDataChannelReadyInternalDelegate callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DataFromDataChannelReadyInternalDelegate(string s);
        public delegate void DataFromDataChannelReadyDelegate(int id, string s);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool RegisterOnDataFromDataChannelReady(
            int peerConnectionId, DataFromDataChannelReadyInternalDelegate callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FailureMessageInternalDelegate(string msg);
        public delegate void FailureMessageDelegate(int id, string msg);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool RegisterOnFailure(int peerConnectionId,
            FailureMessageInternalDelegate callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void AudioBusReadyInternalDelegate(IntPtr data, int bitsPerSample,
            int sampleRate, int numberOfChannels, int numberOfFrames);
        public delegate void AudioBusReadyDelegate(int id, IntPtr data, int bitsPerSample,
            int sampleRate, int numberOfChannels, int numberOfFrames);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool RegisterOnAudioBusReady(int peerConnectionId,
            AudioBusReadyInternalDelegate callback);

        // Video callbacks.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void I420FrameReadyInternalDelegate(int clinetID,
            IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA,
            int strideY, int strideU, int strideV, int strideA,
            uint width, uint height);
        public delegate void I420FrameReadyDelegate(int clinetID,
            IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA,
            int strideY, int strideU, int strideV, int strideA,
            uint width, uint height);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool RegisterOnLocalI420FrameReady(int peerConnectionId,
            I420FrameReadyInternalDelegate callback);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool RegisterOnRemoteI420FrameReady(int peerConnectionId,
            I420FrameReadyInternalDelegate callback);

      
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void IceCandiateReadytoSendInternalDelegate(
            string candidate, int sdpMlineIndex, string sdpMid);
        public delegate void IceCandiateReadytoSendDelegate(
            int id, string candidate, int sdpMlineIndex, string sdpMid);

        
        public PeerConnectionM(string signalServerIP, int port, string roomid, List<string> turnUrls, string username, string credential)
        {
            string[] urls = turnUrls != null ? turnUrls.ToArray() : null;
            int length = turnUrls != null ? turnUrls.Count : 0;
            mPeerConnectionId = CreatePeerConnection(signalServerIP, port, roomid, urls, length, username, credential);
            RegisterCallbacks();
        }
        public void ClosePeerConnection()
        {
            ClosePeerConnection(mPeerConnectionId);
            mPeerConnectionId = -1;
        }
        // Return -1 if Peerconnection is not available.
        public int GetUniqueId()
        {
            return mPeerConnectionId;
        }
        public void AddStream(bool audioOnly)
        {
            AddStream(mPeerConnectionId, audioOnly);
        }
        public void AddDataChannel()
        {
            AddDataChannel(mPeerConnectionId);
        }
        public void CreateOffer()
        {
            CreateOffer(mPeerConnectionId);
        }

        public void SendDataViaDataChannel(string data)
        {
            SendDataViaDataChannel(mPeerConnectionId, data);
        }
        public void SetAudioControl(bool isMute, bool isRecord)
        {
            SetAudioControl(mPeerConnectionId, isMute, isRecord);
        }

        public void I420_PushFrame(IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA,
            int strideY, int strideU, int strideV, int strideA, uint width, uint height)
        {
            I420_PushFrame(mPeerConnectionId, dataY, dataU, dataV, dataA,
             strideY, strideU, strideV, strideA, width, height);
        }

        public void I420_PushFrameRGBA( IntPtr rgbBuf, int bufLen, IntPtr augBuf,
            int augLen, uint width, uint height)
        {
            I420_PushFrameRGBA(mPeerConnectionId, rgbBuf, bufLen, augBuf, augLen, width, height);
        }

 
        private void RegisterCallbacks()
        {
            localDataChannelReadyDelegate = new LocalDataChannelReadyInternalDelegate(
                RaiseLocalDataChannelReady);
            RegisterOnLocalDataChannelReady(mPeerConnectionId, localDataChannelReadyDelegate);
            dataFromDataChannelReadyDelegate = new DataFromDataChannelReadyInternalDelegate(
                RaiseDataFromDataChannelReady);
            RegisterOnDataFromDataChannelReady(mPeerConnectionId, dataFromDataChannelReadyDelegate);
            failureMessageDelegate = new FailureMessageInternalDelegate(RaiseFailureMessage);
            RegisterOnFailure(mPeerConnectionId, failureMessageDelegate);
            localI420FrameReadyDelegate = new I420FrameReadyInternalDelegate(
              RaiseLocalVideoFrameReady);
            RegisterOnLocalI420FrameReady(mPeerConnectionId, localI420FrameReadyDelegate);
            remoteI420FrameReadyDelegate = new I420FrameReadyInternalDelegate(
              RaiseRemoteVideoFrameReady);
            RegisterOnRemoteI420FrameReady(mPeerConnectionId, remoteI420FrameReadyDelegate);

        }
        private void RaiseLocalDataChannelReady()
        {
            if (OnLocalDataChannelReady != null)
                OnLocalDataChannelReady(mPeerConnectionId);
        }
        private void RaiseDataFromDataChannelReady(string data)
        {
            if (OnDataFromDataChannelReady != null)
                OnDataFromDataChannelReady(mPeerConnectionId, data);
        }
        private void RaiseFailureMessage(string msg)
        {
            if (OnFailureMessage != null)
                OnFailureMessage(mPeerConnectionId, msg);
        }
        private void RaiseAudioBusReady(IntPtr data, int bitsPerSample,
          int sampleRate, int numberOfChannels, int numberOfFrames)
        {
            if (OnAudioBusReady != null)
                OnAudioBusReady(mPeerConnectionId, data, bitsPerSample, sampleRate,
                    numberOfChannels, numberOfFrames);
        }
        private void RaiseLocalVideoFrameReady( int clinetID,
            IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA,
            int strideY, int strideU, int strideV, int strideA,
            uint width, uint height)
        {
            if (OnLocalVideoFrameReady != null)
                OnLocalVideoFrameReady(clinetID, dataY, dataU, dataV, dataA, strideY, strideU, strideV, strideA,
                  width, height);
        }
        private void RaiseRemoteVideoFrameReady(int clinetID,
           IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA,
           int strideY, int strideU, int strideV, int strideA,
           uint width, uint height)
        {
            if (OnRemoteVideoFrameReady != null)
                OnRemoteVideoFrameReady(clinetID, dataY, dataU, dataV, dataA, strideY, strideU, strideV, strideA,
                  width, height);
        }
    
   
        private LocalDataChannelReadyInternalDelegate localDataChannelReadyDelegate = null;
        public event LocalDataChannelReadyDelegate OnLocalDataChannelReady;
        private DataFromDataChannelReadyInternalDelegate dataFromDataChannelReadyDelegate = null;
        public event DataFromDataChannelReadyDelegate OnDataFromDataChannelReady;
        private FailureMessageInternalDelegate failureMessageDelegate = null;
        public event FailureMessageDelegate OnFailureMessage;
        private AudioBusReadyInternalDelegate audioBusReadyDelegate = null;
        public event AudioBusReadyDelegate OnAudioBusReady;
        private I420FrameReadyInternalDelegate localI420FrameReadyDelegate = null;
        public event I420FrameReadyDelegate OnLocalVideoFrameReady;
        private I420FrameReadyInternalDelegate remoteI420FrameReadyDelegate = null;
        public event I420FrameReadyDelegate OnRemoteVideoFrameReady;
        private int mPeerConnectionId = -1;
    }
}