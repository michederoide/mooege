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
using Mooege.Common.Storage;
using Mooege.Common.Helpers.Hash;
using Mooege.Core.Cryptography;
using Mooege.Core.MooNet.Friends;
using Mooege.Core.MooNet.Helpers;
using Mooege.Core.MooNet.Objects;
using Mooege.Core.MooNet.Toons;
using Mooege.Net.MooNet;

namespace Mooege.Core.MooNet.Accounts
{
    public class Account : PersistentRPCObject
    {
        //public D3.PartyMessage.ScreenStatus ScreenStatus { get; set; }


        #region PresenceFields
        public ByteStringPresenceField<D3.OnlineService.EntityId> SelectedGameAccountField
            = new ByteStringPresenceField<D3.OnlineService.EntityId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Account, 2, 0);

        public StringPresenceField RealIDTagField
            = new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 1, 0);

        //Probably blizzard will move this to gameaccount soon
        public ByteStringPresenceField<D3.OnlineService.EntityId> LastSelectedHeroField
            = new ByteStringPresenceField<D3.OnlineService.EntityId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Account, 1, 0);

        public BoolPresenceField AccountOnlineField
            = new BoolPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 2, 0);

        public StringPresenceField AccountBattleTagField
            = new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 5, 0);

        public EntityIdPresenceFieldList GameAccountListField
            = new EntityIdPresenceFieldList(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 4);
        #endregion

        public bool IsOnline
        {
            get
            {
                //check if anygameAccounts are online
                foreach (var gameAccount in GameAccounts)
                {
                    if (gameAccount.Value.GameAccountStatusField.Value) return true;
                }
                return false;
            }
        }

        public string Email { get; private set; } // I - Username
        public byte[] Salt { get; private set; }  // s- User's salt.
        public byte[] PasswordVerifier { get; private set; } // v - password verifier.

        //TODO: Eliminate this variable as it is used inside the Field property
        private string _name;
        public string Name
        {
            get
            {
                return this._name;
            }
            private set
            {
                this._name = value;
                this.RealIDTagField.Value = this.Name;
                this.AccountBattleTagField.Value = Name + "#" + HashCode.ToString("D4");
            }
        }

        public int HashCode { get; private set; }
        public string BattleTag
        {
            get
            {
                return Name + "#" + HashCode.ToString("D4");
            }
            set
            {
                if (!value.Contains('#'))
                    throw new Exception("BattleTag must contain '#'");

                var split = value.Split('#');
                this.Name = split[0];
                this.HashCode = Convert.ToInt32(split[1]);
                this.AccountBattleTagField.Value = Name + "#" + HashCode.ToString("D4");
            }
        }

        public UserLevels UserLevel { get; private set; } // user level for account.

        //TODO: Move this out as Accounts need to be loaded before GameAccounts are and then the Owner inside GameAccount can't be properly set if
        //this is not fully loaded.
        public Dictionary<ulong, GameAccount> GameAccounts
        {
            get { return GameAccountManager.GetGameAccountsForAccount(this); }
        }

        //TODO: Eliminate completly this variable as it is stored already in the persistence field;
        private GameAccount _currentGameAccount;
        public GameAccount CurrentGameAccount
        {
            get
            {
                return this._currentGameAccount;
            }
            set
            {
                this.LastSelectedGameAccount = value.D3GameAccountId;
                this.SelectedGameAccountField.Value = value.D3GameAccountId;
                this._currentGameAccount = value;
            }
        }

        public static readonly D3.OnlineService.EntityId AccountHasNoToons =
            D3.OnlineService.EntityId.CreateBuilder().SetIdHigh(0).SetIdLow(0).Build();

        private D3.OnlineService.EntityId _lastSelectedGameAccount = AccountHasNoToons;
        public D3.OnlineService.EntityId LastSelectedGameAccount
        {
            get
            {
                if (_lastSelectedGameAccount == AccountHasNoToons && this.GameAccounts.Count > 0)
                    _lastSelectedGameAccount = this.SelectedGameAccountField.Value = this.GameAccounts.First().Value.D3GameAccountId;
                return _lastSelectedGameAccount;
            }
            set
            {
                _lastSelectedGameAccount = value;
                this.SelectedGameAccountField.Value = value;

            }
        }

        public Account(ulong persistentId, string email, byte[] salt, byte[] passwordVerifier, string battleTagName, int hashCode, UserLevels userLevel) // Account with given persistent ID
            : base(persistentId)
        {
            this.LastSelectedHeroField.Value = Account.AccountHasNoToons;
            LoadGameAccountIds();
            this.SetFields(email, salt, passwordVerifier, battleTagName, hashCode, userLevel);
        }

        public Account(string email, string password, string battleTagName, int hashCode, UserLevels userLevel) // Account with **newly generated** persistent ID
            : base(StringHashHelper.HashIdentity(battleTagName + "#" + hashCode.ToString("D4")))
        {
            this.LastSelectedHeroField.Value = Account.AccountHasNoToons;
            LoadGameAccountIds();

            if (password.Length > 16) password = password.Substring(0, 16); // make sure the password does not exceed 16 chars.

            var salt = SRP6a.GetRandomBytes(32);
            var passwordVerifier = SRP6a.CalculatePasswordVerifierForAccount(email, password, salt);

            this.SetFields(email, salt, passwordVerifier, battleTagName, hashCode, userLevel);
        }

        private static ulong? _persistentIdCounter = null;
        protected override ulong GenerateNewPersistentId()
        {
            if (_persistentIdCounter == null)
                _persistentIdCounter = AccountManager.GetNextAvailablePersistentId();

            return (ulong)++_persistentIdCounter;
        }

        private void SetFields(string email, byte[] salt, byte[] passwordVerifier, string battleTagName, int hashCode, UserLevels userLevel)
        {
            InitPresenceFields();

            this.Email = email;
            this.Salt = salt;
            this.PasswordVerifier = passwordVerifier;
            this.UserLevel = userLevel;

            this.BnetEntityId = bnet.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.AccountId).SetLow(this.PersistentID).Build();

            this.Name = battleTagName;
            this.HashCode = hashCode;
            this.AccountBattleTagField.Value = Name + "#" + HashCode.ToString("D4");

        }

        public bnet.protocol.presence.Field QueryField(bnet.protocol.presence.FieldKey queryKey)
        {
            var field = bnet.protocol.presence.Field.CreateBuilder().SetKey(queryKey);

            switch ((FieldKeyHelper.Program)queryKey.Program)
            {
                case FieldKeyHelper.Program.D3:
                    if (queryKey.Group == 1 && queryKey.Field == 1) // Account's last selected toon.
                    {
                        if (this.IsOnline) // check if the account is online actually.
                            field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.LastSelectedHeroField.Value.ToByteString()).Build());
                    }
                    else if (queryKey.Group == 1 && queryKey.Field == 2) // Account's last selected Game Account
                    {
                        if (this.IsOnline) // check if the account is online actually.
                            field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetMessageValue(this.LastSelectedGameAccount.ToByteString()).Build());
                    }
                    else
                    {
                        Logger.Warn("Account Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
                    }
                    break;
                case FieldKeyHelper.Program.BNet:
                    if (queryKey.Group == 1 && queryKey.Field == 5) // Account's battleTag
                    {
                        field.SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetStringValue(this.BattleTag).Build());
                    }
                    else
                    {
                        Logger.Warn("Account Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
                    }
                    break;
            }


            return field.HasValue ? field.Build() : null;
        }

        #region Notifications

        public void InitPresenceFields()
        {
            this.presenceFieldList = new List<PresenceFieldBase>();

            
            //TODO: Create delegate inside Persistence field so IsOnline can be removed
            //this.AccountOnlineField.Value = this.IsOnline;

            presenceFieldList.Add(this.LastSelectedHeroField);
            presenceFieldList.Add(this.SelectedGameAccountField);
            presenceFieldList.Add(this.RealIDTagField);
            presenceFieldList.Add(this.AccountOnlineField);
            presenceFieldList.Add(this.GameAccountListField);
            presenceFieldList.Add(this.AccountBattleTagField);
        }

        //account class generated
        //D3, Account,1,0 -> D3.OnlineService.EntityId: Last Played Hero
        //D3, Account,2,0 -> LastSelectedGameAccount
        //Bnet, Account,1,0 -> RealId Name
        //Bnet, Account,2,0 -> Account Online
        //Bnet, Account,4,index -> GameAccount EntityIds
        //Bnet, Account,5,0 -> BattleTag

        public override List<bnet.protocol.presence.FieldOperation> GetSubscriptionNotifications()
        {
            //TODO: Create delegate inside Persistence field so IsOnline can be removed
            this.AccountOnlineField.Value = this.IsOnline;
            //TODO: Create delegate-move this out
            this.GameAccountListField.Value.Clear();
            foreach (var pair in this.GameAccounts.Values)
            {
                this.GameAccountListField.Value.Add(pair.BnetEntityId);
            }


            var operationList = new List<bnet.protocol.presence.FieldOperation>();

            if (this.LastSelectedHeroField.Value != AccountHasNoToons)
                operationList.Add(this.LastSelectedHeroField.GetFieldOperation());
            if (this.LastSelectedGameAccount != AccountHasNoToons)
                operationList.Add(this.SelectedGameAccountField.GetFieldOperation());
            operationList.Add(this.RealIDTagField.GetFieldOperation());
            operationList.Add(this.AccountOnlineField.GetFieldOperation());
            operationList.AddRange(this.GameAccountListField.GetFieldOperationList());
            operationList.Add(this.AccountBattleTagField.GetFieldOperation());

            return operationList;
        }



        #endregion

        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (password.Length < 8 || password.Length > 16)
                return false;

            var calculatedVerifier = SRP6a.CalculatePasswordVerifierForAccount(this.Email, password, this.Salt);
            return calculatedVerifier.SequenceEqual(this.PasswordVerifier);
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
                            "UPDATE accounts SET email='{0}', salt=@salt, passwordVerifier=@passwordVerifier, battletagname='{1}', hashcode={2}, userLevel={3}, LastSelectedHeroId={4} WHERE id={5}",
                            this.Email, this.Name, this.HashCode, (byte)this.UserLevel, this.LastSelectedHeroField.Value.IdLow, this.PersistentID);

                    using (var cmd = new SQLiteCommand(query, DBManager.Connection))
                    {
                        cmd.Parameters.Add("@salt", System.Data.DbType.Binary, 32).Value = this.Salt;
                        cmd.Parameters.Add("@passwordVerifier", System.Data.DbType.Binary, 128).Value = this.PasswordVerifier;
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {

                    var query = string.Format("INSERT INTO accounts (id, email, salt, passwordVerifier, battletagname, hashcode, userLevel, LastSelectedHeroId) VALUES({0}, '{1}', @salt, @passwordVerifier, '{2}', {3}, {4}, {5})",
                            this.PersistentID, this.Email, this.Name, this.HashCode, (byte)this.UserLevel, this.LastSelectedHeroField.Value.IdLow);

                    using (var cmd = new SQLiteCommand(query, DBManager.Connection))
                    {
                        cmd.Parameters.Add("@salt", System.Data.DbType.Binary, 32).Value = this.Salt;
                        cmd.Parameters.Add("@passwordVerifier", System.Data.DbType.Binary, 128).Value = this.PasswordVerifier;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "SaveToDB()");
            }
        }

        public void LoadGameAccountIds()
        {
            var query = string.Format("SELECT id from gameaccounts WHERE accountid={0}", this.PersistentID);
            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();

            if (!reader.HasRows) return;

            while (reader.Read())
            {
                var id = reader.GetInt64(0);
                var bnetGameAccountHigh = ((ulong)EntityIdHelper.HighIdType.GameAccountId) + (0x6200004433);
                var bnetGameAccountEntityId = bnet.protocol.EntityId.CreateBuilder().SetHigh(bnetGameAccountHigh).SetLow((ulong)id).Build();
                this.GameAccountListField.Value.Add(bnetGameAccountEntityId);
            }

        }

        public void UpdatePassword(string newPassword)
        {
            this.PasswordVerifier = SRP6a.CalculatePasswordVerifierForAccount(this.Email, newPassword, this.Salt);
            try
            {
                var query = string.Format("UPDATE accounts SET passwordVerifier=@passwordVerifier WHERE id={0}", this.PersistentID);

                using (var cmd = new SQLiteCommand(query, DBManager.Connection))
                {
                    cmd.Parameters.Add("@passwordVerifier", System.Data.DbType.Binary, 128).Value = this.PasswordVerifier;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "UpdatePassword()");
            }
        }

        public void UpdateUserLevel(UserLevels userLevel)
        {
            this.UserLevel = userLevel;
            try
            {
                var query = string.Format("UPDATE accounts SET userLevel={0} WHERE id={1}", (byte)userLevel, this.PersistentID);
                var cmd = new SQLiteCommand(query, DBManager.Connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.ErrorException(e, "UpdateUserLevel()");
            }
        }

        private bool ExistsInDB()
        {
            var query =
                string.Format(
                    "SELECT id from accounts where id={0}",
                    this.PersistentID);

            var cmd = new SQLiteCommand(query, DBManager.Connection);
            var reader = cmd.ExecuteReader();
            return reader.HasRows;
        }

#endregion

        public override string ToString()
        {
            return String.Format("{{ Account: {0} [lowId: {1}] }}", this.Email, this.BnetEntityId.Low);
        }

        /// <summary>
        /// User-levels.
        /// </summary>
        public enum UserLevels : byte
        {
            User,
            GM,
            Admin,
            Owner
        }
    }
}
