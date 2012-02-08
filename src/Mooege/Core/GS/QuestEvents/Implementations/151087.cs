using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Common.MPQ.FileFormats;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Net.GS.Message.Definitions.Animation;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Generators;
using Mooege.Common.Logging;
using System.Threading.Tasks;
using System.Threading;
using Mooege.Core.GS.Common.Types.TagMap;


namespace Mooege.Core.GS.QuestEvents.Implementations
{
    class _151087 : QuestEvent
    {

        private static readonly Logger Logger = LogManager.CreateLogger();

        public _151087()
            : base(151087)
        {
        }

        List<uint> monstersAlive = new List<uint> { }; //We use this for the killeventlistener.
        public override void Execute(Map.World world)
        {

            //Disable RumFord so he doesn't offer the quest.
            setActorOperable(world, 3739, false);
            //Start the conversation between RumFord & Guard.
            StartConversation(world, 198199);
            //Launch first wave.
            var wave1Actors = world.GetActorsInGroup("GizmoGroup1");
            foreach (var actor in wave1Actors)
            {                
                actor.Spawn();                
            }

            //var firstWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(FirstSkinnyWaveCoords, world, 218339, "GizmoGroup0"));

            //firstWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
            //Run Kill Event Listener
            var ListenerFirstWaveTask = Task<bool>.Factory.StartNew(() => OnKillListener(world, "GizmoGroup1"));
            //ListenerFirstWaveTask.Wait();
            //Wait for the mobs to be killed.
            ListenerFirstWaveTask.ContinueWith(delegate //Once killed:
            {
                //Wave two: Torsos.
                //var torsoWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(TorsoWaveCoords, world, 218367, "GizmoGroup1"));
                //torsoWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
                //var ListenerSecondWaveTask = Task<bool>.Factory.StartNew(() => OnKillListener(world, "GizmoGroup1"));
                //ListenerSecondWaveTask.ContinueWith(delegate //Once killed:
                //{
                    //Wave three: Skinnies + RumFord conversation #2
                    StartConversation(world, 80088);
                    var wave2Actors = world.GetActorsInGroup("GizmoGroup2");
                    foreach (var actor in wave2Actors)
                    {
                        actor.Spawn();
                    }
                    //var thirdWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(SecondSkinnyWaveCoords, world, 218339, "GizmoGroup2"));
                    //thirdWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
                    var ListenerThirdWaveTask = Task<bool>.Factory.StartNew(() => OnKillListener(world, "GizmoGroup2"));
                    ListenerThirdWaveTask.Wait();
                    Task.WaitAll();
                    //Event done we advance the quest and play last conversation #3.
                    world.Game.Quests.Advance(87700);
                    Logger.Debug("Event finished");
                    StartConversation(world, 151102);
                    setActorOperable(world, 3739, true);
                //});
            });
        }

        //This is the way we Listen for mob killing events.
        private bool OnKillListener(Map.World world, string group)
        {
            while (world.HasActorsInGroup(group))
            {
            }
            return true;
        }


        //Launch Conversations.
        private bool StartConversation(Map.World world, Int32 conversationId)
        {
            foreach (var player in world.Players)
            {
                player.Value.Conversations.StartConversation(conversationId);
            }
            return true;
        }

        //Not Operable Rumford (To disable giving u the same quest while ur in the event)
        public static bool setActorOperable(Map.World world, Int32 snoId, bool status)
        {
            var actor = world.GetActorBySNO(snoId);
            foreach (var player in world.Players)
            {
                actor.Attributes[Net.GS.Message.GameAttribute.NPC_Is_Operatable] = status;
            }
            return true;
        }

    }
}
