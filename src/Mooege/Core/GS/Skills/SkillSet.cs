/*
 * Copyright (C) 2011 - 2012 mooege project - http://www.mooege.org
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

using System.Linq;
using Mooege.Core.MooNet.Toons;
using Mooege.Net.GS.Message.Fields;
using Mooege.Common.Logging;
using Mooege.Net.MooNet;
using System.Data.SQLite;
using Mooege.Common.Storage;

namespace Mooege.Core.GS.Skills
{
    public class SkillSet
    {
    
        public ToonClass @Class;
        public Toon Toon { get; private set; }
        
        public int[] ActiveSkills;
		public int[] CurrentActiveSkills;
        public HotbarButtonData[] HotBarSkills;
        public int[] PassiveSkills;

        protected static readonly Logger Logger = LogManager.CreateLogger();

        public SkillSet(ToonClass @class,Toon toon)
        {
            this.@Class = @class;

            
            this.CurrentActiveSkills = Skills.GetAllActiveSkillsByClass(this.@Class).Take(6).ToArray();
            
            var query = string.Format("SELECT * from active_skills WHERE id_toon={0}", toon.D3EntityID.IdLow);
            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();

			//30592 base attack

            if (!reader.HasRows)  //this is just in case something goes wrong on  public void SaveToDB() -> we need to change this name ...
            {
                var query_first_insert = string.Format("INSERT INTO  active_skills (id_toon,skill_0,skill_1,skill_2,skill_3,skill_4,skill_5) VALUES ({0},{1},{2},{3},{4},{5},{6} )", toon.D3EntityID.IdLow, this.CurrentActiveSkills[0], 30592, Skills.None, Skills.None, Skills.None, Skills.None, Skills.None);
                var cmd_first_insert = new SQLiteCommand(query_first_insert, DBManager.Connection);
                cmd_first_insert.ExecuteReader();
                Logger.Debug("SkillSet: No Entry for {0}", toon.D3EntityID.IdLow);
				this.HotBarSkills = new HotbarButtonData[9] {     
                new HotbarButtonData { SNOSkill = this.CurrentActiveSkills[0], ItemGBId = -1 }, // left-click
                new HotbarButtonData { SNOSkill = 30592, ItemGBId = -1 }, // right-click
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // hidden-bar - left-click switch - which key??
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // hidden-bar - right-click switch (press x ingame)
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // bar-1
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // bar-2
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // bar-3
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // bar-4 
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = 0x622256D4 } // bar-5 - potion
                };
            }
            else
            {
                this.HotBarSkills = new HotbarButtonData[9] {     
                new HotbarButtonData { SNOSkill = (int)reader["skill_0"], ItemGBId = -1 }, // left-click
                new HotbarButtonData { SNOSkill = (int)reader["skill_1"], ItemGBId = -1 }, // right-click
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // hidden-bar - left-click switch - which key??
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // hidden-bar - right-click switch (press x ingame)
                new HotbarButtonData { SNOSkill = (int)reader["skill_5"], ItemGBId = -1 }, // bar-1
                new HotbarButtonData { SNOSkill = (int)reader["skill_4"], ItemGBId = -1 }, // bar-2
                new HotbarButtonData { SNOSkill = (int)reader["skill_3"], ItemGBId = -1 }, // bar-3
                new HotbarButtonData { SNOSkill = (int)reader["skill_2"], ItemGBId = -1 }, // bar-4 
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = 0x622256D4 } // bar-5 - potion
                };
            }    
			
			
			//Current Active Skills part
			
			var query_as = string.Format("SELECT * from current_active_skills WHERE id_toon={0}", toon.D3EntityID.IdLow);
            var cmd_as = new SQLiteCommand(query_as, DBManager.Connection);
            var reader_as = cmd_as.ExecuteReader();

            if (!reader_as.HasRows)
            {
				var query_first_insert = string.Format("INSERT INTO  current_active_skills (id_toon,a_skill0,a_skill1,a_skill2,a_skill3,a_skill4,a_skill5) VALUES ({0},{1},{2},{3},{4},{5},{6} )", toon.D3EntityID.IdLow, this.CurrentActiveSkills[0], Skills.None, Skills.None, Skills.None, Skills.None, Skills.None, Skills.None);
                var cmd_first_insert = new SQLiteCommand(query_first_insert, DBManager.Connection);
                cmd_first_insert.ExecuteReader();
				Logger.Debug("SkillSet: Current Active Skill Seen for the First Time  Entry for {0}", toon.D3EntityID.IdLow);
				//Logger.Debug("CurrentActiveSkill {0}",reader_as["id_toon"]);
				this.ActiveSkills = new int[6]{this.CurrentActiveSkills[0],Skills.None,Skills.None,Skills.None,Skills.None,Skills.None};	
				
			}else 
			{
				this.ActiveSkills = new int[6]{(int)reader_as["a_skill0"],(int)reader_as["a_skill1"],(int)reader_as["a_skill2"],(int)reader_as["a_skill3"],(int)reader_as["a_skill4"],(int)reader_as["a_skill5"]};
				Logger.Debug("ActiveSkill {0}",reader_as["a_skill0"]);
			}
			

            this.PassiveSkills = new int[3] { -1, -1, -1 }; // setting passive skills here crashes the client, need to figure out the reason. /raist       
        }

        public void UpdateHotbarSkills(int hotBarIndex,int SNOSkill,Toon toon)
        {
            
            /* SKILL MAP Index
             * 0 -> skill_0
             * 1 -> skill_1
             * 7 -> skill_2
             * 6 -> skill_3
             * 5 -> skill_4
             * 4 -> skill_5
             */

            switch (hotBarIndex)
            {
                case 0:
                    Logger.Debug("Update index 0 {0}", SNOSkill);
                    var query_0 = string.Format("UPDATE active_skills SET skill_0={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_0 = new SQLiteCommand(query_0, DBManager.Connection);
                    cmd_0.ExecuteReader();
                    break;
                case 1:
                    Logger.Debug("Update index 1 {0}", SNOSkill);
                    var query_1 = string.Format("UPDATE active_skills SET skill_1={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_1 = new SQLiteCommand(query_1, DBManager.Connection);
                    cmd_1.ExecuteReader();
                    break;
                case 4:
                    Logger.Debug("Update index 4 {0}", SNOSkill);
                    var query_4 = string.Format("UPDATE active_skills SET skill_5={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_4 = new SQLiteCommand(query_4, DBManager.Connection);
                    cmd_4.ExecuteReader();
                    break;
                case 5:
                    Logger.Debug("Update index 5 {0}", SNOSkill);
                    var query_5 = string.Format("UPDATE active_skills SET skill_4={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_5 = new SQLiteCommand(query_5, DBManager.Connection);
                    cmd_5.ExecuteReader();
                    break;
                case 6:
                    Logger.Debug("Update index 6 {0}", SNOSkill);
                    var query_6 = string.Format("UPDATE active_skills SET skill_3={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_6 = new SQLiteCommand(query_6, DBManager.Connection);
                    cmd_6.ExecuteReader();
                    break;
                case 7:
                    Logger.Debug("Update index 7 {0}", SNOSkill);
                    var query_7 = string.Format("UPDATE active_skills SET skill_2={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_7 = new SQLiteCommand(query_7, DBManager.Connection);
                    cmd_7.ExecuteReader();
                    break;
            }

        }
		
		public void UpdateAssignedSkill(int skillIndex, int SNOSkill,Toon toon)
		{
			//Fill DB with some entry if is the first time this toon try to put data on DB.
			switch(skillIndex)
			{
				case 0:
					Logger.Debug("UpdateAssignedSkill: index 0 {0}", SNOSkill);
				    var query_0 = string.Format("UPDATE current_active_skills SET a_skill0={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_0 = new SQLiteCommand(query_0, DBManager.Connection);
                    cmd_0.ExecuteReader();	
					break;
				case 1:
                    Logger.Debug("UpdateAssignedSkill: index 1 {0}", SNOSkill);
                    var query_1 = string.Format("UPDATE current_active_skills SET a_skill1={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_1 = new SQLiteCommand(query_1, DBManager.Connection);
                    cmd_1.ExecuteReader();
                    break;
                case 2:
                    Logger.Debug("UpdateAssignedSkill: index 2 {0}", SNOSkill);
                    var query_2 = string.Format("UPDATE current_active_skills SET a_skill2={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_2 = new SQLiteCommand(query_2, DBManager.Connection);
                    cmd_2.ExecuteReader();
                    break;
                case 3:
                    Logger.Debug("UpdateAssignedSkill: index 3 {0}", SNOSkill);
                    var query_3 = string.Format("UPDATE current_active_skills SET a_skill3={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_3 = new SQLiteCommand(query_3, DBManager.Connection);
                    cmd_3.ExecuteReader();
                    break;
                case 4:
                    Logger.Debug("UpdateAssignedSkill: index 4 {0}", SNOSkill);
                    var query_4 = string.Format("UPDATE current_active_skills SET a_skill4={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_4 = new SQLiteCommand(query_4, DBManager.Connection);
                    cmd_4.ExecuteReader();
                    break;
                case 5:
                    Logger.Debug("UpdateAssignedSkill: index 5 {0}", SNOSkill);
                    var query_5 = string.Format("UPDATE current_active_skills SET a_skill5={1} WHERE id_toon={0} ", toon.D3EntityID.IdLow, SNOSkill);
                    var cmd_5 = new SQLiteCommand(query_5, DBManager.Connection);
                    cmd_5.ExecuteReader();
                    break;
			}
			
		}
		
		public void SwitchUpdateSkills (int oldSNOSkill,int SNOSkill, Toon toon)
		{
			for (int i = 0; i < this.HotBarSkills.Length; i++)
			{
				if(this.HotBarSkills[i].SNOSkill == oldSNOSkill)
				{
					Logger.Debug("SkillSet: SwitchUpdateSkill Oldskill {0} Newskill {1}",oldSNOSkill,SNOSkill);
					this.HotBarSkills[i].SNOSkill = SNOSkill;
					this.UpdateHotbarSkills(i,SNOSkill,toon);
					return;
				}
			}
		}

    }
}
