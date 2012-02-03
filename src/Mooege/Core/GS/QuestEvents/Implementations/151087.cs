using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Common.MPQ.FileFormats;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Generators;


namespace Mooege.Core.GS.QuestEvents.Implementations
{
    class _151087 :QuestEvent
    {
        public _151087()
            : base(151087)
        {
        }

        public override void Execute(Map.World world)
        {
            /*ZombieSkinny's Spawn
                ActorID: 0x796400CA  ZombieSkinny_Custom_A.acr (2036596938) //this is for zombieskinny #1
                 Field1: 0x00000003 (3)
                 Field2: 0
                 tAnim:
                 {
                  PlayAnimationMessageSpec:
                  {
                   Duration: 0x00000083 (131 ticks)
                   AnimationSNO: 0x00002D03:zombie_male_skinny_spawn.ani
                   PermutationIndex: 0x00000000 (0)
                   Speed: 1.938229
                  }

            #region ZombieSkinny_Custom_A #1
            //ActorID: 0x796400CA
            //Script code comes here
            //Scale 1.35
            var actorSNOHandle = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor = actorSNOHandle.Target as Actor;
            var PRTransform = new PRTransform();
            PRTransform.Quaternion = new Quaternion();
            PRTransform.Quaternion.W = 0.7063466f;
            PRTransform.Quaternion.Vector3D = new Vector3D(0, 0, -0.7078662f);
            PRTransform.Vector3D = new Vector3D(2846.162f, 2962.202f, 24.10213f);

            var actorID = WorldGenerator.loadActor(new Common.Types.SNO.SNOHandle(0x000354E3), PRTransform, world, actor.TagMap);

            

            var translateMessage = new ACDTranslateNormalMessage();
            translateMessage.Angle = 3.37346f;
            // Speed.Value: 0.108
            translateMessage.ActorId = (int)actorID;
            translateMessage.AnimationTag = 0x00011070;
            translateMessage.Position = new Vector3D(2844.901f, 2961.904f, 24.24138f);
            //Send message to all players

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(translateMessage, player.Value);
            }
            //There are no other normals
            //world.GetActorByDynamicId(actorID).Move(new Vector3D(2850.241f, 2972.256f, 24.69642f), 0);
            #endregion
            #region ZombieSkinny_Custom_A #2
            //Script code comes here
            //ActorID: 0x796500B7  
            //Scale 1.35
            var actorSNOHandle = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor = actorSNOHandle.Target as Actor;
            var PRTransform = new PRTransform();
            PRTransform.Quaternion = new Quaternion();
            PRTransform.Quaternion.W = 0.6130235f;
            PRTransform.Quaternion.Vector3D = new Vector3D(0, 0, -0.7900648f);
            PRTransform.Vector3D = new Vector3D(2847.069f, 2975.214f, 24.43476f);

            var actorID = WorldGenerator.loadActor(new Common.Types.SNO.SNOHandle(0x000354E3), PRTransform, world, actor.TagMap);

            var translateMessage = new ACDTranslateNormalMessage();
            translateMessage.Angle = 0f;
            // Speed.Value: 0.108
            translateMessage.ActorId = (int)actorID;
            translateMessage.AnimationTag = 0x00011070;
            translateMessage.Position = new Vector3D(2848.365f, 2975.214f, 24.69485f);
            //Send message to all players

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(translateMessage, player.Value);
            }
            //There are no other normals
            //world.GetActorByDynamicId(actorID).Move(new Vector3D(2850.241f, 2972.256f, 24.69642f), 0);
            #endregion
            #region ZombieSkinny_Custom_A #3
            //Script code comes here
            //ActorID: 0x79660031
            //Scale 1.35
            var actorSNOHandle = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor = actorSNOHandle.Target as Actor;
            var PRTransform = new PRTransform();
            PRTransform.Quaternion = new Quaternion();
            PRTransform.Quaternion.W = 0.8882556f;
            PRTransform.Quaternion.Vector3D = new Vector3D(0, 0, -0.4593496f);
            PRTransform.Vector3D = new Vector3D(2822.784f, 2956.344f, 23.94533f);

            var actorID = WorldGenerator.loadActor(new Common.Types.SNO.SNOHandle(0x000354E3), PRTransform, world, actor.TagMap);

            var translateMessage = new ACDTranslateNormalMessage();
            translateMessage.Angle = 5.028199f;
            // Speed.Value: 0.108
            translateMessage.ActorId = (int)actorID;
            translateMessage.AnimationTag = 0x00011070;
            translateMessage.Position = new Vector3D(2823.187f, 2955.112f, 24.04533f);
            //Send message to all players

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(translateMessage, player.Value);
            }
            //There are no other normals
            //world.GetActorByDynamicId(actorID).Move(new Vector3D(2850.241f, 2972.256f, 24.69642f), 0);
            #endregion

            #region ZombieCrawlerBarricade #1
            //ActorID: 0x798500CA  
            //Scale: 1.6
            //Script code comes here
            var actorSNOHandle = new Common.Types.SNO.SNOHandle(0x000354FF);
            var actor = actorSNOHandle.Target as Actor;
            var PRTransform = new PRTransform();
            PRTransform.Quaternion = new Quaternion();
            PRTransform.Quaternion.W = 0.826175f;
            PRTransform.Quaternion.Vector3D = new Vector3D(0, 0, -0.5634137f);
            PRTransform.Vector3D = new Vector3D(2833.75f, 2978.75f, 24.07898f);

            var actorID = WorldGenerator.loadActor(new Common.Types.SNO.SNOHandle(0x000354FF), PRTransform, world, actor.TagMap);

            var translateMessage = new ACDTranslateNormalMessage();
            translateMessage.Angle = 5.086197f;
            translateMessage.ActorId = (int)actorID;
            translateMessage.AnimationTag = 0x00011084;
            translateMessage.Position = new Vector3D(2833.75f, 2978.75f, 24.07898f);
            //Send message to all players

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(translateMessage, player.Value);
            }

            world.GetActorByDynamicId(actorID).Move(new Vector3D(2839.43f, 2964.272f, 24.19856f), 5.086219f);
            #endregion
            #region ZombieCrawlerBarricade #2
            //ActorID: 0x798600B9   
            //Scale: 1.6
            //Script code comes here
            var actorSNOHandle = new Common.Types.SNO.SNOHandle(0x000354FF);
            var actor = actorSNOHandle.Target as Actor;
            var PRTransform = new PRTransform();
            PRTransform.Quaternion = new Quaternion();
            PRTransform.Quaternion.W = 0.7449269f;
            PRTransform.Quaternion.Vector3D = new Vector3D(0, 0, -0.6671461f);
            PRTransform.Vector3D = new Vector3D(2848.75f, 2983.75f, 24.59938f);

            var actorID = WorldGenerator.loadActor(new Common.Types.SNO.SNOHandle(0x000354FF), PRTransform, world, actor.TagMap);

            var translateMessage = new ACDTranslateNormalMessage();
            translateMessage.Angle = 4.822382f;
            translateMessage.ActorId = (int)actorID;
            translateMessage.AnimationTag = 0x00011084;
            translateMessage.Position = new Vector3D(2848.75f, 2983.75f, 24.59938f);
            //Send message to all players

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(translateMessage, player.Value);
            }

            world.GetActorByDynamicId(actorID).Move(new Vector3D(2847.27f, 2964.801f, 24.36039f), 4.634396f);
            #endregion
            #region ZombieCrawlerBarricade #3
            //ActorID: 0x79870031
            //Scale: 1.6
            //Script code comes here
            var actorSNOHandle = new Common.Types.SNO.SNOHandle(0x000354FF);
            var actor = actorSNOHandle.Target as Actor;
            var PRTransform = new PRTransform();
            PRTransform.Quaternion = new Quaternion();
            PRTransform.Quaternion.W = 0.5914196f;
            PRTransform.Quaternion.Vector3D = new Vector3D(0, 0, -0.8063639f);
            PRTransform.Vector3D = new Vector3D(2871.25f, 2978.75f, 26.76203f);

            var actorID = WorldGenerator.loadActor(new Common.Types.SNO.SNOHandle(0x000354FF), PRTransform, world, actor.TagMap);

            var translateMessage = new ACDTranslateNormalMessage();
            translateMessage.Angle = 4.40722f;
            translateMessage.ActorId = (int)actorID;
            translateMessage.AnimationTag = 0x00011070;
            translateMessage.Position = new Vector3D(2871.25f, 2978.75f, 26.76203f);
            //Send message to all players

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(translateMessage, player.Value);
            }

            world.GetActorByDynamicId(actorID).Move(new Vector3D(2845.463f, 2970.245f, 24.45933f), 3.540084f);
            #endregion
            */
        }
    }
}
