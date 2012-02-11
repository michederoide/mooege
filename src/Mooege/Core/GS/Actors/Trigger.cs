using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Games;
using Mooege.Core.GS.Common.Types.SNO;


namespace Mooege.Core.GS.Actors
{
    public class Trigger : Actor
    {

        /// <summary>
        /// What conversation gizmo will trigger
        /// </summary>
        public SNOHandle TriggeredConversation { get; private set; }


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

            //Actor.Data.TagMap contains: {66368 = 291072}
            //public static TagKeyInt Spawn2 = new TagKeyInt(291072);
            //TODO: Find why Tags is not the same as Actor.Data.TagMap
            //            this.Tags.TagMapEntries
            //Count = 3
            //    [0]: {548869 = 15}
            //    [1]: {TriggeredConversation = [Conversation] 176999 - Fol_Manor}
            //    [2]: {OnActorSpawnedScript = [] 194655 - Invalid handle}
            if (Tags.ContainsKey(MarkerKeys.TriggeredConversation))
                this.TriggeredConversation = Tags[MarkerKeys.TriggeredConversation];

            //            Trigger:
            //this.ActorData.TagMap.TagMapEntries
            //Count = 19
            //    [0]: {65597 = 0}
            //    [1]: {65840 = 0}
            //    [2]: {65544 = 0}
            //    [3]: {65689 = 1}
            //    [4]: {65868 = 0}
            //    [5]: {TeamID = 1}
            //    [6]: {Script = [] 138348 - Invalid handle}
            //    [7]: {68673 = 0}
            //    [8]: {65822 = 900}
            //    [9]: {Scale = 10}
            //    [10]: {GizmoGroup = Trigger}
            //    [11]: {65839 = 0}
            //    [12]: {65696 = 1}
            //    [13]: {65823 = 1}
            //    [14]: {67858 = 3}
            //    [15]: {229376 = 0}
            //    [16]: {65650 = 1}
            //    [17]: {65817 = 30}
            //    [18]: {65571 = 14}
            //this.Tags.TagMapEntries
            //Count = 2
            //    [0]: {548869 = 15}
            //    [1]: {528129 = 140617}

            if (Tags.ContainsKey(MarkerKeys.TriggeredConversation1))
                this.TriggeredConversation1 = Tags[MarkerKeys.TriggeredConversation1];

            if (Tags.ContainsKey(MarkerKeys.TriggeredActor))
                this.TriggeredActor = Tags[MarkerKeys.TriggeredActor];

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

            return true;
        }
    }
}
