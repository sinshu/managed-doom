using System.Collections.Generic;

namespace AudioSynthesis.Synthesis
{
    internal class VoiceManager
    {
        public class VoiceNode
        {
            public Voice Value;
            public VoiceNode Next;
            public VoiceNode()
            {
                Value = null;
                Next = null;
            }
        }
        //--Variables
        public enum VoiceStealingMethod { Oldest, Quietest };
        public VoiceStealingMethod stealingMethod;
        public int polyphony;
        public LinkedList<Voice> freeVoices;
        public LinkedList<Voice> activeVoices;
        public VoiceNode[,] registry;
        private Voice[] voicePool;
        private Stack<VoiceNode> vnodes;

        //--Public Methods
        public VoiceManager(int voiceCount, Synthesizer synth)
        {
            stealingMethod = VoiceStealingMethod.Oldest;
            polyphony = voiceCount;
            //initialize voice containers
            voicePool = new Voice[voiceCount];
            VoiceNode[] nodes = new VoiceNode[voiceCount];
            for (int x = 0; x < voicePool.Length; x++)
            {
                voicePool[x] = new Voice(synth);
                nodes[x] = new VoiceNode();
            }
            vnodes = new Stack<VoiceNode>(nodes);
            //free voice list
            freeVoices = new LinkedList<Voice>(voicePool);
            activeVoices = new LinkedList<Voice>();
            registry = new VoiceNode[Synthesizer.DefaultChannelCount, Synthesizer.DefaultKeyCount];
        }
        public Voice GetFreeVoice()
        {
            if (freeVoices.Count > 0)
            {
                Voice voice = freeVoices.First.Value;
                freeVoices.RemoveFirst();
                return voice;
            }
            else if (stealingMethod == VoiceStealingMethod.Oldest)
                return StealOldest();
            else
                return StealQuietestVoice();
        }
        public void AddToRegistry(Voice voice)
        {
            VoiceNode node = vnodes.Pop();
            node.Value = voice;
            node.Next = registry[voice.VoiceParams.channel, voice.VoiceParams.note];
            registry[voice.VoiceParams.channel, voice.VoiceParams.note] = node;
        }
        public void RemoveFromRegistry(int channel, int note)
        {
            VoiceNode node = registry[channel, note];
            while (node != null)
            {
                vnodes.Push(node);
                node = node.Next;
            }
            registry[channel, note] = null;
        }
        public void RemoveFromRegistry(Voice voice)
        {
            VoiceNode node = registry[voice.VoiceParams.channel, voice.VoiceParams.note];
            if (node == null)
                return;
            if (node.Value == voice)
            {
                registry[voice.VoiceParams.channel, voice.VoiceParams.note] = node.Next;
                vnodes.Push(node);
                return;
            }
            else
            {
                VoiceNode node2 = node;
                node = node.Next;
                while (node != null)
                {
                    if (node.Value == voice)
                    {
                        node2.Next = node.Next;
                        vnodes.Push(node);
                        return;
                    }
                    node2 = node;
                    node = node.Next;
                }
            }
        }
        public void ClearRegistry()
        {
            LinkedListNode<Voice> node = activeVoices.First;
            while (node != null)
            {
                VoiceNode vnode = registry[node.Value.VoiceParams.channel, node.Value.VoiceParams.note];
                while (vnode != null)
                {
                    vnodes.Push(vnode);
                    vnode = vnode.Next;
                }
                registry[node.Value.VoiceParams.channel, node.Value.VoiceParams.note] = null;
                node = node.Next;
            }
        }
        public void UnloadPatches()
        {
            for (int x = 0; x < voicePool.Length; x++)
            {
                voicePool[x].Configure(0, 0, 0, null);
                foreach (VoiceNode node in vnodes)
                    node.Value = null;
            }
        }

        private Voice StealOldest()
        {
            LinkedListNode<Voice> node = activeVoices.First;
            //first look for a voice that is not playing
            while (node != null && node.Value.VoiceParams.state == VoiceStateEnum.Playing)
                node = node.Next; 
            //if no stopping voice is found use the oldest
            if (node == null) 
                node = activeVoices.First;
            //check and remove from registry
            RemoveFromRegistry(node.Value);
            activeVoices.Remove(node);
            //stop voice if it is not already
            node.Value.VoiceParams.state = VoiceStateEnum.Stopped;
            return node.Value;
        }
        private Voice StealQuietestVoice()
        {
            float voice_volume = 1000f;
            LinkedListNode<Voice> quietest = null;
            LinkedListNode<Voice> node = activeVoices.First;
            while (node != null)
            {
                if (node.Value.VoiceParams.state != VoiceStateEnum.Playing)
                {
                    float volume = node.Value.VoiceParams.mixing[0] + node.Value.VoiceParams.mixing[1];
                    if (volume < voice_volume)
                    {
                        quietest = node;
                        voice_volume = volume;
                    }
                }
                node = node.Next;
            }
            if (quietest == null)
                quietest = activeVoices.First;
            //check and remove from registry
            RemoveFromRegistry(quietest.Value);
            activeVoices.Remove(quietest);
            //stop voice if it is not already
            quietest.Value.VoiceParams.state = VoiceStateEnum.Stopped;
            return quietest.Value;
        }
    }
}
