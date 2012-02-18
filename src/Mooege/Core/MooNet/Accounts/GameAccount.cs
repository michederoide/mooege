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
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Mooege.Common.Storage;
using Mooege.Common.Helpers.Hash;
using Mooege.Core.Cryptography;
using Mooege.Core.MooNet.Friends;
using Mooege.Core.MooNet.Helpers;
using Mooege.Core.MooNet.Objects;
using Mooege.Core.MooNet.Toons;
using Mooege.Core.MooNet.Channels;
using Mooege.Net.MooNet;

namespace Mooege.Core.MooNet.Accounts
{
    public class GameAccount : PersistentRPCObject
    {
        public Account Owner { get; set; }

        public D3.OnlineService.EntityId D3GameAccountId { get; private set; }

        #region PresenceFields

        public ByteStringPresenceField<D3.Account.BannerConfiguration> BannerConfigurationField
            = new ByteStringPresenceField<D3.Account.BannerConfiguration>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.GameAccount, 1, 0);

        public ByteStringPresenceField<D3.OnlineService.EntityId> CurrentHeroIdField
            = new ByteStringPresenceField<D3.OnlineService.EntityId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.GameAccount, 2, 0);


        //TODO: Move this to the Channel class
        public IntPresenceField ScreenStatusField
            = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Channel, 2, 0);

        public BoolPresenceField GameAccountStatusField
            = new BoolPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 1, 0, false);

        public FourCCPresenceField ProgramField
            = new FourCCPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 4, 0);

        public IntPresenceField GameAccountStatusIdField
            = new IntPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 5, 0);

        public StringPresenceField BattleTagField
            = new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 6, 0);

        public StringPresenceField AccountField
            = new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 7, 0);

        public ByteStringPresenceField<bnet.protocol.EntityId> OwnerIdField
            = new ByteStringPresenceField<bnet.protocol.EntityId>(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 8, 0);


#endregion

        public FieldKeyHelper.Program Program;

        private D3.Account.BannerConfiguration _bannerConfiguration;
        public D3.Account.BannerConfiguration BannerConfiguration
        {
            get
            {
                return _bannerConfiguration;
            }
            set
            {
                _bannerConfiguration = value;
                BannerConfigurationField.Value = value;
            }
        }

        private D3.PartyMessage.ScreenStatus _screenstatus = D3.PartyMessage.ScreenStatus.CreateBuilder().SetScreen(0).SetStatus(0).Build();
        public D3.PartyMessage.ScreenStatus ScreenStatus
        {
            get
            {
                return _screenstatus;
            }
            set
            {
                _screenstatus = value;
                this.ScreenStatusField.Value = value.Status;
            }
        }
        /// <summary>
        /// Selected toon for current account.
        /// </summary>
        private Toon _currentToon;
        public Toon CurrentToon
        {
            get
            {
                return _currentToon;
            }
            set
            {
                this._currentToon = value;
                this.CurrentHeroIdField.Value = value.D3EntityID;
                this.Owner.LastSelectedHeroField.Value = value.D3EntityID;
                //TODO: Move this out
                this.Owner.SaveToDB();

                //Add new hero to "presence notification service"
                //Remove old hero
                RemovePresenceFieldsForSpecificClass(FieldKeyHelper.OriginatingClass.Hero);
                presenceFieldList.AddRange(this.CurrentToon.GetPresenceFields());
            }
        }

        private D3.Client.GameAccountSettings _settings = D3.Client.GameAccountSettings.CreateBuilder().Build();
        public D3.Client.GameAccountSettings Settings
        {
            get
            {
                return this._settings;
            }
            set
            {
                this._settings = value;
            }
        }
        /// <summary>
        /// Away status
        /// </summary>
        public AwayStatusFlag AwayStatus { get; private set; }

        public List<bnet.protocol.achievements.AchievementUpdateRecord> Achievements { get; set; }
        public List<bnet.protocol.achievements.CriteriaUpdateRecord> AchievementCriteria { get; set; }

        public D3.Profile.AccountProfile Profile
        {
            get
            {
                return D3.Profile.AccountProfile.CreateBuilder()
                    .Build();
            }
        }



        public Dictionary<ulong, Toon> Toons
        {
            get { return ToonManager.GetToonsForGameAccount(this); }
        }

        private static ulong? _persistentIdCounter = null;

        protected override ulong GenerateNewPersistentId()
        {
            if (_persistentIdCounter == null)
                _persistentIdCounter = GameAccountManager.GetNextAvailablePersistentId();

            return (ulong)++_persistentIdCounter;
        }

        /// <summary>
        /// Existing GameAccount
        /// </summary>
        /// <param name="persistentId"></param>
        /// <param name="accountId"></param>
        public GameAccount(ulong persistentId, ulong accountId)
            : base(persistentId)
        {
            //
            this.Owner = AccountManager.GetAccountByPersistentID(accountId);
            this.OwnerIdField.Value = Owner.BnetEntityId;
            this.SetField();
        }

        /// <summary>
        /// New GameAccount
        /// </summary>
        /// <param name="account"></param>
        public GameAccount(Account account)
            : base(account.BnetEntityId.Low)
        {
            this.Owner = account;
            this.OwnerIdField.Value = Owner.BnetEntityId;

            this.SetField();

            this.BannerConfiguration =
                D3.Account.BannerConfiguration.CreateBuilder()
                .SetBannerShape(189701627)
                .SetSigilMain(1494901005)
                .SetSigilAccent(3399297034)
                .SetPatternColor(1797588777)
                .SetBackgroundColor(1797588777)
                .SetSigilColor(2045456409)
                .SetSigilPlacement(1015980604)
                .SetPattern(4173846786)
                .SetUseSigilVariant(true)
                .Build();
        }

        private void SetField()
        {
            InitPresenceFields();

            var bnetGameAccountHigh = ((ulong)EntityIdHelper.HighIdType.GameAccountId) + (0x6200004433);
            this.BnetEntityId = bnet.protocol.EntityId.CreateBuilder().SetHigh(bnetGameAccountHigh).SetLow(this.PersistentID).Build();
            this.D3GameAccountId = D3.OnlineService.EntityId.CreateBuilder().SetIdHigh(bnetGameAccountHigh).SetIdLow(this.PersistentID).Build();

            //TODO: Now hardcode all game account notifications to D3
            this.ProgramField.Value = "D3";
            this.AccountField.Value = Owner.BnetEntityId.Low.ToString() + "#1";
            this.BattleTagField.Value = this.Owner.BattleTag;

            this.Achievements = new List<bnet.protocol.achievements.AchievementUpdateRecord>();
            this.AchievementCriteria = new List<bnet.protocol.achievements.CriteriaUpdateRecord>();

        }


        //TODO: Why do we need a logged in client in this class. Each logged in client should have a game account associated.
        private MooNetClient _loggedInClient;

        public MooNetClient LoggedInClient
        {
            get
            {
                return this._loggedInClient;
            }
            set
            {
                this._loggedInClient = value;

                this.GameAccountStatusField.Value = value != null;
                this.GameAccountStatusIdField.Value = (int)(this.GameAccountStatusField.Value == true ? 1324923597904795 : 0);

                //TODO: Remove this set once delegate for set is added to presence field
                this.Owner.AccountOnlineField.Value = this.Owner.IsOnline;
                var operation = this.Owner.AccountOnlineField.GetFieldOperation();
                this.NotifyUpdate();
                this.UpdateSubscribers(this.Subscribers, new List<bnet.protocol.presence.FieldOperation>() { operation });
            }
        }

        public D3.Account.Digest Digest
        {
            get
            {
                var builder = D3.Account.Digest.CreateBuilder().SetVersion(105) // 7447=>99, 7728=> 100, 8801=>102, 8296=>105
                    .SetBannerConfiguration(this.BannerConfigurationField.Value)
                    .SetFlags(0)
                    .SetLastPlayedHeroId(this.Owner.LastSelectedHeroField.Value);

                return builder.Build();
            }
        }

        #region Notifications
        //gameaccount
        //D3,GameAccount,1,0 -> D3.Account.BannerConfiguration
        //D3,GameAccount,2,0 -> ToonId
        //D3,Hero,1,0 -> Hero Class
        //D3,Hero,2,0 -> Hero's current level
        //D3,Hero,3,0 -> D3.Hero.VisualEquipment
        //D3,Hero,4,0 -> Hero's flags
        //D3,Hero,5,0 -> Hero Name
        //D3,Hero,6,0 -> Unk Int64 (0)
        //D3,Hero,7,0 -> Unk Int64 (0)
        //Bnet,GameAccount,1,0 -> GameAccount Online
        //Bnet,GameAccount,4,0 -> FourCC = "D3"
        //Bnet,GameAccount,5,0 -> Unk Int (0 if GameAccount is Offline)
        //Bnet,GameAccount,6,0 -> BattleTag
        //Bnet,GameAccount,7,0 -> Account.Low + "#1"
        //Bnet,GameAccount,8,0 -> Account.EntityId
        public void InitPresenceFields()
        {
            this.presenceFieldList = new List<PresenceFieldBase>();

            presenceFieldList.Add(this.BannerConfigurationField);
            //Not sure we need to add current hero from start need to be added here
            //TODO: This should not be here as a toon is not even selected yet.
            if (this.Owner.LastSelectedHeroField.Value != Account.AccountHasNoToons && this._currentToon != null)
            {
                presenceFieldList.Add(this.CurrentHeroIdField);
                presenceFieldList.AddRange(this.CurrentToon.GetPresenceFields());
            }

            presenceFieldList.Add(this.GameAccountStatusField);
            presenceFieldList.Add(this.ProgramField);
            presenceFieldList.Add(this.GameAccountStatusIdField);
            presenceFieldList.Add(this.BattleTagField);
            presenceFieldList.Add(this.AccountField);
            presenceFieldList.Add(this.OwnerIdField);
        }

        #endregion

        public void Update(bnet.protocol.presence.FieldOperation operation)
        {
            switch (operation.Operation)
            {
                case bnet.protocol.presence.FieldOperation.Types.OperationType.SET:
                    DoSet(operation.Field);
                    break;
                case bnet.protocol.presence.FieldOperation.Types.OperationType.CLEAR:
                    DoClear(operation.Field);
                    break;
                default:
                    Logger.Warn("No operation type.");
                    break;
            }
        }

        private void DoSet(bnet.protocol.presence.Field field)
        {
            var operation = bnet.protocol.presence.FieldOperation.CreateBuilder();

            var returnField = bnet.protocol.presence.Field.CreateBuilder().SetKey(field.Key);

            switch ((FieldKeyHelper.Program)field.Key.Program)
            {
                case FieldKeyHelper.Program.D3:
                    if (field.Key.Group == 4 && field.Key.Field == 1)
                    {
                        if (field.Value.HasMessageValue) //7727 Sends empty SET instead of a CLEAR -Egris
                        {
                            var entityId = D3.OnlineService.EntityId.ParseFrom(field.Value.MessageValue);
                            var channel = ChannelManager.GetChannelByEntityId(entityId);
                            if (this.LoggedInClient.CurrentChannel != channel)
                            {
                                this.LoggedInClient.CurrentChannel = channel;
                                returnField.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(channel.BnetEntityId.ToByteString()).Build());
                                Logger.Trace("{0} set channel to {1}", this, channel);
                            }
                        }
                        else
                        {
                            if (this.LoggedInClient.CurrentChannel != null)
                            {
                                returnField.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(Google.ProtocolBuffers.ByteString.Empty).Build());
                                Logger.Warn("Empty-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group, field.Key.Field);
                            }
                        }
                    }
                    else if (field.Key.Group == 4 && field.Key.Field == 2)
                    {
                        //catch to stop Logger.Warn spam on client start and exit
                        // should D3.4.2 int64 Current screen (0=in-menus, 1=in-menus, 3=in-menus); see ScreenStatus sent to ChannelService.UpdateChannelState call /raist
                        if (this.ScreenStatus.Screen != field.Value.IntValue)
                        {
                            this.ScreenStatus = D3.PartyMessage.ScreenStatus.CreateBuilder().SetScreen((int)field.Value.IntValue).SetStatus(0).Build();
                            returnField.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(field.Value.IntValue).Build());
                            Logger.Trace("{0} set current screen to {1}.", this, field.Value.IntValue);
                        }
                    }
                    else if (field.Key.Group == 4 && field.Key.Field == 3)
                    {
                        returnField.SetValue(field.Value);
                        //Looks to be the ToonFlags of the party leader/inviter when it is an int, OR the message set in an open to friends game when it is a string /dustinconrad
                    }
                    else if (field.Key.Group == 4 && field.Key.Field == 4)
                    {
                        returnField.SetValue(field.Value);
                        //this is some unknown bool
                    }
                    else
                    {
                        Logger.Warn("Unknown set-field: {0}, {1}, {2} := {3}", field.Key.Program, field.Key.Group, field.Key.Field, field.Value);
                    }
                    break;
                case FieldKeyHelper.Program.BNet:
                    if (field.Key.Group == 2 && field.Key.Field == 3) // Away status
                    {
                        this.AwayStatus = (AwayStatusFlag)field.Value.IntValue;
                        returnField.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue((long)this.AwayStatus).Build());
                        Logger.Trace("{0} set AwayStatus to {1}.", this, this.AwayStatus);
                    }
                    else
                    {
                        Logger.Warn("Unknown set-field: {0}, {1}, {2} := {3}", field.Key.Program, field.Key.Group, field.Key.Field, field.Value);
                    }
                    break;
            }

            //We only update subscribers on fields that actually change values.
            if (returnField.HasValue)
            {
                operation.SetField(returnField);
                this.UpdateSubscribers(this.Subscribers, new List<bnet.protocol.presence.FieldOperation>() { operation.Build() });
            }
        }

        private void DoClear(bnet.protocol.presence.Field field)
        {
            switch ((FieldKeyHelper.Program)field.Key.Program)
            {
                case FieldKeyHelper.Program.D3:
                    Logger.Warn("Unknown clear-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group, field.Key.Field);
                    break;
                case FieldKeyHelper.Program.BNet:
                    Logger.Warn("Unknown clear-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group, field.Key.Field);
                    break;
            }
        }

        public bnet.protocol.presence.Field QueryField(bnet.protocol.presence.FieldKey queryKey)
        {
            var field = bnet.protocol.presence.Field.CreateBuilder().SetKey(queryKey);

            switch ((FieldKeyHelper.Program)queryKey.Program)
            {
                case FieldKeyHelper.Program.D3:
                    if (queryKey.Group == 2 && queryKey.Field == 1) // Banner configuration
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.BannerConfigurationField.Value.ToByteString()).Build());
                    }
                    else if (queryKey.Group == 2 && queryKey.Field == 2) //Hero's EntityId
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.Owner.LastSelectedHeroField.Value.ToByteString()).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 1) // Hero's class (GbidClass)
                    {
                        //TODO: Set queryfields as normal persistent fields
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(this.CurrentToon.HeroClassFieldTransform((int)CurrentToon.HeroClassField.Value)).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 2) // Hero's current level
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(this.CurrentToon.HeroLevelField.Value).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 3) // Hero's visible equipment
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.CurrentToon.HeroVisualEquipmentField.Value.ToByteString()).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 4) // Hero's flags (gender and such)
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue((uint)((uint)this.CurrentToon.HeroFlagsField.Value | (uint)ToonFlags.AllUnknowns)).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 5) // Toon name
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetStringValue(this.CurrentToon.HeroNameField.Value).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 6)
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(0).Build());
                    }
                    else if (queryKey.Group == 3 && queryKey.Field == 7)
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(0).Build());
                    }
                    else if (queryKey.Group == 4 && queryKey.Field == 1) // Channel ID if the client is online
                    {
                        if (this.LoggedInClient != null && this.LoggedInClient.CurrentChannel != null) field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.LoggedInClient.CurrentChannel.D3EntityId.ToByteString()).Build());
                        else field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().Build());
                    }
                    else if (queryKey.Group == 4 && queryKey.Field == 2) // Current screen (all known values are just "in-menu"; also see ScreenStatuses sent in ChannelService.UpdateChannelState)
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(this.ScreenStatus.Screen).Build());
                    }
                    else if (queryKey.Group == 4 && queryKey.Field == 4) //Unknown Bool
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetBoolValue(false).Build());
                    }
                    else
                    {
                        Logger.Warn("GameAccount Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
                    }
                    break;
                case FieldKeyHelper.Program.BNet:
                    if (queryKey.Group == 2 && queryKey.Field == 1) //GameAccount Logged in
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetBoolValue(this.GameAccountStatusField.Value).Build());
                    }
                    else if (queryKey.Group == 2 && queryKey.Field == 3) // Away status
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue((long)this.AwayStatus).Build());
                    }
                    else if (queryKey.Group == 2 && queryKey.Field == 4) // Program - always D3
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetFourccValue("D3").Build());
                    }
                    else if (queryKey.Group == 2 && queryKey.Field == 6) // BattleTag
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetStringValue(this.Owner.BattleTag).Build());
                    }
                    else if (queryKey.Group == 2 && queryKey.Field == 8) // Account.EntityId
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetEntityidValue(this.Owner.BnetEntityId).Build());
                    }
                    else
                    {
                        Logger.Warn("GameAccount Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
                    }
                    break;
            }

            return field.HasValue ? field.Build() : null;
        }

        public override string ToString()
        {
            return String.Format("{{ GameAccount: {0} [lowId: {1}] }}", this.Owner.BattleTag, this.BnetEntityId.Low);
        }


        #region DB
        public void SaveToDB()
        {
            try
            {
                if (ExistsInDB())
                {
                    var query =
                        string.Format(
                            "UPDATE gameaccounts SET accountId={0}, banner=@banner WHERE id={1}",
                            this.Owner.PersistentID, this.PersistentID);

                    using (var cmd = new SQLiteCommand(query, DBManager.Connection))
                    {
                        cmd.Parameters.Add("@banner", System.Data.DbType.Binary).Value = this.BannerConfiguration.ToByteArray();
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    var query =
                        string.Format(
                            "INSERT INTO gameaccounts (id, accountId, banner) VALUES({0},{1}, @banner)",
                            this.PersistentID, this.Owner.PersistentID);

                    using (var cmd = new SQLiteCommand(query, DBManager.Connection))
                    {
                        cmd.Parameters.Add("@banner", System.Data.DbType.Binary).Value = this.BannerConfiguration.ToByteArray();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "GameAccount.SaveToDB()");
            }
        }

        public bool DeleteFromDB()
        {
            try
            {
                // Remove from DB
                if (!ExistsInDB()) return false;

                var query = string.Format("DELETE FROM gameaccounts WHERE id={0}", this.PersistentID);
                var cmd = new SQLiteCommand(query, DBManager.Connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "GameAccount.DeleteFromDB()");
                return false;
            }
        }

        private bool ExistsInDB()
        {
            var query =
                string.Format(
                    "SELECT id from gameaccounts where id={0}",
                    this.PersistentID);

            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();
            return reader.HasRows;
        }


        #endregion
        //TODO: figure out what 1 and 3 represent, or if it is a flag since all observed values are powers of 2 so far /dustinconrad
        public enum AwayStatusFlag : uint
        {
            Available = 0x00,
            UnknownStatus1 = 0x01,
            Away = 0x02,
            UnknownStatus2 = 0x03,
            Busy = 0x04
        }
    }
}
