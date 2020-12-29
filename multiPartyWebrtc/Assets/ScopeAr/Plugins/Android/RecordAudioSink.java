package com.scopear.webrtc;

import android.media.MediaCodec;
import android.media.MediaMuxer;
import android.util.Log;
import android.media.MediaCodecInfo;
import android.media.MediaFormat;
import android.os.Handler;
import android.os.HandlerThread;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingDeque;

import org.webrtc.AudioSink;

public class RecordAudioSink extends MediaCodec.Callback implements AudioSink
{
    private static final String TAG = "RecordedAudioSink";

    private MediaMuxer mediaMuxer;
    private boolean muxerStarted = false;

    private BlockingQueue<Integer> freeInputBuffers = new LinkedBlockingDeque<Integer>();

    private final HandlerThread audioThread;
    private final Handler audioThreadHandler;

    private MediaCodec audioEncoder;
    private long presTime = 0L;

    private int trackNum;

    private long size = 0;

    public RecordAudioSink(String outputFilePath) throws IOException
    {
        mediaMuxer = new MediaMuxer(outputFilePath, MediaMuxer.OutputFormat.MUXER_OUTPUT_MPEG_4);
        audioThread = new HandlerThread(TAG);
        audioThread.start();
        audioThreadHandler = new Handler(audioThread.getLooper());
    }

    public void dispose()
    {
        if (audioThreadHandler != null)
        {
            audioThreadHandler.post(() -> {
                if (audioEncoder != null)
                {
                    audioEncoder.stop();
                    audioEncoder.release();
                }

                if(muxerStarted)
                {
                    synchronized (mediaMuxer)
                    {
                        mediaMuxer.stop();
                        mediaMuxer.release();
                        muxerStarted = false;
                    }
                }
                presTime = 0L;

                audioThread.quit();
            });
        }
    }

    public long getTrackSize()
    {
        return size;
    }

    // AudioSink implementation
    @Override
    public void onData(byte[] audioData, int bitsPerSample, int sampleRate, int numberOfChannels, int numberOfFrames)
    {
        if (audioEncoder == null) try
        {
            audioEncoder = MediaCodec.createEncoderByType("audio/mp4a-latm");
            MediaFormat format = new MediaFormat();
            format.setString(MediaFormat.KEY_MIME, "audio/mp4a-latm");
            format.setInteger(MediaFormat.KEY_CHANNEL_COUNT, numberOfChannels);
            format.setInteger(MediaFormat.KEY_SAMPLE_RATE, sampleRate);
            format.setInteger(MediaFormat.KEY_BIT_RATE, 64 * 1024);
            format.setInteger(MediaFormat.KEY_AAC_PROFILE, MediaCodecInfo.CodecProfileLevel.AACObjectLC);
            audioEncoder.configure(format, null, null, MediaCodec.CONFIGURE_FLAG_ENCODE);
            audioEncoder.setCallback(this);
            audioEncoder.start();
        }
        catch (IOException exception)
        {
            Log.wtf(TAG, exception);
        }

        audioThreadHandler.post(()->{
            try
            {
                Integer index = freeInputBuffers.take();
                ByteBuffer buffer = audioEncoder.getInputBuffer(index);

                int dataByteSize = bitsPerSample / 8 * numberOfChannels * numberOfFrames;

                buffer.clear();
                buffer.put(audioData, 0, dataByteSize);
                audioEncoder.queueInputBuffer(index, 0, dataByteSize, presTime, 0);

                // 1000000us in 1s. Divide by 2 at the end because audioData a byte[] and not a short[]
                long bufferDurationUs = 1000000 * (dataByteSize/numberOfChannels) / sampleRate / 2;

                presTime += bufferDurationUs;
            }
            catch (InterruptedException e)
            {
                Log.wtf(TAG, e);
            }
        });
    }

    // MediaCodec Callbacks
    public void onError(MediaCodec codec, MediaCodec.CodecException exception)
    {
        Log.e(TAG, "Encoder error: " + exception);
    }

    public void onOutputFormatChanged(MediaCodec codec, MediaFormat format)
    {
        Log.w(TAG, "encoder output format changed: " + format);
        synchronized (mediaMuxer)
        {
            trackNum = mediaMuxer.addTrack(format);
            mediaMuxer.start();
            muxerStarted = true;
        }
    }

    @Override
    public void onInputBufferAvailable(MediaCodec codec, int index)
    {
        freeInputBuffers.add(index);
    }

    @Override
    public void onOutputBufferAvailable(MediaCodec codec, int index, MediaCodec.BufferInfo info)
    {
        try
        {
            ByteBuffer encodedData = codec.getOutputBuffer(index);
            if (encodedData == null)
            {
                Log.e(TAG, "encoderOutputBuffer " + index + " was null");
            }
            else
            {
                // It's usually necessary to adjust the ByteBuffer values to match BufferInfo.
                encodedData.position(info.offset);
                encodedData.limit(info.offset + info.size);

                if (muxerStarted)
                {
                    synchronized (mediaMuxer)
                    {
                        mediaMuxer.writeSampleData(trackNum, encodedData, info);
                        size += info.size;
                    }
                }
            }
            codec.releaseOutputBuffer(index, false);
        }
        catch (IllegalStateException e)
        {
            // NF: VLC dealt with this the same way
            // https://mailman.videolan.org/pipermail/vlc-commits/2013-January/018884.html
            Log.e(TAG, "mediaEncoder in bad state when accessing output buffers.  This happens on some devices.");
        }
    }
}
