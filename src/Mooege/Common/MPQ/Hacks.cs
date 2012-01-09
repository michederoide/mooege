using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Mooege.Common.MPQ.FileFormats.Types;
using System.Linq;
using Mooege.Core.GS.Common.Types.SNO;
using Mooege.Common.MPQ.FileFormats;
using Mooege.Core.GS.Items;
using System.Text.RegularExpressions;
using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Common.Logging;

namespace Mooege.Common.MPQ
{
    public class Hacks
    {
        public static Logger Logger = LogManager.CreateLogger();

        public static StreamWriter fout;

        public static Dictionary<int, string> SNOS;

        public static Dictionary<int, string> TagNames;

        public static object logsync = new object();

        static Hacks()
        {
            fout = new StreamWriter(File.Create("c:\\users\\michael\\desktop\\hacks.txt"));

            SNOS = new Dictionary<int, string>();
            var snofile = File.OpenText(@"C:\Users\michael\Desktop\d3emu\mpq_research\snodatabase8101.txt");
            string line;
            while ((line = snofile.ReadLine()) != null)
            {
                Match m = Regex.Match(line, @"(\S+)\s+(\d+)\s+(.+)");
                if (m.Success)
                {
                    SNOS[int.Parse(m.Groups[2].Value)] = m.Groups[3].Value;
                }
            }

            TagNames = new Dictionary<int, string>();
            var tagNameFile = File.OpenText(@"C:\Users\michael\Desktop\d3emu\tagid_names_patch4.txt");
            while ((line = tagNameFile.ReadLine()) != null)
            {
                Match m = Regex.Match(line, @"(\S+) (\d+) (.+)");
                if (m.Success)
                {
                    TagNames[int.Parse(m.Groups[2].Value)] = m.Groups[3].Value;
                }
            }
        }

        public static void Log(string msg, params object[] objs)
        {
            fout.WriteLine(msg, objs);
            fout.Flush();
        }

        public static string StringSNO(int sno)
        {
            if (SNOS.ContainsKey(sno))
                return SNOS[sno];
            else
                return "UnknownSNO";
        }

        public static string StringTagID(int tagID)
        {
            if (TagNames.ContainsKey(tagID))
                return TagNames[tagID];
            else
                return "UnknownTagID";
        }

        public static void LogPowerTagmaps()
        {
            foreach (var powerEntry in MPQStorage.Data.Assets[SNOGroup.Power].Values)
            {
                Log("tagmaps for {0}", Path.GetFileName(powerEntry.FileName));
                Power power = (Power)powerEntry.Data;

                var tagmaps = new Dictionary<string, TagMap>()
                {
                    { "TagMap", power.Powerdef.TagMap },
                    { "GeneralTagMap", power.Powerdef.GeneralTagMap },
                    { "PVPGeneralTagMap", power.Powerdef.PVPGeneralTagMap },
                    { "ContactTagMap0", power.Powerdef.ContactTagMap0 },
                    { "ContactTagMap1", power.Powerdef.ContactTagMap1 },
                    { "ContactTagMap2", power.Powerdef.ContactTagMap2 },
                    { "ContactTagMap3", power.Powerdef.ContactTagMap3 },
                    { "PVPContactTagMap0", power.Powerdef.PVPContactTagMap0 },
                    { "PVPContactTagMap1", power.Powerdef.PVPContactTagMap1 },
                    { "PVPContactTagMap2", power.Powerdef.PVPContactTagMap2 },
                    { "PVPContactTagMap3", power.Powerdef.PVPContactTagMap3 },
                };

                foreach (var kv in tagmaps)
                {
                    LogTagmap(kv.Key, kv.Value);
                }

                Log("");
                Log("");
            }
        }

        public static void LogTagmap(string tagMapName, TagMap tagmap)
        {
            int counter = 0;
            foreach (var tgm in tagmap.OrderBy((e) => e.TagID))
            {
                Log("{0}[{1}].TagID {2}", tagMapName, counter, tgm.TagID);
                Log("{0}[{1}].TagIDLookup \"{2}\"", tagMapName, counter, StringTagID(tgm.TagID));
                Log("{0}[{1}].Type {2}", tagMapName, counter, tgm.Type);
                switch (tgm.Type)
                {
                    case 0:
                        Log("{0}[{1}].Int {2}", tagMapName, counter, tgm.Int);
                        break;
                    case 1:
                        Log("{0}[{1}].Float0 {2}", tagMapName, counter, tgm.Float);
                        break;
                    case 2: // SNO
                        Log("{0}[{1}].Int {2}", tagMapName, counter, tgm.Int);
                        Log("{0}[{1}].SNOFile: {2}", tagMapName, counter, StringSNO(tgm.Int));
                        break;
                    case 4:
                        Log("{0}[{1}].OpCodeName: {2}", tagMapName, counter, tgm.ScriptFormula.OpCodeName);
                        break;
                    default:
                        Log("{0}[{1}].Int? {2}", tagMapName, counter, tgm.Int);
                        break;
                }
                Log("");
                ++counter;
            }
        }
        
        public static void LogPowerScripts(bool simple)
        {
            foreach (var powerEntry in MPQStorage.Data.Assets[SNOGroup.Power].Values)
            {
                Log("script formulas for {0}", Path.GetFileName(powerEntry.FileName));
                Power power = (Power)powerEntry.Data;

                var tagmaps = new Dictionary<string, TagMap>()
                {
                    { "TagMap", power.Powerdef.TagMap },
                    { "GeneralTagMap", power.Powerdef.GeneralTagMap },
                    { "PVPGeneralTagMap", power.Powerdef.PVPGeneralTagMap },
                    { "ContactTagMap0", power.Powerdef.ContactTagMap0 },
                    { "ContactTagMap1", power.Powerdef.ContactTagMap1 },
                    { "ContactTagMap2", power.Powerdef.ContactTagMap2 },
                    { "ContactTagMap3", power.Powerdef.ContactTagMap3 },
                    { "PVPContactTagMap0", power.Powerdef.PVPContactTagMap0 },
                    { "PVPContactTagMap1", power.Powerdef.PVPContactTagMap1 },
                    { "PVPContactTagMap2", power.Powerdef.PVPContactTagMap2 },
                    { "PVPContactTagMap3", power.Powerdef.PVPContactTagMap3 },
                };

                foreach (var kv in tagmaps)
                {
                    foreach (var tagEntry in kv.Value.OrderBy(e => e.TagID))
                    {
                        if (tagEntry.Type == 4)
                        {
                            string name = StringTagID(tagEntry.TagID);
                            Log("{0}.\"{1}\"", kv.Key, name);
                            Log("   tagid: {0}", tagEntry.TagID);

                            int sf_n;
                            Match nameMatch = Regex.Match(name, "Script Formula (\\d+)");
                            if (nameMatch.Success)
                            {
                                sf_n = int.Parse(nameMatch.Groups[1].Value);
                            }
                            else
                            {
                                sf_n = -1;
                            }

                            if (sf_n != -1 && power.ScriptFormulaDetails.Count > sf_n)
                            {
                                Log("   detail 1: {0}", power.ScriptFormulaDetails[sf_n].CharArray1);
                                Log("   detail 2: {0}", power.ScriptFormulaDetails[sf_n].CharArray2);
                            }

                            Log("   opname: {0}", tagEntry.ScriptFormula.OpCodeName);

                            if (!simple)
                            {
                                Log("   opbin: {0}", tagEntry.ScriptFormula.OpCodeArray.Aggregate("", (a, b) => a + " " + b));

                                List<int> opcodes = new List<int>();
                                while (opcodes.Count * 4 < tagEntry.ScriptFormula.OpCodeArray.Length)
                                    opcodes.Add(BitConverter.ToInt32(tagEntry.ScriptFormula.OpCodeArray, opcodes.Count * 4));

                                Log("   opdecode: {0}", FormulaScript.ToString(opcodes.ToArray()));
                            }
                            Log("");
                        }
                    }
                }
                Log("");
                Log("");
            }
        }

        public static void LogInterestingEFG(string p, FileFormats.EffectGroup effectGroup)
        {
            if (effectGroup.SnoPower != -1)
            {
                lock (logsync)
                {
                    Hacks.Log("efg file: {0}", p);
                    Hacks.Log("    SnoPower: {0}", effectGroup.SnoPower);
                    Hacks.Log("    I0: {0}", effectGroup.I0);
                    Hacks.Log("    I1: {0}", effectGroup.I1);
                    Hacks.Log("    I2: {0}", effectGroup.I2);
                    Hacks.Log("    I3: {0}", effectGroup.I3);
                    Hacks.Log("    I4: {0}", effectGroup.I4);
                    Hacks.Log("");
                }
            }
        }

        public static void DumpPowerBuffs(string p, FileFormats.Power power)
        {
            lock (logsync)
            {
                Hacks.Log("power file: {0}", p);
                int counter = 0;
                foreach (var bufdef in power.Powerdef.Buffs)
                {
                    if (bufdef.BuffFilterPowers.Count > 0)
                        Hacks.Log("    BuffDef[{0}]   {1}", counter, bufdef.BuffFilterPowers.Aggregate("", (a, n) => a + " " + n));
                    else
                        Hacks.Log("    BuffDef[{0}]   Empty", counter);
                    counter++;
                }
                Hacks.Log("");
            }
        }

        public static void DumpInterestingSNOS()
        {
            //SNOGroup[] toget = new SNOGroup[]
            //{
            //    SNOGroup.Actor,
            //    SNOGroup.Anim,
            //    SNOGroup.Rope,
            //    SNOGroup.Power,
            //    SNOGroup.EffectGroup,
            //};
            SNOGroup[] toget = (SNOGroup[])Enum.GetValues(typeof(SNOGroup));
            
            foreach (SNOGroup grp in toget)
            {
                foreach (var asset in MPQStorage.Data.Assets[grp].Values.OrderBy((v) => Path.GetFileName(v.FileName)))
                {
                    Log("{0,-8} {1,-8} {2}", asset.SNOId.ToString("x"), asset.SNOId, Path.GetFileName(asset.FileName));
                }
            }
        }

        public static void DumpPowersForMonsters()
        {
            foreach (var powerEntry in MPQStorage.Data.Assets[SNOGroup.Power].Values)
            {
                List<Asset> users = new List<Asset>();

                foreach (var monsterEntry in MPQStorage.Data.Assets[SNOGroup.Monster].Values)
                {
                    var monData = (Monster)monsterEntry.Data;
                    foreach (var skill in monData.SkillDeclarations)
                    {
                        if (skill.SNOPower == powerEntry.SNOId)
                        {
                            users.Add(monsterEntry);
                        }
                    }
                }

                if (users.Count > 0)
                {
                    Log("monsters using {0} ({1})", StringSNO(powerEntry.SNOId), powerEntry.SNOId);
                    foreach (var monsterEntry in users.OrderBy(mon => StringSNO(mon.SNOId)))
                        Log("   {0} ({1})", StringSNO(monsterEntry.SNOId), monsterEntry.SNOId);

                    Log("");
                }
            }
        }

        public static void DumpBehaviorsForMonsters()
        {
            foreach (var aibEntry in MPQStorage.Data.Assets[SNOGroup.AiBehavior].Values)
            {
                List<Asset> users = new List<Asset>();

                foreach (var monsterEntry in MPQStorage.Data.Assets[SNOGroup.Monster].Values)
                {
                    var monData = (Monster)monsterEntry.Data;
                    foreach (var behavior in monData.AIBehavior)
                    {
                        if (aibEntry.SNOId == behavior)
                        {
                            users.Add(monsterEntry);
                        }
                    }
                }

                if (users.Count > 0)
                {
                    Log("monsters using {0} ({1})", StringSNO(aibEntry.SNOId), aibEntry.SNOId);
                    foreach (var monsterEntry in users.OrderBy(mon => StringSNO(mon.SNOId)))
                        Log("   {0} ({1})", StringSNO(monsterEntry.SNOId), monsterEntry.SNOId);

                    Log("");
                }
            }
        }

        public static void DumpActorsWithTag(int tagKey)
        {
            foreach (var entry in MPQStorage.Data.Assets[SNOGroup.Actor].Values)
            {
                if (entry.Data != null)
                {
                    var data = (Actor)entry.Data;
                    if (data.TagMap.ContainsKey(tagKey))
                    {
                        Log("{0} in {1} ({2})", data.TagMap[tagKey], StringSNO(entry.SNOId), entry.SNOId);
                    }
                }
            }
        }

        public static void OnMPQLoaded()
        {
            DumpActorsWithTag(67858);

            Logger.Debug("OnMPQLoaded() finished!");
        }
    }
}
