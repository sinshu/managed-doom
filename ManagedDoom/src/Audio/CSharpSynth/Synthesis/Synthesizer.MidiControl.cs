using System.Collections.Generic;
using AudioSynthesis.Midi;
using AudioSynthesis.Bank;
using AudioSynthesis.Bank.Patches;

namespace AudioSynthesis.Synthesis
{
    public partial class Synthesizer
    {
        internal Queue<MidiMessage> midiEventQueue;
        internal int[] midiEventCounts;
        public IEnumerator<MidiMessage> MidiMessageEnumerator
        {
            get { return midiEventQueue.GetEnumerator(); }
        }
        /// <summary>
        /// Starts a voice with the given key and velocity.
        /// </summary>
        /// <param name="channel">The midi channel this voice is on.</param>
        /// <param name="note">The key the voice will play in.</param>
        /// <param name="velocity">The volume of the voice.</param>
        public void NoteOn(int channel, int note, int velocity)
        {
            // Get the correct instrument depending if it is a drum or not
            Patch inst = bank.GetPatch(bankSelect[channel], programs[channel]);
            if (inst == null)
                return;
            // A NoteOn can trigger multiple voices via layers
            List<Patch> layers = new List<Patch>();
            if (inst is MultiPatch)
            {
                ((MultiPatch)inst).FindPatches(channel, note, velocity, layers);
                if (layers.Count == 0) 
                    return;
            }
            else
                layers.Add(inst);
            // If a key with the same note value exists, stop it
            if (voiceManager.registry[channel, note] != null)
            {
                VoiceManager.VoiceNode node = voiceManager.registry[channel, note];
                while (node != null)
                {
                    node.Value.Stop();
                    node = node.Next;
                }
                voiceManager.RemoveFromRegistry(channel, note);
            }
            // Check exclusive groups
            for (int x = 0; x < layers.Count; x++)
            {
                bool notseen = true;
                for (int i = x - 1; i >= 0; i--)
                {
                    if (layers[x].ExclusiveGroupTarget == layers[i].ExclusiveGroupTarget)
                    {
                        notseen = false;
                        break;
                    }
                }
                if (layers[x].ExclusiveGroupTarget != 0 && notseen)
                {
                    LinkedListNode<Voice> node = voiceManager.activeVoices.First;
                    while (node != null)
                    {
                        if (layers[x].ExclusiveGroupTarget == node.Value.Patch.ExclusiveGroup)
                        {
                            node.Value.Stop();
                            voiceManager.RemoveFromRegistry(node.Value);
                        }
                        node = node.Next;
                    }
                }
            }
            // Assign a voice to each layer
            for (int x = 0; x < layers.Count; x++)
            {
                Voice voice = voiceManager.GetFreeVoice();
                voice.Configure(channel, note, velocity, layers[x]);
                voiceManager.AddToRegistry(voice);
                voiceManager.activeVoices.AddLast(voice);
                voice.Start();
            }
        }
        /// <summary>
        /// Attempts to stop a voice by putting it into its release phase. 
        /// If there is no release phase defined the voice will stop immediately.
        /// </summary>
        /// <param name="channel">The channel of the voice.</param>
        /// <param name="note">The key of the voice.</param>
        public void NoteOff(int channel, int note)
        {
            if (holdPedal[channel])
            {
                VoiceManager.VoiceNode node = voiceManager.registry[channel, note];
                while (node != null)
                {
                    node.Value.VoiceParams.noteOffPending = true;
                    node = node.Next;
                }
            }
            else
            {
                VoiceManager.VoiceNode node = voiceManager.registry[channel, note];
                while (node != null)
                {
                    node.Value.Stop();
                    node = node.Next;
                }
                voiceManager.RemoveFromRegistry(channel, note);
            }
        }
        /// <summary>
        /// Stops all voices.
        /// </summary>
        /// <param name="immediate">If true all voices will stop immediately regardless of their release phase.</param>
        public void NoteOffAll(bool immediate)
        {
            LinkedListNode<Voice> node = voiceManager.activeVoices.First;
            if (immediate)
            {//if immediate ignore hold pedals and clear the entire registry
                voiceManager.ClearRegistry();
                while (node != null)
                {
                    node.Value.StopImmediately();
                    LinkedListNode<Voice> delnode = node;
                    node = node.Next;
                    voiceManager.activeVoices.Remove(delnode);
                    voiceManager.freeVoices.AddFirst(delnode);
                }
            }
            else
            {//otherwise we have to check for hold pedals and double check the registry before removing the voice
                while (node != null)
                {
                    //if hold pedal is enabled do not stop the voice
                    if (holdPedal[node.Value.VoiceParams.channel])
                        node.Value.VoiceParams.noteOffPending = true;
                    else
                        node.Value.Stop();
                    node = node.Next;
                }
            }
        }
        /// <summary>
        /// Stops all voices on a given channel.
        /// </summary>
        /// <param name="channel">The midi channel.</param>
        /// <param name="immediate">If true the voices will stop immediately regardless of their release phase.</param>
        public void NoteOffAll(int channel, bool immediate)
        {
            LinkedListNode<Voice> node = voiceManager.activeVoices.First;
            while (node != null)
            {
                if (channel == node.Value.VoiceParams.channel)
                {
                    if (immediate)
                    {
                        node.Value.StopImmediately();
                        LinkedListNode<Voice> delnode = node;
                        node = node.Next;
                        voiceManager.activeVoices.Remove(delnode);
                        voiceManager.freeVoices.AddFirst(delnode);
                    }
                    else
                    {
                        //if hold pedal is enabled do not stop the voice
                        if (holdPedal[channel])
                            node.Value.VoiceParams.noteOffPending = true;
                        else
                            node.Value.Stop();
                        node = node.Next;
                    }
                }
            }
        }
        /// <summary>
        /// Executes a midi command without queueing it first.
        /// </summary>
        /// <param name="midimsg">A midi message struct.</param>
        public void ProcessMidiMessage(int channel, int command, int data1, int data2)
        {
            switch (command)
            {
                case 0x80: //NoteOff
                    NoteOff(channel, data1);
                    break;
                case 0x90: //NoteOn
                    if (data2 == 0)
                        NoteOff(channel, data1);
                    else
                        NoteOn(channel, data1, data2);
                    break;
                case 0xA0: //NoteAftertouch
                    //synth uses channel after touch instead
                    break;
                case 0xB0: //Controller
                    #region Controller Switch  
                    switch (data1)
                    {
                        case 0x00: //Bank select coarse
                            if (channel == MidiHelper.DrumChannel)
                                data2 += PatchBank.DrumBank;
                            if (bank.IsBankLoaded(data2))
                                bankSelect[channel] = data2;
                            else
                                bankSelect[channel] = (channel == MidiHelper.DrumChannel) ? PatchBank.DrumBank : 0;
                            break;
                        case 0x01: //Modulation wheel coarse
                            modRange[channel] = (short)((modRange[channel] & 0x7F) | data2 << 7);
                            modWheel[channel] = Synthesizer.DefaultModDepth * (modRange[channel] / 16383.0); 
                            break;
                        case 0x21: //Modulation wheel fine
                            modRange[channel] = (short)((modRange[channel] & 0xFF80) | data2);
                            modWheel[channel] = Synthesizer.DefaultModDepth * (modRange[channel] / 16383.0);
                            break;
                        case 0x07: //Channel volume coarse
                            volume[channel] = (short)((volume[channel] & 0x7F) | data2 << 7);
                            UpdateTotalVolume(channel);
                            break;
                        case 0x27: //Channel volume fine
                            volume[channel] = (short)((volume[channel] & 0xFF80) | data2);
                            UpdateTotalVolume(channel);
                            break;
                        case 0x0A: //Pan coarse
                            pan[channel] = (short)((pan[channel] & 0x7F) | data2 << 7);
                            UpdatePan(channel);
                            break;
                        case 0x2A: //Pan fine
                            pan[channel] = (short)((pan[channel] & 0xFF80) | data2);
                            UpdatePan(channel);
                            break;
                        case 0x0B: //Expression coarse
                            expression[channel] = (short)((expression[channel] & 0x7F) | data2 << 7);
                            UpdateTotalVolume(channel);
                            break;
                        case 0x2B: //Expression fine
                            expression[channel] = (short)((expression[channel] & 0xFF80) | data2);
                            UpdateTotalVolume(channel);
                            break;
                        case 0x40: //Hold pedal
                            if (holdPedal[channel] && !(data2 > 63))
                            {//if hold pedal is released stop any voices with pending release tags
                                LinkedListNode<Voice> node = voiceManager.activeVoices.First;
                                while (node != null)
                                {
                                    if (node.Value.VoiceParams.noteOffPending)
                                    {
                                        node.Value.Stop();
                                        voiceManager.RemoveFromRegistry(node.Value);
                                    }
                                    node = node.Next;
                                }
                            }
                            holdPedal[channel] = data2 > 63;
                            break;
                        case 0x63: //NRPN Coarse Select   //fix for invalid DataEntry after unsupported NRPN events
                            rpn[channel] = -1; //todo implement NRPN
                            break;
                        case 0x62: //NRPN Fine Select     //fix for invalid DataEntry after unsupported NRPN events
                            rpn[channel] = -1; //todo implement NRPN
                            break;
                        case 0x65: //RPN Coarse Select
                            rpn[channel] = (short)((rpn[channel] & 0x7F) | data2 << 7);
                            break;
                        case 0x64: //RPN Fine Select
                            rpn[channel] = (short)((rpn[channel] & 0xFF80) | data2);
                            break;
                        case 0x7B: //Note Off All
                            NoteOffAll(false);
                            break;
                        case 0x06: //DataEntry Coarse
                            if (rpn[channel] == 0)//change semitone, pitchwheel
                            {
                                pitchBendRange[channel] = (short)((pitchBendRange[channel] & 0x7F) | data2 << 7);
                                UpdateTotalPitch(channel);
                            }
                            else if (rpn[channel] == 1)//master fine tune coarse
                            {
                                masterFineTune[channel] = (short)((masterFineTune[channel] & 0x7F) | data2 << 7);
                                UpdateTotalPitch(channel);
                            }
                            else if (rpn[channel] == 2)//master coarse tune coarse
                            {//in semitones
                                masterCoarseTune[channel] = (short)(data2 - 64);
                                UpdateTotalPitch(channel);
                            }
                            break;
                        case 0x26: //DataEntry Fine
                            if (rpn[channel] == 0)//change cents, pitchwheel
                            {
                                pitchBendRange[channel] = (short)((pitchBendRange[channel] & 0xFF80) | data2);
                                UpdateTotalPitch(channel);
                            }
                            else if (rpn[channel] == 1) //master fine tune fine
                            {
                                masterFineTune[channel] = (short)((masterFineTune[channel] & 0xFF80) | data2);
                                UpdateTotalPitch(channel);
                            }
                            break;
                        case 0x79: //Reset All
                            ResetSynthControls();
                            break;
                        default:
                            return;
                    }
                    #endregion
                    break;
                case 0xC0: //Program Change
                    programs[channel] = data1;
                    break;
                case 0xD0: //Channel Aftertouch
                    channelPressure[channel] = data2 / 127.0f;
                    UpdateTotalVolume(channel);
                    break;
                case 0xE0: //Pitch Bend
                    pitchBend[channel] = (short)(data1 | (data2 << 7));
                    UpdateTotalPitch(channel);
                    break;
                default:
                    return;
            }
        }
    }
}
