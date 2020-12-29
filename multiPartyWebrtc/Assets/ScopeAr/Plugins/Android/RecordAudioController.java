package com.scopear.webrtc;

import java.io.IOException;
import android.util.Log;
import org.webrtc.AudioTrack;

public class RecordAudioController
{
    private static final String TAG = "RecordAudioController";

    private AudioTrack localTrack, remoteTrack;
    private RecordAudioSink localSink, remoteSink;

    public RecordAudioController(AudioTrack localTrack, String localFile, AudioTrack remoteTrack, String remoteFile) throws IOException
    {
        this.localTrack = localTrack;
        this.localSink = new RecordAudioSink(localFile);
        this.localTrack.addSink(localSink);

        this.remoteTrack = remoteTrack;
        this.remoteSink = new RecordAudioSink(remoteFile);
        this.remoteTrack.addSink(remoteSink);
    }

    public void dispose()
    {
        localTrack.removeSink(localSink);
        remoteTrack.removeSink(remoteSink);

        localSink.dispose();
        remoteSink.dispose();
    }

    public long getRecordingSize()
    {
        return localSink.getTrackSize() + remoteSink.getTrackSize();
    }
}
