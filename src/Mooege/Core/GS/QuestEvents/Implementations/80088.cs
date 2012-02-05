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
    //This is not a QuestEvent but a questfloat. It should happen as soon as you've killed the Zombie_Barricade Zombies (2nd wave of GateIntro)
    class _80088 : QuestEvent
    {
        public _80088()
            : base(80088)
        {
        }

        public override void Execute(Map.World world)
        {
            // TristramZombieGuardAttack.cnv -> "They Keep Coming!"
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
                    W =  0.7441373f,
                    Vector3D = new Vector3D(0, 0, -0.6680267f)
                },
                Vector3D = new Vector3D(2845.132f, 2975.559f, 24.43476f)
            };

            var actorID = WorldGenerator.loadActor(actorSNOHandle, PRTransform, world, actor.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 1.512316f,
                    ActorId = (int)actorID,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2845.176f, 2976.314f, 24.55417f)
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
                        Duration = 0x000000B2,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.423634f
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
                    W = 0.602631f,
                    Vector3D = new Vector3D(0, 0, -0.79802f)
                },
                Vector3D = new Vector3D(2867.742f, 2962.25f, 25.26077f)
            };

            var actorID2 = WorldGenerator.loadActor(actorSNOHandle2, PRTransform2, world, actor2.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 4.74911f,
                    ActorId = (int)actorID2,
                    AnimationTag = 0x00011060,
                    Position = new Vector3D(2867.789f, 2960.955f, 25.44457f)
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
                        Duration = 0x000000AC,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.470013f
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
                    W = 0.8089566f,
                    Vector3D = new Vector3D(-0.01573627f, -0.001452722f, -0.587656f)
                },
                Vector3D = new Vector3D(2826.886f, 2965.174f, 23.94533f)
            };

            var actorID3 = WorldGenerator.loadActor(actorSNOHandle3, PRTransform3, world, actor3.TagMap);

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 4.936723f,
                    ActorId = (int)actorID3,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2827.175f, 2963.911f, 24.06284f)
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
                        Duration = 0x000000B0,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.437554f
                    }
                }

                }, player.Value);
            }
            #endregion
            #region ZombieSkinny_Custom_A #4
            //ActorID: 0x7991001D  
            //Scale: 1.35
            //Script code comes here
            var actorSNOHandle4 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor4 = actorSNOHandle.Target as Actor;
            var PRTransform4 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.8897762f,
                    Vector3D = new Vector3D(0, 0, -0.510671f)
                },
                Vector3D = new Vector3D(2842.001f, 2975.481f, 24.25938f)
            };

            var actorID4 = WorldGenerator.loadActor(actorSNOHandle4, PRTransform4, world, actor4.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 3.887427f,
                    ActorId = (int)actorID4,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2841.05f, 2974.601f, 24.39156f)
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
                        Duration = 0x00000090,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.754749f
                    }
                }

                }, player.Value);
            }
            #endregion
            #region ZombieSkinny_Custom_A #5
            //Scale: 1.35
            //Script code comes here
            var actorSNOHandle5 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor5 = actorSNOHandle.Target as Actor;
            var PRTransform5 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.602631f,
                    Vector3D = new Vector3D(0, 0, -0.79802f)
                },
                Vector3D = new Vector3D(2861.012f, 2970.908f, 25.18782f)
            };

            var actorID5 = WorldGenerator.loadActor(actorSNOHandle5, PRTransform5, world, actor5.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 1.06897f,
                    ActorId = (int)actorID5,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2861.22f, 2971.286f, 25.47387f)
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
                        Duration = 0x00000087,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.874283f
                    }
                }

                }, player.Value);
            }
            #endregion
            #region ZombieSkinny_Custom_A #6
            //Scale: 1.35
            //Script code comes here
            var actorSNOHandle6 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor6 = actorSNOHandle.Target as Actor;
            var PRTransform6 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.7441373f,
                    Vector3D = new Vector3D(0, 0, -0.6680267f)
                },
                Vector3D = new Vector3D(2863.467f, 2969.561f, 25.25561f)
            };

            var actorID6 = WorldGenerator.loadActor(actorSNOHandle6, PRTransform6, world, actor6.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 3.676926f,
                    ActorId = (int)actorID6,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2862.353f, 2968.9f, 25.44267f)
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
                        Duration = 0x00000086,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.891467f
                    }
                }

                }, player.Value);
            }

            #endregion
            #region ZombieSkinny_Custom_A #7
            //Script code comes here
            //ActorID: 0x79660031
            //Scale 1.35
            var actorSNOHandle7 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor7 = actorSNOHandle.Target as Actor;
            var PRTransform7 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.8089566f,
                    Vector3D = new Vector3D(-0.01573627f, -0.001452722f, -0.587656f)
                },
                Vector3D = new Vector3D(2829.686f, 2965.184f, 23.96912f)
            };

            var actorID7 = WorldGenerator.loadActor(actorSNOHandle7, PRTransform7, world, actor7.TagMap);

            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 4.681164f,
                    ActorId = (int)actorID7,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2829.646f, 2963.888f, 24.08491f)
                }, player.Value);

                world.BroadcastIfRevealed(new PlayAnimationMessage
                {
                    ActorID = actorID7,
                    Field1 = 3,
                    Field2 = 0,
                    tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 0x00000084,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.9149f
                    }
                }

                }, player.Value);
            }
            #endregion
            #region ZombieSkinny_Custom_A #8
            //ActorID: 0x7991001D  
            //Scale: 1.35
            //Script code comes here
            var actorSNOHandle8 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor8 = actorSNOHandle.Target as Actor;
            var PRTransform8 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.8897762f,
                    Vector3D = new Vector3D(0, 0, -0.510671f)
                },
                Vector3D = new Vector3D(2836.097f, 2971.245f, 24.0959f)
            };

            var actorID8 = WorldGenerator.loadActor(actorSNOHandle8, PRTransform8, world, actor8.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 4.278242f,
                    ActorId = (int)actorID8,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2835.552f, 2970.07f, 24.20798f)
                }, player.Value);

                world.BroadcastIfRevealed(new PlayAnimationMessage
                {
                    ActorID = actorID8,
                    Field1 = 3,
                    Field2 = 0,
                    tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 0x0000009C,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.619311f
                    }
                }

                }, player.Value);
            }
            #endregion
            #region ZombieSkinny_Custom_A #9
            //Scale: 1.35
            //Script code comes here
            var actorSNOHandle9 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor9 = actorSNOHandle.Target as Actor;
            var PRTransform9 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.602631f,
                    Vector3D = new Vector3D(0, 0, -0.79802f)
                },
                Vector3D = new Vector3D(2858.462f, 2969.751f, 24.81519f)
            };

            var actorID9 = WorldGenerator.loadActor(actorSNOHandle9, PRTransform9, world, actor9.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 3.793459f,
                    ActorId = (int)actorID9,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2857.432f, 2968.965f, 25.01513f)
                }, player.Value);

                world.BroadcastIfRevealed(new PlayAnimationMessage
                {
                    ActorID = actorID9,
                    Field1 = 3,
                    Field2 = 0,
                    tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 0x000000B5,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.402444f
                    }
                }

                }, player.Value);
            }
            #endregion
            #region ZombieSkinny_Custom_A #10
            //Scale: 1.35
            //Script code comes here
            var actorSNOHandle10 = new Common.Types.SNO.SNOHandle(0x000354E3);
            var actor10 = actorSNOHandle.Target as Actor;
            var PRTransform10 = new PRTransform()
            {
                Quaternion = new Quaternion()
                {
                    W = 0.8089566f,
                    Vector3D = new Vector3D(-0.01573627f, -0.001452722f, -0.587656f)
                },
                Vector3D = new Vector3D(2823.161f, 2956.929f, 23.94533f)
            };

            var actorID10 = WorldGenerator.loadActor(actorSNOHandle10, PRTransform10, world, actor10.TagMap);

            //Send message to all players
            foreach (var player in world.Players)
            {
                world.BroadcastIfRevealed(new ACDTranslateNormalMessage
                {
                    Angle = 5.332213f,
                    ActorId = (int)actorID10,
                    AnimationTag = 0x00011070,
                    Position = new Vector3D(2823.914f, 2955.874f, 24.04533f)
                }, player.Value);

                world.BroadcastIfRevealed(new PlayAnimationMessage
                {
                    ActorID = actorID10,
                    Field1 = 3,
                    Field2 = 0,
                    tAnim = new Net.GS.Message.Fields.PlayAnimationMessageSpec[]
                {
                    new Net.GS.Message.Fields.PlayAnimationMessageSpec()
                    {
                        Duration = 0x000000AE,
                        AnimationSNO = 0x00002D03,
                        PermutationIndex = 0x00000000,
                        Speed = 1.459143f
                    }
                }

                }, player.Value);
            }

            #endregion

            foreach (var player in world.Players)
            {
                //OpenGates.cnv
                player.Value.Conversations.StartConversation(151102);
                world.Game.Quests.Advance(87700);
            }
        }
    }
}
