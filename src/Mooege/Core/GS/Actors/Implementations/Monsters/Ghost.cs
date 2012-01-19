﻿/*
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
using Mooege.Common.MPQ.FileFormats.Types;
using Mooege.Core.GS.AI.Brains;
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Net.GS.Message;

namespace Mooege.Core.GS.Actors.Implementations.Monsters
{
    [HandledSNO(136943)]
    public class Ghost : Monster
    {
        public Ghost(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 308.25f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 216.25f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 3.051758E-05f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Vitality] = 92;
            this.Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 4;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 308.25f;
            this.Attributes[GameAttribute.Experience_Next] = 0x003C19;
            this.Attributes[GameAttribute.Experience_Granted] = 0x28;
            this.Attributes[GameAttribute.Armor_Total] = 20;
            this.Attributes[GameAttribute.Armor_Item_Total] = 20;
            this.Attributes[GameAttribute.Armor_Item_SubTotal] = 20;
            this.Attributes[GameAttribute.Armor_Item] = 20;
            this.Attributes[GameAttribute.Defense] = 23;
            this.Attributes[GameAttribute.Vitality] = 23;
            this.Attributes[GameAttribute.Precision] = 23;
            this.Attributes[GameAttribute.Attack] = 23;
            this.Attributes[GameAttribute.General_Cooldown] = 0;
            this.Attributes[GameAttribute.Level_Cap] = 60;
        }
    }
}
