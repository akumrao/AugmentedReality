using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using SimplePeerConnectionM;
using System.Text;

public class CameraScript : MonoBehaviour
{

    public PeerConnectionM peer = null;

    public bool broswer = false;

    static WebCamTexture backCam;

    private GCHandle pixelsHandle;

    public struct RGB
    {
        private byte _r;
        private byte _g;
        private byte _b;

        public RGB(byte r, byte g, byte b)
        {
            this._r = r;
            this._g = g;
            this._b = b;
        }

        public byte R
        {
            get { return this._r; }
            set { this._r = value; }
        }

        public byte G
        {
            get { return this._g; }
            set { this._g = value; }
        }

        public byte B
        {
            get { return this._b; }
            set { this._b = value; }
        }

        public bool Equals(RGB rgb)
        {
            return (this.R == rgb.R) && (this.G == rgb.G) && (this.B == rgb.B);
        }
    }

    public struct YUV
    {
        private double _y;
        private double _u;
        private double _v;

        public YUV(double y, double u, double v)
        {
            this._y = y;
            this._u = u;
            this._v = v;
        }

        public double Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public double U
        {
            get { return this._u; }
            set { this._u = value; }
        }

        public double V
        {
            get { return this._v; }
            set { this._v = value; }
        }

        public bool Equals(YUV yuv)
        {
            return (this.Y == yuv.Y) && (this.U == yuv.U) && (this.V == yuv.V);
        }
    }

    public static YUV RGBToYUV(RGB rgb)
    {
        double y = rgb.R * .299000 + rgb.G * .587000 + rgb.B * .114000;
        double u = rgb.R * -.168736 + rgb.G * -.331264 + rgb.B * .500000 + 128;
        double v = rgb.R * .500000 + rgb.G * -.418688 + rgb.B * -.081312 + 128;

        return new YUV(y, u, v);
    }

    uint count = 0;

    void Start()
    {
        if (backCam == null)
            backCam = new WebCamTexture();

        GetComponent<Renderer>().material.mainTexture = backCam;

      //  pixelsHandle = GCHandle.Alloc(rgb, GCHandleType.Pinned);

        if (!backCam.isPlaying)
            backCam.Play();

    }

    void Update()
    {
       // count++;


        if (peer != null)
            StartCoroutine(DoPushFrame( ));

    
    }

    public static YUV RGBToYUV(Color32 rgb)
    {
        double y = rgb.r * .299000 + rgb.g * .587000 + rgb.b * .114000;
        double u = rgb.r * -.168736 + rgb.g * -.331264 + rgb.b * .500000 + 128;
        double v = rgb.r * .500000 + rgb.g * -.418688 + rgb.b * -.081312 + 128;

        return new YUV(y, u, v);
    }


    void CopyBufferToYou(Color32[] rgb, int w, int h)
    {


        unsafe
        {/*
            byte* ptrY = (byte*)dataY.ToPointer();

            int r1 = ptrY[0];
            int g1 = ptrY[1];
            int b1 = ptrY[2];
            int a1 = ptrY[3];
            a = 9;

       */

            int strideY = w;
            int strideU = (w + 1) / 2;
            int strideV = strideU;

            byte[] bufferY = new byte[w*h];
            byte[] bufferU = new byte[(w * h)/4];
            byte[] bufferV = new byte[(w * h)/4];

            for (int i = 0; i < h/2; i++)
            {
                for (int j = 0; j < w/2; j++)
                {
                    bufferY[i*2*strideY + j*2] = (byte)RGBToYUV(rgb[i*2*strideY + j*2]).Y;
                    bufferY[i*2*strideY + j*2 + 1] = (byte)RGBToYUV(rgb[i*2*strideY + j*2 + 1]).Y;
                    bufferY[(2*i+1)*strideY + j*2 ]  = (byte)RGBToYUV(rgb[(2*i+1)*strideY + j*2]).Y;
                    bufferY[(2*i+1)*strideY + j*2 + 1] = (byte)RGBToYUV(rgb[(2*i+1)*strideY + j*2 + 1]).Y;

                    bufferU[i*strideU + j] = (byte)RGBToYUV(rgb[i* 2*strideY + j*2]).U;
                    bufferV[i*strideV + j] = (byte)RGBToYUV(rgb[i* 2*strideY + j*2]).V;
                }
            }


            string str = "Camera Serial data";

            //reading all characters as byte and storing them to byte[]
            byte[] bufferA = Encoding.ASCII.GetBytes(str);

            fixed (byte* pbufferY = bufferY, pbufferU = bufferU, pbufferV = bufferV, pbufferA = bufferA)
            {
               IntPtr dataY = (IntPtr)pbufferY;
               IntPtr dataU = (IntPtr)pbufferU;
               IntPtr dataV = (IntPtr)pbufferV;
               IntPtr dataA = (IntPtr)pbufferA;
                // do you stuff here
                if(broswer)
                 peer.I420_PushFrame(dataY, dataU, dataV, dataA, strideY, strideU, strideV, 0, (uint)w, (uint)h);
                else
                  peer.I420_PushFrame(dataY, dataU, dataV, dataA, strideY, strideU, strideV, str.Length, (uint)w, (uint)h);
            }
        }
    }

    IEnumerator DoPushFrame( )
    {
    
        int h = backCam.height;
        int w = backCam.width;

        Type type = backCam.GetType();

        Color32[] rgb = backCam.GetPixels32();

        CopyBufferToYou(rgb, w, h);

        // handle.Free();
        yield return null;
    }

}