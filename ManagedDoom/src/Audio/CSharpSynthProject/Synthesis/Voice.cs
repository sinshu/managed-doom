using System;
using AudioSynthesis.Bank.Patches;

namespace AudioSynthesis.Synthesis
{
    internal class Voice
    {
        // Variables
        private Patch patch;
        private VoiceParameters voiceparams;
        // Properties
        public Patch Patch
        {
            get { return patch; }
        }
        public VoiceParameters VoiceParams
        {
            get { return voiceparams; }
        }
        // Public
        public Voice(Synthesizer synth)
        {
            voiceparams = new VoiceParameters(synth);
        }
        public void Start()
        {
            if (voiceparams.state != VoiceStateEnum.Stopped)
                return;
            if (patch.Start(voiceparams))
                voiceparams.state = VoiceStateEnum.Playing;
        }
        public void Stop()
        {
            if (voiceparams.state != VoiceStateEnum.Playing)
                return;
            voiceparams.state = VoiceStateEnum.Stopping;
            patch.Stop(voiceparams);
        }
        public void StopImmediately()
        {
            voiceparams.state = VoiceStateEnum.Stopped;
        }
        public void Process(int startIndex, int endIndex)
        {
            //do not process if the voice is stopped
            if (voiceparams.state == VoiceStateEnum.Stopped)
                return;            
            //process using the patch's algorithm
            patch.Process(voiceparams, startIndex, endIndex);
        }
        public void Configure(int channel, int note, int velocity, Patch patch)
        {
            Array.Clear(voiceparams.mixing, 0, voiceparams.mixing.Length);
            voiceparams.pitchOffset = 0;
            voiceparams.volOffset = 0;
            voiceparams.noteOffPending = false;
            voiceparams.channel = channel;
            voiceparams.note = note;
            voiceparams.velocity = velocity;
            this.patch = patch;
            if (patch == null)//null removes all refrences to sample
                voiceparams.generators = null;
            else
                voiceparams.generators = patch.GeneratorInfo;
        }
        public override string ToString()
        {
            return voiceparams.ToString() + ", PatchName: " + (patch == null ? "null" : patch.Name);
        }
    }
}
