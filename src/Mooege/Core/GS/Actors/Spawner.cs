using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Games;


namespace Mooege.Core.GS.Actors
{
    public class Spawner : Actor
    {
        public override ActorType ActorType
        {
            get { return ActorType.ServerProp; }
        }

        public Spawner(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Field2 = 8;
            this.Field7 = 0x00000000;
            this.CollFlags = 0; // a hack for passing through blockers /fasbat

            // Listen for quest progress if the actor has a QuestRange attached to it
            foreach (var quest in World.Game.Quests)
                if (_questRange != null)
                    quest.OnQuestProgress += new Games.Quest.QuestProgressDelegate(quest_OnQuestProgressSpawner);
        }

        private void quest_OnQuestProgressSpawner(Quest quest)
        {
            //Spawn if this is spawner
            if (World.Game.Quests.IsInQuestRange(_questRange) && this.Tags != null)
            {
                if (Tags.ContainsKey(MarkerKeys.SpawnActor))
                {
                    this.Spawn();
                }
            }
        }

        public void Spawn()
        {
            if (Tags != null)
            {
                if (Tags.ContainsKey(MarkerKeys.SpawnActor))
                {
                    var ActorSNO = Tags[MarkerKeys.SpawnActor];
                    var location = new PRTransform()
                    {
                        Quaternion = new Quaternion
                        {
                            W = this.RotationW,
                            Vector3D = this.RotationAxis
                        },
                        Vector3D = this.Position
                    };

                    Mooege.Core.GS.Generators.WorldGenerator.loadActor(ActorSNO, location, this.World, ((Mooege.Common.MPQ.FileFormats.Actor)ActorSNO.Target).TagMap);

                    //once target spawned this can be destroyed
                    this.Destroy();
                }
            }
        }

        public override bool Reveal(Players.Player player)
        {
            //Do not reveal spawner gizmos
            return false;
        }
    }
}
