/*
 *  Copyright 2014 The WebRTC Project Authors. All rights reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. An additional intellectual property rights grant can be found
 *  in the file PATENTS.  All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */

package com.scopear.webrtc;

import android.content.Context;
import android.os.SystemClock;
import android.util.Log;
import android.view.View;

import androidx.annotation.Nullable;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Iterator;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.TimeUnit;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import org.webrtc.AudioSource;
import org.webrtc.AudioTrack;
import org.webrtc.CalledByNative;
import org.webrtc.CameraVideoCapturer;
import org.webrtc.CapturerObserver;
import org.webrtc.DataChannel;
import org.webrtc.IceCandidate;
import org.webrtc.JavaI420Buffer;
import org.webrtc.MediaConstraints;
import org.webrtc.MediaStream;
import org.webrtc.MediaStreamTrack;
import org.webrtc.PeerConnection;
import org.webrtc.PeerConnection.IceConnectionState;
import org.webrtc.RtpParameters;
import org.webrtc.RtpReceiver;
import org.webrtc.RtpSender;
import org.webrtc.RtpTransceiver;
import org.webrtc.SdpObserver;
import org.webrtc.SessionDescription;
import org.webrtc.StatsObserver;
import org.webrtc.StatsReport;
import org.webrtc.SurfaceTextureHelper;
import org.webrtc.VideoCapturer;
import org.webrtc.VideoFrame;
import org.webrtc.VideoSink;
import org.webrtc.VideoSource;
import org.webrtc.VideoTrack;

import org.webrtc.Camera1Enumerator;
import org.webrtc.CameraEnumerator;
import org.webrtc.Logging;


public class PeerConnectionClient
{
    private static final String TAG = "PeerConnectionClient";

    private static final int HD_VIDEO_WIDTH = 1280;
    private static final int HD_VIDEO_HEIGHT = 720;
    private static final int BPS_IN_KBPS = 1000;

    private final PCObserver pcObserver = new PCObserver();
    private final SDPObserver sdpObserver = new SDPObserver();

    private final com.scopear.webrtc.PeerConnectionFactory factory;

    private PeerConnectionEvents events;

    private PeerConnection peerConnection;

    private boolean isError;

    private MediaConstraints sdpMediaConstraints;
    @Nullable private SessionDescription localSdp; // either offer or answer SDP

    @Nullable private SurfaceTextureHelper surfaceTextureHelper;
    @Nullable private VideoSource videoSource;
    private boolean videoCapturerStopped = true;
    @Nullable private VideoCapturer videoCapturer;
   // @Nullable private VideoSink localRender;
   // @Nullable private List<VideoSink> remoteSinks;
    private boolean videoCallEnabled;
    private int videoWidth;
    private int videoHeight;
    private int videoFps;
    // renderVideo is set to true if video should be rendered and sent.
    private boolean renderVideo = true;
    @Nullable private VideoTrack localVideoTrack;
    //@Nullable private VideoTrack remoteVideoTrack;
    @Nullable private RtpSender localVideoSender;

    @Nullable private AudioSource audioSource;
    // enableAudio is set to true if audio should be sent.
    private boolean enableAudio = true;
    @Nullable private AudioTrack localAudioTrack;
    @Nullable private AudioTrack remoteAudioTrack;

    @Nullable private RecordAudioController audioRecorder;

    private List<PeerConnection.IceServer> iceServers = new ArrayList<PeerConnection.IceServer>();

    /**
     * Peer connection events.
     */
    public interface PeerConnectionEvents
    {
        /**
         * Callback fired once local SDP is created and set.
         */
        void onLocalDescription(final String type, final String sdp);

        /**
         * Callback fired once local Ice candidate is generated.
         */
        void onIceCandidate(final String sdp, final int sdpMlineindex, final String sdpMid);

        /**
         * Callback fired once local ICE candidates are removed.
         */
        // void onIceCandidatesRemoved(final IceCandidate[] candidates);

        /**
         * Callback fired when ICE state changes
         */
        void onIceConnectionChange(final int newState);

        /**
         * Callback fired once peer connection is closed.
         */
        void onPeerConnectionClosed();

        /**
         * Callback fired once peer connection statistics is ready.
         */
        //void onPeerConnectionStatsReady(final StatsReport[] reports);

        /**
         * Callback fired if peer connection requires renegotiation
         */
        void onRenegotiationNeeded();

        /**
         * Callback fired once peer connection error happened.
         */
        void onPeerConnectionError(final String description);
    }

    public PeerConnectionClient(com.scopear.webrtc.PeerConnectionFactory factory, PeerConnectionEvents events)
    {

        Log.e(TAG, "PeerConnectionClient");
        this.factory = factory;
        this.events = events;

        videoCallEnabled = true;
        videoWidth = 0;
        videoHeight = 0;
        videoFps = 0;
      //  localRender = null;
      //  remoteSinks = null;
        videoCapturer = null;

        //videoCapturer = createCameraCapturer(new Camera1Enumerator(false));  // for camera testing

        videoCapturer = new MyVideoCapturer();

        isError = false;

        try
        {
            createMediaConstraints();
            createPeerConnection();
        }
        catch (Exception e)
        {
            reportError("Failed to create peer connection: " + e.getMessage());
            throw e;
        }
    }

    private VideoCapturer createCameraCapturer(CameraEnumerator enumerator) {
        final String[] deviceNames = enumerator.getDeviceNames();

        // First, try to find front facing camera
        Logging.e(TAG, "Looking for front facing cameras.");
        for (String deviceName : deviceNames) {
            if (enumerator.isFrontFacing(deviceName)) {
                Logging.d(TAG, "Creating front facing camera capturer.");
                VideoCapturer videoCapturer = enumerator.createCapturer(deviceName, null);

                if (videoCapturer != null) {
                    return videoCapturer;
                }
            }
        }

        // Front facing camera not found, try something else
        Logging.e(TAG, "Looking for other cameras.");
        for (String deviceName : deviceNames) {
            if (!enumerator.isFrontFacing(deviceName)) {
                Logging.d(TAG, "Creating other camera capturer.");
                VideoCapturer videoCapturer = enumerator.createCapturer(deviceName, null);

                if (videoCapturer != null) {
                    return videoCapturer;
                }
            }
        }

        return null;
    }
    private boolean isVideoCallEnabled()
    {
        return videoCallEnabled && videoCapturer != null;
    }

    private void createMediaConstraints()
    {
        // Create video constraints if video call is enabled.
        if (isVideoCallEnabled())
        {
            // If video resolution is not specified, default to HD.
            if (videoWidth == 0 || videoHeight == 0)
            {
                videoWidth = HD_VIDEO_WIDTH;
                videoHeight = HD_VIDEO_HEIGHT;
            }

            // If fps is not specified, default to 30.
            if (videoFps == 0)
            {
                videoFps = 30;
            }
            Log.d(TAG, "Capturing format: " + videoWidth + "x" + videoHeight + "@" + videoFps);
        }

        // Create SDP constraints.
        sdpMediaConstraints = new MediaConstraints();
        sdpMediaConstraints.mandatory.add(new MediaConstraints.KeyValuePair("OfferToReceiveAudio", "true"));
        sdpMediaConstraints.mandatory.add(new MediaConstraints.KeyValuePair("OfferToReceiveVideo", Boolean.toString(isVideoCallEnabled())));
    }

    private void createPeerConnection()
    {


    }

    public void disconnect()
    {
        Log.e(TAG, "Closing peer connection.");
        if (peerConnection != null)
        {
            peerConnection.dispose();
            peerConnection = null;
        }
        Log.d(TAG, "Closing audio source.");
        if (audioSource != null)
        {
            audioSource.dispose();
            audioSource = null;
        }
        localAudioTrack = null;
        remoteAudioTrack = null;

        Log.d(TAG, "Stopping capture.");
        if (videoCapturer != null)
        {
            try
            {
                videoCapturer.stopCapture();
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException(e);
            }
            videoCapturerStopped = true;
            videoCapturer.dispose();
            videoCapturer = null;
        }
        Log.d(TAG, "Closing video source.");
        if (videoSource != null)
        {
            videoSource.dispose();
            videoSource = null;
        }
        if (surfaceTextureHelper != null)
        {
            surfaceTextureHelper.dispose();
            surfaceTextureHelper = null;
        }
        if (audioRecorder != null)
        {
            Log.d(TAG, "Closing audio file for recorded input audio.");
            audioRecorder.dispose();
            audioRecorder = null;
        }
        localVideoSender = null;
        localVideoTrack = null;
      //  remoteVideoTrack = null;
        //localRender = null;
        //remoteSinks = null;

        localSdp = null;

        Log.d(TAG, "Closing peer connection done.");
        events.onPeerConnectionClosed();
    }

    public void startRecording(String localAudioFilePath, String remoteAudioFilePath)
    {
        try
        {
            audioRecorder = new RecordAudioController(localAudioTrack, localAudioFilePath, remoteAudioTrack, remoteAudioFilePath);
        }
        catch (IOException e)
        {
            Log.e(TAG, "onWebRTCStartAudioRecordingError: " + e.getMessage());
            reportError(e.getMessage());
        }
    }

    public void stopRecording()
    {
        if(audioRecorder != null)
        {
            audioRecorder.dispose();
            audioRecorder = null;
        }
    }

    public long getRecordingSize()
    {
        if(audioRecorder != null)
        {
            return audioRecorder.getRecordingSize();
        }
        else
        {
            return 0;
        }
    }

    @SuppressWarnings("deprecation") // TODO(sakal): getStats is deprecated.
    private void getStats()
    {
        if (peerConnection == null || isError)
        {
            return;
        }
        boolean success = peerConnection.getStats(new StatsObserver()
        {
            @Override
            public void onComplete(final StatsReport[] reports)
            {
                //events.onPeerConnectionStatsReady(reports);
            }
        }, null);
        if (!success)
        {
            Log.e(TAG, "getStats() returns false!");
        }
    }

    public void setAudioEnabled(final boolean enable)
    {
        enableAudio = enable;
        if (localAudioTrack != null)
        {
            localAudioTrack.setEnabled(enableAudio);
        }
    }

    public void setVideoEnabled(final boolean enable)
    {
        renderVideo = enable;
        if (localVideoTrack != null)
        {
            localVideoTrack.setEnabled(renderVideo);
        }
        //if (remoteVideoTrack != null)
        //{
       //     remoteVideoTrack.setEnabled(renderVideo);
       // }
    }

    public void createOffer()
    {
        if (peerConnection != null && !isError)
        {
            Log.e(TAG, "PC Create OFFER");
            peerConnection.createOffer(sdpObserver, sdpMediaConstraints);
        }
    }

    public void createAnswer()
    {
        if (peerConnection != null && !isError)
        {
            Log.e(TAG, "PC create ANSWER");
            peerConnection.createAnswer(sdpObserver, sdpMediaConstraints);
        }
    }

    public void addRemoteIceCandidate(String candidatestr, int sdpMlineindex, String sdpMid)
    {
        Log.e(TAG, "addRemoteIceCandidate");

        IceCandidate candidate = new IceCandidate(sdpMid, sdpMlineindex, candidatestr);
        peerConnection.addIceCandidate(candidate);
    }

    public void removeRemoteIceCandidates(final IceCandidate[] candidates)
    {
        Log.e(TAG, "removeRemoteIceCandidates");

        if (peerConnection == null || isError)
        {
            return;
        }

        peerConnection.removeIceCandidates(candidates);
    }

    public void setRemoteDescription(String type, String desc)
    {

        Log.e(TAG, "Set remote " + "type" + type + " SDP." +  desc);
        
      	SessionDescription sdpRemote = new SessionDescription(SessionDescription.Type.fromCanonicalForm(type), desc);
        if (peerConnection == null || isError)
        {
            return;
        }
        Log.d(TAG, "Set remote SDP.");
        peerConnection.setRemoteDescription(sdpObserver, sdpRemote);
    }

    private static class ProxyVideoSink implements VideoSink {
        private VideoSink target;
        @Override
        synchronized public void onFrame(VideoFrame frame,  int augLen, byte[] augData) {
            if (target == null) {
               // Logging.d("onFrame", "Dropping frame in proxy because target is null.");
                //Log.e("TAG",   " Got frame  augLen=" + Integer.toString(augLen) +  " augData "+ new String(augData)   + " w=" + Integer.toString(frame.getBuffer().getWidth()) + " h="  + Integer.toString(frame.getBuffer().getHeight())        );


                System.arraycopy( augData,  0, augData, 0, 256);
                return;
            }
            Log.e("TAG",   " augLen=" + Integer.toString(augLen) +  " augData "+ new String(augData)   + " w=" + Integer.toString(frame.getBuffer().getWidth()) + " h="  + Integer.toString(frame.getBuffer().getHeight())        );


           // target.onFrame(frame , augLen, augData );
        }

        synchronized public void setTarget(VideoSink target) {
            this.target = target;
        }
    }
    public class MyVideoCapturer implements VideoCapturer {

        private static final int frameWidth = HD_VIDEO_WIDTH;
        private static final int frameHeight = HD_VIDEO_HEIGHT;

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

        public void PushVideoFrame(VideoFrame videoFrame, byte[] augData, int augLen)
        {
            capturerObserver.onFrameCapturedAug(videoFrame, augLen, augData);

           Log.e( TAG,"PushVideoFrame" );

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
            byte[] byteArr = new byte[256];

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


    public void muteMicrophone(final boolean muteEnabled)
    {
        localAudioTrack.setEnabled(!muteEnabled);
    }

    public void stopVideoSource()
    {
        if (videoCapturer != null && !videoCapturerStopped)
        {
            Log.d(TAG, "Stop video source.");
            try
            {
                videoCapturer.stopCapture();
            }
            catch (InterruptedException e)
            {
            }
            videoCapturerStopped = true;
        }
    }

    public void startVideoSource()
    {
        Log.e(TAG, "startVideoSource");
        if (videoCapturer != null && videoCapturerStopped)
        {
            Log.e(TAG, "Restart video source.");
            videoCapturer.startCapture(videoWidth, videoHeight, videoFps);
            videoCapturerStopped = false;
        }
    }

    public void setVideoMaxBitrate(@Nullable final Integer maxBitrateKbps)
    {
        if (peerConnection == null || localVideoSender == null || isError)
        {
            return;
        }
        Log.d(TAG, "Requested max video bitrate: " + maxBitrateKbps);
        if (localVideoSender == null)
        {
            Log.w(TAG, "Sender is not ready.");
            return;
        }

        RtpParameters parameters = localVideoSender.getParameters();
        if (parameters.encodings.size() == 0)
        {
            Log.w(TAG, "RtpParameters are not ready.");
            return;
        }

        for (RtpParameters.Encoding encoding : parameters.encodings)
        {
            // Null value means no limit.
            encoding.maxBitrateBps = maxBitrateKbps == null ? null : maxBitrateKbps * BPS_IN_KBPS;
        }
        if (!localVideoSender.setParameters(parameters))
        {
            Log.e(TAG, "RtpSender.setParameters failed.");
        }
        Log.d(TAG, "Configured max video bitrate to: " + maxBitrateKbps);
    }

    private void reportError(final String errorMessage)
    {
        Log.e(TAG, "Peerconnection error: " + errorMessage);
        if (!isError)
        {
            events.onPeerConnectionError(errorMessage);
            isError = true;
        }
    }

    @Nullable
    private AudioTrack createAudioTrack()
    {
        Log.e( TAG, "createAudioTrack" );

        return localAudioTrack;
    }

    @Nullable
    private VideoTrack createVideoTrack(VideoCapturer capturer)
    {
        Log.e(TAG, "createVideoTrack");



        return localVideoTrack;
    }

    private void findVideoSender()
    {
        for (RtpSender sender : peerConnection.getSenders())
        {
            if (sender.track() != null)
            {
                String trackType = sender.track().kind();
                if (trackType == MediaStreamTrack.VIDEO_TRACK_KIND)
                {
                    Log.d(TAG, "Found video sender.");
                    localVideoSender = sender;
                }
            }
        }
    }

    // Implementation detail: observe ICE & stream changes and react accordingly.
    private class PCObserver implements PeerConnection.Observer
    {
        @Override
        public void onIceCandidate(final IceCandidate candidate)
        {
            events.onIceCandidate(candidate.sdp, candidate.sdpMLineIndex, candidate.sdpMid);
        }

        @Override
        public void onIceCandidatesRemoved(final IceCandidate[] candidates)
        {
//            events.onIceCandidatesRemoved(candidates);
        }

        @Override
        public void onSignalingChange(PeerConnection.SignalingState newState)
        {
            Log.d(TAG, "SignalingState: " + newState);
        }

        @Override
        public void onIceConnectionChange(final PeerConnection.IceConnectionState newState)
        {
            Log.d(TAG, "IceConnectionState: " + newState);
            events.onIceConnectionChange(newState.ordinal());
        }

        @Override
        public void onIceGatheringChange(PeerConnection.IceGatheringState newState)
        {
            Log.d(TAG, "IceGatheringState: " + newState);
        }

        @Override
        public void onIceConnectionReceivingChange(boolean receiving)
        {
            Log.d(TAG, "IceConnectionReceiving changed to " + receiving);
        }

        @Override
        public void onAddStream(final MediaStream stream) {}

        @Override
        public void onRemoveStream(final MediaStream stream) {}

        @Override
        public void onDataChannel(final DataChannel dc) {}

        @Override
        public void onRenegotiationNeeded()
        {
            events.onRenegotiationNeeded();
        }

        @Override
        public void onAddTrack(final RtpReceiver receiver, final MediaStream[] mediaStreams) {}

        @Override
        public void onTrack(RtpTransceiver transceiver)
        {
            MediaStreamTrack track = transceiver.getReceiver().track();
            if (track.kind() == MediaStreamTrack.AUDIO_TRACK_KIND)
            {
                remoteAudioTrack = (AudioTrack)track;
            }
            else if (track.kind() == MediaStreamTrack.VIDEO_TRACK_KIND)
            {
                VideoTrack remoteVideoTrack = (VideoTrack)track;
                remoteVideoTrack.setEnabled(renderVideo);
                /*for (VideoSink remoteSink : remoteSinks)
                {
                    remoteVideoTrack.addSink(remoteSink);
                }*/
                  ProxyVideoSink remoteVideoSink = new ProxyVideoSink();
                    remoteVideoTrack.addSink(remoteVideoSink);
                    remoteVideoSink.setTarget(null);


            }
        }
    }

    // Implementation detail: handle offer creation/signaling and answer setting,
    // as well as adding remote ICE candidates once the answer SDP is set.
    private class SDPObserver implements SdpObserver
    {
        @Override
        public void onCreateSuccess(final SessionDescription origSdp)
        {
            if (localSdp != null)
            {
                reportError("Multiple SDP create.");
                return;
            }
            localSdp = origSdp;
            if (peerConnection != null && !isError)
            {
                Log.e(TAG, "Set local SDP from " + origSdp.type + " des " + localSdp.description);
                peerConnection.setLocalDescription(sdpObserver, origSdp);
                events.onLocalDescription(localSdp.type.toString(), localSdp.description);
            }
        }

        @Override
        public void onSetSuccess() {

            Log.e(TAG, "onSetSuccess");

        }

        @Override
        public void onCreateFailure(final String error)
        {
            reportError("createSDP error: " + error);
        }

        @Override
        public void onSetFailure(final String error)
        {
            reportError("setSDP error: " + error);
        }
    }
}
