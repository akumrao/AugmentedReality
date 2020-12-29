package com.scopear.webrtc;

import android.app.Activity;
import android.content.Context;
import android.os.SystemClock;
import android.util.Log;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Base64;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.Toast;

import androidx.annotation.NonNull;

import org.json.JSONException;
import org.json.JSONObject;
import org.webrtc.AudioSource;
import org.webrtc.AudioTrack;
import org.webrtc.Camera1Enumerator;
import org.webrtc.CameraEnumerator;
import org.webrtc.CapturerObserver;
import org.webrtc.JavaI420Buffer;
import org.webrtc.MultiplexVideoDecoderFactory;
import org.webrtc.MultiplexVideoEncoderFactory;
import org.webrtc.VideoDecoderFactory;
import org.webrtc.VideoEncoderFactory;


import org.webrtc.EglBase;
import org.webrtc.IceCandidate;
import org.webrtc.Logging;
import org.webrtc.MediaConstraints;
import org.webrtc.MediaStream;
import org.webrtc.PeerConnection;
//import org.webrtc.PeerConnectionFactory;
import org.webrtc.SessionDescription;
import org.webrtc.SurfaceTextureHelper;
import org.webrtc.SurfaceViewRenderer;
import org.webrtc.VideoCapturer;
import org.webrtc.VideoFrame;
import org.webrtc.VideoSink;
import org.webrtc.VideoSource;
import org.webrtc.VideoTrack;

import java.io.UnsupportedEncodingException;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;


import org.webrtc.Logging.Severity;
import org.webrtc.Loggable;

import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.TimeUnit;
import java.util.logging.Level;
import java.util.logging.Logger;



public class PeerConnectionFactory implements SignallingClient.SignalingInterface
{
    private static final String TAG = "ScopePeerConnectionFactory";


    private static final boolean ENABLE_H264_HIGH_PROFILE = false;

    private org.webrtc.PeerConnectionFactory factory;
    public Activity activity;

    MediaConstraints audioConstraints;
    MediaConstraints videoConstraints;
    MediaConstraints sdpConstraints;
    VideoSource videoSource;
    VideoTrack localVideoTrack;
    AudioSource audioSource;
    AudioTrack localAudioTrack;
    SurfaceTextureHelper surfaceTextureHelper;

//    MediaConstraints audioConstraints;
  //  MediaConstraints videoConstraints;
   // MediaConstraints sdpConstraints;
   // VideoSource videoSource;
    //VideoTrack localVideoTrack;
   // AudioSource audioSource;
    //AudioTrack localAudioTrack;
    //SurfaceTextureHelper surfaceTextureHelper;

   // SurfaceViewRenderer localVideoView;
    //SurfaceViewRenderer remoteVideoView;

    Button hangup;
    PeerConnection localPeer;
    List<IceServer> iceServers;
   // EglBase rootEglBase;

    boolean gotUserMedia;
    List<PeerConnection.IceServer> peerIceServers = new ArrayList<>();

    final int ALL_PERMISSIONS_CODE = 1;

//    private static final String TAG = "MainActivity";

//    @Override
//    protected void onCreate(Bundle savedInstanceState) {
//        super.onCreate(savedInstanceState);
//        setContentView(R.layout.activity_main);
//
//        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED
//                || ContextCompat.checkSelfPermission(this, Manifest.permission.RECORD_AUDIO) != PackageManager.PERMISSION_GRANTED) {
//            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA, Manifest.permission.RECORD_AUDIO}, ALL_PERMISSIONS_CODE);
//        } else {
//            // all permissions already granted
//            start();
//        }
//    }
//
//    @Override
//    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
//        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
//
//        if (requestCode == ALL_PERMISSIONS_CODE
//                && grantResults.length == 2
//                && grantResults[0] == PackageManager.PERMISSION_GRANTED
//                && grantResults[1] == PackageManager.PERMISSION_GRANTED) {
//            // all permissions granted
//            start();
//        } else {
//            finish();
//        }
//    }
//
//    private void initViews() {
//        hangup = findViewById(R.id.end_call);
//        localVideoView = findViewById(R.id.local_gl_surface_view);
//        remoteVideoView = findViewById(R.id.remote_gl_surface_view);
//        hangup.setOnClickListener(this);
//    }

    private void initVideos() {
       // rootEglBase = EglBase.create();
      //  localVideoView.init(rootEglBase.getEglBaseContext(), null);
      //  remoteVideoView.init(rootEglBase.getEglBaseContext(), null);
      //  localVideoView.setZOrderMediaOverlay(true);
       // remoteVideoView.setZOrderMediaOverlay(true);
    }

    private void getIceServers() {
        //get Ice servers using xirsys
        byte[] data = new byte[0];
        try {
            data = ("<xirsys_ident>:<xirsys_secret>").getBytes("UTF-8");
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
        String authToken = "Basic " + Base64.encodeToString(data, Base64.NO_WRAP);
        Utils.getInstance().getRetrofitInstance().getIceCandidates(authToken).enqueue(new Callback<TurnServerPojo>() {
            @Override
            public void onResponse(@NonNull Call<TurnServerPojo> call, @NonNull Response<TurnServerPojo> response) {
                TurnServerPojo body = response.body();
                if (body != null) {
                    iceServers = body.iceServerList.iceServers;
                }
                for (IceServer iceServer : iceServers) {
                    if (iceServer.credential == null) {
                        PeerConnection.IceServer peerIceServer = PeerConnection.IceServer.builder(iceServer.url).createIceServer();
                        peerIceServers.add(peerIceServer);
                    } else {
                        PeerConnection.IceServer peerIceServer = PeerConnection.IceServer.builder(iceServer.url)
                                .setUsername(iceServer.username)
                                .setPassword(iceServer.credential)
                                .createIceServer();
                        peerIceServers.add(peerIceServer);
                    }
                }
                Log.d("onApiResponse", "IceServers\n" + iceServers.toString());
            }

            @Override
            public void onFailure(@NonNull Call<TurnServerPojo> call, @NonNull Throwable t) {
                t.printStackTrace();
            }
        });
    }

    private final static Logger LOGGER =  Logger.getLogger("arvind");  //Logger.GLOBAL_LOGGER_NAME

    private static class MockLoggable implements Loggable {
        private ArrayList<String> messages = new ArrayList<>();
        private ArrayList<Severity> sevs = new ArrayList<>();
        private ArrayList<String> tags = new ArrayList<>();

        @Override
        public void onLogMessage(String message, Severity sev, String tag) {
            // messages.add(message);
            sevs.add(sev);
            tags.add(tag);
            LOGGER.log(Level.INFO, tag+ ":" + message);
        }

    }
    private final MockLoggable mockLoggable = new MockLoggable();

    //private static final boolean ENABLE_H264_HIGH_PROFILE = false;
///////////////////////////////////////////////////////////////////////////////////

    public void pushFrame(int width, int height, byte[]  dataY, int strideY, byte[]  dataU, int strideU, byte[]  dataV, int strideV, byte[] serialize, int auglen)  {

    }
    public class MyVideoCapturer implements VideoCapturer {

        private static final int frameWidth = 1024;
        private static final int frameHeight = 720;


        public VideoFrame getNextFrame() {
            final long captureTimeNs = TimeUnit.MILLISECONDS.toNanos(SystemClock.elapsedRealtime());
            final JavaI420Buffer buffer = JavaI420Buffer.allocate(frameWidth, frameHeight);
            final ByteBuffer dataY = buffer.getDataY();
            final ByteBuffer dataU = buffer.getDataU();
            final ByteBuffer dataV = buffer.getDataV();
            final int chromaHeight = (frameHeight + 1) / 2;
            final int sizeY = frameHeight * buffer.getStrideY();
            final int sizeU = chromaHeight * buffer.getStrideU();
            final int sizeV = chromaHeight * buffer.getStrideV();


            String str = "Arvind";
            byte[] byteArr = str.getBytes();

            //  buffer.setAugData(byteArr);
            // buffer.setAugLen(str.length());
            return new VideoFrame(buffer, 0 /* rotation */, captureTimeNs);
        }

        public void PushVideoFrame(VideoFrame videoFrame, byte[] SerializedCameraData, int length)
        {
            capturerObserver.onFrameCapturedAug(videoFrame, length, SerializedCameraData);
        }

//            @Override
//            public void close() {
//
//            }

        private final static String TAG = "MyVideoCapturer";
        private CapturerObserver capturerObserver;
        private final Timer timer = new Timer();

        private final TimerTask tickTask = new TimerTask() {
            @Override
            public void run() {
                tick();
            }
        };


        public void tick() {
            VideoFrame videoFrame = getNextFrame();

            String str = "ArvindUmrao";
            byte[] byteArr = new  byte[256];

            PushVideoFrame(videoFrame, byteArr, 256 );

            //capturerObserver.onFrameCaptured(videoFrame);
            videoFrame.release();
        }

        @Override
        public void initialize(SurfaceTextureHelper surfaceTextureHelper, Context applicationContext,
                               CapturerObserver capturerObserver) {
            this.capturerObserver = capturerObserver;
        }

        @Override
        public void startCapture(int width, int height, int framerate) {
            timer.schedule(tickTask, 0, 1000 / framerate);
        }

        @Override
        public void stopCapture() throws InterruptedException {
            timer.cancel();
        }

        @Override
        public void changeCaptureFormat(int width, int height, int framerate) {
            // Empty on purpose
        }

        @Override
        public void dispose() {

        }

        @Override
        public boolean isScreencast() {
            return false;
        }

    };

    /////////////////////////////////////////////////////////////////////////////
    public void start() {
        // keep screen on

        Log.e( TAG, " start and create  multiplex factory");

        /// test HTTPSignalling


        // HTTPSignalling mysignal;
        // videoCapturerAndroid = new MyVideoCapturer();  // for random buffer testing
        // mysignal = new HTTPSignalling();  // for camera testing

        // mysignal.startCapture(1024, 720, 30);

        ///////////////////////////////////////////////////////////////


        //getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        //initViews();
        initVideos();
        getIceServers();

        SignallingClient.getInstance().init(this);



        org.webrtc.PeerConnectionFactory.initialize(
                org.webrtc.PeerConnectionFactory.InitializationOptions.builder(this.activity)
                        .setInjectableLogger(mockLoggable, Logging.Severity.LS_VERBOSE)
                        .setEnableInternalTracer(false)
                        .createInitializationOptions());

        // Initialize PeerConnectionFactory globals.
//        PeerConnectionFactory.InitializationOptions initializationOptions =
//                PeerConnectionFactory.InitializationOptions.builder(this)
//                        .createInitializationOptions();
//        PeerConnectionFactory.initialize(initializationOptions);

        //Create a new PeerConnectionFactory instance - using Hardware encoder and decoder.
        org.webrtc.PeerConnectionFactory.Options options = new org.webrtc.PeerConnectionFactory.Options();


        VideoEncoderFactory encoderFactory = new MultiplexVideoEncoderFactory(
                null, ENABLE_H264_HIGH_PROFILE);

        VideoDecoderFactory decoderFactory = new MultiplexVideoDecoderFactory(null);


        factory = org.webrtc.PeerConnectionFactory.builder()
                .setOptions(options)
                .setVideoEncoderFactory(encoderFactory)
                .setVideoDecoderFactory(decoderFactory)
                .createPeerConnectionFactory();

        //Now create a VideoCapturer instance.
        VideoCapturer videoCapturerAndroid;
       // videoCapturerAndroid = createCameraCapturer(new Camera1Enumerator(false));
        videoCapturerAndroid = new MyVideoCapturer();

        //Create MediaConstraints - Will be useful for specifying video and audio constraints.
        audioConstraints = new MediaConstraints();
        videoConstraints = new MediaConstraints();

        //Create a VideoSource instance
        if (videoCapturerAndroid != null) {
            surfaceTextureHelper = SurfaceTextureHelper.create("CaptureThread", null);
            videoSource = factory.createVideoSource(videoCapturerAndroid.isScreencast());
            videoCapturerAndroid.initialize(surfaceTextureHelper, this.activity, videoSource.getCapturerObserver());
        }
        localVideoTrack = factory.createVideoTrack("100", videoSource);

        //create an AudioSource instance
        audioSource = factory.createAudioSource(audioConstraints);
        localAudioTrack = factory.createAudioTrack("101", audioSource);

        localAudioTrack.setEnabled(true);
        if (videoCapturerAndroid != null) {
            videoCapturerAndroid.startCapture(1024, 720, 30);
        }

        //localVideoView.setVisibility(View.VISIBLE);
        // And finally, with our VideoRenderer ready, we
        // can add our renderer to the VideoTrack.
        //localVideoTrack.addSink(localVideoView);

        //localVideoView.setMirror(true);
        //remoteVideoView.setMirror(true);

        ProxyVideoSink localVideoSink = new ProxyVideoSink();
        localVideoTrack.addSink(localVideoSink);


        gotUserMedia = true;
        if (SignallingClient.getInstance().isInitiator) {
            onTryToStart();
        }
    }

    /**
     * This method will be called directly by the app when it is the initiator and has got the local media
     * or when the remote peer sends a message through socket that it is ready to transmit AV data
     */
    @Override
    public void onTryToStart() {
        Log.e( TAG, "onTryToStart");
        this.activity.runOnUiThread(() -> {
            if (!SignallingClient.getInstance().isStarted && localVideoTrack != null && SignallingClient.getInstance().isChannelReady) {
                createPeerConnection();
                SignallingClient.getInstance().isStarted = true;
                if (SignallingClient.getInstance().isInitiator) {
                    doCall();
                }
            }
        });
    }

    /**
     * Creating the local peerconnection instance
     */
    private void createPeerConnection() {

        Log.e(TAG, "createPeerConnection");

        PeerConnection.RTCConfiguration rtcConfig =
                new PeerConnection.RTCConfiguration(peerIceServers);
        // TCP candidates are only useful when connecting to a server that supports
        // ICE-TCP.
        rtcConfig.tcpCandidatePolicy = PeerConnection.TcpCandidatePolicy.DISABLED;
        rtcConfig.bundlePolicy = PeerConnection.BundlePolicy.MAXBUNDLE;
        rtcConfig.rtcpMuxPolicy = PeerConnection.RtcpMuxPolicy.REQUIRE;
        rtcConfig.continualGatheringPolicy = PeerConnection.ContinualGatheringPolicy.GATHER_CONTINUALLY;
        // Use ECDSA encryption.
        rtcConfig.keyType = PeerConnection.KeyType.ECDSA;
        localPeer = factory.createPeerConnection(rtcConfig, new CustomPeerConnectionObserver("localPeerCreation") {
            @Override
            public void onIceCandidate(IceCandidate iceCandidate) {
                super.onIceCandidate(iceCandidate);
                onIceCandidateReceived(iceCandidate);
            }

            @Override
            public void onAddStream(MediaStream mediaStream) {
                showToast("Received Remote stream");
                super.onAddStream(mediaStream);
                gotRemoteStream(mediaStream);
            }
        });

        addStreamToLocalPeer();
    }

    /**
     * Adding the stream to the localpeer
     */
    private void addStreamToLocalPeer() {
        //creating local mediastream
        MediaStream stream = factory.createLocalMediaStream("102");
        stream.addTrack(localAudioTrack);
        stream.addTrack(localVideoTrack);
        localPeer.addStream(stream);
    }

    /**
     * This method is called when the app is the initiator - We generate the offer and send it over through socket
     * to remote peer
     */
    private void doCall() {
        Log.e(TAG, "doCall");
        sdpConstraints = new MediaConstraints();
        sdpConstraints.mandatory.add(
                new MediaConstraints.KeyValuePair("OfferToReceiveAudio", "true"));
        sdpConstraints.mandatory.add(
                new MediaConstraints.KeyValuePair("OfferToReceiveVideo", "true"));

        Log.e(TAG, "createOffer");
        localPeer.createOffer(new CustomSdpObserver("localCreateOffer") {
            @Override
            public void onCreateSuccess(SessionDescription sessionDescription) {
                super.onCreateSuccess(sessionDescription);
                localPeer.setLocalDescription(new CustomSdpObserver("localSetLocalDesc"), sessionDescription);
                Log.d("onCreateSuccess", "SignallingClient emit ");
                SignallingClient.getInstance().emitMessage(sessionDescription);
            }
        }, sdpConstraints);
    }


    private static class ProxyVideoSink implements VideoSink {
        private VideoSink target;

        @Override
        synchronized public void onFrame(VideoFrame frame, int augLen, byte[] augData) {
            if (target == null) {
                Log.e("TAG",   " augLen=" + Integer.toString(augLen) +  " augData "+ new String(augData)   + " w=" + Integer.toString(frame.getBuffer().getWidth()) + " h="  + Integer.toString(frame.getBuffer().getHeight())        );
                return;
            }
            Log.e("TAG",   " augLen=" + Integer.toString(augLen) +  " augData "+ new String(augData)   + " w=" + Integer.toString(frame.getBuffer().getWidth()) + " h="  + Integer.toString(frame.getBuffer().getHeight())        );

            target.onFrame(frame , augLen, augData );
        }

        synchronized public void setTarget(VideoSink target) {
            this.target = target;
        }
    }

    /**
     * Received remote peer's media stream. we will get the first video track and render it
     */
    private void gotRemoteStream(MediaStream stream) {
        //we have remote video stream. add to the renderer.

        Log.e(TAG, "gotRemoteStream");

        final VideoTrack videoTrack = stream.videoTracks.get(0);
        this.activity.runOnUiThread(() -> {
            try {
                //remoteVideoView.setVisibility(View.VISIBLE);
                //videoTrack.addSink(remoteVideoView);

                ProxyVideoSink remoteVideoSink = new ProxyVideoSink();
                videoTrack.addSink(remoteVideoSink);
                remoteVideoSink.setTarget(null);


            } catch (Exception e) {
                e.printStackTrace();
            }
        });
    }

    /**
     * Received local ice candidate. Send it to remote peer through signalling for negotiation
     */
    public void onIceCandidateReceived(IceCandidate iceCandidate) {
        //we have received ice candidate. We can set it to the other peer.
        SignallingClient.getInstance().emitIceCandidate(iceCandidate);
    }

    /**
     * SignallingCallback - called when the room is created - i.e. you are the initiator
     */
    @Override
    public void onCreatedRoom() {
        showToast("You created the room " + gotUserMedia);
        if (gotUserMedia) {
            SignallingClient.getInstance().emitMessage("got user media");
        }
    }

    /**
     * SignallingCallback - called when you join the room - you are a participant
     */
    @Override
    public void onJoinedRoom() {
        showToast("You joined the room " + gotUserMedia);
        if (gotUserMedia) {
            SignallingClient.getInstance().emitMessage("got user media");
        }
    }

    @Override
    public void onNewPeerJoined() {
        Log.e(TAG, "onNewPeerJoined");
        showToast("Remote Peer Joined");
    }

    @Override
    public void onRemoteHangUp(String msg) {
        Log.e(TAG, "onRemoteHangUp");
        showToast("Remote Peer hungup");
        this.activity.runOnUiThread(this::hangup);
    }

    /**
     * SignallingCallback - Called when remote peer sends offer
     */
    @Override
    public void onOfferReceived(final JSONObject data) {
        showToast("Received Offer");
        Log.e("TAG", "onOfferReceived.");

        this.activity.runOnUiThread(() -> {
            if (!SignallingClient.getInstance().isInitiator && !SignallingClient.getInstance().isStarted) {
                onTryToStart();
            }

            try {
                localPeer.setRemoteDescription(new CustomSdpObserver("localSetRemote"), new SessionDescription(SessionDescription.Type.OFFER, data.getString("sdp")));
                Log.e("TAG", "setRemoteDescription.");
                doAnswer();
                // Log.e("TAG", "doAnswer");

                updateVideoViews(true);
            } catch (JSONException e) {
                e.printStackTrace();
            }
        });
    }

    private void doAnswer() {
        Log.e("TAG", "doAnswer.");

        localPeer.createAnswer(new CustomSdpObserver("localCreateAns") {
            @Override
            public void onCreateSuccess(SessionDescription sessionDescription) {
                super.onCreateSuccess(sessionDescription);
                Log.e("TAG", "setLocalDescription.");
                localPeer.setLocalDescription(new CustomSdpObserver("localSetLocal"), sessionDescription);
                SignallingClient.getInstance().emitMessage(sessionDescription);
            }
        }, new MediaConstraints());
    }

    /**
     * SignallingCallback - Called when remote peer sends answer to your offer
     */

    @Override
    public void onAnswerReceived(JSONObject data) {

        Log.e("TAG", "onAnswerReceived.");

        showToast("Received Answer");
        try {
            Log.e("TAG", "setRemoteDescription");
            localPeer.setRemoteDescription(new CustomSdpObserver("localSetRemote"), new SessionDescription(SessionDescription.Type.fromCanonicalForm(data.getString("type").toLowerCase()), data.getString("sdp")));
            updateVideoViews(true);
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    /**
     * Remote IceCandidate received
     */
    @Override
    public void onIceCandidateReceived(JSONObject data) {
        try {
            localPeer.addIceCandidate(new IceCandidate(data.getString("id"), data.getInt("label"), data.getString("candidate")));
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    private void updateVideoViews(final boolean remoteVisible) {
//        this.activity.runOnUiThread(() -> {
//            ViewGroup.LayoutParams params = localVideoView.getLayoutParams();
//            if (remoteVisible) {
//                params.height = dpToPx(100);
//                params.width = dpToPx(100);
//            } else {
//                params = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT);
//            }
//            localVideoView.setLayoutParams(params);
//        });
    }

    /**
     * Closing up - normal hangup and app destroye
     */


    private void hangup() {
        try {
            if (localPeer != null) {
                localPeer.close();
            }
            localPeer = null;
            SignallingClient.getInstance().close();
            updateVideoViews(false);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

//    @Override
//    protected void onDestroy() {
//        SignallingClient.getInstance().close();
//        super.onDestroy();
//
//        if (surfaceTextureHelper != null) {
//            surfaceTextureHelper.dispose();
//            surfaceTextureHelper = null;
//        }
//    }

    /**
     * Util Methods
     */
    public int dpToPx(int dp) {
        DisplayMetrics displayMetrics = this.activity.getResources().getDisplayMetrics();
        return Math.round(dp * (displayMetrics.xdpi / DisplayMetrics.DENSITY_DEFAULT));
    }

    public void showToast(final String msg) {
        //runOnUiThread(() -> Toast.makeText(MainActivity.this, msg, Toast.LENGTH_SHORT).show());
    }

    private VideoCapturer createCameraCapturer(CameraEnumerator enumerator) {
        final String[] deviceNames = enumerator.getDeviceNames();

        // First, try to find front facing camera
        Log.e(TAG, "Looking for front facing cameras.");
        for (String deviceName : deviceNames) {
            if (enumerator.isFrontFacing(deviceName)) {
                Log.e(TAG, "Creating front facing camera capturer.");
                VideoCapturer videoCapturer = enumerator.createCapturer(deviceName, null);

                if (videoCapturer != null) {
                    return videoCapturer;
                }
            }
        }

        // Front facing camera not found, try something else
        Log.e(TAG, "Looking for other cameras.");
        for (String deviceName : deviceNames) {
            if (!enumerator.isFrontFacing(deviceName)) {
                Log.e(TAG, "Creating other camera capturer.");
                VideoCapturer videoCapturer = enumerator.createCapturer(deviceName, null);

                if (videoCapturer != null) {
                    return videoCapturer;
                }
            }
        }

        return null;
    }




    public PeerConnectionFactory(Activity activity, Loggable logger)
    {
        this.activity = activity;

        start();

//        // Set libjingle (native C++) logging.
//        // NOTE: setInjectableLogger _must_ happen while |factory| is alive!
//        org.webrtc.PeerConnectionFactory.initialize(
//                org.webrtc.PeerConnectionFactory.InitializationOptions.builder(activity.getApplicationContext())
//                        .setInjectableLogger(logger, Logging.Severity.LS_WARNING)
//                        .setEnableInternalTracer(false)
//                        .createInitializationOptions());
//
//        org.webrtc.PeerConnectionFactory.Options options = new org.webrtc.PeerConnectionFactory.Options();
//
//        final AudioDeviceModule adm = createJavaAudioDevice();
//
//        // TODO: If you're in here doing video, make sure you use a hardware encoder!
//        //final VideoEncoderFactory encoderFactory = new SoftwareVideoEncoderFactory();
//        //final VideoDecoderFactory decoderFactory = new SoftwareVideoDecoderFactory();
//
//        VideoEncoderFactory encoderFactory = new MultiplexVideoEncoderFactory(
//                null, ENABLE_H264_HIGH_PROFILE);
//
//        VideoDecoderFactory decoderFactory = new MultiplexVideoDecoderFactory(null);
//
//
//        factory = org.webrtc.PeerConnectionFactory.builder()
//                .setOptions(options)
//                .setAudioDeviceModule(adm)
//                .setVideoEncoderFactory(encoderFactory)
//                .setVideoDecoderFactory(decoderFactory)
//                .createPeerConnectionFactory();
//        Log.e(TAG, "Peer connection factory created.");
//        adm.release();
    }

//    AudioDeviceModule createJavaAudioDevice()
//    {
//        // Set audio record error callbacks.
//        JavaAudioDeviceModule.AudioRecordErrorCallback audioRecordErrorCallback = new JavaAudioDeviceModule.AudioRecordErrorCallback()
//        {
//            @Override
//            public void onWebRtcAudioRecordInitError(String errorMessage)
//            {
//                Log.e(TAG, "onWebRtcAudioRecordInitError: " + errorMessage);
//                throw new RuntimeException(errorMessage);
//            }
//
//            @Override
//            public void onWebRtcAudioRecordStartError(JavaAudioDeviceModule.AudioRecordStartErrorCode errorCode, String errorMessage)
//            {
//                Log.e(TAG, "onWebRtcAudioRecordStartError: " + errorCode + ". " + errorMessage);
//                throw new RuntimeException(errorMessage);
//            }
//
//            @Override
//            public void onWebRtcAudioRecordError(String errorMessage)
//            {
//                Log.e(TAG, "onWebRtcAudioRecordError: " + errorMessage);
//                throw new RuntimeException(errorMessage);
//            }
//        };
//
//        JavaAudioDeviceModule.AudioTrackErrorCallback audioTrackErrorCallback = new JavaAudioDeviceModule.AudioTrackErrorCallback()
//        {
//            @Override
//            public void onWebRtcAudioTrackInitError(String errorMessage)
//            {
//                Log.e(TAG, "onWebRtcAudioTrackInitError: " + errorMessage);
//                throw new RuntimeException(errorMessage);
//            }
//
//            @Override
//            public void onWebRtcAudioTrackStartError(JavaAudioDeviceModule.AudioTrackStartErrorCode errorCode, String errorMessage)
//            {
//                Log.e(TAG, "onWebRtcAudioTrackStartError: " + errorCode + ". " + errorMessage);
//                throw new RuntimeException(errorMessage);
//            }
//
//            @Override
//            public void onWebRtcAudioTrackError(String errorMessage)
//            {
//                Log.e(TAG, "onWebRtcAudioTrackError: " + errorMessage);
//                throw new RuntimeException(errorMessage);
//            }
//        };
//
//        return JavaAudioDeviceModule.builder(activity.getApplicationContext())
//                .setUseHardwareAcousticEchoCanceler(true)
//                .setUseHardwareNoiseSuppressor(true)
//                .setAudioRecordErrorCallback(audioRecordErrorCallback)
//                .setAudioTrackErrorCallback(audioTrackErrorCallback)
//                .createAudioDeviceModule();
//    }
//
//    public void addIceServer(String uri, String username, String password)
//    {
//        PeerConnection.IceServer.Builder builder = PeerConnection.IceServer.builder(uri);
//
//        builder.setUsername(username);
//        builder.setPassword(password);
//
//        iceServers.add(builder.createIceServer());
//    }
//
//    public void clearIceServers()
//    {
//        iceServers.clear();
//    }
//
//    public void setProxy(String address, int port, String username, String password)
//    {
//        this.proxyType = PeerConnection.ProxyType.HTTPS;
//        this.proxyAddress = address;
//        this.proxyPort = port;
//        this.proxyUsername = username;
//        this.proxyPassword = password;
//    }
//
//    public void setPortRange(int minPort, int maxPort)
//    {
//        this.minPort = minPort;
//        this.maxPort = maxPort;
//    }
//
//    public org.webrtc.PeerConnection createPeerConnection(PeerConnection.Observer observer)
//    {
//        return factory.createPeerConnection(createConfig(), observer);
//    }

//    PeerConnection.RTCConfiguration createConfig()
//    {
//        PeerConnection.RTCConfiguration config = new PeerConnection.RTCConfiguration(iceServers);
//        config.sdpSemantics = PeerConnection.SdpSemantics.UNIFIED_PLAN;
//        // TCP candidates are only useful when connecting to a server that supports ICE-TCP.
//        config.tcpCandidatePolicy = PeerConnection.TcpCandidatePolicy.DISABLED;
//        config.bundlePolicy = PeerConnection.BundlePolicy.BALANCED;
//        config.rtcpMuxPolicy = PeerConnection.RtcpMuxPolicy.REQUIRE;
//        config.continualGatheringPolicy = PeerConnection.ContinualGatheringPolicy.GATHER_CONTINUALLY;
//
//       // rtcConfig.enableDtlsSrtp = !peerConnectionParameters.loopback;
//
//        // Use ECDSA encryption.
//        config.keyType = PeerConnection.KeyType.ECDSA;
//
//        // Enable DTLS for normal calls and disable for loopback calls.
//        config.enableDtlsSrtp = true;
//
//        config.iceServers = iceServers;
//
//        config.minPort = minPort;
//        config.maxPort = maxPort;
//
//        config.proxyType = proxyType;
//        config.proxyAddress = proxyAddress;
//        config.proxyPort = proxyPort;
//        config.proxyUsername = proxyUsername;
//        config.proxyPassword = proxyPassword;
//
//        return config;
//    }

//    public AudioSource createAudioSource()
//    {
//        MediaConstraints audioConstraints = new MediaConstraints();
//        AudioSource audioSource = factory.createAudioSource(audioConstraints);
//        return audioSource;
//    }
//
//    public AudioTrack createAudioTrack(AudioSource audioSource)
//    {
//        AudioTrack audioTrack = factory.createAudioTrack(AUDIO_TRACK_ID, audioSource);
//        return audioTrack;
//    }
//
//    public VideoSource createVideoSource(SurfaceTextureHelper surfaceTextureHelper, VideoCapturer videoCapturer, int videoWidth, int videoHeight, int videoFps)
//    {
//        VideoSource videoSource = factory.createVideoSource(videoCapturer.isScreencast());
//        videoCapturer.initialize(surfaceTextureHelper, activity.getApplicationContext(), videoSource.getCapturerObserver());
//        videoCapturer.startCapture(videoWidth, videoHeight, videoFps);
//        return videoSource;
//    }
//
//    public VideoTrack createVideoTrack(VideoSource videoSource)
//    {
//        VideoTrack localVideoTrack = factory.createVideoTrack(VIDEO_TRACK_ID, videoSource);
//        return localVideoTrack;
//    }
}
