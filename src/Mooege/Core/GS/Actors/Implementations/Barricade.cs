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
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Net.GS.Message.Definitions.Animation;
using Mooege.Core.GS.Common.Types.SNO;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Definitions.Misc;
using Mooege.Core.GS.Ticker;
using Mooege.Core.GS.Items;

namespace Mooege.Core.GS.Actors.Implementations
{
    /// <summary>
    /// Class that implements behaviour for killable gizmos.
    /// Play die animation on click, then set idle animation, drop loot and remove from server
    /// </summary>
    class Barricade : Monster
    {
        public Barricade(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            base.Attributes[GameAttribute.Experience_Granted] = 0;
            base.Attributes[GameAttribute.DropsNoLoot] = true;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 1;
        }


        public void ReceiveDamage(Actor source, float damage /* critical, type */)
        {
            World.BroadcastIfRevealed(new FloatingNumberMessage
            {
                Number = damage,
                ActorID = this.DynamicID,
                Type = FloatingNumberMessage.FloatType.White
            }, this);


            Attributes[GameAttribute.Hitpoints_Cur] = Math.Max(Attributes[GameAttribute.Hitpoints_Cur] - damage, 0);
            Attributes[GameAttribute.Last_Damage_ACD] = unchecked((int)source.DynamicID);

            Attributes.BroadcastChangedIfRevealed();

            if (Attributes[GameAttribute.Hitpoints_Cur] == 0)
            {
                Die();
            }
        }

        public void Die()
        {

            World.BroadcastIfRevealed(new PlayAnimationMessage
            {
                ActorID = this.DynamicID,
                Field1 = 11,
                Field2 = 0,
                tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 10,
                        AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault],
                        PermutationIndex = 0,
                        Speed = 1
                    }
                }

            }, this);

            this.Attributes[GameAttribute.Deleted_On_Server] = true;
            this.Attributes[GameAttribute.Could_Have_Ragdolled] = true;
            Attributes.BroadcastChangedIfRevealed();
            this.Destroy();
        }


        public override void OnTargeted(Players.Player player, Net.GS.Message.Definitions.World.TargetMessage message)
        {
            ReceiveDamage(player, 100);
        }

    }
}
