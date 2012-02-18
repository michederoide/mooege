﻿﻿/*
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
    #region Ghost
    [HandledSNO(370, 136943)]
    public class Ghost : Monster
    {
        public Ghost(World world, int snoId, TagMap tags)
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
        6644, 6646, 6647, 6651)]

    public class Zombie : Monster
    {
        public Zombie(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30550); //Summon_Zombie_Crawler.pow, no idea what this would do.
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

    [HandledSNO(203121)] //ZombieSkinny_A_LeahInn.acr
    public class InnZombie : Monster
    {
        public InnZombie(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(7789); //Weapon_Ranged_Wand.pow?
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 4.132813f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 4.132813f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 4.132813f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 4f; //HardCoded, this information aint contained in the packets.. it seems.
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 4f; //HardCoded, this information aint contained in the packets.. it seems.
        }
    }

    [HandledSNO(218367)] //ZombieCrawler_Barricade_A.acr
    public class ZombieCrawler : Monster
    {
        public ZombieCrawler(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(7789); //Weapon_Ranged_Wand.pow?
            //this.Attributes[GameAttribute.Hitpoints_Max_Total] = 5f;
            //this.Attributes[GameAttribute.Hitpoints_Max] = 1.602539f;
            //this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            //this.Attributes[GameAttribute.Hitpoints_Cur] = 1.602539f;
            //this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            //this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 4f; //HardCoded, this information aint contained in the packets.. it seems.
            //this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 4f; //HardCoded, this information aint contained in the packets.. it seems.
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 4.132813f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 4.132813f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 4.132813f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 4f; //HardCoded, this information aint contained in the packets.. it seems.
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 4f; //HardCoded, this information aint contained in the packets.. it seems.
        }
    }

    [HandledSNO(218339)] //ZombieSkinny_Custom_A.acr (2036596938)
    public class ZombieSkinny : Monster
    {
        public ZombieSkinny(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            //Not sure how to actually make the AI for the Buffs used for this mob.
            (Brain as MonsterBrain).AddPresetPower(30290); //InvulnerableDuringBuff.pow 
            (Brain as MonsterBrain).AddPresetPower(79486); //UninterruptibleDuringBuff.pow
            (Brain as MonsterBrain).AddPresetPower(30582); //UntargetableDuringBuff.pow 
            (Brain as MonsterBrain).AddPresetPower(225599); //CannotDieDuringBuff.pow 
            //this.Attributes[GameAttribute.Buff_Active,488] = true; //Can we handle buffs?.
            //Below are the actual stats from the beta packets, still, if used that stats for the zombies are not correct,
            //So theres probably something wrong in some formula. - Wesko
            //this.Attributes[GameAttribute.Hitpoints_Max_Total] = 3.6875f;
            //this.Attributes[GameAttribute.Hitpoints_Max] = 3.6875f;
            //this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 3.051758f;
            //this.Attributes[GameAttribute.Hitpoints_Cur] = 1.602539f;
            //this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            //this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 4f;
            //this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 4f;
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 4.132813f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 4.132813f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 4.132813f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 4f; //HardCoded, this information aint contained in the packets.. it seems.
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 4f; //HardCoded, this information aint contained in the packets.. it seems.
        }
    }

    #endregion
    #region Skeleton
    //No Uniques Added
    [HandledSNO(5393, 87012, 5395, 5397, 80652, 5407, 5408, 5411, 434)]
    public class Skeleton : Monster
    {
        public Skeleton(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
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
    #region Skeleton_TemplerIntro_NoWander
    [HandledSNO(105863)]
    public class Skeleton_TemplerIntro_NoWander : Monster
    {
        public Skeleton_TemplerIntro_NoWander(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30592);
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
    #region SkeletonSummoner
    //No Uniques Added
    [HandledSNO(5387)]
    public class SkeletonSummoner : Monster
    {
        public SkeletonSummoner(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30503);
            (Brain as MonsterBrain).AddPresetPower(29990);
            (Brain as MonsterBrain).AddPresetPower(30001);
            (Brain as MonsterBrain).AddPresetPower(30005);
            (Brain as MonsterBrain).AddPresetPower(30543); //Summon Skeletons
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
    #region SkeletonArcher
    [HandledSNO(5346, 218400, 5347)]
    public class SkeletonArcher : Monster
    {
        public SkeletonArcher(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30495);
            (Brain as MonsterBrain).AddPresetPower(30334);
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
    #region ShieldSkeleton
    //No Uniques Added
    [HandledSNO(5275, 5276, 5277)]
    public class ShieldSkeletonSkeleton : Monster
    {
        public ShieldSkeletonSkeleton(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30474);
            (Brain as MonsterBrain).AddPresetPower(30473);
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
    #region Grotesque
    //No Uniques Added
    [HandledSNO(3847, 3848, 218307, 218308, 218405, 4564)]
    public class Grotesque : Monster
    {
        //3851 suicide blood, 220536 suicide imps = these happen on different SNOs and happen as they are dying.

        public Grotesque(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30531);
            (Brain as MonsterBrain).AddPresetPower(30530);
            //(Brain as MonsterBrain).AddPresetPower(30529); //Explode
            (Brain as MonsterBrain).AddPresetPower(30592);
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
    #region FleshPitFlyers
    //No Uniques Added
    [HandledSNO(4156, 218314, 218362, 4157, 81954, 368, 195747)]
    public class FleshPitFlyers : Monster
    {
        public FleshPitFlyers(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(30334);
            (Brain as MonsterBrain).AddPresetPower(30592);
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
    #region Wretched Mothers

    [HandledSNO(219725, 108444, 176889 /*the queen*/)] // ZombieFemale_A_TristramQuest_Unique.acr 
    public class WretchedMother : Monster
    {
        public WretchedMother(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Brain = new MonsterBrain(this);
            (Brain as MonsterBrain).AddPresetPower(110518); //spit
            (Brain as MonsterBrain).AddPresetPower(94734); // Vomit Without the Spawns
            (Brain as MonsterBrain).AddPresetPower(30592); //Instant Wep
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 13.38281f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 13.38281f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 13.38281f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 4f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 6f;  
        }
    }
   #endregion
   #region Scavenger
    [HandledSNO(5235, 5236)]
    public class Scavenger : Monster
    {

        public Scavenger(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {   
            Scale = 0.85f;
            WalkSpeed = 0.30f;       
            this.Brain = new MonsterBrain(this);
           (Brain as MonsterBrain).AddPresetPower(30592);
           (Brain as MonsterBrain).AddPresetPower(1752); // Leap ; They leap even without this power here.
           (Brain as MonsterBrain).AddPresetPower(30450); //Burrow In - Not implemented.
           (Brain as MonsterBrain).AddPresetPower(30451);  //Burrow Out - Not implemented.
            this.Attributes[GameAttribute.Hitpoints_Max_Total] = 12f;
            this.Attributes[GameAttribute.Hitpoints_Max] = 12f;
            this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 2f;
            this.Attributes[GameAttribute.Hitpoints_Cur] = 12f;
            this.Attributes[GameAttribute.Attacks_Per_Second_Total] = 1.0f;
            this.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = 4f;
            this.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = 6f; 
        }
    }
    #endregion
}