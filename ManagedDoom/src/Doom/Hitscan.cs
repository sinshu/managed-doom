using System;

namespace ManagedDoom
{
    public sealed class Hitscan
    {
        private World world;

        public Hitscan(World world)
        {
            this.world = world;


        }

        //
        // P_LineAttack
        //

        // who got hit (or NULL)
        public Mobj linetarget;

        private Mobj shootthing;

        // Height if not aiming up or down
        // ???: use slope for monsters?
        private Fixed shootz;

        private int la_damage;
        private Fixed attackrange;

        private Fixed aimslope;

        // slopes to top and bottom of target
        private Fixed topslope;
        private Fixed bottomslope;




        //
        // PTR_AimTraverse
        // Sets linetaget and aimslope when a target is aimed at.
        //
        private bool PTR_AimTraverse(Intercept ic)
        {
            var mc = world.MapCollision;

            if (ic.Line != null)
            {
                var li = ic.Line;

                if ((li.Flags & LineFlags.TwoSided) == 0)
                {
                    // stop
                    return false;
                }

                // Crosses a two sided line.
                // A two sided line will restrict
                // the possible target ranges.
                mc.LineOpening(li);

                if (mc.OpenBottom >= mc.OpenTop)
                {
                    // stop
                    return false;
                }

                var dist = attackrange * ic.Frac;

                if (li.FrontSector.FloorHeight != li.BackSector.FloorHeight)
                {
                    var slope = (mc.OpenBottom - shootz) / dist;
                    if (slope > bottomslope)
                    {
                        bottomslope = slope;
                    }
                }

                if (li.FrontSector.CeilingHeight != li.BackSector.CeilingHeight)
                {
                    var slope = (mc.OpenTop - shootz) / dist;
                    if (slope < topslope)
                    {
                        topslope = slope;
                    }
                }

                if (topslope <= bottomslope)
                {
                    // stop
                    return false;
                }

                // shot continues
                return true;
            }

            // shoot a thing
            var th = ic.Thing;
            if (th == shootthing)
            {
                // can't shoot self
                return true;
            }

            {
                if ((th.Flags & MobjFlags.Shootable) == 0)
                {
                    // corpse or something
                    return true;
                }

                // check angles to see if the thing can be aimed at
                var dist = attackrange * ic.Frac;
                var thingtopslope = (th.Z + th.Height - shootz) / dist;

                if (thingtopslope < bottomslope)
                {
                    // shot over the thing
                    return true;
                }

                var thingbottomslope = (th.Z - shootz) / dist;

                if (thingbottomslope > topslope)
                {
                    // shot under the thing
                    return true;
                }

                // this thing can be hit!
                if (thingtopslope > topslope)
                {
                    thingtopslope = topslope;
                }

                if (thingbottomslope < bottomslope)
                {
                    thingbottomslope = bottomslope;
                }

                aimslope = (thingtopslope + thingbottomslope) / 2;
                linetarget = th;

                // don't go any farther
                return false;
            }
        }




        //
        // PTR_ShootTraverse
        //
        private bool PTR_ShootTraverse(Intercept ic)
        {
            var pt = world.PathTraversal;
            var mc = world.MapCollision;

            if (ic.Line != null)
            {
                var li = ic.Line;

                if (li.Special != 0)
                {
                    //P_ShootSpecialLine(shootthing, li);
                }

                if ((li.Flags & LineFlags.TwoSided) == 0)
                {
                    goto hitline;
                }

                // crosses a two sided line
                mc.LineOpening(li);

                var dist = attackrange * ic.Frac;

                if (li.FrontSector.FloorHeight != li.BackSector.FloorHeight)
                {
                    var slope = (mc.OpenBottom - shootz) / dist;
                    if (slope > aimslope)
                    {
                        goto hitline;
                    }
                }

                if (li.FrontSector.CeilingHeight != li.BackSector.CeilingHeight)
                {
                    var slope = (mc.OpenTop - shootz) / dist;
                    if (slope < aimslope)
                    {
                        goto hitline;
                    }
                }

                // shot continues
                return true;

            // hit line
            hitline:
                // position a bit closer
                var frac = ic.Frac - Fixed.FromInt(4) / attackrange;
                var x = pt.Trace.X + pt.Trace.Dx * frac;
                var y = pt.Trace.Y + pt.Trace.Dy * frac;
                var z = shootz + aimslope * (frac * attackrange);

                if (li.FrontSector.CeilingFlat == world.Map.SkyFlatNumber)
                {
                    // don't shoot the sky!
                    if (z > li.FrontSector.CeilingHeight)
                    {
                        return false;
                    }

                    // it's a sky hack wall
                    if (li.BackSector != null && li.BackSector.CeilingFlat == world.Map.SkyFlatNumber)
                    {
                        return false;
                    }
                }

                // Spawn bullet puffs.
                SpawnPuff(x, y, z);

                // don't go any farther
                return false;
            }

            // shoot a thing
            var th = ic.Thing;
            if (th == shootthing)
            {
                // can't shoot self
                return true;
            }

            {
                if ((th.Flags & MobjFlags.Shootable) == 0)
                {
                    // corpse or something
                    return true;
                }

                // check angles to see if the thing can be aimed at
                var dist = attackrange * ic.Frac;
                var thingtopslope = (th.Z + th.Height - shootz) / dist;

                if (thingtopslope < aimslope)
                {
                    // shot over the thing
                    return true;
                }

                var thingbottomslope = (th.Z - shootz) / dist;

                if (thingbottomslope > aimslope)
                {
                    // shot under the thing
                    return true;
                }

                // hit thing
                // position a bit closer
                var frac = ic.Frac - Fixed.FromInt(10) / attackrange;

                var x = pt.Trace.X + pt.Trace.Dx * frac;
                var y = pt.Trace.Y + pt.Trace.Dy * frac;
                var z = shootz + aimslope * (frac * attackrange);

                // Spawn bullet puffs or blod spots,
                // depending on target type.
                if ((ic.Thing.Flags & MobjFlags.NoBlood) != 0)
                {
                    SpawnPuff(x, y, z);
                }
                else
                {
                    SpawnBlood(x, y, z, la_damage);
                }

                if (la_damage != 0)
                {
                    world.ThingInteraction.DamageMobj(th, shootthing, shootthing, la_damage);
                }

                // don't go any farther
                return false;
            }
        }


        //
        // P_AimLineAttack
        //
        public Fixed AimLineAttack(Mobj t1, Angle angle, Fixed distance)
        {
            var pt = world.PathTraversal;

            //angle >>= ANGLETOFINESHIFT;
            shootthing = t1;

            var x2 = t1.X + (distance.Data >> Fixed.FracBits) * Trig.Cos(angle); // finecosine[angle];
            var y2 = t1.Y + (distance.Data >> Fixed.FracBits) * Trig.Sin(angle); // finesine[angle];
            shootz = t1.Z + new Fixed((t1.Height.Data >> 1) + 8 * Fixed.FracUnit);

            // can't shoot outside view angles
            topslope = new Fixed(100 * Fixed.FracUnit / 160);
            bottomslope = new Fixed(-100 * Fixed.FracUnit / 160);

            attackrange = distance;
            linetarget = null;

            pt.PathTraverse(t1.X, t1.Y,
                x2, y2,
                PathTraverseFlags.AddLines | PathTraverseFlags.AddThings,
                ic => PTR_AimTraverse(ic));

            if (linetarget != null)
            {
                return aimslope;
            }

            return Fixed.Zero;
        }


        //
        // P_LineAttack
        // If damage == 0, it is just a test trace
        // that will leave linetarget set.
        //
        public void LineAttack(
            Mobj t1,
            Angle angle,
            Fixed distance,
            Fixed slope,
            int damage)
        {
            var pt = world.PathTraversal;

            //angle >>= ANGLETOFINESHIFT;
            shootthing = t1;
            la_damage = damage;
            var x2 = t1.X + (distance.Data >> Fixed.FracBits) * Trig.Cos(angle); // finecosine[angle];
            var y2 = t1.Y + (distance.Data >> Fixed.FracBits) * Trig.Sin(angle); // finesine[angle];
            shootz = t1.Z + new Fixed((t1.Height.Data >> 1) + 8 * Fixed.FracUnit);
            attackrange = distance;
            aimslope = slope;

            pt.PathTraverse(t1.X, t1.Y,
                x2, y2,
                PathTraverseFlags.AddLines | PathTraverseFlags.AddThings,
                ic => PTR_ShootTraverse(ic));
        }



        private void SpawnPuff(Fixed x, Fixed y, Fixed z)
        {
            z += new Fixed((world.Random.Next() - world.Random.Next()) << 10);

            var th = world.ThingAllocation.SpawnMobj(x, y, z, MobjType.Puff);
            th.MomZ = Fixed.One;
            th.Tics -= world.Random.Next() & 3;

            if (th.Tics < 1)
            {
                th.Tics = 1;
            }

            // don't make punches spark on the wall
            if (attackrange == World.MELEERANGE)
            {
                th.SetState(State.Puff3);
            }
        }



        //
        // P_SpawnBlood
        // 
        private void SpawnBlood(Fixed x, Fixed y, Fixed z, int damage)
        {
            z += new Fixed((world.Random.Next() - world.Random.Next()) << 10);
            var th = world.ThingAllocation.SpawnMobj(x, y, z, MobjType.Blood);
            th.MomZ = Fixed.FromInt(2);
            th.Tics -= world.Random.Next() & 3;

            if (th.Tics < 1)
            {
                th.Tics = 1;
            }

            if (damage <= 12 && damage >= 9)
            {
                th.SetState(State.Blood2);
            }
            else if (damage < 9)
            {
                th.SetState(State.Blood3);
            }
        }


        public Fixed bulletslope;

        public Fixed BottomSlope => bottomslope;
        public Fixed TopSlope => topslope;
    }
}
