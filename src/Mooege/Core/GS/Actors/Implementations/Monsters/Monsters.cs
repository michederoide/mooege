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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Core.GS.AI.Brains;
using Mooege.Net.GS.Message;

namespace Mooege.Core.GS.Actors.Implementations.Monsters
{
    #region TreasureGoblin
    //Unknown: Wrong Way! you aren't supposed to attack!
    [HandledSNO(5984, 5985, 5987, 5988)] //54862 is goblin portal
    public class TreasureGoblin : Monster
    {
        public TreasureGoblin(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(105371);
            (Brain as MonsterBrain).AddPresetPower(54055);
            //(Brain as MonsterBrain).AddPresetPower(54836); //ThrowPortal
            //(Brain as MonsterBrain).AddPresetPower(105665); //ThrowPortal_Fast
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 5f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 5f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 7f;

        }
    }
    #endregion
    #region Spore
    //Unknown: These should spawn when Woodwraiths use that spell
    [HandledSNO(5482)]
    public class Spore : Monster
    {
        public Spore(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30525);
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 5f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 5f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 7f;

        }
    }
    #endregion
    #region QuillDemon
    [HandledSNO(4982)]
    public class QuillDemon : Monster
    {
        public QuillDemon(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30005);
            (Brain as MonsterBrain).AddPresetPower(30001);
            (Brain as MonsterBrain).AddPresetPower(107729);
            (Brain as MonsterBrain).AddPresetPower(30592);
            (Brain as MonsterBrain).AddPresetPower(30550);
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 5f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 5f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 7f;

        }
    }
    #endregion
    #region Ghost_A
    //This shows ingame as Enraged Phantom.
    [HandledSNO(370)]
    public class Ghost_A : Monster
    {
        public Ghost_A(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30592);
            (Brain as MonsterBrain).AddPresetPower(30244);
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 15f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 15f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 15f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 15f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 7f;
        }
    }
    #endregion
    #region Unburied
    //Unknown: These guys dont want to move :)
    [HandledSNO(6356, 6359)]
    public class Unburied : Monster
    {
        public Unburied(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30580);
            (Brain as MonsterBrain).AddPresetPower(30581);
            (Brain as MonsterBrain).AddPresetPower(202344);
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 5f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 5f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 7f;

        }
    }
    #endregion
    #region WoodWraith
    //Unknown: Doesn't spawn them?
    [HandledSNO(6572, 139454, 139456, 170324, 170325, 495)]
    public class WoodWraith : Monster
    {
        public WoodWraith(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30596); //Static_Pose/Unique
            (Brain as MonsterBrain).AddPresetPower(29990); //Static_Pose/Unique 
            (Brain as MonsterBrain).AddPresetPower(30800); //Unique
            //if WoodWraith uses 30800, release spores. -> this is only for unique woodwraith.

            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 5f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 5f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 7f;

        }
    }
    #endregion
    #region Zombie
    //No Uniques Added
    [HandledSNO(6652, 6653, 6654, 204256, //Zombies
        6644, 6646, 6647, 6651)] //ZombieSkinny
    public class Zombie : Monster
    {
        public Zombie(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30550);
            (Brain as MonsterBrain).AddPresetPower(30592);
            (Brain as MonsterBrain).AddPresetPower(30005);
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 5f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 5f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 5f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 7f;
        }
    }
    #endregion
}