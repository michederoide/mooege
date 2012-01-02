/*
 * Copyright (C) 2011 mooege project
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Core.GS.Ticker;
using Mooege.Net.GS.Message.Definitions.Effect;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Powers.Payloads;
using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Core.GS.Actors;
using Mooege.Core.GS.Actors.Movement;
using Mooege.Net.GS.Message.Definitions.Actor;
using Mooege.Net.GS.Message;

namespace Mooege.Core.GS.Powers.Implementations
{
    //Complete skill/Runes by MDZ, TODO: fix positioning of hit actors.
    #region PoisonDart
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.PoisonDart)]
    public class WitchDoctorPoisonDart : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            int numProjectiles = Rune_B > 0 ? (int)ScriptFormula(4) : 1;
            for (int n = 0; n < numProjectiles; ++n)
            {
                if (Rune_B > 0)
                    yield return WaitSeconds(ScriptFormula(17));

                var proj = new Projectile(this,
                                          RuneSelect(107011, 107030, 107035, 107223, 107265, 107114),
                                          User.Position);
                proj.Position.Z += 3f;
                proj.OnCollision = (hit) =>
                {
                    // TODO: fix positioning of hit actors. possibly increase model scale? 
                    SpawnEffect(RuneSelect(112327, 112338, 112327, 112345, 112347, 112311), proj.Position);

                    proj.Destroy();

                    if (Rune_E > 0 && Rand.NextDouble() < ScriptFormula(11))
                        hit.PlayEffectGroup(107163);
                    
                    if (Rune_A > 0)
                        WeaponDamage(hit, ScriptFormula(2), DamageType.Fire);
                    else
                        WeaponDamage(hit, ScriptFormula(0), DamageType.Poison);
                };
                proj.Launch(TargetPosition, 1f);
            }
        }
    }
    #endregion

    //Pet Class
    #region SummonZombieDogs
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.Support.SummonZombieDogs)]
    public class SummonZombieDogs : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    //Hacky BigToad by MDZ
    #region PlagueOfToads
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.PlagueOfToads)]
    public class WitchDoctorPlagueOfToads : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            if (Rune_C > 0)
            {
                // NOTE: not normal plague of toads right now but Obsidian runed "Toad of Hugeness"
                Vector3D userCastPosition = new Vector3D(User.Position);
                Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 7f);
                var bigtoad = SpawnEffect(109906, inFrontOfUser, TargetPosition, WaitInfinite());

                // HACK: holy hell there is alot of hardcoded animation timings here

                bigtoad.PlayActionAnimation(110766); // spawn ani
                yield return WaitSeconds(1f);

                bigtoad.PlayActionAnimation(110520); // attack ani
                TickTimer waitAttackEnd = WaitSeconds(1.5f);
                yield return WaitSeconds(0.3f); // wait for attack ani to play a bit

                var tongueEnd = SpawnProxy(TargetPosition, WaitInfinite());
                bigtoad.AddRopeEffect(107892, tongueEnd);

                yield return WaitSeconds(0.3f); // have tongue hang there for a bit

                var tongueMover = new Implementations.KnockbackBuff(-0.01f, 3f, -0.1f);
                this.World.BuffManager.AddBuff(bigtoad, tongueEnd, tongueMover);
                if (ValidTarget())
                    this.World.BuffManager.AddBuff(bigtoad, Target, new Implementations.KnockbackBuff(-0.01f, 3f, -0.1f));

                yield return tongueMover.ArrivalTime;
                tongueEnd.Destroy();

                if (ValidTarget())
                {
                    _SetHiddenAttribute(Target, true);

                    if (!waitAttackEnd.TimedOut)
                        yield return waitAttackEnd;

                    bigtoad.PlayActionAnimation(110636); // disgest ani, 5 seconds
                    for (int n = 0; n < 5 && ValidTarget(); ++n)
                    {
                        WeaponDamage(Target, 0.039f, DamageType.Poison);
                        yield return WaitSeconds(1f);
                    }

                    if (ValidTarget())
                    {
                        _SetHiddenAttribute(Target, false);

                        bigtoad.PlayActionAnimation(110637); // regurgitate ani
                        this.World.BuffManager.AddBuff(bigtoad, Target, new Implementations.KnockbackBuff(36f));
                        Target.PlayEffectGroup(18281); // actual regurgitate efg isn't working so use generic acid effect
                        yield return WaitSeconds(0.9f);
                    }
                }

                bigtoad.PlayActionAnimation(110764); // despawn ani
                yield return WaitSeconds(0.7f);
                bigtoad.Destroy();
            }
        }

        private void _SetHiddenAttribute(Actor actor, bool active)
        {
            actor.Attributes[GameAttribute.Hidden] = active;
            actor.Attributes.BroadcastChangedIfRevealed();
        }
    }
    #endregion

    //TODO:Rune_E
    #region GraspOfTheDead
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.Support.GraspOfTheDead)]
    public class GraspOfTheDead : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            UsePrimaryResource(ScriptFormula(5));
            StartDefaultCooldown();

            if (Rune_B > 0)
            {
                for (int i = 0; i < 4; ++i)
                {
                    var Target = GetEnemiesInRadius(TargetPosition, ScriptFormula(14)).GetClosestTo(TargetPosition);
                    if (Target != null)
                    {
                        SpawnEffect(105955, Target.Position);
                        WeaponDamage(GetEnemiesInRadius(Target.Position, ScriptFormula(15)), ScriptFormula(10), DamageType.Holy);
                        yield return WaitSeconds(ScriptFormula(13));
                    }
                }
            }
            else
            {
                var Ground = SpawnEffect(RuneSelect(69308, 105953, -1, 105956, 105957, 105958), TargetPosition, 0, WaitSeconds(ScriptFormula(8)));
                Ground.UpdateDelay = 0.5f;
                Ground.OnUpdate = () =>
                    {
                        foreach (Actor enemy in GetEnemiesInRadius(TargetPosition, ScriptFormula(3)).Actors)
                        {
                            if (!AddBuff(enemy, new DebuffSlowed(ScriptFormula(19), WaitSeconds(ScriptFormula(8)))))
                            {
                                AddBuff(enemy, new DebuffSlowed(ScriptFormula(19), WaitSeconds(ScriptFormula(8))));
                            }
                            if (!AddBuff(enemy, new DamageGroundDebuff()))
                            {
                                AddBuff(enemy, new DamageGroundDebuff());
                            }
                        }
                    };
            }
            yield break;
        }
        [ImplementsPowerBuff(0)]
        class DamageGroundDebuff : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;

            public override void Init()
            {
                base.Init();
                Timeout = WaitSeconds(ScriptFormula(8));
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                if (_damageTimer == null || _damageTimer.TimedOut)
                {
                    _damageTimer = WaitSeconds(_damageRate);

                    AttackPayload attack = new AttackPayload(this);
                    attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
                    attack.Apply();
                    attack.OnDeath = (DeathPayload) =>
                        {
                            if (Rune_E > 0)
                            {
                                if (Rand.NextDouble() < ScriptFormula(21))
                                {
                                    //produce a health globe
                                }
                            }
                        };
                }

                return false;
            }
        }
    }
    #endregion

    //TODO:lots! :)
    #region Haunt
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.SpiritRealm.Haunt)]
    public class Haunt : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            //Need to check for all Haunt Buffs in this radius.
            //Max Haunt Check Radius(ScriptFormula(9)) -> 90f
            if (Target != null)
            {
                AddBuff(Target, new CheckHaunts());
            }
            //if the Target Dies before time is over, find a new target within search check radius (SF(10))
            yield break;
        }

        [ImplementsPowerBuff(0)]
        class CheckHaunts : PowerBuff
        {
            public override void Init()
            {
                MaxStackCount = (int)ScriptFormula(8);
            }
            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                _AddHaunt();

                return true;
            }
            public override bool Stack(Buff buff)
            {
                bool stacked = StackCount < MaxStackCount;

                base.Stack(buff);

                if (stacked)
                    _AddHaunt();

                return true;
            }
            public override void Remove()
            {
                base.Remove();
            }
            private void _AddHaunt()
            {
                AddBuff(Target, new Haunt1());
            }
        }

        [ImplementsPowerBuff(0)]
        class Haunt1 : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;

            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(1));
            }
            public override bool Update()
            {
                if (base.Update())
                    return true;
                if (_damageTimer == null || _damageTimer.TimedOut)
                {
                    _damageTimer = WaitSeconds(_damageRate);

                    if (Rune_D > 0)
                    {
                        //GeneratePrimaryResource(ScriptFormula(3));
                    }

                    AttackPayload attack = new AttackPayload(this);
                    attack.SetSingleTarget(Target);
                    attack.AddWeaponDamage((ScriptFormula(0) / ScriptFormula(1)), DamageType.Holy);
                    attack.OnHit = hit =>
                        {
                            if (Rune_A > 0)
                            {
                                //45% of damage healed back to user
                            } 
                            if (Rune_C > 0)
                            {
                                //DebuffSlowed Target 
                            }
                        };
                    attack.Apply();
                }
                return false;
            }
        }
        //Rune_B
        [ImplementsPowerBuff(1)]
        class HauntLinger : PowerBuff
        {
            const float _damageRate = 1.25f;
            TickTimer _damageTimer = null;

            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(4));
            }

            //Search 
            public override bool Update()
            {
                if (base.Update())
                    return true;
                if (_damageTimer == null || _damageTimer.TimedOut)
                {
                    _damageTimer = WaitSeconds(_damageRate);

                    /*if(GetEnemiesInRadius(Wherever Monster Died, ScriptFormula(10)).Actors.Count > 0)
                    { AddBuff(GetEnemiesInRadius(Monster's Death, ScriptFormula(10)).GetClosestTo(Monster's Death), new Haunt1())) }
                     */
                }
                return false;
            }
        }
    }
    #endregion

    //TODO:Rune_D
    #region ZombieCharger
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.ZombieCharger)]
    public class WitchDoctorZombieCharger : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            if (Rune_A > 0)
            {
                Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, -20f);
                Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, 10f, 2);

                    var BearProj1 = new Projectile(this, RuneSelect(74056, 105501, 105543, 105463, 105969, 105812), inFrontOfUser);
                    BearProj1.Position.Z -= 3f;
                    BearProj1.OnCollision = (hit) =>
                    {
                        WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
                    };
                    BearProj1.Launch(projDestinations[0], ScriptFormula(19));

                    yield return WaitSeconds(0.5f);
                    var BearProj2 = new Projectile(this, RuneSelect(74056, 105501, 105543, 105463, 105969, 105812), inFrontOfUser);
                    BearProj2.Position.Z -= 3f;
                    BearProj2.OnCollision = (hit) =>
                    {
                        WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
                    };
                    BearProj2.Launch(projDestinations[1], ScriptFormula(19));

                    yield return WaitSeconds(0.5f);
                    var BearProj3 = new Projectile(this, RuneSelect(74056, 105501, 105543, 105463, 105969, 105812), inFrontOfUser);
                    BearProj3.Position.Z -= 3f;
                    BearProj3.OnCollision = (hit) =>
                    {
                        WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
                    };
                    BearProj3.Launch(projDestinations[0], ScriptFormula(19));
            }
            else if (Rune_B > 0)
            {
                Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 3f);
                Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, ScriptFormula(10), (int)ScriptFormula(3));

                for (int i = 1; i < projDestinations.Length; i++)
                {
                    var multiproj = new Projectile(this, RuneSelect(74056, 105501, 105543, 105463, 105969, 105812), inFrontOfUser);
                    multiproj.Position.Z -= 3f;
                    multiproj.OnCollision = (hit) =>
                    {
                        WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
                    }; 
                    multiproj.Launch(projDestinations[i], ScriptFormula(1));
                }
            }
            else if (Rune_D > 0)
            {
                Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 3f);
                var proj = new Projectile(this, RuneSelect(74056, 105501, 105543, 105463, 105969, 105812), inFrontOfUser);
                proj.Position.Z -= 3f;
                proj.OnCollision = (hit) =>
                {
                    AttackPayload attack = new AttackPayload(this);
                    attack.SetSingleTarget(hit);
                    attack.AddWeaponDamage(ScriptFormula(4), DamageType.Poison);
                    attack.Apply();
                    attack.OnDeath = DeathPayload =>
                        {
                            //TODO:need this stuff.
                            //zombie new distance (SF(13))
                            //zombie speed (SF(14))
                            //New zombie search range (SF(15))
                            //new zombie chance (SF18)
                            //max new zombie per projectile (SF24)
                            //damage scalar -> SF31
                            //damage reduction per zombie -> SF30
                        };
                };
                proj.Launch(TargetPosition, ScriptFormula(1));
            }
            else if (Rune_E > 0)
            {
                Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 3f);
                var proj = new Projectile(this, RuneSelect(74056, 105501, 105543, 105463, 105969, 105812), inFrontOfUser); 
                proj.Position.Z -= 3f;
                proj.OnCollision = (hit) =>
                {
                    WeaponDamage(GetEnemiesInRadius(hit.Position, ScriptFormula(11)), ScriptFormula(4), DamageType.Fire);
                };
                proj.Launch(TargetPosition, ScriptFormula(7));
            }
            else
            {
                Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 3f);
                var proj = new Projectile(this, RuneSelect(74056, 105501, 105543, 105463, 105969, 105812), inFrontOfUser);
                proj.Position.Z -= 3f;
                proj.OnCollision = (hit) =>
                {
                    WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);

                    if (Rune_C > 0)
                    {
                        var Puddle = SpawnEffect(105502, hit.Position, 0, WaitSeconds(ScriptFormula(17)));
                        Puddle.UpdateDelay = 1f;
                        Puddle.OnUpdate = () =>
                            {
                                AttackPayload attack = new AttackPayload(this);
                                attack.Targets = GetEnemiesInRadius(hit.Position, ScriptFormula(8));
                                attack.AddWeaponDamage(ScriptFormula(6), DamageType.Poison);
                                attack.Apply();
                            };
                    }
                };
                proj.Launch(TargetPosition, ScriptFormula(1));
            }

            yield break;
        }
    }
    #endregion

    //Pet Class?
    #region Hex
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.Support.Hex)]
    public class Hex : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    //Jarthrow complete, unknown how to do spiders.
    #region CorpseSpiders
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.CorpseSpiders)]
    public class CorpseSpiders : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 5f);
            var proj = new Projectile(this, RuneSelect(106504, 215811, 215815, 215816, 215814, 215813), User.Position);
            proj.Position.Z += 5f;
            proj.LaunchArc(inFrontOfUser, 5f, -0.07f);
            yield return WaitSeconds(0.4f);
            proj.Destroy();
            SpawnEffect(110714, inFrontOfUser);

            //the rest of this is spiders, which are pets i presume?
            yield return WaitSeconds(0.05f);
            SpawnEffect(107031, inFrontOfUser);

            yield break;
        }
    }
    #endregion

    #region Horrify
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.SpiritRealm.Horrify)]
    public class Horrify : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(ScriptFormula(14));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            AddBuff(User, new CastEffect());
            if (Rune_A > 0)
            {
                AddBuff(User, new CrimsonBuff());
            } 
            if (Rune_E > 0)
            {
                AddBuff(User, new SprintBuff());
            }
            foreach (Actor enemy in GetEnemiesInRadius(User.Position, ScriptFormula(2)).Actors)
            {
                AddBuff(enemy, new DebuffFeared(WaitSeconds(ScriptFormula(3))));
                if (Rune_D > 0)
                {
                    GeneratePrimaryResource(ScriptFormula(8));
                }
            }
            yield break;
        }
        [ImplementsPowerBuff(0)]
        class CastEffect : PowerBuff
        {
            //switch.efg
            public override void Init()
            {
                Timeout = WaitSeconds(2f);
            }
        }
        [ImplementsPowerBuff(1)]
        class SprintBuff : PowerBuff
        {
            //spring.etf alabaster
            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(10));
            }
            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += ScriptFormula(9);
                Target.Attributes.BroadcastChangedIfRevealed();

                return true;
            }
            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= ScriptFormula(9);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
        [ImplementsPowerBuff(2)]
        class CrimsonBuff : PowerBuff
        {
            //crimson buff
            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(12));
            }
            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(5);
                Target.Attributes.BroadcastChangedIfRevealed();

                return true;
            }
            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(5);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
    }
    #endregion

    #region Firebats
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.Firebats)]
    public class Firebats : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    #region Firebomb
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.Firebomb)]
    public class Firebomb : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    #region SpiritWalk
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.SpiritRealm.SpiritWalk)]
    public class SpiritWalk : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    #region SoulHarvest
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.SpiritRealm.SoulHarvest)]
    public class SoulHarvest : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    #region Sacrifice
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.Support.Sacrifice)]
    public class Sacrifice : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    //Hacky by MDZ -> Pet Class
    #region Gargantuan
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.Support.Gargantuan)]
    public class Gargantuan : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            // HACK: made up garggy spell :)

            Vector3D inFrontOfTarget = PowerMath.TranslateDirection2D(TargetPosition, User.Position, TargetPosition, 11f);
            inFrontOfTarget.Z = User.Position.Z;
            var garggy = SpawnEffect(122305, inFrontOfTarget, TargetPosition, WaitInfinite());

            garggy.PlayActionAnimation(155988);

            yield return WaitSeconds(2f);

            for (int n = 0; n < 3; ++n)
            {
                garggy.PlayActionAnimation(211382);

                yield return WaitSeconds(0.5f);

                SpawnEffect(192210, TargetPosition);
                WeaponDamage(GetEnemiesInRadius(TargetPosition, 12f), 1.00f, DamageType.Poison);

                yield return WaitSeconds(0.4f);
            }

            garggy.PlayActionAnimation(155536); //mwhaha
            yield return WaitSeconds(1.5f);

            garggy.PlayActionAnimation(171024);
            yield return WaitSeconds(2f);

            garggy.Destroy();

            yield break;
        }
    }
    #endregion

    #region LocustSwarm
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.LocustSwarm)]
    public class LocustSwarm : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    #region SpiritBarrage
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.SpiritRealm.SpiritBarrage)]
    public class SpiritBarrage : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    #region AcidCloud
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.AcidCloud)]
    public class AcidCloud : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    #region MassConfusion
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.SpiritRealm.MassConfusion)]
    public class MassConfusion : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    //Pet Class?
    #region BigBadVoodoo
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.Support.BigBadVoodoo)]
    public class BigBadVoodoo : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    //Pet Class
    #region WallOfZombies
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.PhysicalRealm.WallOfZombies)]
    public class WallOfZombies : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion

    //Pet Class
    #region FetishArmy
    [ImplementsPowerSNO(Skills.Skills.WitchDoctor.Support.FetishArmy)]
    public class FetishArmy : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {

            yield break;
        }
    }
    #endregion
}
