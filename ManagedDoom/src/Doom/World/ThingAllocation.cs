using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class ThingAllocation
    {
        private World world;

        public ThingAllocation(World world)
        {
            this.world = world;

            InitSpawnMapThing();
            InitRespawnSpecials();
        }


        private MapThing[] playerStarts;
        private List<MapThing> deathmatchStarts;

        private void InitSpawnMapThing()
        {
            playerStarts = new MapThing[Player.MaxPlayerCount];
            deathmatchStarts = new List<MapThing>();
        }

        public void SpawnMapThing(MapThing mt)
        {
            // Count deathmatch start positions.
            if (mt.Type == 11)
            {
                if (deathmatchStarts.Count < 10)
                {
                    deathmatchStarts.Add(mt);
                }

                return;
            }

            // Check for players specially.
            if (mt.Type <= 4)
            {
                var playerNumber = mt.Type - 1;

                // This check is neccesary in Plutonia MAP12,
                // which contains an unknown thing with type 0.
                if (playerNumber < 0)
                {
                    return;
                }

                // Save spots for respawning in network games.
                playerStarts[playerNumber] = mt;

                if (world.Options.Deathmatch == 0)
                {
                    SpawnPlayer(mt);
                }

                return;
            }

            if (mt.Type == 11 || mt.Type <= 4)
            {
                return;
            }

            // Check for apropriate skill level.
            if (!world.Options.NetGame && ((int)mt.Flags & 16) != 0)
            {
                return;
            }

            int bit;
            if (world.Options.Skill == GameSkill.Baby)
            {
                bit = 1;
            }
            else if (world.Options.Skill == GameSkill.Nightmare)
            {
                bit = 4;
            }
            else
            {
                bit = 1 << ((int)world.Options.Skill - 1);
            }

            if (((int)mt.Flags & bit) == 0)
            {
                return;
            }

            // Find which type to spawn.
            int i;
            for (i = 0; i < DoomInfo.MobjInfos.Length; i++)
            {
                if (mt.Type == DoomInfo.MobjInfos[i].DoomEdNum)
                {
                    break;
                }
            }

            if (i == DoomInfo.MobjInfos.Length)
            {
                throw new Exception("Unknown type!");
            }

            // Don't spawn keycards and players in deathmatch.
            if (world.Options.Deathmatch != 0 &&
                (DoomInfo.MobjInfos[i].Flags & MobjFlags.NotDeathmatch) != 0)
            {
                return;
            }

            // Don't spawn any monsters if -nomonsters.
            if (world.Options.NoMonsters &&
                (i == (int)MobjType.Skull || (DoomInfo.MobjInfos[i].Flags & MobjFlags.CountKill) != 0))
            {
                return;
            }

            // Spawn it.
            Fixed x = mt.X;
            Fixed y = mt.Y;
            Fixed z;
            if ((DoomInfo.MobjInfos[i].Flags & MobjFlags.SpawnCeiling) != 0)
            {
                z = Mobj.OnCeilingZ;
            }
            else
            {
                z = Mobj.OnFloorZ;
            }

            var mobj = SpawnMobj(x, y, z, (MobjType)i);

            mobj.SpawnPoint = mt;

            if (mobj.Tics > 0)
            {
                mobj.Tics = 1 + (world.Random.Next() % mobj.Tics);
            }

            if ((mobj.Flags & MobjFlags.CountKill) != 0)
            {
                world.totalKills++;
            }

            if ((mobj.Flags & MobjFlags.CountItem) != 0)
            {
                world.totalItems++;
            }

            mobj.Angle = mt.Angle;

            if ((mt.Flags & ThingFlags.Ambush) != 0)
            {
                mobj.Flags |= MobjFlags.Ambush;
            }
        }

        public void SpawnPlayer(MapThing mt)
        {
            var players = world.Options.Players;
            var playerNumber = mt.Type - 1;

            // Not playing?
            if (!players[playerNumber].InGame)
            {
                return;
            }

            var player = players[playerNumber];

            if (player.PlayerState == PlayerState.Reborn)
            {
                players[playerNumber].Reborn();
            }

            var x = mt.X;
            var y = mt.Y;
            var z = Mobj.OnFloorZ;
            var mobj = SpawnMobj(x, y, z, MobjType.Player);

            // Set color translations for player sprites.
            if (playerNumber >= 1)
            {
                mobj.Flags |= (MobjFlags)((mt.Type - 1) << (int)MobjFlags.TransShift);
            }

            mobj.Angle = mt.Angle;
            mobj.Player = player;
            mobj.Health = player.Health;

            player.Mobj = mobj;
            player.PlayerState = PlayerState.Live;
            player.Refire = 0;
            player.Message = null;
            player.MessageTime = 0;
            player.DamageCount = 0;
            player.BonusCount = 0;
            player.ExtraLight = 0;
            player.FixedColorMap = 0;
            player.ViewHeight = Player.VIEWHEIGHT;

            // Setup gun psprite.
            world.PlayerBehavior.SetupPlayerSprites(player);

            // Give all cards in death match mode.
            if (world.Options.Deathmatch != 0)
            {
                for (var i = 0; i < (int)CardType.Count; i++)
                {
                    player.Cards[i] = true;
                }
            }

            if (mt.Type - 1 == world.Options.ConsolePlayer)
            {
                world.StatusBar.Reset();
                world.Options.Audio.SetListener(mobj);
            }
        }

        public IReadOnlyList<MapThing> PlayerStarts => playerStarts;
        public IReadOnlyList<MapThing> DeathmatchStarts => deathmatchStarts;


        public Mobj SpawnMobj(Fixed x, Fixed y, Fixed z, MobjType type)
        {
            var mobj = ThinkerPool.RentMobj(world);

            var info = DoomInfo.MobjInfos[(int)type];

            mobj.Type = type;
            mobj.Info = info;
            mobj.X = x;
            mobj.Y = y;
            mobj.Radius = info.Radius;
            mobj.Height = info.Height;
            mobj.Flags = info.Flags;
            mobj.Health = info.SpawnHealth;

            if (world.Options.Skill != GameSkill.Nightmare)
            {
                mobj.ReactionTime = info.ReactionTime;
            }

            mobj.LastLook = world.Random.Next() % Player.MaxPlayerCount;

            // Do not set the state with P_SetMobjState,
            // because action routines can not be called yet.
            var st = DoomInfo.States[(int)info.SpawnState];

            mobj.State = st;
            mobj.Tics = st.Tics;
            mobj.Sprite = st.Sprite;
            mobj.Frame = st.Frame;

            // Set subsector and/or block links.
            world.ThingMovement.SetThingPosition(mobj);

            mobj.FloorZ = mobj.Subsector.Sector.FloorHeight;
            mobj.CeilingZ = mobj.Subsector.Sector.CeilingHeight;

            if (z == Mobj.OnFloorZ)
            {
                mobj.Z = mobj.FloorZ;
            }
            else if (z == Mobj.OnCeilingZ)
            {
                mobj.Z = mobj.CeilingZ - mobj.Info.Height;
            }
            else
            {
                mobj.Z = z;
            }

            world.Thinkers.Add(mobj);

            return mobj;
        }

        public void RemoveMobj(Mobj mobj)
        {
            var tm = world.ThingMovement;

            if ((mobj.Flags & MobjFlags.Special) != 0 &&
                (mobj.Flags & MobjFlags.Dropped) == 0 &&
                (mobj.Type != MobjType.Inv) &&
                (mobj.Type != MobjType.Ins))
            {
                itemrespawnque[iquehead] = mobj.SpawnPoint;
                itemrespawntime[iquehead] = world.levelTime;
                iquehead = (iquehead + 1) & (ITEMQUESIZE - 1);

                // Lose one off the end?
                if (iquehead == iquetail)
                {
                    iquetail = (iquetail + 1) & (ITEMQUESIZE - 1);
                }
            }

            // Unlink from sector and block lists.
            tm.UnsetThingPosition(mobj);

            // Stop any playing sound.
            world.StopSound(mobj);

            // Free block.
            world.Thinkers.Remove(mobj);
        }

        public void CheckMissileSpawn(Mobj missile)
        {
            missile.Tics -= world.Random.Next() & 3;
            if (missile.Tics < 1)
            {
                missile.Tics = 1;
            }

            // Move a little forward so an angle can be computed if it immediately explodes.
            missile.X += (missile.MomX >> 1);
            missile.Y += (missile.MomY >> 1);
            missile.Z += (missile.MomZ >> 1);

            if (!world.ThingMovement.TryMove(missile, missile.X, missile.Y))
            {
                world.ThingInteraction.ExplodeMissile(missile);
            }
        }

        public Mobj SpawnMissile(Mobj source, Mobj dest, MobjType type)
        {
            var missile = SpawnMobj(
                source.X,
                source.Y,
                source.Z + Fixed.FromInt(32), type);

            if (missile.Info.SeeSound != 0)
            {
                world.StartSound(missile, missile.Info.SeeSound, SfxType.Misc);
            }

            // Where it came from?
            missile.Target = source;

            var angle = Geometry.PointToAngle(
                source.X, source.Y,
                dest.X, dest.Y);

            // Fuzzy player.
            if ((dest.Flags & MobjFlags.Shadow) != 0)
            {
                var random = world.Random;
                angle += new Angle((random.Next() - random.Next()) << 20);
            }

            var speed = GetMissileSpeed(missile.Type);

            missile.Angle = angle;
            missile.MomX = new Fixed(speed) * Trig.Cos(angle);
            missile.MomY = new Fixed(speed) * Trig.Sin(angle);

            var dist = Geometry.AproxDistance(
                dest.X - source.X,
                dest.Y - source.Y);

            var num = (dest.Z - source.Z).Data;
            var den = (dist / speed).Data;
            if (den < 1)
            {
                den = 1;
            }

            missile.MomZ = new Fixed(num / den);

            CheckMissileSpawn(missile);

            return missile;
        }

        private int GetMissileSpeed(MobjType type)
        {
            if (world.Options.FastMonsters || world.Options.Skill == GameSkill.Nightmare)
            {
                switch (type)
                {
                    case MobjType.Bruisershot:
                    case MobjType.Headshot:
                    case MobjType.Troopshot:
                        return 20 * Fixed.FracUnit;
                    default:
                        return DoomInfo.MobjInfos[(int)type].Speed;
                }
            }
            else
            {
                return DoomInfo.MobjInfos[(int)type].Speed;
            }
        }

        public void SpawnPlayerMissile(Mobj source, MobjType type)
        {
            var hs = world.Hitscan;

            // See which target is to be aimed at.
            var angle = source.Angle;
            var slope = hs.AimLineAttack(source, angle, Fixed.FromInt(16 * 64));

            if (hs.LineTarget == null)
            {
                angle += new Angle(1 << 26);
                slope = hs.AimLineAttack(source, angle, Fixed.FromInt(16 * 64));

                if (hs.LineTarget == null)
                {
                    angle -= new Angle(2 << 26);
                    slope = hs.AimLineAttack(source, angle, Fixed.FromInt(16 * 64));
                }

                if (hs.LineTarget == null)
                {
                    angle = source.Angle;
                    slope = Fixed.Zero;
                }
            }

            var x = source.X;
            var y = source.Y;
            var z = source.Z + Fixed.FromInt(32);

            var missile = SpawnMobj(x, y, z, type);

            if (missile.Info.SeeSound != 0)
            {
                world.StartSound(missile, missile.Info.SeeSound, SfxType.Misc);
            }

            missile.Target = source;
            missile.Angle = angle;
            missile.MomX = new Fixed(missile.Info.Speed) * Trig.Cos(angle);
            missile.MomY = new Fixed(missile.Info.Speed) * Trig.Sin(angle);
            missile.MomZ = new Fixed(missile.Info.Speed) * slope;

            CheckMissileSpawn(missile);
        }











        private static readonly int BODYQUESIZE = 32;
        private Mobj[] bodyque = new Mobj[BODYQUESIZE];
        private int bodyqueslot;

        //
        // G_CheckSpot  
        // Returns false if the player cannot be respawned
        // at the given mapthing_t spot  
        // because something is occupying it 
        //
        public bool CheckSpot(int playernum, MapThing mthing)
        {
            var players = world.Options.Players;

            if (players[playernum].Mobj == null)
            {
                // First spawn of level, before corpses.
                for (var i = 0; i < playernum; i++)
                {
                    if (players[i].Mobj.X == mthing.X && players[i].Mobj.Y == mthing.Y)
                    {
                        return false;
                    }
                }
                return true;
            }

            var x = mthing.X;
            var y = mthing.Y;

            if (!world.ThingMovement.CheckPosition(players[playernum].Mobj, x, y))
            {
                return false;
            }

            // Flush an old corpse if needed.
            if (bodyqueslot >= BODYQUESIZE)
            {
                RemoveMobj(bodyque[bodyqueslot % BODYQUESIZE]);
            }
            bodyque[bodyqueslot % BODYQUESIZE] = players[playernum].Mobj;
            bodyqueslot++;

            // Spawn a teleport fog.
            var ss = Geometry.PointInSubsector(x, y, world.Map);

            var an = (Angle.Ang45.Data >> Trig.AngleToFineShift) * ((int)Math.Round(mthing.Angle.ToDegree()) / 45);

            Fixed xa;
            Fixed ya;

            switch (an)
            {
                case 4096:  // -4096:
                    xa = Trig.Tan(2048);    // finecosine[-4096]
                    ya = Trig.Tan(0);       // finesine[-4096]
                    break;
                case 5120:  // -3072:
                    xa = Trig.Tan(3072);    // finecosine[-3072]
                    ya = Trig.Tan(1024);    // finesine[-3072]
                    break;
                case 6144:  // -2048:
                    xa = Trig.Sin(0);          // finecosine[-2048]
                    ya = Trig.Tan(2048);    // finesine[-2048]
                    break;
                case 7168:  // -1024:
                    xa = Trig.Sin(1024);       // finecosine[-1024]
                    ya = Trig.Tan(3072);    // finesine[-1024]
                    break;
                case 0:
                case 1024:
                case 2048:
                case 3072:
                    xa = Trig.Cos((int)an);
                    ya = Trig.Sin((int)an);
                    break;
                default:
                    throw new Exception("G_CheckSpot: unexpected angle " + an);
            }

            var mo = SpawnMobj(
                x + 20 * xa, y + 20 * ya,
                ss.Sector.FloorHeight,
                MobjType.Tfog);

            if (!world.FirstTicIsNotYetDone)
            {
                // Don't start sound on first frame.
                world.StartSound(mo, Sfx.TELEPT, SfxType.Misc);
            }

            return true;
        }

        //
        // G_DeathMatchSpawnPlayer 
        // Spawns a player at one of the random death match spots 
        // called at level load and each death 
        //
        public void DeathMatchSpawnPlayer(int playernum)
        {
            var selections = deathmatchStarts.Count;
            if (selections < 4)
            {
                throw new Exception("Only " + selections + " deathmatch spots, 4 required");
            }

            var random = world.Random;
            for (var j = 0; j < 20; j++)
            {
                var i = random.Next() % selections;
                if (CheckSpot(playernum, deathmatchStarts[i]))
                {
                    deathmatchStarts[i].Type = playernum + 1;
                    SpawnPlayer(deathmatchStarts[i]);
                    return;
                }
            }

            // no good spot, so the player will probably get stuck 
            SpawnPlayer(playerStarts[playernum]);
        }








        private static readonly int ITEMQUESIZE = 128;
        private MapThing[] itemrespawnque;
        private int[] itemrespawntime;
        private int iquehead;
        private int iquetail;

        private void InitRespawnSpecials()
        {
            itemrespawnque = new MapThing[ITEMQUESIZE];
            itemrespawntime = new int[ITEMQUESIZE];
            iquehead = 0;
            iquetail = 0;
        }

        public void RespawnSpecials()
        {
            // Only respawn items in deathmatch.
            if (world.Options.Deathmatch != 2)
            {
                return;
            }

            // Nothing left to respawn?
            if (iquehead == iquetail)
            {
                return;
            }

            // Wait at least 30 seconds.
            if (world.levelTime - itemrespawntime[iquetail] < 30 * 35)
            {
                return;
            }

            var mthing = itemrespawnque[iquetail];

            var x = mthing.X;
            var y = mthing.Y;

            // Spawn a teleport fog at the new spot.
            var ss = Geometry.PointInSubsector(x, y, world.Map);
            var mo = SpawnMobj(x, y, ss.Sector.FloorHeight, MobjType.Ifog);
            world.StartSound(mo, Sfx.ITMBK, SfxType.Misc);

            int i;
            // Find which type to spawn.
            for (i = 0; i < DoomInfo.MobjInfos.Length; i++)
            {
                if (mthing.Type == DoomInfo.MobjInfos[i].DoomEdNum)
                {
                    break;
                }
            }

            // Spawn it.
            Fixed z;
            if ((DoomInfo.MobjInfos[i].Flags & MobjFlags.SpawnCeiling) != 0)
            {
                z = Mobj.OnCeilingZ;
            }
            else
            {
                z = Mobj.OnFloorZ;
            }

            mo = SpawnMobj(x, y, z, (MobjType)i);
            mo.SpawnPoint = mthing;
            mo.Angle = mthing.Angle;

            // Pull it from the que.
            iquetail = (iquetail + 1) & (ITEMQUESIZE - 1);
        }
    }
}
