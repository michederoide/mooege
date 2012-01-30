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
using Mooege.Core.GS.Skills;
using Mooege.Core.GS.Ticker;
using Mooege.Core.GS.Actors;
using Mooege.Core.GS.Common.Types.TagMap;

namespace Mooege.Core.GS.Powers.Implementations
{
    //30592
    [ImplementsPowerSNO(0x00007780)]  // Weapon_Melee_Instant.pow
    public class WeaponMeleeInstant : ActionTimedSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            WeaponDamage(GetBestMeleeEnemy(), 1.00f, DamageType.Physical);
            yield break;
        }

        public override float GetActionSpeed()
        {
            return base.GetActionSpeed();
        }
    }

    [ImplementsPowerSNO(30596)]  // Weapon_Melee_Reach_Instant.pow
    public class WeaponMeleeReachInstant : ActionTimedSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            WeaponDamage(GetEnemiesInRadius(User.Position, EvalTag(PowerKeys.AttackRadius)), 1.00f, DamageType.Physical);
            yield break;
        }

        public override float GetActionSpeed()
        {
            return base.GetActionSpeed();
        }
    }

    [ImplementsPowerSNO(30474)]  // Shield_Skeleton_Melee_Instant.pow
    public class ShieldSkeletonMeleeInstant : ActionTimedSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            WeaponDamage(GetBestMeleeEnemy(), 1.00f, DamageType.Physical);
            yield break;
        }

        public override float GetActionSpeed()
        {
            return base.GetActionSpeed();
        }
    }

    [ImplementsPowerSNO(30530)]  // StitchMeleeAlternate.pow
    public class StitchMeleeAlternate : ActionTimedSkill
    {
        //Crit Chance = 30/100
        public override IEnumerable<TickTimer> Main()
        {
            WeaponDamage(GetEnemiesInRadius(User.Position, EvalTag(PowerKeys.AttackRadius)), 1.00f, DamageType.Physical);
            yield break;
        }

        public override float GetActionSpeed()
        {
            return base.GetActionSpeed();
        }
    }

    [ImplementsPowerSNO(30531)]  // StitchPush.pow
    public class StitchPush : ActionTimedSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            WeaponDamage(GetEnemiesInRadius(User.Position, EvalTag(PowerKeys.AttackRadius)), 1.00f, DamageType.Physical);
            foreach (Actor enemy in GetEnemiesInRadius(User.Position, EvalTag(PowerKeys.AttackRadius)).Actors)
            Knockback(enemy, 14f);
            yield break;
        }

        public override float GetActionSpeed()
        {
            return base.GetActionSpeed();
        }
    }
}
