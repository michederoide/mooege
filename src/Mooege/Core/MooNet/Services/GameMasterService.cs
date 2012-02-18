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
using Google.ProtocolBuffers;
using Mooege.Common.Logging;
using Mooege.Core.MooNet.Games;
using Mooege.Core.MooNet.Helpers;
using Mooege.Core.MooNet.Accounts;
using Mooege.Core.MooNet.Toons;
using Mooege.Net.MooNet;

namespace Mooege.Core.MooNet.Services
{
    [Service(serviceID: 0x7, serviceName: "bnet.protocol.game_master.GameMaster")]
    public class GameMasterService : bnet.protocol.game_master.GameMaster, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public MooNetClient Client { get; set; }
        public bnet.protocol.Header LastCallHeader { get; set; }

        public override void JoinGame(IRpcController controller, bnet.protocol.game_master.JoinGameRequest request, Action<bnet.protocol.game_master.JoinGameResponse> done)
        {
            Logger.Trace("Client {0} attempted to join game {1}.", this.Client, request.GameHandle.GameId.Low);
            //var game = GameFactoryManager.FindGameByEntityId(request.GameHandle.GameId);

            var builder = bnet.protocol.game_master.JoinGameResponse.CreateBuilder();
                //.AddConnectInfo(game.GetConnectionInfoForClient(this.Client));

            done(builder.Build());
            //throw new NotImplementedException();
        }

        public override void ListFactories(IRpcController controller, bnet.protocol.game_master.ListFactoriesRequest request, Action<bnet.protocol.game_master.ListFactoriesResponse> done)
        {
            Logger.Trace("ListFactories() {0}", this.Client);

            var description = bnet.protocol.game_master.GameFactoryDescription.CreateBuilder().SetId(14249086168335147635);
            var attributes = new bnet.protocol.attribute.Attribute[4]
                {
                    bnet.protocol.attribute.Attribute.CreateBuilder().SetName("min_players").SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(2)).Build(),
                    bnet.protocol.attribute.Attribute.CreateBuilder().SetName("max_players").SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(4)).Build(),
                    bnet.protocol.attribute.Attribute.CreateBuilder().SetName("num_teams").SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(1)).Build(),
                    bnet.protocol.attribute.Attribute.CreateBuilder().SetName("version").SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetStringValue("0.6.0")).Build() //This should be a static string so all versions are the same -Egris
                };

            description.AddRangeAttribute(attributes);
            description.AddStatsBucket(bnet.protocol.game_master.GameStatsBucket.CreateBuilder()
               .SetBucketMin(0)
               .SetBucketMax(4267296)
               .SetWaitMilliseconds(1354)
               .SetGamesPerHour(0)
               .SetActiveGames(69)
               .SetActivePlayers(75)
               .SetFormingGames(5)
               .SetWaitingPlayers(0).Build());

            var builder = bnet.protocol.game_master.ListFactoriesResponse.CreateBuilder().AddDescription(description).SetTotalResults(1);
            done(builder.Build());
        }

        public override void FindGame(IRpcController controller, bnet.protocol.game_master.FindGameRequest request, Action<bnet.protocol.game_master.FindGameResponse> done)
        {            
            Logger.Trace("FindGame() {0}", this.Client);

            // find the game.
            var gameFound = GameFactoryManager.FindGame(this.Client, request, ++GameFactoryManager.RequestIdCounter);
            //TODO: Find out why on rejoin game this is null
            if (Client.CurrentChannel != null)
            {
                //TODO: All these ChannelState updates can be moved to functions someplace else after packet flow is discovered and working -Egris
                //Send current JoinPermission to client before locking it
                var channelStatePermission = bnet.protocol.channel.ChannelState.CreateBuilder()
                    .AddAttribute(bnet.protocol.attribute.Attribute.CreateBuilder()
                    .SetName("D3.Party.JoinPermissionPreviousToLock")
                    .SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetIntValue(1).Build())
                    .Build()).Build();

                var notificationPermission = bnet.protocol.channel.UpdateChannelStateNotification.CreateBuilder()
                    .SetAgentId(this.Client.Account.CurrentGameAccount.BnetEntityId)
                    .SetStateChange(channelStatePermission)
                    .Build();

                this.Client.MakeTargetedRPC(Client.CurrentChannel, () =>
                    bnet.protocol.channel.ChannelSubscriber.CreateStub(this.Client).NotifyUpdateChannelState(null, notificationPermission, callback => { }));
            }
            var builder = bnet.protocol.game_master.FindGameResponse.CreateBuilder().SetRequestId(gameFound.RequestId);
            done(builder.Build());

            var clients = new List<MooNetClient>();
            foreach (var player in request.PlayerList)
            {
                foreach (var gameAccount in GameAccountManager.GameAccountsList)
                {
                    if (player.Identity.GameAccountId.Low == gameAccount.BnetEntityId.Low)
                    {
                        clients.Add(gameAccount.LoggedInClient);
                    }
                }
            }

            // send game found notification.
            var notification = bnet.protocol.notification.Notification.CreateBuilder()
                .SetSenderId(bnet.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.GameAccountId).SetLow(0).Build())
                .SetTargetId(this.Client.Account.CurrentGameAccount.BnetEntityId)
                .SetType("MM_START");
            var attr = bnet.protocol.attribute.Attribute.CreateBuilder()
                .SetName("game_request_id")
                .SetValue(bnet.protocol.attribute.Variant.CreateBuilder().SetUintValue(gameFound.RequestId).Build());
            notification.AddAttribute(attr);

            this.Client.MakeRPC(() =>
                bnet.protocol.notification.NotificationListener.CreateStub(this.Client).OnNotificationReceived(null, notification.Build(), callback => { }));

            if(gameFound.Started)
            {
                Logger.Warn("Client {0} joining game with FactoryID:{1}", this.Client.Account.CurrentGameAccount.CurrentToon.HeroNameField.Value, gameFound.FactoryID);
                gameFound.JoinGame(clients, request.FactoryObjectId);
            }
            else
            {
                Logger.Warn("Client {0} creating new game", this.Client.Account.CurrentGameAccount.CurrentToon.HeroNameField.Value);
                gameFound.StartGame(clients, request.FactoryObjectId);
            }
        }

        public override void CancelGame(IRpcController controller, bnet.protocol.game_master.CancelGameRequest request, Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }
        public override void GameEnded(IRpcController controller, bnet.protocol.game_master.GameEndedNotification request, Action<bnet.protocol.NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }

        public override void PlayerLeft(IRpcController controller, bnet.protocol.game_master.PlayerLeftNotification request, Action<bnet.protocol.NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }

        public override void RegisterServer(IRpcController controller, bnet.protocol.game_master.RegisterServerRequest request, Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void UnregisterServer(IRpcController controller, bnet.protocol.game_master.UnregisterServerRequest request, Action<bnet.protocol.NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }

        public override void RegisterUtilities(IRpcController controller, bnet.protocol.game_master.RegisterUtilitiesRequest request, Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void UnregisterUtilities(IRpcController controller, bnet.protocol.game_master.UnregisterUtilitiesRequest request, Action<bnet.protocol.NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }

        public override void Subscribe(IRpcController controller, bnet.protocol.game_master.SubscribeRequest request, Action<bnet.protocol.game_master.SubscribeResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe(IRpcController controller, bnet.protocol.game_master.UnsubscribeRequest request, Action<bnet.protocol.NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }

        public override void ChangeGame(IRpcController controller, bnet.protocol.game_master.ChangeGameRequest request, Action<bnet.protocol.NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void GetFactoryInfo(IRpcController controller, bnet.protocol.game_master.GetFactoryInfoRequest request, Action<bnet.protocol.game_master.GetFactoryInfoResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetGameStats(IRpcController controller, bnet.protocol.game_master.GetGameStatsRequest request, Action<bnet.protocol.game_master.GetGameStatsResponse> done)
        {
            var response = bnet.protocol.game_master.GetGameStatsResponse.CreateBuilder()
                .AddStatsBucket(GameFactoryManager.GetGameStats(request).Build())
                .Build();

            done(response);
        }
    }
}
