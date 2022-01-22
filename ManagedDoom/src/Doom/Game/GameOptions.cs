//
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
using ManagedDoom.Video;
using ManagedDoom.Audio;
using ManagedDoom.UserInput;

namespace ManagedDoom
{
    public sealed class GameOptions
    {
        private GameVersion gameVersion;
        private GameMode gameMode;
        private MissionPack missionPack;

        private Player[] players;
        private int consolePlayer;

        private int episode;
        private int map;
        private GameSkill skill;

        private bool demoPlayback;
        private bool netGame;

        private int deathmatch;
        private bool fastMonsters;
        private bool respawnMonsters;
        private bool noMonsters;

        private IntermissionInfo intermissionInfo;

        private DoomRandom random;

        private IVideo video;
        private ISound sound;
        private IMusic music;
        private IUserInput userInput;

        public GameOptions()
        {
            gameVersion = GameVersion.Version109;
            gameMode = GameMode.Commercial;
            missionPack = MissionPack.Doom2;

            players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i] = new Player(i);
            }
            players[0].InGame = true;
            consolePlayer = 0;

            episode = 1;
            map = 1;
            skill = GameSkill.Medium;

            demoPlayback = false;
            netGame = false;

            deathmatch = 0;
            fastMonsters = false;
            respawnMonsters = false;
            noMonsters = false;

            intermissionInfo = new IntermissionInfo();

            random = new DoomRandom();

            video = NullVideo.GetInstance();
            sound = NullSound.GetInstance();
            music = NullMusic.GetInstance();
            userInput = NullUserInput.GetInstance();
        }

        public GameOptions(CommandLineArgs args, GameContent content) : this()
        {
            if (args.solonet.Present)
            {
                netGame = true;
            }

            gameVersion = content.Wad.GameVersion;
            gameMode = content.Wad.GameMode;
            missionPack = content.Wad.MissionPack;
        }

        public GameVersion GameVersion
        {
            get => gameVersion;
            set => gameVersion = value;
        }

        public GameMode GameMode
        {
            get => gameMode;
            set => gameMode = value;
        }

        public MissionPack MissionPack
        {
            get => missionPack;
            set => missionPack = value;
        }

        public Player[] Players
        {
            get => players;
        }

        public int ConsolePlayer
        {
            get => consolePlayer;
            set => consolePlayer = value;
        }

        public int Episode
        {
            get => episode;
            set => episode = value;
        }

        public int Map
        {
            get => map;
            set => map = value;
        }

        public GameSkill Skill
        {
            get => skill;
            set => skill = value;
        }

        public bool DemoPlayback
        {
            get => demoPlayback;
            set => demoPlayback = value;
        }

        public bool NetGame
        {
            get => netGame;
            set => netGame = value;
        }

        public int Deathmatch
        {
            get => deathmatch;
            set => deathmatch = value;
        }

        public bool FastMonsters
        {
            get => fastMonsters;
            set => fastMonsters = value;
        }

        public bool RespawnMonsters
        {
            get => respawnMonsters;
            set => respawnMonsters = value;
        }

        public bool NoMonsters
        {
            get => noMonsters;
            set => noMonsters = value;
        }

        public IntermissionInfo IntermissionInfo
        {
            get => intermissionInfo;
        }

        public DoomRandom Random
        {
            get => random;
        }

        public IVideo Video
        {
            get => video;
            set => video = value;
        }

        public ISound Sound
        {
            get => sound;
            set => sound = value;
        }

        public IMusic Music
        {
            get => music;
            set => music = value;
        }

        public IUserInput UserInput
        {
            get => userInput;
            set => userInput = value;
        }
    }
}
