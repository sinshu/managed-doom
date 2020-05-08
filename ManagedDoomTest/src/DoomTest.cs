using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManagedDoom;

namespace ManagedDoomTest
{
    public static class DoomTest
    {
        public static Player[] GetDefaultPlayers()
        {
            var players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i] = new Player(i);
                players[i].PlayerState = PlayerState.Reborn;
            }
            players[0].InGame = true;

            return players;
        }

        public static Player[] GetDefaultPlayers(GameOptions options)
        {
            var players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i] = new Player(i);
                players[i].InGame = options.PlayerInGame[i];
            }

            return players;
        }
    }
}
