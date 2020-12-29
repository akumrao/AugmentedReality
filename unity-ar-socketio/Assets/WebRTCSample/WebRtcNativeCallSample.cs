using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplePeerConnectionM;
using System;
using System.Runtime.InteropServices;
using System.Threading;

#if(UNITY_2018_3_OR_NEWER)
using UnityEngine.Android;
#endif
public class WebRtcNativeCallSample : MonoBehaviour {

   // bool first = true;
    Texture2D textureLocal;
    Texture2D textureRemote;
    public Material localTargetMaterial;
    public Material remoteTargetMaterial;
    public FrameQueue frameQueueLocal = new FrameQueue(3);
    public FrameQueue frameQueueRemote = new FrameQueue(5);
    public WebRtcVideoPlayer localPlayer;
    public WebRtcVideoPlayer remotePlayer;
    public CameraScript cameraPlayer; 

    public string status;
    public string roomID;

    public PeerConnectionM peer = null;

 
    private ArrayList permissionList = new ArrayList();

    private static int sFrameCount =0;

    private void CheckPermission()
    {
#if (UNITY_2018_3_OR_NEWER)
        foreach (string permission in permissionList)
        {
            if (Permission.HasUserAuthorizedPermission(permission))
            {

            }
            else
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }



    // Use this for initialization
    void Start() {

        sFrameCount = 0;

        #if (UNITY_2018_3_OR_NEWER)
        permissionList.Add(Permission.Microphone);
                permissionList.Add(Permission.Camera);
        #endif

        Debug.Log("Sample.Start() + " + " thread: " + Thread.CurrentThread.ManagedThreadId + ":" + Thread.CurrentThread.Name);
        if (localPlayer != null)
        {
            localPlayer.frameQueue = frameQueueLocal;
        }
        if (remotePlayer != null)
        {
            remotePlayer.frameQueue = frameQueueRemote;
        }

         InitWebRTC();
    }

    public void InitWebRTC() {

#if UNITY_ANDROID
        AndroidJavaClass systemClass = new AndroidJavaClass("java.lang.System");
        string libname = "jingle_peerconnection_so";
        systemClass.CallStatic("loadLibrary", new object[1] { libname });
        Debug.Log("loadLibrary loaded : " + libname);

        /*
         * Below is equivalent of this java code:
         * PeerConnectionFactory.InitializationOptions.Builder builder = 
         *   PeerConnectionFactory.InitializationOptions.builder(UnityPlayer.currentActivity);
         * PeerConnectionFactory.InitializationOptions options = 
         *   builder.createInitializationOptions();
         * PeerConnectionFactory.initialize(options);
         */

        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass webrtcClass = new AndroidJavaClass("org.webrtc.PeerConnectionFactory");
        AndroidJavaClass initOptionsClass = new AndroidJavaClass("org.webrtc.PeerConnectionFactory$InitializationOptions");
        AndroidJavaObject builder = initOptionsClass.CallStatic<AndroidJavaObject>("builder", new object[1] { activity });
        AndroidJavaObject options = builder.Call<AndroidJavaObject>("createInitializationOptions");
        if (webrtcClass != null)

        {
            Debug.Log("PeerConnectionFactory.initialize calling");
            webrtcClass.CallStatic("initialize", new object[1] { options });
            Debug.Log("PeerConnectionFactory.initialize called.");
        }

#endif
        List<string> servers = new List<string>();
        servers.Add("stun: stun.skyway.io:3478");
        servers.Add("stun: stun.l.google.com:19302");
        peer = new PeerConnectionM("44.226.10.202", 8080, roomID, servers, "", "");

        int id = peer.GetUniqueId();
        Debug.Log("PeerConnectionM.GetUniqueId() : " + id);


        peer.OnLocalVideoFrameReady += OnI420LocalFrameReady;
        peer.OnRemoteVideoFrameReady += OnI420RemoteFrameReady;
        // peer.OnFailureMessage += OnFailureMessage;

        peer.AddStream(false);

        cameraPlayer.peer = peer;

    }

//    WebRtcSocket socket;
   


    public void ReConnect()
    {

        Close();
        InitWebRTC();

    }

    public void Close()
    {
        if (peer != null)
        {
            peer.ClosePeerConnection();
            peer = null;
        }
    }

    public void OfferWithAndroid()
    {
       // Close();
        //cameraPlayer.broswer = false;
 
        if (peer != null)
        {

               
            Debug.Log("calling peer.CreateOffer()");
            peer.CreateOffer();
            Debug.Log("called peer.CreateOffer()");
        }
    }



    // Update is called once per frame
    void Update() {

    }


    IEnumerator DoSpriteAnimationInternal()
    {
       // IntPtr dataY = new IntPtr(2);
      //  peer.I420_PushFrame(dataY, dataY, dataY, dataY, 1, 2, 3, 4, 680, 480);

        yield return null;
    }



    public void OnI420LocalFrameReady(int id,
            IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA,
            int strideY, int strideU, int strideV, int strideA,
            uint width, uint height)
    {

     
           
        //Debug.Log("OnI420LocalFrameReady called! w=" + width + " h=" + height+" thread:"+ Thread.CurrentThread.ManagedThreadId + ":" + Thread.CurrentThread.Name);
        FramePacket packet = frameQueueLocal.GetDataBufferWithoutContents((int) (width * height * 4));
        if (packet == null)
        {
            //Debug.LogError("OnI420LocalFrameReady: FramePacket is null!");
            return;
        }
        CopyYuvToBuffer(dataY, dataU, dataV, strideY, strideU, strideV, width, height, packet.Buffer);
        packet.width = (int)width;
        packet.height = (int)height;
        frameQueueLocal.Push(packet);
    }

    public void OnI420RemoteFrameReady(int id,
        IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA,
        int strideY, int strideU, int strideV, int strideA,
        uint width, uint height)
    {
        
       /* if (  (++sFrameCount % 25) == 0)
        {
            Debug.Log("OnI420RemoteFrameReady clientID" +  id);
        }
        */

        //Debug.Log("OnI420RemoteFrameReady called! w=" + width + " h=" + height + " thread:" + Thread.CurrentThread.ManagedThreadId);

        //Debug.Log("OnI420RemoteFrameReady called! w=" + width + " h=" + height + " thread:" + Thread.CurrentThread.ManagedThreadId);
        FramePacket packet = frameQueueRemote.GetDataBufferWithoutContents((int)(width * height * 4));
        if (packet == null)
        {
            Debug.LogError("OnI420RemoteFrameReady: FramePacket is null!");
            return;
        }
        CopyYuvToBuffer(dataY, dataU, dataV, strideY, strideU, strideV, width, height, packet.Buffer);
        packet.width = (int)width;
        packet.height = (int)height;
        frameQueueRemote.Push(packet);
    }

    void CopyYuvToBuffer(IntPtr dataY, IntPtr dataU, IntPtr dataV,
        int strideY, int strideU, int strideV,
        uint width, uint height, byte[] buffer)
    {
        unsafe
        {
            byte* ptrY = (byte*)dataY.ToPointer();
            byte* ptrU = (byte*)dataU.ToPointer();
            byte* ptrV = (byte*)dataV.ToPointer();
            int srcOffsetY = 0;
            int srcOffsetU = 0;
            int srcOffsetV = 0;
            int destOffset = 0;
            for (int i = 0; i < height ; i++)
            {
                srcOffsetY = i * strideY;
                srcOffsetU = (i/2) * strideU;
                srcOffsetV = (i/2) * strideV;
                destOffset = i * (int)width * 4;
                for (int j = 0; j < width ; j+=2)
                {
                    {
                        byte y = ptrY[srcOffsetY];
                        byte u = ptrU[srcOffsetU];
                        byte v = ptrV[srcOffsetV];
                        srcOffsetY++;
                        srcOffsetU++;
                        srcOffsetV++;
                        
                        buffer[destOffset] = y;
                        buffer[destOffset + 1] = u;
                        buffer[destOffset + 2] = v;
                        buffer[destOffset + 3] = 0xff;
                        destOffset += 4;

                        // use same u, v values
                        byte y2 = ptrY[srcOffsetY];
                        srcOffsetY++;
                        
                        buffer[destOffset] = y2;
                        buffer[destOffset + 1] = u;
                        buffer[destOffset + 2] = v;
                        buffer[destOffset + 3] = 0xff;
                        destOffset += 4;
                    }
                }
            }
        }
    }

    public void OnFailureMessage(int id, string msg)
    {
        Debug.Log("OnFailureMessage called! id=" + id + " msg=" + msg);
    }

    void OnApplicationQuit()
    {
        Debug.Log("On Quit");
        Close();
    }

}
