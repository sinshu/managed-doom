/*
 *    ______   __ __     _____             __  __  
 *   / ____/__/ // /_   / ___/__  ______  / /_/ /_ 
 *  / /    /_  _  __/   \__ \/ / / / __ \/ __/ __ \
 * / /___ /_  _  __/   ___/ / /_/ / / / / /_/ / / /
 * \____/  /_//_/     /____/\__, /_/ /_/\__/_/ /_/ 
 *                         /____/                  
 * Synthesizer
 *  A synth class that follows the GM spec (for the most part). Use a sequencer to take advantage of easy midi playback, but
 *  the synth can also be used with and external sequencer. See Synthesizer.MidiControl.cs for information about which midi 
 *  events are supported.
 */
using System;
using AudioSynthesis.Bank;
using System.Collections.Generic;
using AudioSynthesis.Midi;
using AudioSynthesis.Bank.Patches;

namespace AudioSynthesis.Synthesis
{
    public partial class Synthesizer
    {
        #region Fields
        //synth variables
        internal float[] sampleBuffer;
        private VoiceManager voiceManager;
        private int audioChannels;
        private bool littleEndian;
        private PatchBank bank;
        private int sampleRate;
        //private int sampleBufferSize;
        private float mainVolume = 1.0f;
        private float synthGain = .35f;
        private int microBufferSize;
        private int microBufferCount;
        //midi controller variables
        private int[] programs;
        private int[] bankSelect;
        private float[] channelPressure;//(volume) volume control via channel pressure event
        private short[] pan;    //(volume) pan positions controlling both right and left output levels
        private short[] volume;     //(volume) volume control per midi channel
        private short[] expression; //(volume) volume control from the expression controller
        private short[] modRange;   //(pitch) mod wheel pitch modifier in partial cents ie. 22.3
        private short[] pitchBend;  //(pitch) pitch bend including both semitones and cents
        private short[] pitchBendRange; //controls max and min pitch bend range default is +-2.0
        private short[] masterCoarseTune;  //(pitch) transposition in semitones
        private short[] masterFineTune; //(pitch) transposition in cents
        private bool[] holdPedal;  //hold pedal status (true) for active
        private short[] rpn;  //register parameter number
        internal double[] modWheel;
        internal PanComponent[] panPositions;
        internal int[] totalPitch;
        internal float[] totalVolume;
        #endregion
        #region Properties
        public int[] Programs
        {
            get { return programs; }
        }
        /// <summary>
        /// The number of voices in use.
        /// </summary>
        public int ActiveVoices
        {
            get { return voiceManager.activeVoices.Count; }
        }
        /// <summary>
        /// The number of voices that are not being used.
        /// </summary>
        public int FreeVoices
        {
            get { return voiceManager.freeVoices.Count; }
        }
        /// <summary>
        /// The size of the individual sub buffers in samples
        /// </summary>
        public int MicroBufferSize
        {
            get { return microBufferSize; }
        }
        /// <summary>
        /// The number of sub buffers
        /// </summary>
        public int MicroBufferCount
        {
            get { return microBufferCount; }
        }
        /// <summary>
        /// The size of the entire buffer in bytes
        /// </summary>
        public int RawBufferSize
        {//Assuming 16 bit data;
            get { return sampleBuffer.Length * 2; }
        }
        /// <summary>
        /// The size of the entire buffer in samples
        /// </summary>
        public int WorkingBufferSize
        {
            get { return sampleBuffer.Length; }
        }
        /// <summary>
        /// The number of voices
        /// </summary>
        public int Polyphony
        {
            get { return voiceManager.polyphony; }
        }
        /// <summary>
        /// Global volume control
        /// </summary>
        public float MasterVolume
        {
            get { return mainVolume; }
            set { mainVolume = SynthHelper.Clamp(value, 0.0f, 3.0f); }
        }
        /// <summary>
        /// The mix volume for each voice
        /// </summary>
        public float MixGain
        {
            get { return synthGain; }
            set { synthGain = SynthHelper.Clamp(value, .05f, 1f); }
        }
        /// <summary>
        /// The number of samples per second produced per channel
        /// </summary>
        public int SampleRate
        {
            get { return sampleRate; }
        }
        /// <summary>
        /// The number of audio channels
        /// </summary>
        public int AudioChannels
        {
            get { return audioChannels; }
        }
        /// <summary>
        /// The patch bank that holds all of the currently loaded instrument patches
        /// </summary>
        public PatchBank SoundBank
        {
            get { return bank; }
        }
        #endregion
        #region Methods
        public Synthesizer(int sampleRate, int audioChannels)
            : this(sampleRate, audioChannels, (int)(.01 * sampleRate), 3, DefaultPolyphony) { }
        public Synthesizer(int sampleRate, int audioChannels, int bufferSize, int bufferCount)
            : this(sampleRate, audioChannels, bufferSize, bufferCount, DefaultPolyphony) { }
        public Synthesizer(int sampleRate, int audioChannels, int bufferSize, int bufferCount, int polyphony)
        {
            const int MinSampleRate = 8000;
            const int MaxSampleRate = 96000;
            //Setup synth parameters
            if (sampleRate < MinSampleRate || sampleRate > MaxSampleRate)
                throw new ArgumentException("Invalid paramater: (sampleRate) Valid ranges are " + MinSampleRate + " to " + MaxSampleRate, "sampleRate");
            this.sampleRate = sampleRate;
            if (audioChannels < 1 || audioChannels > 2)
                throw new ArgumentException("Invalid paramater: (audioChannels) Valid ranges are " + 1 + " to " + 2, "audioChannels");
            this.audioChannels = audioChannels;
            this.microBufferSize = SynthHelper.Clamp(bufferSize, (int)(MinBufferSize * sampleRate), (int)(MaxBufferSize * sampleRate));
            this.microBufferSize = (int)Math.Ceiling(this.microBufferSize / (double)DefaultBlockSize) * DefaultBlockSize; //ensure multiple of block size
            this.microBufferCount = Math.Max(1, bufferCount);
            sampleBuffer = new float[microBufferSize * microBufferCount * audioChannels];
            littleEndian = true;
            //Setup Controllers
            bankSelect = new int[DefaultChannelCount];
            programs = new int[DefaultChannelCount];
            channelPressure = new float[DefaultChannelCount];
            pan = new short[DefaultChannelCount];
            volume = new short[DefaultChannelCount];
            expression = new short[DefaultChannelCount];
            modRange = new short[DefaultChannelCount];
            pitchBend = new short[DefaultChannelCount];
            pitchBendRange = new short[DefaultChannelCount];
            masterCoarseTune = new short[DefaultChannelCount];
            masterFineTune = new short[DefaultChannelCount];
            holdPedal = new bool[DefaultChannelCount];
            rpn = new short[DefaultChannelCount];
            modWheel = new double[DefaultChannelCount];
            panPositions = new PanComponent[DefaultChannelCount];
            totalPitch = new int[DefaultChannelCount];
            totalVolume = new float[DefaultChannelCount];
            //set controls to default values
            ResetSynthControls();
            //create synth voices
            voiceManager = new VoiceManager(SynthHelper.Clamp(polyphony, MinPolyphony, MaxPolyphony), this);
            //create midi containers
            midiEventQueue = new Queue<MidiMessage>();
            midiEventCounts = new int[this.microBufferCount];
        }
        public bool IsLittleEndian()
        {
            return littleEndian;
        }
        public void SetEndianMode(bool isLittleEndian)
        {
            this.littleEndian = isLittleEndian;
        }
        public void LoadBank(string fileName)
        {
            LoadBank(new PatchBank(fileName));
        }
        public void LoadBank(PatchBank bank)
        {
            if (bank == null)
                throw new ArgumentNullException("The parameter bank was null.");
            UnloadBank();
            this.bank = bank;
        }
        public void UnloadBank()
        {
            if (this.bank != null)
            {
                NoteOffAll(true);
                voiceManager.UnloadPatches();
                this.bank = null;
            }
        }
        public void ResetSynthControls()
        {
            for (int x = 0; x < DefaultChannelCount; x++)
            {
                bankSelect[x] = 0;
                channelPressure[x] = 1.0f; //Reset Channel Pressure to 1.0f
                pan[x] = 0x2000;
                volume[x] = (short)(90 << 7); //Reset Vol Positions back to 90/127 (GM spec)
                expression[x] = (short)(100 << 7); //Reset Expression positions back to 100/127
                pitchBend[x] = 0x2000;
                modRange[x] = 0;
                modWheel[x] = 0;
                pitchBendRange[x] = (short)(2 << 7); //Reset pitch wheel to +-2 semitones (GM spec)
                masterCoarseTune[x] = 0;
                masterFineTune[x] = 0x2000; //Reset fine tune
                holdPedal[x] = false;
                rpn[x] = -1; //Reset rpn
                UpdateTotalPitch(x);
                UpdateTotalVolume(x);
                UpdatePan(x);
            }
            bankSelect[MidiHelper.DrumChannel] = PatchBank.DrumBank;
        }
        public void ResetPrograms()
        {
            //Reset instruments
            Array.Clear(programs, 0, programs.Length);
        }
        public string GetProgramName(int channel)
        {
            if (bank != null)
            {
                Patch inst = bank.GetPatch(channel == MidiHelper.DrumChannel ? PatchBank.DrumBank : bankSelect[channel], programs[channel]);
                if (inst != null)
                    return inst.Name;
            }
            return "Null";
        }
        public Patch GetProgram(int channel)
        {
            if (bank != null)
            {
                Patch inst = bank.GetPatch(channel == MidiHelper.DrumChannel ? PatchBank.DrumBank : bankSelect[channel], programs[channel]);
                if (inst != null)
                    return inst;
            }
            return null;
        }
        public void SetAudioChannelCount(int channels)
        {
            channels = SynthHelper.Clamp(channels, 1, 2);
            if (audioChannels != channels)
            {
                audioChannels = channels;
                sampleBuffer = new float[microBufferSize * microBufferCount * audioChannels];
            }
        }
        public void GetNext()
        {
            //CrossPlatformHelper.Assert(sampleBuffer.Length * 2 == buffer.Length, "Output buffer length must equal RawBufferSize!");
            Array.Clear(sampleBuffer, 0, sampleBuffer.Length);
            FillWorkingBuffer();
            //ConvertWorkingBuffer(buffer, sampleBuffer);
        }
        #region Getters
        public float GetChannelVolume(int channel) { return volume[channel] / 16383f; }
        public float GetChannelExpression(int channel) { return expression[channel] / 16383f; }
        public float GetChannelPan(int channel) { return (pan[channel] - 8192.0f) / 8192f; }
        public float GetChannelPitchBend(int channel) { return (pitchBend[channel] - 8192.0f) / 8192f; }
        public bool GetChannelHoldPedalStatus(int channel) { return holdPedal[channel]; }
        #endregion
        // private
        private void FillWorkingBuffer()
        {
            /*Break the process loop into sections representing the smallest timeframe before the midi controls need to be updated
            the bigger the timeframe the more efficent the process is, but playback quality will be reduced.*/
            int sampleIndex = 0;
            for (int x = 0; x < microBufferCount; x++)
            {
                if (midiEventQueue.Count > 0)
                {
                    for (int i = 0; i < midiEventCounts[x]; i++)
                    {
                        MidiMessage m = midiEventQueue.Dequeue();
                        ProcessMidiMessage(m.channel, m.command, m.data1, m.data2);
                    }
                }
                //voice processing loop
                LinkedListNode<Voice> node = voiceManager.activeVoices.First; //node used to traverse the active voices
                while (node != null)
                {
                    node.Value.Process(sampleIndex, sampleIndex + microBufferSize * audioChannels);
                    //if an active voice has stopped remove it from the list
                    if (node.Value.VoiceParams.state == VoiceStateEnum.Stopped)
                    {
                        LinkedListNode<Voice> delnode = node; //node used to remove inactive voices
                        node = node.Next;
                        voiceManager.RemoveFromRegistry(delnode.Value);
                        voiceManager.activeVoices.Remove(delnode);
                        voiceManager.freeVoices.AddFirst(delnode);
                    }
                    else
                    {
                        node = node.Next;
                    }
                }
                sampleIndex += microBufferSize * audioChannels;
            }
            Array.Clear(midiEventCounts, 0, midiEventCounts.Length);
        }
        private void ConvertWorkingBuffer(byte[] to, float[] from)
        {
            if (littleEndian)
            {
                for (int x = 0, i = 0; x < from.Length; x++)
                {
                    short sample = (short)(SynthHelper.Clamp(from[x] * mainVolume, -1.0f, 1.0f) * 32767f);
                    to[i] = (byte)sample;
                    to[i + 1] = (byte)(sample >> 8);
                    i += 2;
                }
            }
            else
            {
                for (int x = 0, i = 0; x < from.Length; x++)
                {
                    short sample = (short)(SynthHelper.Clamp(from[x] * mainVolume, -1.0f, 1.0f) * 32767f);
                    to[i] = (byte)(sample >> 8);
                    to[i + 1] = (byte)sample;
                    i += 2;
                }
            }
        }
        private void UpdateTotalPitch(int channel)
        {
            double cents = ((pitchBend[channel] - 8192.0) / 8192.0) * (100 * (pitchBendRange[channel] >> 7) + (pitchBendRange[channel] & 0x7F));
            cents += 100.0 * (masterCoarseTune[channel] + (masterFineTune[channel] - 8192.0) / 8192.0);
            totalPitch[channel] = (int)cents;
        }
        private void UpdateTotalVolume(int channel)
        {
            totalVolume[channel] = channelPressure[channel] * (volume[channel] / 16383f) * (expression[channel] / 16383f);
        }
        private void UpdatePan(int channel)
        {
            double value = Synthesizer.HalfPi * (pan[channel] / 16383.0);
            panPositions[channel].Left = (float)Math.Cos(value);
            panPositions[channel].Right = (float)Math.Sin(value);
        }
        #endregion
    }
}
