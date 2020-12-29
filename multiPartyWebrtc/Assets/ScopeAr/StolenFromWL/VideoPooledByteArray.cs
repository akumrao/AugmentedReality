using System;

namespace Scope.RemoteAR
{
    public class VideoPooledByteArray : PooledByteArray
    {
        // 3.68 mb - The Biggest thing that our AR platforms will return (visionlib, in this case)
        private const int BASE_IMAGE_PIXELBUFFER_SIZE = 3686400;

        // I have no idea if this value can be lower.  My dumb reasoning:
        // we send video at 15 FPS, so maybe we'll have that many frames kicking around
        // I want some wiggle room, so add 10 to that.
        // Can use a tool like PerfView to get a better idea.
        // refer to this blog post: https://adamsitnik.com/Array-Pool/
        private const int MAX_ACTIVE_FRAMES = 25;

       // private static ArrayPool<byte> videoBytePool = ArrayPool<byte>.Create(BASE_IMAGE_PIXELBUFFER_SIZE, MAX_ACTIVE_FRAMES);

       
    }
}