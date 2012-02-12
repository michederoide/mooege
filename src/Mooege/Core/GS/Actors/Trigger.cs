using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Games;
using Mooege.Core.GS.Common.Types.SNO;
using Mooege.Core.GS.Games.Scripts;

namespace Mooege.Core.GS.Actors
{
    public class Trigger : Actor
    {

        /// <summary>
        /// What conversation gizmo will trigger
        /// </summary>
        public SNOHandle TriggeredConversation { get; private set; }

        /// <summary>
        /// Script to be triggered on actor spawned
        /// </summary>
        public SNOHandle OnActorSpawnedScript { get; private set; }

        /// <summary>
        /// What converation gizmo will trigger
        /// </summary>
        public SNOHandle TriggeredConversation1 { get; private set; }

        /// <summary>
        /// What actor this gizmo will trigger
        /// </summary>
        public SNOHandle TriggeredActor { get; private set; }

        public override ActorType ActorType
        {
            get { return ActorType.Gizmo; }
        }

        public Trigger(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Field2 = 8;
            this.Field7 = 0x00000000;

            if (Tags.ContainsKey(MarkerKeys.TriggeredConversation))
                this.TriggeredConversation = Tags[MarkerKeys.TriggeredConversation];


            if (Tags.ContainsKey(MarkerKeys.TriggeredConversation1))
                this.TriggeredConversation1 = Tags[MarkerKeys.TriggeredConversation1];

            if (Tags.ContainsKey(MarkerKeys.TriggeredActor))
                this.TriggeredActor = Tags[MarkerKeys.TriggeredActor];

            if (Tags.ContainsKey(MarkerKeys.OnActorSpawnedScript))
                this.OnActorSpawnedScript = Tags[MarkerKeys.OnActorSpawnedScript];

        }

        //Launch Conversations.
        private bool StartConversation(Map.World world, int conversationId)
        {
            foreach (var player in world.Players)
            {
                player.Value.Conversations.StartConversation(conversationId);
            }
            return true;
        }

        /// <summary>
        /// Reveal Override. For Spawner Gizmos there is no reveal necessary.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool Reveal(Players.Player player)
        {
            if (player.RevealedObjects.ContainsKey(this.DynamicID)) return false; // already revealed
            player.RevealedObjects.Add(this.DynamicID, this);
            //Start triggered conversation
            if (this.TriggeredConversation != null)
            {
                StartConversation(this.World, this.TriggeredConversation.Id);
                Logger.Debug("triggered conversation: {0}", this.TriggeredConversation.Id);
            }
            if (this.TriggeredConversation1 != null)
            {
                StartConversation(this.World, this.TriggeredConversation1.Id);
                Logger.Debug("triggered conversation: {0}", this.TriggeredConversation1.Id);
            }
            if (this.TriggeredActor != null)
            {
                //TODO: Verify spawn position?

            }
            if (this.OnActorSpawnedScript != null)
            {
                this.World.Game.Scripts[OnActorSpawnedScript.Id].Execute();
            }

            return true;
        }
    }
}
