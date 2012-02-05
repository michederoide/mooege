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
    class _151087 :QuestEvent
    {
        public _151087()
            : base(151087)
        {
        }

        public override void Execute(Map.World world)
        {

            foreach (var player in world.Players)
            {
                player.Value.Conversations.StartConversation(198199);
            }

            #region ZombieSkinny_Custom_A #1
            //ActorID: 0x796400CA
            //Script code comes here
            //Scale 1.35
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
            #endregion
            #region ZombieSkinny_Custom_A #2
            //Script code comes here
            //ActorID: 0x796500B7  
            //Scale 1.35
            var actorSNOHandle2 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor2 = actorSNOHandle.Target as Actor;
            var PRTransform2 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.6130235f,
                    Vector3D = new Vector3D(0, 0, -0.7900648f)
                },
                Vector3D = new Vector3D(2847.069f, 2975.214f, 24.43476f)
            };

            var actorID2 = WorldGenerator.loadActor(actorSNOHandle2, PRTransform2, world, actor2.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 0f,
                    ActorId = (int)actorID2,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2848.365f, 2975.214f, 24.69485f)
                }, player.Value);

                world.BroadcastIfRevealed(new PlayAnimationMessage
                {
                    ActorID = actorID2,
                    Field1 = 3,
                    Field2 = 0,
                    tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 0x00000097,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.673818f
                    }
                }

                }, player.Value);
            }
            #endregion
            #region ZombieSkinny_Custom_A #3
            //Script code comes here
            //ActorID: 0x79660031
            //Scale 1.35
            var actorSNOHandle3 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor3 = actorSNOHandle.Target as Actor;
            var PRTransform3 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.8882556f,
                    Vector3D = new Vector3D(0, 0, -0.4593496f)
                },
                Vector3D = new Vector3D(2822.784f, 2956.344f, 23.94533f)
            };

            var actorID3 = WorldGenerator.loadActor(actorSNOHandle3, PRTransform3, world, actor3.TagMap);

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 5.028199f,
                    ActorId = (int)actorID3,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2823.187f, 2955.112f, 24.04533f)
                }, player.Value);

                world.BroadcastIfRevealed(new PlayAnimationMessage
                {
                    ActorID = actorID3,
                    Field1 = 3,
                    Field2 = 0,
                    tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 0x00000091,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.744957f
                    }
                }

                }, player.Value);
            }
            #endregion

            //this should happen after first 3 skinnies are killed.
            #region ZombieCrawlerBarricade #1
            //ActorID: 0x798500CA  
            //Scale: 1.6
            //Script code comes here
            var actorSNOHandle4 = new Common.Types.SNO.SNOHandle(0x000354FF);
            var actor4 = actorSNOHandle.Target as Actor;
            var PRTransform4 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.826175f,
                    Vector3D = new Vector3D(0, 0, -0.5634137f)
                },
                Vector3D = new Vector3D(2833.75f, 2978.75f, 24.07898f)
            };

            var actorID4 = WorldGenerator.loadActor(actorSNOHandle4, PRTransform4, world, actor4.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 5.086197f,
                    ActorId = (int)actorID4,
                    AnimationTag = 0x00011084,
                    Position = new Vector3D(2833.75f, 2978.75f, 24.07898f)
                }, player.Value);
            }

            world.GetActorByDynamicId(actorID4).Move(new Vector3D(2839.43f, 2964.272f, 24.19856f), 5.086219f);
            #endregion
            #region ZombieCrawlerBarricade #2
            //ActorID: 0x798600B9   
            //Scale: 1.6
            //Script code comes here
            var actorSNOHandle5 = new Common.Types.SNO.SNOHandle(0x000354FF);
            var actor5 = actorSNOHandle.Target as Actor;
            var PRTransform5 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.7449269f,
                    Vector3D = new Vector3D(0, 0, -0.6671461f)
                },
                Vector3D = new Vector3D(2848.75f, 2983.75f, 24.59938f)
            };

            var actorID5 = WorldGenerator.loadActor(actorSNOHandle5, PRTransform5, world, actor5.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 4.822382f,
                    ActorId = (int)actorID5,
                    AnimationTag = 0x00011084,
                    Position = new Vector3D(2848.75f, 2983.75f, 24.59938f)
                }, player.Value);
            }

            world.GetActorByDynamicId(actorID5).Move(new Vector3D(2847.27f, 2964.801f, 24.36039f), 4.634396f);
            #endregion
            #region ZombieCrawlerBarricade #3
            //ActorID: 0x79870031
            //Scale: 1.6
            //Script code comes here
            var actorSNOHandle6 = new Common.Types.SNO.SNOHandle(0x000354FF);
            var actor6 = actorSNOHandle.Target as Actor;
            var PRTransform6 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.5914196f,
                    Vector3D = new Vector3D(0, 0, -0.8063639f)
                },
                Vector3D = new Vector3D(2871.25f, 2978.75f, 26.76203f)
            };

            var actorID6 = WorldGenerator.loadActor(actorSNOHandle6, PRTransform6, world, actor6.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 4.40722f,
                    ActorId = (int)actorID6,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2871.25f, 2978.75f, 26.76203f)
                }, player.Value);
            }

            world.GetActorByDynamicId(actorID6).Move(new Vector3D(2845.463f, 2970.245f, 24.45933f), 3.540084f);
            #endregion*/

            foreach (var player in world.Players)
            {
                player.Value.Conversations.StartConversation(80088);
            }
        }
    }
}
