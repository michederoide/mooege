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
            //The spawning positions for each monster in its wave. Basically, you add here the "number" of mobs, accoring to each vector LaunchWave() will spawn every mob in its position.
            Vector3D[] FirstSkinnyWaveCoords = { new Vector3D(2846.162f, 2962.202f, 24.10213f), new Vector3D(2847.069f, 2975.214f, 24.43476f), new Vector3D(2822.784f, 2956.344f, 23.94533f) };
            Vector3D[] TorsoWaveCoords = { new Vector3D(2820.969f, 2960.441f, 24.04534f), new Vector3D(2837.987f, 2974.384f, 24.29964f), new Vector3D(2855.945f, 2966.754f, 24.78838f), new Vector3D(2881.435f, 2968.71f, 27.64387f) };
            Vector3D[] SecondSkinnyWaveCoords = { new Vector3D(2891.899f, 2953.503f, 27.09192f), new Vector3D(2876.486f, 2969.06f, 27.63562f), new Vector3D(2877.566f, 2955.966f, 26.1463f), new Vector3D(2823.161f, 2956.929f, 23.94533f),
                                                    new Vector3D(2858.462f, 2969.751f, 24.81519f), new Vector3D(2835.552f, 2970.07f, 24.20798f), new Vector3D(2829.646f, 2963.888f, 24.08491f), new Vector3D(2862.353f, 2968.9f, 25.44267f),
                                                    new Vector3D(2861.012f, 2970.908f, 25.18782f) };

            //Disable RumFord so he doesn't offer the quest.
            setActorOperable(world, 3739, false);
            //Start the conversation between RumFord & Guard.
            StartConversation(world, 198199);
            //Launch first wave.
            var firstWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(FirstSkinnyWaveCoords, world, 218339));

            firstWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
            //Run Kill Event Listener
            var ListenerFirstWaveTask = Task<bool>.Factory.StartNew(() => OnKillListener(monstersAlive, world));
            //Wait for the mobs to be killed.
            ListenerFirstWaveTask.ContinueWith(delegate //Once killed:
            {
                //Wave two: Torsos.
                var torsoWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(TorsoWaveCoords, world, 218367));
                torsoWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
                var ListenerSecondWaveTask = Task<bool>.Factory.StartNew(() => OnKillListener(monstersAlive, world));
                ListenerSecondWaveTask.ContinueWith(delegate //Once killed:
                {
                    //Wave three: Skinnies + RumFord conversation #2
                    StartConversation(world, 80088);
                    var thirdWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(SecondSkinnyWaveCoords, world, 218339));
                    thirdWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
                    var ListenerThirdWaveTask = Task<bool>.Factory.StartNew(() => OnKillListener(monstersAlive, world));
                    ListenerThirdWaveTask.Wait();
                    Task.WaitAll();
                    //Event done we advance the quest and play last conversation #3.
                    world.Game.Quests.Advance(87700);
                    Logger.Debug("Event finished");
                    StartConversation(world, 151102);
                    setActorOperable(world, 3739, true);
                });
            });
        }

        //This is the way we Listen for mob killing events.
        private bool OnKillListener(List<uint> monstersAlive, Map.World world)
        {
            Int32 monstersKilled = 0;
            var monsterCount = monstersAlive.Count; //Since we are removing values while iterating, this is set at the first real read of the mob counting.
            while (monstersKilled != monsterCount)
            {
                //Iterate through monstersAlive List, if found dead we start to remove em till all of em are dead and removed.
                for (int i = monstersAlive.Count - 1; i >= 0; i--)
                {
                    if (world.HasMonster(monstersAlive[i]))
                    {
                        //Alive: Nothing.
                    }
                    else
                    {
                        //If dead we remove it from the list and keep iterating.
                        Logger.Debug(monstersAlive[i] + " has been killed");
                        monstersAlive.RemoveAt(i);
                        monstersKilled++;
                    }
                }
            }
            return true;
        }

        //We use this function to launch/spawn new mobs.
        private bool LaunchWave(Vector3D[] Coordinates, Map.World world, Int32 SnoId)
        {
            var counter = 0;
            var monsterSNOHandle = new Common.Types.SNO.SNOHandle(SnoId);
            var monsterActor = monsterSNOHandle.Target as Actor;

            foreach (Vector3D coords in Coordinates)
            {
                Parallel.ForEach(world.Players, player => //Threading because many spawns at once with out Parallel freezes D3.
                    {
                        var PRTransform = new PRTransform()
                        {
                            Quaternion = new Quaternion()
                            {
                                W = 0.7063466f,
                                Vector3D = new Vector3D(0, 0, 0)
                            },Vector3D = Coordinates[counter]
                        };

                        //Load the actor here.
                        var actor = WorldGenerator.loadActor(monsterSNOHandle, PRTransform, world, monsterActor.TagMap);
                        monstersAlive.Add(actor);
                        counter++;

                        //If Revealed play animation.
                        world.BroadcastIfRevealed(new PlayAnimationMessage
                        {
                            ActorID = actor,
                            Field1 = 3,
                            Field2 = 0,
                            tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                        {
                            new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                            {
                                Duration = 0x00000083,
                                AnimationSNO = 0x00002D03,
                                PermutationIndex = 0x00000000,
                                Speed = 1f
                            }
                        }
                        }, player.Value);
                    });
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
