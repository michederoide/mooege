using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Common.MPQ.FileFormats;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Net.GS.Message.Definitions.Animation;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Generators;


namespace Mooege.Core.GS.QuestEvents.Implementations
{
    class _151123 : QuestEvent
    {
        public _151123()
            : base(151123)
        {
        }

        public override void Execute(Map.World world)
        {

            foreach (var player in world.Players)
            {
                player.Value.Conversations.StartConversation(204113);
            }

            //Grab the Actors that are standing there, Once conversation over, Play Animation on Males/Females, Destroy Actors, Spawn Zombies.
          
            /*#region ZombieSkinny_Custom_A #1
            //Script code comes here
            var actorSNOHandle = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor = actorSNOHandle.Target as Actor;
            var PRTransform = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.7063466f,
                    Vector3D = new Vector3D(0, 0, -0.7078662f)
                },
                Vector3D = new Vector3D(2846.162f, 2962.202f, 24.10213f)
            };

            var actorID = WorldGenerator.loadActor(actorSNOHandle, PRTransform, world, actor.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 3.37346f,
                    ActorId = (int)actorID,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2844.901f, 2961.904f, 24.24138f)
                }, player.Value);

                world.BroadcastIfRevealed(new PlayAnimationMessage
                {
                    ActorID = actorID,
                    Field1 = 3,
                    Field2 = 0,
                    tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 0x00000083,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.938229f
                    }
                }

                }, player.Value);
            }
            #endregion*/

            foreach (var player in world.Players)
            {
                //"This is killing Business..."
                player.Value.Conversations.StartConversation(151156);
                world.Game.Quests.Advance(87700);
            }
        }
    }
}