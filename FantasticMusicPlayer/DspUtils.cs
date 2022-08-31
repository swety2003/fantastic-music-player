using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using NAudio.Dsp;
using System.IO;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace FantasticMusicPlayer
{
    public class DspSwitcher : DSPClass
    {
        private int sampleRate=44100, channels=2, bitdepth=4;

        private DSPClass wrappedDSP = null;
        public override void init(int sampleRate, int channels, int bitdepth = 4)
        {
            this.sampleRate = sampleRate;
            this.channels = channels;
            this.bitdepth = bitdepth;
            wrappedDSP?.init(sampleRate, channels, bitdepth);
        }


        public DSPClass WrappedDSP
        {
            get
            {
                return wrappedDSP;
            }
            set
            {
                wrappedDSP = value;
                value?.init(sampleRate, channels, bitdepth);
            }
        }

        public override unsafe void processAudio(float* buffer, int len)
        {
            wrappedDSP?.processAudio(buffer, len);
        }
    }


    public class SpeakerInRoomDSP : DSPClass
    {
        const int processBuffer = 1024;
        bool canSurround = false;

        private static byte[] firData = null;

        private static int lastSampleRate = 0;
        private static object syncObjectInit = new object();
        public override void init(int sampleRate, int channels, int bitdepth = 4)
        {
            lock (syncObjectInit)
            {
                canSurround = false;
                if (channels == 2)
                {
                    canSurround = true;
                    if (lastSampleRate != sampleRate)
                    {

                        float[][] IRs = genIR(sampleRate);
                        if (IRs.Length != 4)
                        {
                            canSurround = false;
                            return;
                        }
                        FFTConvolver.con01_reset();
                        FFTConvolver.con02_reset();
                        FFTConvolver.con03_reset();
                        FFTConvolver.con04_reset();
                        int irLen = IRs[0].Length;
                        int fftSize = 1024;
                        unsafe
                        {
                            fixed (float* ir0 = IRs[0]) { test(FFTConvolver.con01_init(fftSize, ir0, irLen)); }
                            fixed (float* ir0 = IRs[1]) { test(FFTConvolver.con02_init(fftSize, ir0, irLen)); }
                            fixed (float* ir0 = IRs[2]) { test(FFTConvolver.con03_init(fftSize, ir0, irLen)); }
                            fixed (float* ir0 = IRs[3]) { test(FFTConvolver.con04_init(fftSize, ir0, irLen)); }
                        }
                        lastSampleRate = sampleRate;
                    }
                }
            }
        }
        void test(bool b)
        {
            if (!b) { throw new Exception("Operation failed!"); }
        }

        public string firSource = null;

        private float[][] genIR(int sampleRate)
        {

            List<float>[] ret;
            if (firData == null)
            {
                if (firSource == null)
                {
                    firData = Properties.Resources.fir2;
                }
                else
                {
                    try
                    {
                        firData = File.ReadAllBytes(firSource);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("不能读取Wav文件", ex);
                    }
                }
            }
            using (MemoryStream ms = new MemoryStream(firData))
            using (WaveFileReader irIn = new WaveFileReader(ms))
            {
                ret = new List<float>[irIn.WaveFormat.Channels];
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = new List<float>();
                }
                ISampleProvider sampleProvider = irIn.ToSampleProvider(); ;
                if (sampleRate != irIn.WaveFormat.SampleRate)
                {
                    sampleProvider = new WdlResamplingSampleProvider(sampleProvider, sampleRate);
                }
                float[] buffer = new float[ret.Length * 100];
                int count = 0;
                while ((count = sampleProvider.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < count; i += ret.Length)
                    {
                        for (int c = 0; c < ret.Length; c++)
                        {
                            ret[c].Add(buffer[i + c]);
                        }
                    }
                }
            }
            return ret.Select(r => r.ToArray()).ToArray();
        }


        float[] leftIn = new float[processBuffer];
        float[] rightIn = new float[processBuffer];
        float[] leftOutL = new float[processBuffer];
        float[] leftOutR = new float[processBuffer];
        float[] rightOutL = new float[processBuffer];
        float[] rightOutR = new float[processBuffer];
        public SpeakerInRoomDSP(string fxfile)
        {
            this.firSource = fxfile;
            lastSampleRate = 0;
            firData = null;
        }
        public SpeakerInRoomDSP() : this(null)
        {
        }
        public override unsafe void processAudio(float* buffer, int len)
        {
            if (canSurround)
            {

                int unprocessed = len;
                int begin = 0;
                while (unprocessed > 0)
                {
                    int thistime = unprocessed > processBuffer * 2 ? processBuffer * 2 : unprocessed;
                    int ptr = 0;
                    for (int i = begin; i < thistime + begin; i += 2, ptr++)
                    {
                        leftIn[ptr] = buffer[i];
                        rightIn[ptr] = buffer[i + 1];


                    }
                    unsafe
                    {
                        fixed (float* leftInPtr = leftIn)
                        fixed (float* rightInPtr = rightIn)
                        fixed (float* leftOutLPtr = leftOutL)
                        fixed (float* rightOutLPtr = rightOutL)
                        fixed (float* leftOutRPtr = leftOutR)
                        fixed (float* rightOutRPtr = rightOutR)
                        {
                            FFTConvolver.con01_process(leftInPtr, leftOutLPtr, thistime / 2);
                            FFTConvolver.con02_process(leftInPtr, leftOutRPtr, thistime / 2);
                            FFTConvolver.con03_process(rightInPtr, rightOutLPtr, thistime / 2);
                            FFTConvolver.con04_process(rightInPtr, rightOutRPtr, thistime / 2);
                        }

                    }
                    ptr = 0;
                    for (int i = begin; i < thistime + begin; i += 2, ptr++)
                    {
                        buffer[i] = leftOutL[ptr] + rightOutL[ptr];
                        buffer[i + 1] = leftOutR[ptr] + rightOutR[ptr];
                    }

                    begin += thistime;
                    unprocessed -= thistime;
                }
            }
        }
    }


    public class RingBuffer
    {
        private float[] buffer;
        private int bufferPtr = 0;
        public RingBuffer(int len)
        {
            buffer = new float[len];
        }

        public float pop(float queueIn)
        {
            float v = buffer[bufferPtr];
            buffer[bufferPtr] = queueIn;
            bufferPtr++;
            if(bufferPtr >= buffer.Length)
            {
                bufferPtr = 0;
            }
            return v;
        }
    }

    public class SurroundDSP : DSPClass
    {
        bool canSurround = false;

        private RingBuffer buffer;

        public override void init(int sampleRate, int channels, int bitdepth = 4)
        {
            canSurround = false;
            if(channels == 2)
            {
                canSurround = true;
                buffer = new RingBuffer(sampleRate / 200);
            }
        }

        public override unsafe void processAudio(float* buffer, int len)
        {
            if (canSurround)
            {
                for (int i = 0; i < len; i++)
                {
                    if (((i) & 1) == 1)
                    {
                        float d = buffer[i];
                        buffer[i] = this.buffer.pop(d);
                    }
                }
            }
        }
    }


    public abstract class DSPClass
    {
        public virtual bool Enabled { get; set; } = true;
        public DSPPROC DspDelegate { get; private set; }
        public DSPClass()
        {
            DspDelegate = new DSPPROC(doDsp);
        }

        public abstract void init(int sampleRate, int channels, int bitdepth = 4);
        public void doDsp(int handle, int channel, IntPtr buffer, int bufferlen, IntPtr user)
        {
            if (bufferlen == 0 || buffer == IntPtr.Zero || (!Enabled))
            {
                return;
            }
            unsafe
            {
                float* audio = (float*)buffer;
                
                processAudio(audio, bufferlen / 4);
            }
        }
        /// <summary>
        /// Process 32bit float audio data
        /// </summary>
        /// <param name="array">data buffer</param>
        /// <param name="arrayLen">data length</param>
        public unsafe abstract void processAudio(float* buffer, int len);
    }
}
