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

using System;
using System.Linq;
using System.Collections.Generic;
using Mooege.Common.Helpers.Hash;
using Mooege.Common.Helpers.Math;
using Mooege.Common.Logging;
using Mooege.Core.GS.Players;
using Mooege.Net.GS.Message;
using Mooege.Common.MPQ.FileFormats;
using Mooege.Common.MPQ;
using Mooege.Core.GS.Common.Types.SNO;
using System.Reflection;

// FIXME: Most of this stuff should be elsewhere and not explicitly generate items to the player's GroundItems collection / komiga?

namespace Mooege.Core.GS.Items
{
    public static class ItemGenerator
    {
        public static readonly Logger Logger = LogManager.CreateLogger();

        private static readonly Dictionary<int, ItemTable> Items = new Dictionary<int, ItemTable>();
        private static readonly Dictionary<int, ItemTable> Equips = new Dictionary<int, ItemTable>();
        private static readonly Dictionary<int, ItemTable> AtLeastMagic = new Dictionary<int, ItemTable>();
        private static readonly Dictionary<int, Type> GBIDHandlers = new Dictionary<int, Type>();
        private static readonly Dictionary<int, Type> TypeHandlers = new Dictionary<int, Type>();
        private static readonly HashSet<int> AllowedItemTypes = new HashSet<int>();

        public static int TotalItems
        {
            get { return Items.Count; }
        }

        static ItemGenerator()
        {
            LoadItems();
            LoadEquips();
            LoadAtLeastMagics();
            LoadHandlers();
            SetAllowedTypes();
        }

        public static ItemTable GetDefinitionFromGBID(int Gbid)
        {
            return (from pair in Items where pair.Value.Hash == Gbid select pair.Value).FirstOrDefault();
        }

        private static void LoadHandlers()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(Item))) continue;

                var attributes = (HandledItemAttribute[])type.GetCustomAttributes(typeof(HandledItemAttribute), true);
                if (attributes.Length != 0)
                {
                    foreach (var name in attributes.First().Names)
                    {
                        GBIDHandlers.Add(StringHashHelper.HashItemName(name), type);
                    }
                }

                var typeAttributes = (HandledTypeAttribute[])type.GetCustomAttributes(typeof(HandledTypeAttribute), true);
                if (typeAttributes.Length != 0)
                {
                    foreach (var typeName in typeAttributes.First().Types)
                    {
                        TypeHandlers.Add(StringHashHelper.HashItemName(typeName), type);
                    }
                }
            }
        }

        private static void LoadItems()
        {
            foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
            {
                GameBalance data = asset.Data as GameBalance;
                if (data != null && data.Type == BalanceType.Items)
                {
                    foreach (var itemDefinition in data.Item)
                    {
                        Items.Add(itemDefinition.Hash, itemDefinition);
                    }
                }
            }
        }

        private static void LoadEquips()
        {
            HashSet<int> EquipTypes = new HashSet<int>();

            foreach (var itemtype in ItemGroup.ItemTypes.Values)
            {
                if (itemtype.Flags.HasFlag(ItemFlags.NotEquipable1)) continue;
                if (itemtype.Flags.HasFlag(ItemFlags.NotEquipable2)) continue;
                if (itemtype.Flags.HasFlag(ItemFlags.Unknown)) continue;
                if (itemtype.Flags.HasFlag(ItemFlags.AtLeastMagical)) continue;
                if (itemtype.Name.ToLower().Contains("gold")) continue;

                EquipTypes.Add(itemtype.Hash);
            }

            foreach (var item in Items.Values)
            {
                if (!EquipTypes.Contains(item.ItemType1)) continue;
                if (item.Name.ToLower().Contains("unique")) continue;
                if (item.Name.ToLower().Contains("debug")) continue;

                Equips.Add(item.Hash, item);
            }
        }

        private static void LoadAtLeastMagics()
        {
            HashSet<int> MagicTypes = new HashSet<int>();
            foreach (var itemtype in ItemGroup.ItemTypes.Values)
            {
                if (itemtype.Flags.HasFlag(ItemFlags.AtLeastMagical))
                {
                    MagicTypes.Add(itemtype.Hash);
                }
            }

            foreach (var item in Items.Values)
            {
                if (MagicTypes.Contains(item.ItemType1))
                {
                    if (item.Name.ToLower().Contains("unique")) continue;

                    AtLeastMagic.Add(item.Hash, item);
                }
            }
        }

        private static void SetAllowedTypes()
        {
            foreach (int hash in ItemGroup.SubTypesToHashList("Weapon"))
                AllowedItemTypes.Add(hash);
            foreach (int hash in ItemGroup.SubTypesToHashList("Armor"))
                AllowedItemTypes.Add(hash);
            foreach (int hash in ItemGroup.SubTypesToHashList("Offhand"))
                AllowedItemTypes.Add(hash);
            foreach (int hash in ItemGroup.SubTypesToHashList("Jewelry"))
                AllowedItemTypes.Add(hash);
            foreach (int hash in ItemGroup.SubTypesToHashList("Utility"))
                AllowedItemTypes.Add(hash);
            foreach (int hash in ItemGroup.SubTypesToHashList("CraftingPlan"))
                AllowedItemTypes.Add(hash);
            foreach (int hash in TypeHandlers.Keys)
            {
                if (AllowedItemTypes.Contains(hash)) {
                    // already added structure
                    continue;
                }
                foreach (int subhash in ItemGroup.SubTypesToHashList(ItemGroup.FromHash(hash).Name))
                {
                    if (AllowedItemTypes.Contains(subhash)) {
                        // already added structure
                        continue;
                    }
                    AllowedItemTypes.Add(subhash);
                }
            }

        }

        // generates a random item.
        public static Item GenerateRandom(Mooege.Core.GS.Actors.Actor owner)
        {
            var itemDefinition = GetRandom(Items.Values.ToList());
            return CreateItem(owner, itemDefinition);
        }

        public static Item GenerateDrop(Mooege.Core.GS.Actors.Actor owner, Mooege.Core.GS.Actors.Monster monster)
        {
            ItemTable.ItemQuality quality = ItemTable.ItemQuality.Inferior;
            int level = monster.Attributes[GameAttribute.Level];
            int drop_window = level / 2;
            int rQuality = RandomHelper.Next(100) + drop_window;
            List<ItemTable> PickItem = new List<ItemTable>();
            PickItem = Equips.Values.ToList();

            //roll for normal
            if (rQuality > 30) quality = ItemTable.ItemQuality.Normal;
            if (rQuality > 60) quality = ItemTable.ItemQuality.Superior;
            if (rQuality > (85 - Math.Max(level / 3, 1))) //roll for magic
            {
                rQuality = RandomHelper.Next(100);
                drop_window = Math.Max(level / 3, 1);
                quality = ItemTable.ItemQuality.Magic1;
                if (rQuality > (100 - drop_window * 3)) quality = ItemTable.ItemQuality.Magic2;
                if (rQuality > (100 - drop_window * 2)) quality = ItemTable.ItemQuality.Magic3;
                if (rQuality > (100 - drop_window)) //roll for rare+legendary
                {
                    rQuality = RandomHelper.Next(100);
                    drop_window = Math.Max((level / 2) - 10, 1);
                    quality = ItemTable.ItemQuality.Rare4;
                    if (rQuality > (100 - drop_window * 3)) quality = ItemTable.ItemQuality.Rare5;
                    if (rQuality > (100 - drop_window * 2)) quality = ItemTable.ItemQuality.Rare6;
                    if (rQuality > (100 - drop_window)) quality = ItemTable.ItemQuality.Legendary;
                }
            }

            if (quality > ItemTable.ItemQuality.Superior)
                PickItem.AddRange(AtLeastMagic.Values.ToList());

            var itemDefinition = GetDrop(PickItem, monster);
            if (itemDefinition == null)
                return null;

            itemDefinition.Quality = quality;
            Item item = CreateItem(owner, itemDefinition);
            Logger.Info("level = {0} | itemname = {1}", item.ItemLevel, itemDefinition.Name);
            return item;
        }

        private static ItemTable GetDrop(List<ItemTable> pool, Mooege.Core.GS.Actors.Monster monster)
        {
            var found = false;
            ItemTable itemDefinition = null;
            while (!found)
            {
                if (pool.Count == 0)
                    return null;

                int r = RandomHelper.Next(0, pool.Count() - 1);
                itemDefinition = pool[r];
                pool.Remove(pool[r]);

                if (itemDefinition.SNOActor == -1) continue;
                if (itemDefinition.Name.ToLower().Contains("debug")) continue;
                if (itemDefinition.Name.ToLower().Contains("missing")) continue;
                if (itemDefinition.ItemLevel > monster.Attributes[GameAttribute.Level] + 2) continue;
                if (itemDefinition.ItemLevel < monster.Attributes[GameAttribute.Level] - 2) continue;

                found = true;
            }
            return itemDefinition;
        }

        // generates a random item from given type category.
        // we can also set a difficulty mode parameter here, but it seems current db doesnt have nightmare or hell-mode items with valid snoId's /raist.
        public static Item GenerateRandom(Mooege.Core.GS.Actors.Actor player, ItemTypeTable type)
        {
            var itemDefinition = GetRandom(Items.Values
                .Where(def => ItemGroup
                    .HierarchyToHashList(ItemGroup.FromHash(def.ItemType1)).Contains(type.Hash)).ToList());
            return CreateItem(player, itemDefinition);
        }

        private static ItemTable GetRandom(List<ItemTable> pool)
        {
            var found = false;
            ItemTable itemDefinition = null;

            while (!found)
            {
                itemDefinition = pool[RandomHelper.Next(0, pool.Count() - 1)];

                if (itemDefinition.SNOActor == -1) continue;
                
                // if ((itemDefinition.ItemType1 == StringHashHelper.HashItemName("Book")) && (itemDefinition.BaseGoldValue != 0)) return itemDefinition; // testing books /xsochor
                // if (itemDefinition.ItemType1 != StringHashHelper.HashItemName("Book")) continue; // testing books /xsochor
                // if (!ItemGroup.SubTypesToHashList("SpellRune").Contains(itemDefinition.ItemType1)) continue; // testing spellrunes /xsochor

                // ignore gold and healthglobe, they should drop only when expect, not randomly
                if (itemDefinition.Name.ToLower().Contains("gold")) continue;
                if (itemDefinition.Name.ToLower().Contains("healthglobe")) continue;
                if (itemDefinition.Name.ToLower().Contains("pvp")) continue;
                if (itemDefinition.Name.ToLower().Contains("unique")) continue;
                if (itemDefinition.Name.ToLower().Contains("crafted")) continue;
                if (itemDefinition.Name.ToLower().Contains("debug")) continue;
                if (itemDefinition.Name.ToLower().Contains("missing")) continue; //I believe I've seen a missing item before, may have been affix though. //Wetwlly
                if (itemDefinition.Name.ToLower().Contains("dye")) continue; //TODO: Fix these from crashing in-game when hovering over icon //Wetwlly
                if ((itemDefinition.ItemType1 == StringHashHelper.HashItemName("Book")) && (itemDefinition.BaseGoldValue == 0)) continue; // i hope it catches all lore with npc spawned /xsochor

                if (!GBIDHandlers.ContainsKey(itemDefinition.Hash) &&
                    !AllowedItemTypes.Contains(itemDefinition.ItemType1)) continue;
                              
                found = true;
            }

            return itemDefinition;
        }

        public static Type GetItemClass(ItemTable definition)
        {
            Type type = typeof(Item);

            if (GBIDHandlers.ContainsKey(definition.Hash))
            {
                type = GBIDHandlers[definition.Hash];
            }
            else
            {
                foreach (var hash in ItemGroup.HierarchyToHashList(ItemGroup.FromHash(definition.ItemType1)))
                {
                    if (TypeHandlers.ContainsKey(hash))
                    {
                        type = TypeHandlers[hash];
                        break;
                    }
                }
            }

            return type;
        }

        // Creates an item based on supplied definition.
        public static Item CreateItem(Mooege.Core.GS.Actors.Actor owner, ItemTable definition)
        {
            // Logger.Trace("Creating item: {0} [sno:{1}, gbid {2}]", definition.Name, definition.SNOActor, StringHashHelper.HashItemName(definition.Name));

            Type type = GetItemClass(definition);

            var item = (Item)Activator.CreateInstance(type, new object[] { owner.World, definition });

            return item;
        }

        // Allows cooking a custom item.
        public static Item Cook(Player player, string name)
        {
            int hash = StringHashHelper.HashItemName(name);
            ItemTable definition = Items[hash];
            return CookFromDefinition(player, definition);
        }

        // Allows cooking a custom item.
        public static Item CookFromDefinition(Player player, ItemTable definition)
        {
            Type type = GetItemClass(definition);

            var item = (Item)Activator.CreateInstance(type, new object[] { player.World, definition });
            //player.GroundItems[item.DynamicID] = item;

            return item;
        }

        public static Item DropGold(Player player)
        {
            int Min = Math.Max(player.Attributes[GameAttribute.Level] - 10, 1);
            int Max = player.Attributes[GameAttribute.Level] + 3;
            int amount = RandomHelper.Next(Min, Max);
            String goldname = "Gold1";

            if (amount > 15)
                goldname = "Gold2";
            if (amount > 50)
                goldname = "Gold3";
            if (amount > 100)
                goldname = "Gold4";
            if (amount > 250)
                goldname = "Gold5";

            var item = Cook(player, goldname);
            item.Attributes[GameAttribute.Gold] = amount;

            return item;
        }

        public static ItemTable GetItemDefinition(int gbid)
        {
            return (Items.ContainsKey(gbid)) ? Items[gbid] : null;
        }

        public static Item CreateGold(Player player, int amount)
        {
            var item = Cook(player, "Gold1");
            item.Attributes[GameAttribute.Gold] = amount;

            return item;
        }

        public static Item CreateGlobe(Player player, int amount)
        {
            if (amount > 10)
                amount = 10 + ((amount - 10) * 5);

            var item = Cook(player, "HealthGlobe" + amount);
            item.Attributes[GameAttribute.Health_Globe_Bonus_Health] = amount;

            return item;
        }

        public static bool IsValidItem(string name)
        {
            return Items.ContainsKey(StringHashHelper.HashItemName(name));
        }
    }

}

