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
using System.Threading;
using System.Threading.Tasks;

namespace Mooege.Core.GS.QuestEvents.Implementations
{
    class _151087 : QuestEvent
    {
        
        private static readonly Logger Logger = LogManager.CreateLogger();

        public _151087()
            : base(151087)
        {
        }

        public List<uint> monstersAlive = new List<uint> { }; //We use this for the killeventlistener.
        public override void Execute(Map.World world)
        {
            //The spawning positions for each monster in its wave. Basically, you add here the "number" of mobs, accoring to each vector LaunchWave() will spawn every mob in its position.
            Vector3D[] FirstSkinnyWaveCoords = { new Vector3D(2846.162f, 2962.202f, 24.10213f), new Vector3D(2847.069f, 2975.214f, 24.43476f), new Vector3D(2822.784f, 2956.344f, 23.94533f) };
            Vector3D[] TorsoWaveCoords = { new Vector3D(2820.969f, 2960.441f, 24.04534f), new Vector3D(2837.987f, 2974.384f, 24.29964f), new Vector3D(2855.945f, 2966.754f, 24.78838f), new Vector3D(2881.435f, 2968.71f, 27.64387f) };
            Vector3D[] SecondSkinnyWaveCoords = { new Vector3D(2891.899f, 2953.503f, 27.09192f), new Vector3D(2876.486f, 2969.06f, 27.63562f), new Vector3D(2877.566f, 2955.966f, 26.1463f) };

            //Launch first wave.
            var firstWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(FirstSkinnyWaveCoords, world, 0x000354E3));

            //Hack conversation.
            foreach (var player in world.Players) 	
            { 	
               player.Value.Conversations.StartConversation(198199); 	
            }

            firstWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
            //Run Kill Event Listener
            var ListenerFirstWaveTask = Task<bool>.Factory.StartNew(() => KillEventListener(monstersAlive,world));
            //Wait for the mobs to be killed.
            ListenerFirstWaveTask.ContinueWith(delegate //Once killed:
            {
                //Wave two: Torsos.
                var torsoWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(TorsoWaveCoords, world, 0x000354FF));
                torsoWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
                var ListenerSecondWaveTask = Task<bool>.Factory.StartNew(() => KillEventListener(monstersAlive, world));
                ListenerSecondWaveTask.ContinueWith(delegate //Once killed:
                {
                    //Wave three: Skinnies
                    var thirdWaveTask = Task<bool>.Factory.StartNew(() => LaunchWave(TorsoWaveCoords, world, 0x000354E3));
                    thirdWaveTask.Wait(); //We need to wait in order for the listener to grab the Monster counting, if this runs asyn with the spawn procedure listener will grab a value of 0 mobs.
                    var ListenerThirdWaveTask = Task<bool>.Factory.StartNew(() => KillEventListener(monstersAlive, world));
                    ListenerThirdWaveTask.Wait();
                    //Event done we advance the quest.
                    world.Game.Quests.Advance(87700);
                    Logger.Debug("Event finished");
                });
            });          
        }

        //This is the way we Listen for mob killing events.
        private bool KillEventListener(List<uint> monstersAlive, Map.World world)
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
                var PRTransform = new PRTransform()
                {
                    Quaternion = new Quaternion()
                    {
                        W = 0.7063466f,
                        Vector3D = new Vector3D(0, 0, 0)
                    },
                    Vector3D = Coordinates[counter]
                };
                var actor = WorldGenerator.loadActor(monsterSNOHandle, PRTransform, world, monsterActor.TagMap);
                monstersAlive.Add(actor);
                counter++;
            }
            return true;
        }
    }
}
