﻿/*
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

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Mooege.Common.Logging;
using Mooege.Common.Storage;
using Mooege.Core.MooNet.Accounts;
using Mooege.Core.GS.Items;
using Mooege.Common.MPQ.FileFormats;
using Mooege.Core.GS.Common.Types.Math;

namespace Mooege.Core.MooNet.Toons
{
    // Just a quick hack - not to be meant final
    public static class ToonManager
    {
        private static readonly Dictionary<ulong, Toon> Toons =
            new Dictionary<ulong, Toon>();

        private static readonly Logger Logger = LogManager.CreateLogger();

        static ToonManager()
        {
            LoadToons();
        }

        public static Account GetOwnerAccountByToonLowId(ulong id)
        {
            var toon = (from pair in Toons where pair.Value.PersistentID == id select pair.Value).FirstOrDefault();
            return (toon != null) ? toon.GameAccount.Owner : null;
        }

        public static GameAccount GetOwnerGameAccountByToonLowId(ulong id)
        {
            var toon = (from pair in Toons where pair.Value.PersistentID == id select pair.Value).FirstOrDefault();
            return (toon != null) ? toon.GameAccount : null;
        }

        public static Toon GetToonByLowID(ulong id)
        {
            return (from pair in Toons where pair.Value.PersistentID == id select pair.Value).FirstOrDefault();
        }

        public static Dictionary<ulong, Toon> GetToonsForGameAccount(GameAccount account)
        {
            return Toons.Where(pair => pair.Value.GameAccount != null).Where(pair => pair.Value.GameAccount.PersistentID == account.PersistentID).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        //public static Dictionary<ulong, Toon> GetToonsForAccount(Account account)
        //{
        //    return Toons.Where(pair => pair.Value.GameAccount.Owner != null).Where(pair => pair.Value.GameAccount.Owner.PersistentID == account.PersistentID).ToDictionary(pair => pair.Key, pair => pair.Value);
        //}

        public static int TotalToons
        {
            get { return Toons.Count; }
        }

        //Method only used when creating a Toon for the first time, ambiguous method name - Tharuler
        public static bool SaveToon(Toon toon)
        {

            if (Toons.ContainsKey(toon.PersistentID)) //this should never happen again thanks to hashcode, but lets leave it in for now - Tharuler
            {
                Logger.Error("Duplicate persistent toon id: {0}", toon.PersistentID);
                return false;
            }

            Toons.Add(toon.PersistentID, toon);
            toon.SaveToDB();

            Logger.Trace("Character {0} added to database", toon.HeroNameField.Value);

            return true;
        }

        public static void DeleteToon(Toon toon)
        {
            if (!Toons.ContainsKey(toon.PersistentID))
            {
                Logger.Error("Attempting to delete toon that does not exist: {0}", toon.PersistentID);
                return;
            }

            if (toon.DeleteFromDB()) Toons.Remove(toon.PersistentID);
			Logger.Debug("Deleting toon {0}",toon.PersistentID);
        }

        private static void LoadToons()
        {
            var query = "SELECT * from toons";
            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();

            if (!reader.HasRows) return;

            while (reader.Read())
            {
                var itemCreate = new Dictionary<uint, Item>();
                var databaseId = (ulong)reader.GetInt64(0);
                //TODO: Move this to toon class only create a toon with id and call load from DB
                var toon = new Toon(databaseId, reader.GetString(1), (int)reader.GetInt32(6), reader.GetByte(2), reader.GetByte(3), reader.GetByte(4), reader.GetInt64(5), (uint)reader.GetInt32(7), (int)reader.GetInt32(8),null);
                //add visual equipment
                //TODO: Load all visualEquipment at once
                D3.Hero.VisualItem[] visualItems = new D3.Hero.VisualItem[8];
                var vItemQuery =
                    string.Format("SELECT * from inventory WHERE toon_id = {0} and inventory_slot <> -1", databaseId);
                var itemCmd = new SQLiteCommand(vItemQuery, DBManager.Connection);
                var itemReader = itemCmd.ExecuteReader();
                if (itemReader.HasRows)
                {
                    while (itemReader.Read())
                    {
                        var slot = (int)itemReader.GetInt64(3);
                        var gbid = (int)itemReader.GetInt64(4);
                        visualItems[slot] = D3.Hero.VisualItem.CreateBuilder()
                            .SetGbid(gbid)
                            .SetEffectLevel(0)
                            .Build();
                    }

                    toon.HeroVisualEquipmentField.Value = D3.Hero.VisualEquipment.CreateBuilder().AddRangeVisualItem(visualItems).Build();
                }

                //add inventory items
                var itemQuery =
                    string.Format("SELECT * from inventory WHERE toon_id = {0} and inventory_slot = -1", databaseId);
                itemCmd = new SQLiteCommand(itemQuery, DBManager.Connection);
                itemReader = itemCmd.ExecuteReader();
                if (itemReader.HasRows)
                {
                    uint count = 0;
                    toon.ItemsTable = new Dictionary<uint, KeyValuePair<ItemTable, Vector2D>>();
                    while (itemReader.Read())
                    {
                        var x = (int)itemReader.GetInt64(1);
                        var y = (int)itemReader.GetInt64(2);
                        var slot = (int)itemReader.GetInt64(3);
                        var gbid = (int)itemReader.GetInt64(4);
                        ItemTable it = ItemGenerator.GetItemDefinition(gbid);
                        Vector2D v2d = new Vector2D(x,y);
                        toon.ItemsTable.Add((uint)gbid,new KeyValuePair<ItemTable, Vector2D>(it,v2d));
                        count++;
                    }

                    toon.Items = itemCreate;
                }


                Toons.Add(databaseId, toon);
            }
        }

        public static int GetUnusedHashCodeForToonName(string name)
        {
            var query = string.Format("SELECT hashCode from toons WHERE name='{0}'", name);
            Logger.Trace(query);
            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return GenerateHashCodeNotInList(null);

            var codes = new HashSet<int>();
            while (reader.Read())
            {
                var hashCode = reader.GetInt32(0);
                codes.Add(hashCode);
            }
            return GenerateHashCodeNotInList(codes);
        }

        public static void Sync()
        {
            foreach (var pair in Toons)
            {
                pair.Value.SaveToDB();
            }
        }

        private static int GenerateHashCodeNotInList(HashSet<int> codes)
        {
            Random rnd = new Random();
            if (codes == null) return rnd.Next(1, 1000);

            int hashCode;
            do
            {
                hashCode = rnd.Next(1, 1000);
            } while (codes.Contains(hashCode)) ;
            return hashCode;

        }
    }
}
