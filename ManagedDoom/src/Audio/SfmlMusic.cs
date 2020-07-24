﻿//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//



using System;
using System.Runtime.ExceptionServices;
using SFML.Audio;
using SFML.System;
using AudioSynthesis;
using AudioSynthesis.Sf2;
using AudioSynthesis.Synthesis;

namespace ManagedDoom.Audio
{
    public sealed class SfmlMusic : IMusic
    {
        private Config config;
        private Wad wad;

        private MusStream stream;

        public SfmlMusic(Config config, Wad wad, string sfPath)
        {
            this.config = config;
            this.wad = wad;
            
            stream = new MusStream(sfPath);
        }

        public void StartMusic(Bgm bgm, bool loop)
        {
            var lump = "D_" + DoomInfo.BgmNames[(int)bgm];
            var data = wad.ReadLump(lump);
            var decoder = new MusDecoder(data, true);
            stream.SetDecoder(decoder);
        }



        private class MusStream : SoundStream
        {
            private static readonly int stepCount = 28;
            private static readonly int synthLength = 2 * MusDecoder.BufferLength;
            private static readonly int batchLength = synthLength * stepCount;

            private Synthesizer synthesizer;

            private MusDecoder current;
            private MusDecoder reserved;

            private short[] batch;

            public MusStream(string sfPath)
            {
                synthesizer = new Synthesizer(MusDecoder.SampleRate, 2, MusDecoder.BufferLength, 1);
                synthesizer.LoadBank(sfPath);

                batch = new short[batchLength];

                Initialize(2, (uint)MusDecoder.SampleRate);
            }

            public void SetDecoder(MusDecoder decoder)
            {
                reserved = decoder;

                if (Status == SoundStatus.Stopped)
                {
                    Play();
                }
            }

            protected override bool OnGetData(out short[] samples)
            {
                if (reserved != current)
                {
                    synthesizer.NoteOffAll(true);
                    synthesizer.ResetSynthControls();
                    current = reserved;
                }

                var t = 0;
                for (var i = 0; i < stepCount; i++)
                {
                    current.FillBuffer(synthesizer);
                    var buffer = synthesizer.sampleBuffer;
                    for (var j = 0; j < synthLength; j++)
                    {
                        var sample = (int)(32768 * buffer[j]);
                        if (sample < short.MinValue)
                        {
                            sample = short.MinValue;
                        }
                        else if (sample > short.MaxValue)
                        {
                            sample = short.MaxValue;
                        }
                        batch[t++] = (short)sample;
                    }
                }

                samples = batch;

                return true;
            }

            protected override void OnSeek(Time timeOffset)
            {
            }
        }



        private class MusDecoder
        {
            public static readonly int SampleRate = 44100;
            public static readonly int BufferLength = SampleRate / 140;

            private static readonly byte[] header = new byte[]
            {
                (byte)'M',
                (byte)'U',
                (byte)'S',
                0x1A
            };

            private byte[] data;
            private bool loop;

            private int scoreLength;
            private int scoreStart;
            private int channelCount;
            private int channelCount2;
            private int instrumentCount;
            private int[] instruments;

            private MusEvent[] events;
            private int eventCount;

            private int[] lastVolume;
            private int p;
            private int delay;

            public MusDecoder(byte[] data, bool loop)
            {
                CheckHeader(data);

                this.data = data;
                this.loop = loop;

                scoreLength = BitConverter.ToUInt16(data, 4);
                scoreStart = BitConverter.ToUInt16(data, 6);
                channelCount = BitConverter.ToUInt16(data, 8);
                channelCount2 = BitConverter.ToUInt16(data, 10);
                instrumentCount = BitConverter.ToUInt16(data, 12);
                instruments = new int[instrumentCount];
                for (var i = 0; i < instruments.Length; i++)
                {
                    instruments[i] = BitConverter.ToUInt16(data, 16 + 2 * i);
                }

                events = new MusEvent[64];
                for (var i = 0; i < events.Length; i++)
                {
                    events[i] = new MusEvent();
                }
                eventCount = 0;

                lastVolume = new int[16];

                Reset();
            }

            private static void CheckHeader(byte[] data)
            {
                for (var p = 0; p < header.Length; p++)
                {
                    if (data[p] != header[p])
                    {
                        throw new Exception("Invalid format!");
                    }
                }
            }

            public void FillBuffer(Synthesizer synthesizer)
            {
                if (delay > 0)
                {
                    delay--;
                }

                if (delay == 0)
                {
                    delay = ReadSingleEventGroup();
                    SendEvents(synthesizer);

                    if (delay == -1 && loop)
                    {
                        Reset();
                    }
                }

                synthesizer.GetNext();
            }

            private void Reset()
            {
                for (var i = 0; i < lastVolume.Length; i++)
                {
                    lastVolume[i] = 0;
                }

                p = scoreStart;

                delay = 0;
            }

            private int ReadSingleEventGroup()
            {
                eventCount = 0;
                while (true)
                {
                    var result = ReadSingleEvent();
                    if (result == ReadResult.EndOfGroup)
                    {
                        break;
                    }
                    else if (result == ReadResult.EndOfFile)
                    {
                        return -1;
                    }
                }

                var time = 0;
                while (true)
                {
                    var value = data[p++];
                    time = time * 128 + (value & 127);
                    if ((value & 128) == 0)
                    {
                        break;
                    }
                }

                if ((data[p] & 0x70) >> 4 == 6)
                {
                    return -1;
                }

                return time;
            }

            private ReadResult ReadSingleEvent()
            {
                var channelNumber = data[p] & 0xF;
                if (channelNumber == 15)
                {
                    channelNumber = 9;
                }

                var eventType = (data[p] & 0x70) >> 4;
                var last = (data[p] >> 7) != 0;

                p++;

                var me = events[eventCount];
                eventCount++;

                switch (eventType)
                {
                    case 0: // RELEASE NOTE
                        me.Type = 0;
                        me.Channel = channelNumber;

                        var releaseNote = data[p++];

                        me.Data1 = releaseNote;
                        me.Data2 = 0;

                        break;

                    case 1: // PLAY NOTE
                        me.Type = 1;
                        me.Channel = channelNumber;

                        var playNote = data[p++];
                        var noteNumber = playNote & 127;
                        var noteVolume = (playNote & 128) != 0 ? data[p++] : -1;

                        me.Data1 = noteNumber;
                        if (noteVolume == -1)
                        {
                            me.Data2 = lastVolume[channelNumber];
                        }
                        else
                        {
                            me.Data2 = noteVolume;
                            lastVolume[channelNumber] = noteVolume;
                        }

                        break;

                    case 2: // PITCH WHEEL
                        me.Type = 2;
                        me.Channel = channelNumber;

                        var pitchWheel = data[p++];

                        var pw2 = (pitchWheel << 7) / 2;
                        var pw1 = pw2 & 127;
                        pw2 >>= 7;
                        me.Data1 = pw1;
                        me.Data2 = pw2;

                        break;

                    case 3: // SYSTEM EVENT
                        me.Type = 3;
                        me.Channel = -1;

                        var systemEvent = data[p++];
                        me.Data1 = systemEvent;
                        me.Data2 = 0;

                        break;

                    case 4: // CONTROL CHANGE
                        me.Type = 4;
                        me.Channel = channelNumber;

                        var controllerNumber = data[p++];
                        var controllerValue = data[p++];

                        me.Data1 = controllerNumber;
                        me.Data2 = controllerValue;

                        break;

                    case 6: // END OF FILE
                        return ReadResult.EndOfFile;

                    default:
                        throw new Exception("Unknown event type!");
                }

                if (last)
                {
                    return ReadResult.EndOfGroup;
                }
                else
                {
                    return ReadResult.Ongoing;
                }
            }

            private void SendEvents(Synthesizer synthesizer)
            {
                for (var i = 0; i < eventCount; i++)
                {
                    var me = events[i];
                    switch (me.Type)
                    {
                        case 0: // RELEASE NOTE
                            synthesizer.NoteOff(me.Channel, me.Data1);
                            break;

                        case 1: // PLAY NOTE
                            synthesizer.NoteOn(me.Channel, me.Data1, me.Data2);
                            break;

                        case 2: // PITCH WHEEL
                            synthesizer.ProcessMidiMessage(me.Channel, 0xE0, me.Data1, me.Data2);
                            break;

                        case 3: // SYSTEM EVENT
                            switch (me.Data1)
                            {
                                case 11: // ALL NOTES OFF
                                    synthesizer.NoteOffAll(true);
                                    break;

                                case 14: // RESET ALL CONTROLS
                                    synthesizer.ResetSynthControls();
                                    break;
                            }
                            break;

                        case 4: // CONTROL CHANGE
                            switch (me.Data1)
                            {
                                case 0: // PROGRAM CHANGE
                                    if (me.Channel == 9)
                                    {
                                        break;
                                    }
                                    synthesizer.ProcessMidiMessage(me.Channel, 0xC0, me.Data2, 0);
                                    break;

                                case 1: // BANK SELECTION
                                    synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x00, me.Data2);
                                    break;

                                case 2: // MODULATION
                                    synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x01, me.Data2);
                                    break;

                                case 3: // VOLUME
                                    synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x07, me.Data2);
                                    break;

                                case 4: // PAN
                                    synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x0A, me.Data2);
                                    break;

                                case 5: // EXPRESSION
                                    synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x0B, me.Data2);
                                    break;

                                case 8: // PEDAL
                                    synthesizer.ProcessMidiMessage(me.Channel, 0xB0, 0x40, me.Data2);
                                    break;
                            }
                            break;
                    }
                }
            }



            private class MusEvent
            {
                public int Type;
                public int Channel;
                public int Data1;
                public int Data2;
            }

            private enum ReadResult
            {
                Ongoing,
                EndOfGroup,
                EndOfFile
            }
        }
    }
}
