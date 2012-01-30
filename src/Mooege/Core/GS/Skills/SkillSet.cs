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
        public HotbarButtonData[] HotBarSkills;
        public int[] PassiveSkills;

        protected static readonly Logger Logger = LogManager.CreateLogger();

        public SkillSet(ToonClass @class,Toon toon)
        {
            this.@Class = @class;

            public int[] ActiveSkills;
            public HotbarButtonData[] HotBarSkills;
            public int[] PassiveSkills;

            this.ActiveSkills = Skills.GetAllActiveSkillsByClass(this.@Class).Take(6).ToArray();
            
            var query = string.Format("SELECT * from active_skills WHERE id_toon={0}", toon.D3EntityID.IdLow);
            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();



            if (!reader.HasRows)
            {
                var query_first_insert = string.Format("INSERT INTO  active_skills (id_toon,skill_0,skill_1,skill_2,skill_3,skill_4,skill_5) VALUES ({0},{1},{2},{3},{4},{5},{6} )", toon.D3EntityID.IdLow, this.ActiveSkills[0], Skills.None, Skills.None, Skills.None, Skills.None, Skills.None, Skills.None);
                var cmd_first_insert = new SQLiteCommand(query_first_insert, DBManager.Connection);
                var reader_first_install = cmd_first_insert.ExecuteReader();


                Logger.Debug("SkillSet: No Entry for {0}", toon.D3EntityID.IdLow);
<<<<<<< HEAD
				this.HotBarSkills = new HotbarButtonData[9] {     
                new HotbarButtonData { SNOSkill = this.ActiveSkills[0], ItemGBId = -1 }, // left-click
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // right-click
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // hidden-bar - left-click switch - which key??
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // hidden-bar - right-click switch (press x ingame)
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // bar-1
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // bar-2
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // bar-3
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = -1 }, // bar-4 
                new HotbarButtonData { SNOSkill = Skills.None, ItemGBId = 0x622256D4 } // bar-5 - potion
                };
=======

             
>>>>>>> 94c978ba11b9c3e9d48a8deedc55b9cbb14b6119
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

    }
}
