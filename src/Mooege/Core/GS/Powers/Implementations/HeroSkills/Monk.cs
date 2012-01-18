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

using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mooege.Core.GS.Skills;
using Mooege.Net.GS.Message.Fields;
using Mooege.Core.GS.Actors;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Definitions.Actor;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Players;
using Mooege.Core.GS.Ticker;
using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Core.GS.Powers.Payloads;
using Mooege.Net.GS.Message.Definitions.ACD;

namespace Mooege.Core.GS.Powers.Implementations
{
    //TODO Runes
    #region DeadlyReach
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritGenerator.DeadlyReach)]
    public class MonkDeadlyReach : ComboSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            float reachLength;
            float reachThickness;

            switch(ComboIndex)
            {
                case 0:
                    reachLength = 13;
                    reachThickness = 3f;
                    break;
                case 1:
                    reachLength = 14;
                    reachThickness = 4.5f;
                    break;
                case 2:
                    reachLength = 18;
                    reachThickness = 4.5f;
                    break;
                default:
                    yield break;
            }

            bool hitAnything = false;
            AttackPayload attack = new AttackPayload(this);
            attack.Targets = GetEnemiesInBeamDirection(User.Position, TargetPosition, reachLength, reachThickness);
            attack.AddWeaponDamage(1.20f, DamageType.Physical);
            attack.OnHit = hitPayload => { hitAnything = true; };
            attack.Apply();

            if (hitAnything)
                GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));

            yield break;
        }
    }
    #endregion

    //TODO Runes
    #region FistsOfThunder
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritGenerator.FistsOfThunder)]
    public class MonkFistsOfThunder : ComboSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            switch (TargetMessage.Field5)
            {
                case 0:
                case 1:
                    MeleeStageHit();
                    break;
                case 2:
                    //AddBuff(User, new ComboStage3Buff());

                    // put target position a little bit in front of the monk. represents the lightning ball
                    TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition,
                                        User.Position, 8f);

                    bool hitAnything = false;
                    AttackPayload attack = new AttackPayload(this);
                    attack.Targets = GetEnemiesInRadius(TargetPosition, 7f);
                    attack.AddWeaponDamage(1.20f, DamageType.Lightning);
                    attack.OnHit = hitPayload => {
                        hitAnything = true;
                        Knockback(hitPayload.Target, 12f);
                    };
                    attack.Apply();

                    if (hitAnything)
                        GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));

                    break;
            }

            yield break;
        }

        private void MeleeStageHit()
        {
            AttackPayload attack = new AttackPayload(this);
            attack.Targets = GetBestMeleeEnemy();
            attack.AddWeaponDamage(1.20f, DamageType.Lightning);
            attack.OnHit = hitPayload =>
            {
                GeneratePrimaryResource(6f);
            };
            attack.Apply();
        }

        [ImplementsPowerBuff(7)]
        class ComboStage3Buff : PowerBuff
        {
            public override void Init()
            {
                Timeout = WaitSeconds(0.5f / EvalTag(PowerKeys.ComboAttackSpeed3));
            }
        }
    }
    #endregion

    //TODO Runes
    #region SevenSidedStrike
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.SevenSidedStrike)]
    public class MonkSevenSidedStrike : Skill
    {
        //Max Teleport Distance added in last patch 8101.
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
            
            var groundEffect = SpawnProxy(TargetPosition, WaitInfinite());
            groundEffect.PlayEffectGroup(145041);

            for (int n = 0; n < 7; ++n)
            {
                IList<Actor> nearby = GetEnemiesInRadius(TargetPosition, 25f).Actors;
                if (nearby.Count > 0)
                {
                    var target = nearby[Rand.Next(0, nearby.Count)];

                    SpawnEffect(99063, target.Position, -1);                    
                    yield return WaitSeconds(0.2f);

                    if (Rune_E > 0)
                    {
                        target.PlayEffectGroup(99098);
                        var splashTargets = GetEnemiesInRadius(target.Position, 5f);
                        splashTargets.Actors.Remove(target); // don't hit target with splash
                        WeaponDamage(splashTargets, 0.31f, DamageType.Holy);
                    }

                    WeaponDamage(target, 1.15f, DamageType.Physical);
                }
                else
                {
                    break;
                }
            }

            groundEffect.Destroy();
        }
    }
    #endregion

    //TODO Runes
    #region CripplingWave
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritGenerator.CripplingWave)]
    public class MonkCripplingWave : ComboSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            int effectSNO;
            switch (ComboIndex)
            {
                case 0:
                    effectSNO = 18987;
                    break;
                case 1:
                    effectSNO = 18988;
                    break;
                case 2:
                    effectSNO = 96519;
                    break;
                default:
                    yield break;
            }

            User.PlayEffectGroup(effectSNO);

            bool hitAnything = false;
            AttackPayload attack = new AttackPayload(this);
            if (ComboIndex != 2)
                attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(5), ScriptFormula(6));
            else
                attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(8));

            attack.AddWeaponDamage(1.10f, DamageType.Physical);
            attack.OnHit = hitPayload => { hitAnything = true; };
            attack.Apply();

            if (hitAnything)
                GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));

            yield break;
        }
    }
    #endregion

    //TODO Runes
    #region ExplodingPalm
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritGenerator.ExplodingPalm)]
    public class MonkExplodingPalm : ComboSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            AttackPayload attack = new AttackPayload(this);
            switch (ComboIndex)
            {
                case 0:
                case 1:
                    attack.Targets = GetBestMeleeEnemy();
                    attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
                    if (Rune_C > 0)
                        attack.AddBuffOnHit<RuneCDebuff>();
                    break;

                case 2:
                    if (Rune_B > 0)
                    {
                        attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(19), ScriptFormula(20));
                        int maxTargets = (int)ScriptFormula(18);
                        if (maxTargets < attack.Targets.Actors.Count)
                            attack.Targets.Actors.RemoveRange(maxTargets, attack.Targets.Actors.Count - maxTargets);
                    }
                    else
                    {
                        attack.Targets = GetBestMeleeEnemy();
                    }

                    attack.AutomaticHitEffects = false;
                    attack.AddBuffOnHit<MainDebuff>();
                    break;

                default:
                    yield break;
            }
            bool hitAnything = false;
            attack.OnHit = hitPayload => { hitAnything = true; };
            attack.Apply();

            if (hitAnything)
                GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));
            yield break;
        }

        [ImplementsPowerBuff(0)]
        class MainDebuff : PowerBuff
        {
            const float _damageRate = 1.0f;
            TickTimer _damageTimer = null;

            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(3));
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                Target.Attributes[GameAttribute.Bleeding] = true;
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();

                Target.Attributes[GameAttribute.Bleeding] = false;
                Target.Attributes.BroadcastChangedIfRevealed();
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                if (_damageTimer == null || _damageTimer.TimedOut)
                {
                    _damageTimer = WaitSeconds(_damageRate);

                    AttackPayload attack = new AttackPayload(this);
                    attack.SetSingleTarget(Target);
                    attack.AddWeaponDamage(ScriptFormula(6) * _damageRate, DamageType.Physical);
                    attack.AutomaticHitEffects = false;
                    attack.Apply();
                }
                return false;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is DeathPayload)
                {
                    AttackPayload attack = new AttackPayload(this);
                    attack.Targets = GetEnemiesInRadius(Target.Position, ScriptFormula(11));
                    attack.AddDamage(ScriptFormula(9) * Target.Attributes[GameAttribute.Hitpoints_Max_Total],
                                     ScriptFormula(10), DamageType.Physical);
                    if (Rune_D > 0)
                    {
                        attack.OnHit = (hitPayload) =>
                        {
                            GeneratePrimaryResource(ScriptFormula(14));
                        };
                    }
                    attack.Apply();

                    SpawnProxy(Target.Position).PlayEffectGroup(18991);
                }
            }
        }

        [ImplementsPowerBuff(1, true)]
        class RuneCDebuff : PowerBuff
        {
            public override void Init()
            {
                base.Init();
                Timeout = WaitSeconds(ScriptFormula(25));
                MaxStackCount = (int)ScriptFormula(17);
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                _AddAmp();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Amplify_Damage_Percent] -= StackCount * ScriptFormula(15);
                Target.Attributes.BroadcastChangedIfRevealed();
            }

            public override bool Stack(Buff buff)
            {
                bool stacked = StackCount < MaxStackCount;

                base.Stack(buff);

                if (stacked)
                    _AddAmp();

                return true;
            }

            private void _AddAmp()
            {
                Target.Attributes[GameAttribute.Amplify_Damage_Percent] += ScriptFormula(15);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
    }
    #endregion

    //TODO Runes
    #region SweepingWind
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritGenerator.SweepingWind)]
    public class MonkSweepingWind : ComboSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            AttackPayload attack = new AttackPayload(this);
            attack.Targets = GetBestMeleeEnemy();
            attack.AddWeaponDamage(1.00f, DamageType.Physical);
            attack.Apply();

            yield break;
        }
    }
    #endregion

    //TODO Basic Skill
    #region WayOfTheHundredFists
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritGenerator.WayOfTheHundredFists)]
    public class MonkWayOfTheHundredFists : ComboSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            yield break;
        }

        public override float GetContactDelay()
        {
            // no contact delay for hundred fists
            return 0f;
        }
    }
    #endregion

    //TODO:Runes
    #region DashingStrike
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.DashingStrike)]
    public class MonkDashingStrike : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            // dashing strike never specifies the target's id so we just search for the closest target
            Target = GetEnemiesInRadius(TargetPosition, ScriptFormula(0)).GetClosestTo(TargetPosition);

            if (Target != null)
            {
                // put dash destination just beyond target
                TargetPosition = PowerMath.TranslateDirection2D(User.Position, Target.Position, Target.Position, 7f);
            }
            else
            {
                // if no target, always dash fixed amount
                TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 13f);
            }

            var dashBuff = new DashMoverBuff(TargetPosition);
            AddBuff(User, dashBuff);
            yield return dashBuff.Timeout;

            if (Target != null && Target.World != null) // target could've died or left world
            {
                User.TranslateFacing(Target.Position, true);
                yield return WaitSeconds(0.1f);
                User.PlayEffectGroup(113720);
                WeaponDamage(Target, 1.60f, DamageType.Physical);
            }
        }

        [ImplementsPowerBuff(0)]
        class DashMoverBuff : PowerBuff
        {
            private Vector3D _destination;
            private ActorMover _mover;

            public DashMoverBuff(Vector3D destination)
            {
                _destination = destination;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                // dash speed seems to always be actor speed * 10
                float speed = Target.Attributes[GameAttribute.Running_Rate_Total] * ScriptFormula(5);

                Target.TranslateFacing(_destination, true);
                _mover = new ActorMover(Target);
                _mover.Move(_destination, speed, new ACDTranslateNormalMessage
                {
                    TurnImmediately = true,
                    Field5 = 0x9206, // alt: 0x920e, not sure what this param is for.
                    AnimationTag = 69808, // dashing strike attack animation
                    Field7 = 6, // ticks to wait before playing attack animation
                });

                // make sure buff timeout is big enough otherwise the client will sometimes ignore the visual effects.
                TickTimer minDashWait = WaitSeconds(0.15f);
                Timeout = minDashWait.TimeoutTick > _mover.ArrivalTime.TimeoutTick ? minDashWait : _mover.ArrivalTime;

                Target.Attributes[GameAttribute.Hidden] = true;
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Hidden] = false;
                Target.Attributes.BroadcastChangedIfRevealed();
            }

            public override bool Update()
            {
                _mover.Update();
                return base.Update();
            }
        }
    }
    #endregion

    //TODO Runes
    #region MantraOfEvasion
    [ImplementsPowerSNO(Skills.Skills.Monk.Mantras.MantraOfEvasion)]
    public class MonkMantraOfEvasion : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            //No more cooldown
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            AddBuff(User, new CasterBuff());
            AddBuff(User, new CastBonusBuff());
            foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(0)).Actors)
                AddBuff(User, new CastBonusBuff());

            yield break;
        }

        class BaseDodgeBuff : PowerBuff
        {
            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                Target.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(2);
                Target.Attributes.BroadcastChangedIfRevealed();

                return true;
            }

            public override void Remove()
            {
                base.Remove();

                Target.Attributes[GameAttribute.Dodge_Chance_Bonus] -= ScriptFormula(2);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }

        class BaseFullEffectsBuff : BaseDodgeBuff
        {
            // TODO: rune buff effects and such will go here

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void Remove()
            {
                base.Remove();
            }
        }

        [ImplementsPowerBuff(0)]
        class CasterBuff : BaseFullEffectsBuff
        {
            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(1));
            }

            public override void Remove()
            {
                base.Remove();

                // aura fade effect
                Target.PlayEffectGroup(199677);
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                foreach (Actor ally in GetAlliesInRadius(Target.Position, ScriptFormula(0)).Actors)
                    AddBuff(ally, new AllyBuff());

                return false;
            }
        }

        [ImplementsPowerBuff(7)]
        class CastBonusBuff : BaseDodgeBuff
        {
            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(13));
            }
        }

        [ImplementsPowerBuff(1)]
        class AllyBuff : BaseFullEffectsBuff
        {
            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(11));
            }
        }
    }
    #endregion

    //TODO RUnes
    #region BlindingFlash
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.BlindingFlash)]
    public class MonkBlindingFlash : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            AttackPayload attack = new AttackPayload(this);
            attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(1));
            attack.OnHit = (hit) =>
            {
                TickTimer waitBuffEnd = WaitSeconds(ScriptFormula(0));

                // add main effect buff only if blind debuff took effect
                if (AddBuff(hit.Target, new DebuffBlind(waitBuffEnd)))
                    AddBuff(hit.Target, new MainEffectBuff(waitBuffEnd));
            };

            attack.Apply();

            yield break;
        }

        [ImplementsPowerBuff(5)]
        class MainEffectBuff : PowerBuff
        {
            public MainEffectBuff(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                Target.Attributes[GameAttribute.Hit_Chance] -= ScriptFormula(8);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Hit_Chance] += ScriptFormula(8);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
    }
    #endregion

    //Complete
    #region LashingTailKick
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.LashingTailKick)]
    public class LashingTailKick : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            if (Rune_A > 0 || Rune_D > 0)
            {
                AttackPayload attack = new AttackPayload(this);
                attack.Targets = GetEnemiesInRadius(User.Position, 10f);
                attack.AddWeaponDamage(ScriptFormula(0), Rune_A > 0 ? DamageType.Fire : DamageType.Physical);
                attack.OnHit = HitPayload =>
                    {
                        Knockback(HitPayload.Target, ScriptFormula(3), ScriptFormula(4), ScriptFormula(5));
                        if (Rune_D > 0)
                        {
                            AddBuff(HitPayload.Target, new DebuffSlowed(0.6f, WaitSeconds(ScriptFormula(15))));
                        }
                    };
                attack.Apply();
            }
            else if (Rune_B > 0)
            {
                var proj = new Projectile(this, 136893, User.Position);
                //max distance sf(16)
                //proj.Position.Z += 5f;  // fix height
                proj.OnCollision = (hit) =>
                {
                    hit.PlayEffectGroup(143439);
                    WeaponDamage(hit, ScriptFormula(0), DamageType.Fire);
                };
                proj.Launch(TargetPosition, 1f);
            }
            else if (Rune_C > 0)
            {
                SpawnEffect(136925, TargetPosition);
                AttackPayload attack = new AttackPayload(this);
                attack.Targets = GetEnemiesInRadius(TargetPosition, ScriptFormula(12));
                attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
                attack.OnHit = HitPayload =>
                    {
                        AddBuff(HitPayload.Target, new DebuffSlowed(ScriptFormula(18), WaitSeconds(ScriptFormula(21))));
                    };
                attack.Apply();
            }
            else
            {
                AttackPayload attack = new AttackPayload(this);
                attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(6), ScriptFormula(7));
                attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
                attack.OnHit = HitPayload =>
                    {
                        if (Rune_E > 0)
                        {
                            AddBuff(HitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(8))));
                        }
                        else
                            Knockback(HitPayload.Target, ScriptFormula(3), ScriptFormula(4), ScriptFormula(5));
                    };
                attack.Apply();
            }

            yield break;
        }
    }
    #endregion

    //Close to complete for base skill. TODO:Runes
    #region TempestRush
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.TempestRush)]
    public class TempestRush : ChanneledSkill
    {
        private Actor _target = null;

        public override void OnChannelOpen()
        {
            EffectsPerSecond = 0.25f;
            UsePrimaryResource(ScriptFormula(16));
            //User.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += ScriptFormula(14);
            if (Rune_C > 0)
            {
                //damage reduction
            }
        }

        public override void OnChannelClose()
        {
            if (_target != null)
                _target.Destroy();
            //User.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= ScriptFormula(14);
            if (Rune_C > 0)
            {
                //damage reduction
            }
        }

        public override void OnChannelUpdated()
        {
            UsePrimaryResource(ScriptFormula(16));
            User.TranslateFacing(TargetPosition);
            // client updates target actor position
        }

        public override IEnumerable<TickTimer> Main()
        {
            AttackPayload attack = new AttackPayload(this);
            //TODO: damage offset from ground?? where does this go..
            attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(2), ScriptFormula(1));
            attack.AddWeaponDamage(ScriptFormula(4), DamageType.Physical);
            attack.OnHit = HitPayload =>
                {
                    Knockback(HitPayload.Target, ScriptFormula(6), ScriptFormula(7), ScriptFormula(8));
                    AddBuff(HitPayload.Target, new DebuffSlowed(ScriptFormula(9), WaitSeconds(ScriptFormula(10))));
                    if (Rune_A > 0)
                    {
                    }
                };
            attack.Apply();

            yield return WaitSeconds(ScriptFormula(1));
        }
    }
    #endregion

    //TODO:Actors need adjustments.
    #region WaveOfLight
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.WaveOfLight)]
    public class WaveOfLight : Skill
    {
        public override int GetCastEffectSNO()
        {
            return base.GetCastEffectSNO();
        }
        public override int GetContactEffectSNO()
        {
            return base.GetContactEffectSNO();
        }
        //TODO: Change actor animations to be Proxy first, then Bell, (then shattered then destroyed?)
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            //projectile distance (50)
            if (Rune_B > 0)
            {
                Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, 45f, (int)ScriptFormula(25));

                yield return WaitSeconds(0.35f);

                for (int i = 0; i < projDestinations.Length; i++)
                {
                    var proj = new Projectile(this, 172310, User.Position);
                    proj.Launch(projDestinations[i], ScriptFormula(22));
                    proj.OnCollision = (hit) =>
                    {
                        hit.PlayEffectGroup(145443);
                        proj.Destroy();
                        WeaponDamage(hit, ScriptFormula(23), DamageType.Holy);
                    };
                }
            }
            else if (Rune_C > 0)
            {
                //add in pillar [temple.efg and hit_hp.acr and pillar_a.acr?]
                Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, ScriptFormula(8));
                
                yield return WaitSeconds(0.35f);
                
                AttackPayload attack = new AttackPayload(this);
                attack.Targets = GetEnemiesInRadius(inFrontOfUser, 20f);
                attack.AddWeaponDamage(ScriptFormula(17), DamageType.Holy);
                attack.OnHit = hit =>
                    {
                        Knockback(hit.Target, ScriptFormula(5), ScriptFormula(6), ScriptFormula(7));
                            AddBuff(hit.Target, new DOTbuff(WaitSeconds(ScriptFormula(9))));
                    };

            }
            else
            {
                Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, ScriptFormula(8));
                AttackPayload attack = new AttackPayload(this);
                attack.Targets = GetEnemiesInRadius(inFrontOfUser, 10f);
                attack.AddWeaponDamage(ScriptFormula(14), DamageType.Holy);
                attack.OnHit = hit =>
                {
                    if (hit.IsCriticalHit)
                    {
                        AddBuff(hit.Target, new DebuffStunned(WaitSeconds(ScriptFormula(39))));
                    }
                };

                yield return WaitSeconds(0.35f);
                var proj = new Projectile(this, 6441, User.Position);
                proj.Position.Z += 5f;  // fix height
                proj.OnCollision = (hit) =>
                {
                    hit.PlayEffectGroup(144079);
                    WeaponDamage(hit, ScriptFormula(3), DamageType.Holy);
                    Knockback(User, ScriptFormula(15), ScriptFormula(18), ScriptFormula(20));
                };
                proj.Launch(TargetPosition, ScriptFormula(2));
            }
            yield break;
        }
        [ImplementsPowerBuff(4)]
        class DOTbuff : PowerBuff
        {
            const float _damageRate = 0.5f;
            TickTimer _damageTimer = null;

            public DOTbuff(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                if (_damageTimer == null || _damageTimer.TimedOut)
                {
                    _damageTimer = WaitSeconds(_damageRate);

                    WeaponDamage(GetEnemiesInRadius(Target.Position, 15f), ScriptFormula(31), DamageType.Holy);
                }

                return false;
            }
        }
    }
    #endregion

    //TODO:Healing
    #region BreathOfHeaven
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.BreathOfHeaven)]
    public class BreathOfHeaven : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            AttackPayload attack = new AttackPayload(this);
            attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(4));
            attack.AddWeaponDamage(ScriptFormula(0), DamageType.Holy);
            attack.OnHit = hit =>
                {
                    if (Rune_C > 0)
                    {
                        AddBuff(hit.Target, new FireDamageBuff(WaitSeconds(ScriptFormula(8))));
                    }
                    if (Rune_E > 0)
                    {
                        AddBuff(hit.Target, new DebuffFeared(WaitSeconds(ScriptFormula(13))));
                        AddBuff(hit.Target, new FearBuff());
                    }
                };
            if (Rune_D > 0)
            {
                AddBuff(User, new SpiritBuff(WaitSeconds(ScriptFormula(11))));
            }
            //Heals self for Heal Min and Heal Delta
            foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(4)).Actors)
            {
                //heal ally for Heal Min and Heal Delta
            }
            yield break;
        }
        [ImplementsPowerBuff(0)]
        class FireDamageBuff : PowerBuff
        {
            //firedamage
            public FireDamageBuff(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(9);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= ScriptFormula(9);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
        [ImplementsPowerBuff(1)]
        class SpiritBuff : PowerBuff
        {
            //spirit
            public SpiritBuff(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                Target.Attributes[GameAttribute.Resource_On_Hit] += ScriptFormula(12);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Resource_On_Hit] -= ScriptFormula(12);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
        [ImplementsPowerBuff(2)]
        class FearBuff : PowerBuff
        {
            //Fear
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;

            public override void Init()
            {
                base.Init();
                Timeout = WaitSeconds(ScriptFormula(13));
            }
            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                return true;
            }
            public override bool Update()
            {
                if (base.Update())
                    return true;

                if (_damageTimer == null || _damageTimer.TimedOut)
                {
                    _damageTimer = WaitSeconds(_damageRate);

                        AttackPayload attack = new AttackPayload(this);
                        attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(4));
                        attack.AddWeaponDamage(ScriptFormula(14), DamageType.Physical);
                        attack.Apply();
                }

                return false;
            }
            public override void Remove()
            {
                base.Remove();
            }
        }
    }
    #endregion

    //TODO:instant buff, after some seconds, better buff.
    //TODO:CastGroupBuff -> when Monk removes buff, remove from other members
    #region MantraOfRetribution
    [ImplementsPowerSNO(Skills.Skills.Monk.Mantras.MantraOfRetribution)]
    public class MantraOfRetribution : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            //No more cooldown since latest patch 8101
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            AddBuff(User, new CastEffect(WaitSeconds(ScriptFormula(5))));

            foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(0)).Actors)
            {
                AddBuff(ally, new CastGroupBuff(WaitSeconds(ScriptFormula(5))));
            }
            yield break;
        }
        [ImplementsPowerBuff(0)]
        class CastEffect : PowerBuff
        {
            //masterFX
            public CastEffect(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                if (Rune_B > 0)
                {
                    Target.Attributes[GameAttribute.Attacks_Per_Second_Bonus] += ScriptFormula(22);
                }
                Target.Attributes[GameAttribute.Thorns_Percent] += ScriptFormula(6);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {
                    if (Rune_C > 0)
                    {
                        if (Rand.NextDouble() < ScriptFormula(17))
                        {
                            AddBuff(payload.Context.Target, new DebuffStunned(WaitSeconds(ScriptFormula(18))));
                        }
                    }
                    if (Rune_D > 0)
                    {
                        //says there's a chance, but no formula
                        if (Rand.NextDouble() < 0.3f + (0.1f * Rune_D))
                        {
                            GeneratePrimaryResource(ScriptFormula(23));
                        }
                    }
                    if (Rune_E > 0)
                    {
                        if (Rand.NextDouble() < ScriptFormula(10))
                        {
                            WeaponDamage(GetEnemiesInRadius(payload.Context.Target.Position, ScriptFormula(20)), ScriptFormula(14), DamageType.Holy);
                        }
                    }
                }
            }

            public override void Remove()
            {
                base.Remove();
                if (Rune_B > 0)
                {
                    Target.Attributes[GameAttribute.Attacks_Per_Second_Bonus] -= ScriptFormula(22);
                }
                Target.Attributes[GameAttribute.Thorns_Percent] -= ScriptFormula(6);
                Target.Attributes.BroadcastChangedIfRevealed();
                
            }
        }
        [ImplementsPowerBuff(1)]
        class CastGroupBuff : PowerBuff
        {
            //grantee
            public CastGroupBuff(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                if (Rune_B > 0)
                {
                    Target.Attributes[GameAttribute.Attacks_Per_Second_Bonus] += ScriptFormula(22);
                }
                Target.Attributes[GameAttribute.Thorns_Percent] += ScriptFormula(6);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                if (Rune_B > 0)
                {
                    Target.Attributes[GameAttribute.Attacks_Per_Second_Bonus] -= ScriptFormula(22);
                }
                Target.Attributes[GameAttribute.Thorns_Percent] -= ScriptFormula(6);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
    }
    #endregion

    //TODO: instant buff, after some seconds, better buff.
    #region MantraOfHealing
    [ImplementsPowerSNO(Skills.Skills.Monk.Mantras.MantraOfHealing)]
    public class MantraOfHealing : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            //No more cooldown
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            AddBuff(User, new CastEffect(WaitSeconds(ScriptFormula(4) * 60f)));
            if (Rune_B > 0)
            {
                AddBuff(User, new HealingShield(WaitSeconds(ScriptFormula(9))));
            }
            foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(0)).Actors)
            {
                AddBuff(ally, new CastGroupBuff(WaitSeconds(ScriptFormula(4) * 60f)));
                if (Rune_B > 0)
                {
                    AddBuff(User, new HealingShield(WaitSeconds(ScriptFormula(9))));
                }
            }
            yield break;
        }
        [ImplementsPowerBuff(0)]
        class CastEffect : PowerBuff
        {
            //grantor
            public CastEffect(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                if (Rune_C > 0)
                {
                    Target.Attributes[GameAttribute.Vitality_Bonus_Percent] += ScriptFormula(6);
                }
                if (Rune_D > 0)
                {
                    Target.Attributes[GameAttribute.Resource_Regen_Per_Second] += ScriptFormula(7);
                }
                if (Rune_E > 0)
                {
                    Target.Attributes[GameAttribute.Resistance_All] += ScriptFormula(8);
                }
                Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += ScriptFormula(1);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                if (Rune_C > 0)
                {
                    Target.Attributes[GameAttribute.Vitality_Bonus_Percent] -= ScriptFormula(6);
                }
                if (Rune_D > 0)
                {
                    Target.Attributes[GameAttribute.Resource_Regen_Per_Second] -= ScriptFormula(7);
                }
                if (Rune_E > 0)
                {
                    Target.Attributes[GameAttribute.Resistance_All] -= ScriptFormula(8);
                }
                Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] -= ScriptFormula(1);
                Target.Attributes.BroadcastChangedIfRevealed();

            }
        }
        [ImplementsPowerBuff(2)]
        class CastGroupBuff : PowerBuff
        {
            //grantee
            public CastGroupBuff(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                if (Rune_C > 0)
                {
                    Target.Attributes[GameAttribute.Vitality_Bonus_Percent] += ScriptFormula(6);
                }
                if (Rune_D > 0)
                {
                    Target.Attributes[GameAttribute.Resource_Regen_Per_Second] += ScriptFormula(7);
                }
                if (Rune_E > 0)
                {
                    Target.Attributes[GameAttribute.Resistance_All] += ScriptFormula(8);
                }
                Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += ScriptFormula(1);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                if (Rune_C > 0)
                {
                    Target.Attributes[GameAttribute.Vitality_Bonus_Percent] -= ScriptFormula(6);
                }
                if (Rune_D > 0)
                {
                    Target.Attributes[GameAttribute.Resource_Regen_Per_Second] -= ScriptFormula(7);
                }
                if (Rune_E > 0)
                {
                    Target.Attributes[GameAttribute.Resistance_All] -= ScriptFormula(8);
                }
                Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] -= ScriptFormula(1);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
        [ImplementsPowerBuff(3)]
        class HealingShield : PowerBuff
        {
            //holyAuraRune_shield.efg
            public HealingShield(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                Target.Attributes[GameAttribute.Damage_Absorb_Percent] += ScriptFormula(5);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Damage_Absorb_Percent] -= ScriptFormula(5);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
    }
    #endregion

    //TODO:OnPayload for Rune_D?
    //TODO: maybe while(enemy is in radius), keep the buff?
    #region MantraOfConviction
    [ImplementsPowerSNO(Skills.Skills.Monk.Mantras.MantraOfConviction)]
    public class MantraOfConviction : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            //No more cooldown
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));


            AddBuff(User, new ConvictionAura(WaitSeconds(ScriptFormula(0) * 60f)));

            if (Rune_D > 0)
            {
                foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(0)).Actors)
                {
                    AddBuff(ally, new ReclamationAura(WaitSeconds(ScriptFormula(0) * 60f)));
                }
            }
            yield break;
        }
        [ImplementsPowerBuff(0)]
        class ConvictionAura : PowerBuff
        {
            //AuraBuff

            const float _damageRate = 0.5f;
            TickTimer _damageTimer = null;

            public ConvictionAura(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                return true;
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                if (_damageTimer == null || _damageTimer.TimedOut)
                {
                    _damageTimer = WaitSeconds(_damageRate);

                    foreach (Actor Enemy in GetEnemiesInRadius(User.Position, ScriptFormula(5)).Actors)
                    {
                        AddBuff(Enemy, new DeBuff());
                        if (Rune_C > 0)
                        {
                            AddBuff(Enemy, new DebuffSlowed(ScriptFormula(7), WaitSeconds(0.5f)));
                        }
                    }
                }

                return false;
            }
            public override void Remove()
            {
                base.Remove();
            }
        }
        [ImplementsPowerBuff(1)]
        class DeBuff : PowerBuff
        {
            //Debuff

            const float _damageRate = 1f;
            TickTimer _damageTimer = null;

            public override void Init()
            {
                Timeout = WaitSeconds(ScriptFormula(3));
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                if (Rune_E > 0)
                {
                    Target.Attributes[GameAttribute.Damage_Done_Reduction_Percent] += ScriptFormula(10);
                }
                Target.Attributes[GameAttribute.Defense_Reduction_Percent] += ScriptFormula(2);
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }
            public override bool Update()
            {
                if (base.Update())
                    return true;

                if (Rune_B > 0)
                {
                    if (_damageTimer == null || _damageTimer.TimedOut)
                    {
                        _damageTimer = WaitSeconds(_damageRate);

                        WeaponDamage(Target, ScriptFormula(12), DamageType.Holy);

                    }
                }

                return false;
            }
            public override void Remove()
            {
                base.Remove();
                if (Rune_E > 0)
                {
                    Target.Attributes[GameAttribute.Damage_Done_Reduction_Percent] -= ScriptFormula(10);
                }
                Target.Attributes[GameAttribute.Defense_Reduction_Percent] -= ScriptFormula(2);
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
        [ImplementsPowerBuff(0)]
        class ReclamationAura : PowerBuff
        {
            //AuraBuff
            public ReclamationAura(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                return true;
            }

            public override void Remove()
            {
                base.Remove();
            }
        }
    }
    #endregion

    //Rune_A -> Healing
    //Rune_E -> OnPayload HitTarget collect all damage that would have been done to you, Remove() of Buff, grab that number and get (SF(4)) of damage.
    //          Deal that much damage (Max of ___  % of your max life)
    #region Serenity
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.Serenity)]
    public class Serenity : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            AddBuff(User, new SerenityBuff(WaitSeconds(ScriptFormula(0))));

            if (Rune_D > 0)
            {
                foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(12)).Actors)
                {
                    AddBuff(ally, new SerenityAlliesBuff(WaitSeconds(ScriptFormula(11))));
                }
            }
            yield break;
        }
        [ImplementsPowerBuff(0)]
        class SerenityBuff : PowerBuff
        {
            public SerenityBuff(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                //If(Rune_A) -> Heal ScriptFormula(1)
                if (Rune_B > 0)
                {
                    Target.Attributes[GameAttribute.Projectile_Reflect_Chance] += ScriptFormula(2);
                    Target.Attributes[GameAttribute.Thorns_Percent] += ScriptFormula(2);
                }

                Target.Attributes[GameAttribute.Gethit_Immune] = true;
                Target.Attributes[GameAttribute.Immunity] = true;
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                if (Rune_B > 0)
                {
                    Target.Attributes[GameAttribute.Projectile_Reflect_Chance] -= ScriptFormula(2);
                    Target.Attributes[GameAttribute.Thorns_Percent] -= ScriptFormula(2);
                }
                Target.Attributes[GameAttribute.Gethit_Immune] = false;
                Target.Attributes[GameAttribute.Immunity] = false;
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
        [ImplementsPowerBuff(1)]
        class SerenityAlliesBuff : PowerBuff
        {
            public SerenityAlliesBuff(TickTimer timeout)
            {
                Timeout = timeout;
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                Target.Attributes[GameAttribute.Gethit_Immune] = true;
                Target.Attributes[GameAttribute.Immunity] = true;
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                Target.Attributes[GameAttribute.Gethit_Immune] = false;
                Target.Attributes[GameAttribute.Immunity] = false;
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
    }
    #endregion

    //TODO: Make sure enemies cannot come back into bubble
    #region InnerSanctuary
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.InnerSanctuary)]
    public class InnerSanctuary : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            var GroundSpot = SpawnProxy(User.Position);
            var Sanctuary = SpawnEffect(RuneSelect(98557, 98823, 149848, 142312, 98559, 142305), GroundSpot.Position, 0, WaitSeconds(ScriptFormula(0)));
            AttackPayload attack = new AttackPayload(this);
            attack.Targets = GetEnemiesInRadius(GroundSpot.Position, ScriptFormula(1));
            attack.OnHit = hit =>
                {
                    if (Rune_A > 0)
                    {
                        WeaponDamage(hit.Target, ScriptFormula(10), DamageType.Holy);
                    }
                    AddBuff(hit.Target, new DebuffFeared(WaitSeconds(ScriptFormula(2))));
                    Knockback(hit.Target, ScriptFormula(3), ScriptFormula(4), ScriptFormula(5));
                };
            attack.Apply();
            if (Rune_D > 0 || Rune_C > 0)
            {
                Sanctuary.UpdateDelay = 0.3f;
                Sanctuary.OnUpdate = () =>
                    {
                            AddBuff(User, new RegenBuff());
                        foreach (Actor ally in GetAlliesInRadius(GroundSpot.Position, ScriptFormula(1)).Actors)
                        {
                                AddBuff(User, new RegenAllyBuff());
                        }
                    };
            }
            //outer proxy
            SpawnEffect(RuneSelect(142719, 142851, 149849, 142788, 142737, 142845), GroundSpot.Position, 0, WaitSeconds(ScriptFormula(7)));

            if (Rune_E > 0)
            {
                yield return WaitSeconds(ScriptFormula(0));
                var PreSanctified = SpawnEffect(149851, GroundSpot.Position, 0, WaitSeconds(ScriptFormula(31)));
                PreSanctified.UpdateDelay = 0.3f;
                PreSanctified.OnUpdate = () =>
                    {
                        foreach (Actor enemy in GetEnemiesInRadius(GroundSpot.Position, ScriptFormula(1)).Actors)
                        {
                            AddBuff(enemy, new DebuffSlowed(ScriptFormula(30), WaitSeconds(ScriptFormula(31))));
                        }
                    };
            }

            yield break;
        }
        [ImplementsPowerBuff(0)]
        class RegenBuff : PowerBuff
        {
            public override void Init()
            {
                base.Init();
                Timeout = WaitSeconds(ScriptFormula(13));
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                if (Rune_C > 0)
                {
                    Target.Attributes[GameAttribute.Defense_Bonus_Percent] += ScriptFormula(20);
                }
                if (Rune_D > 0)
                {
                    Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += ScriptFormula(25);
                }
                Target.Attributes.BroadcastChangedIfRevealed();
                return true;
            }

            public override void Remove()
            {
                base.Remove();
                if (Rune_C > 0)
                {
                    Target.Attributes[GameAttribute.Defense_Bonus_Percent] -= ScriptFormula(20);
                }
                if (Rune_D > 0)
                {
                    Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] -= ScriptFormula(25);
                }
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
        [ImplementsPowerBuff(1)]
        class RegenAllyBuff : PowerBuff
        {
            public override void Init()
            {
                base.Init();
                Timeout = WaitSeconds(ScriptFormula(13));
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;
                if (Rune_C > 0)
                {
                    Target.Attributes[GameAttribute.Defense_Bonus_Percent] += ScriptFormula(20);
                }
                if (Rune_D > 0)
                {
                    Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += ScriptFormula(25);
                }
                Target.Attributes.BroadcastChangedIfRevealed();

                return true;
            }

            public override void Remove()
            {
                base.Remove();
                if (Rune_C > 0)
                {
                    Target.Attributes[GameAttribute.Defense_Bonus_Percent] -= ScriptFormula(20);
                }
                if (Rune_D > 0)
                {
                    Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] -= ScriptFormula(25);
                }
                Target.Attributes.BroadcastChangedIfRevealed();
            }
        }
    }
    #endregion

    //Pet Class
    #region MysticAlly
    [ImplementsPowerSNO(Skills.Skills.Monk.SpiritSpenders.MysticAlly)]
    public class MysticAlly : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            yield break;
        }
    }
    #endregion

    //TODO: Runes.
    #region CycloneStrike
    [ImplementsPowerSNO(223473)]
    public class CycloneStrike : Skill
    {
        public override IEnumerable<TickTimer> Main()
        {
            StartCooldown(EvalTag(PowerKeys.CooldownTime));
            UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

            //rune-D -> spirit
            //crits -> Rune_E
            //debuff -> Rune_C
            //multi -> Rune_B
            //randomAOE -> Rune_A
            AttackPayload attack = new AttackPayload(this);
            attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(2), (int)ScriptFormula(5));
            attack.OnHit = hit =>
                {
                    Knockback(hit.Target, -25f, ScriptFormula(1), ScriptFormula(29));
                };
            attack.Apply();
            yield return WaitSeconds(0.5f);
            User.PlayEffectGroup(224247);
            WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(16) + ScriptFormula(17)), ScriptFormula(10), Rune_A > 0 ? DamageType.Fire : DamageType.Holy);

            yield break;
        }
    }
    #endregion
    //11 Passives
}
