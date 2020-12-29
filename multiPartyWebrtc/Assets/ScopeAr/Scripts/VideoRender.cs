using System;


namespace agora_gaming_rtc
{
    public abstract class IVideoRender 
    {
        /**
		 * choose the rendreMode of video.
		 * 1:  VIDEO_RENDER_MODE.RENDER_RAWDATA
         * this way can support any Unity Graphic API
         *
         * 2: VIDEO_RENDER_MODE.REDNER_OPENGL_ES2
         * this way only support openGLES2 and do not support multiTherad Rendering.
         *
         * 3: VIDEO_RENDER_MODE.RENDER_UNITY_LOW_LEVEL_INTERFACE
         * this way use Unity Low level native Interface to render video.
         *
		 * @return return effect volume
		 */
        public abstract int SetVideoRenderMode(bool _renderMode);

         // load data to texture
        public abstract int UpdateTexture(int tex, uint uid, IntPtr data, ref int width, ref int height);

        public abstract int UpdateVideoRawData(uint uid, IntPtr data, ref int width, ref int height);    
        /**
         * create Native texture and return textureId.
         */
        public abstract int GenerateNativeTexture();
        
        /**
         * Delete native texture according to the textureId.
         */
        public abstract void DeleteTexture(int tex);

        public abstract int AddUserVideoInfo(uint userId, uint textureId);

        public abstract int RemoveUserVideoInfo(uint _userId);
    }

    public sealed class VideoRender : IVideoRender
    {
        private static VideoRender _videoRenderInstance = null;
        

        private VideoRender()
        {
            
        }

        public static VideoRender GetInstance()
        {
            if (_videoRenderInstance == null)
            {
                _videoRenderInstance = new VideoRender();
            }
            return _videoRenderInstance;
        }

        public static void ReleaseInstance()
		{
			_videoRenderInstance = null;
		}

        public void SetEngine()
        {
           
        }

        public override int SetVideoRenderMode(bool _renderMode)
        {


            return 0;
        }

        public override int UpdateVideoRawData(uint uid, IntPtr data, ref int width, ref int height)
        {
            /*
            if (_rtcEngine == null)
                return (int)ERROR_CODE.ERROR_NOT_INIT_ENGINE;

            int rc = IRtcEngineNative.updateVideoRawData(data, uid);
            if (rc == -1)
                return -1;
           */

            int rc = 0;

            width = (int)rc >> 16;
            height = (int)(rc & 0xffff);
            return 0;
        }  

         // load data to texture
        public override int UpdateTexture(int tex, uint uid, IntPtr data, ref int width, ref int height)
        {
            /*
            if (_rtcEngine == null)
                return (int)ERROR_CODE.ERROR_NOT_INIT_ENGINE;

            int rc = IRtcEngineNative.updateTexture(tex, data, uid);

            if (rc == -1)
                return -1;
                */
            int rc = 0;
            width = (int)rc >> 16;
            height = (int)(rc & 0xffff);
            return 0;
        }

        public override int AddUserVideoInfo(uint userId, uint textureId)
        {
            return 0;
        }

        public override int RemoveUserVideoInfo(uint _userId)
        {
            return 0;
        }

        public override int GenerateNativeTexture()
        {
            return 0;
        }

        public override void DeleteTexture(int tex)
        {
           
        }
    }
}