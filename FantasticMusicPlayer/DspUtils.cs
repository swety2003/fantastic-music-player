using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

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
        const int sideDelay_us = 429;
        bool canSurround = false;
        RingBuffer leftBuffer = null;
        RingBuffer rightBuffer = null;

        public override void init(int sampleRate, int channels, int bitdepth = 4)
        {
            canSurround = false;
            int bufferLen = sampleRate / 1000;
            if(bufferLen > 0 && channels == 2)
            {
                canSurround = true;
                leftBuffer = new RingBuffer(bufferLen);
                rightBuffer = new RingBuffer(bufferLen);
            }
        }
        public float CrossIn { 
            get {
                return mainWeight;
            } 
            set {
                mainWeight = value;
                subWeight = 1 - value;
            }
        }

        private float mainWeight = 0.7f;
        private float subWeight = 0.3f;
        public override unsafe void processAudio(float* buffer, int len)
        {
            if (canSurround)
            {
                for (int i = 0; i < len; i+=2)
                {
                    float l = buffer[i];
                    float r = buffer[i+1];
                    buffer[i] = l * mainWeight + rightBuffer.pop(r) * subWeight;
                    buffer[i + 1] = r * mainWeight + leftBuffer.pop(l) * subWeight;
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
