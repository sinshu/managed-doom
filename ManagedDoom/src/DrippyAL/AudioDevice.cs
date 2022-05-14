using System;
using System.Numerics;
using System.Runtime.ExceptionServices;
using Silk.NET.OpenAL;

namespace DrippyAL
{
    public unsafe sealed class AudioDevice : IDisposable
    {
        private ALContext? alc;
        private AL? al;

        private Device* device;
        private Context* context;

        private Vector3 listenerPosition;
        private float[] listenerOrientation;

        public AudioDevice()
        {
            try
            {
                alc = ALContext.GetApi();
                al = AL.GetApi();

                device = alc.OpenDevice("");
                context = alc.CreateContext(device, null);

                alc.MakeContextCurrent(context);
                al.GetError();

                listenerPosition = new Vector3(0F, 0F, 0F);
                al.SetListenerProperty(ListenerVector3.Position, in listenerPosition);

                listenerOrientation = new float[]
                {
                    0F, 0F, -1F,
                    0F, 1F,  0F
                };
                fixed (float* p = listenerOrientation)
                {
                    al.SetListenerProperty(ListenerFloatArray.Orientation, p);
                }
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public void Dispose()
        {
            if (alc != null)
            {
                if (context != default(Context*))
                {
                    alc.DestroyContext(context);
                    context = default(Context*);
                }

                if (device != default(Device*))
                {
                    alc.CloseDevice(device);
                    device = default(Device*);
                }
            }

            if (al != null)
            {
                al.Dispose();
                al = null;
            }

            if (alc != null)
            {
                alc.Dispose();
                alc = null;
            }
        }

        internal AL AL
        {
            get
            {
                if (al == null)
                {
                    throw new ObjectDisposedException(nameof(AudioDevice));
                }

                return al;
            }
        }

        public Vector3 ListernerPosition
        {
            get
            {
                if (al == null)
                {
                    throw new ObjectDisposedException(nameof(AudioDevice));
                }

                return listenerPosition;
            }

            set
            {
                if (al == null)
                {
                    throw new ObjectDisposedException(nameof(AudioDevice));
                }

                listenerPosition = value;
                al.SetListenerProperty(ListenerVector3.Position, in listenerPosition);
            }
        }

        public Vector3 ListernerDirection
        {
            get
            {
                if (al == null)
                {
                    throw new ObjectDisposedException(nameof(AudioDevice));
                }

                return new Vector3(
                    listenerOrientation[0],
                    listenerOrientation[1],
                    listenerOrientation[2]);
            }

            set
            {
                if (al == null)
                {
                    throw new ObjectDisposedException(nameof(AudioDevice));
                }

                listenerOrientation[0] = value.X;
                listenerOrientation[1] = value.Y;
                listenerOrientation[2] = value.Z;
                fixed (float* p = listenerOrientation)
                {
                    al.SetListenerProperty(ListenerFloatArray.Orientation, p);
                }
            }
        }

        public Vector3 ListernerUpVector
        {
            get
            {
                if (al == null)
                {
                    throw new ObjectDisposedException(nameof(AudioDevice));
                }

                return new Vector3(
                    listenerOrientation[3],
                    listenerOrientation[4],
                    listenerOrientation[5]);
            }

            set
            {
                if (al == null)
                {
                    throw new ObjectDisposedException(nameof(AudioDevice));
                }

                listenerOrientation[3] = value.X;
                listenerOrientation[4] = value.Y;
                listenerOrientation[5] = value.Z;
                fixed (float* p = listenerOrientation)
                {
                    al.SetListenerProperty(ListenerFloatArray.Orientation, p);
                }
            }
        }
    }
}
