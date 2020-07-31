/* Patches are basically algorithms for creating instruments.
 * They contain textual descriptions of how to configure the voices
 * using generators, filters, envelopes, and other components.

 * When creating patches remember that they are used to configure multiple voices and
 * should not contain any code/fields that mutate its state after initial loading.
 * All code that will effect each voice instance should instead be put into the
 * Process(...) and Start(...) abstract implementations. */

using System;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Bank.Descriptors;
using AudioSynthesis.Bank.Components.Generators;

namespace AudioSynthesis.Bank.Patches
{
    public abstract class Patch
    {
        protected string patchName;
        protected Generator[] genList;
        protected LfoDescriptor[] lfoList;
        protected FilterDescriptor[] fltrList;
        protected EnvelopeDescriptor[] envList;
        protected int exTarget;
        protected int exGroup;
        //public properties and methods
        public int ExclusiveGroupTarget
        {
            get { return exTarget; }
            set { exTarget = value; }
        }
        public int ExclusiveGroup
        {
            get { return exGroup; }
            set { exGroup = value; }
        }
        public Generator[] GeneratorInfo
        {
            get { return genList; }
        }
        public EnvelopeDescriptor[] EnvelopeInfo
        {
            get { return envList; }
        }
        public LfoDescriptor[] LfoInfo
        {
            get { return lfoList; }
        }
        public FilterDescriptor[] FilterInfo
        {
            get { return fltrList; }
        }
        public string Name
        {
            get { return patchName; }
        }

        protected Patch(string name)
        {
            patchName = name;
            genList = new Generator[Synthesizer.MaxVoiceComponents];
            lfoList = new LfoDescriptor[Synthesizer.MaxVoiceComponents];
            fltrList = new FilterDescriptor[Synthesizer.MaxVoiceComponents];
            envList = new EnvelopeDescriptor[Synthesizer.MaxVoiceComponents];
            exTarget = 0;
            exGroup = 0;
        }     
        public void ClearDescriptors()
        {
            Array.Clear(genList, 0, genList.Length);
            Array.Clear(envList, 0, envList.Length);
            Array.Clear(fltrList, 0, fltrList.Length);
            Array.Clear(lfoList, 0, lfoList.Length);
        }    
        public override string ToString()
        {
            return string.Format("Name: {0}, GeneratorCount: {1}", patchName, GetArrayCount(genList));
        }
        //abstract methods 
        public abstract void Process(VoiceParameters voiceparams, int startIndex, int endIndex);
        public abstract bool Start(VoiceParameters voiceparams);
        public abstract void Stop(VoiceParameters voiceparams);
        public abstract void Load(DescriptorList description, AssetManager assets);
        //static methods
        private static int GetArrayCount(object[] array)
        {
            int count = 0;
            for (int x = 0; x < array.Length; x++)
            {
                if (array[x] != null)
                    count++;
            }
            return count;
        }
    }
}
