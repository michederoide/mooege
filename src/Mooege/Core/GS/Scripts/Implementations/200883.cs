using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mooege.Core.GS.Games.Scripts
{
    public class _200883 : Script
    {
        /// <summary>
        /// Ctor
        /// TODO: Refactor with properties
        /// </summary>
        /// <param name="game"></param>
        public _200883(Game game)
            : base(game, 200883, "Player_EnteredGame_Callout")
        {
            //TODO: Add other init logic here
        }

        public override void Execute()
        {
            base.Execute();
            Logger.Warn("Unimplemented Execute Method for script: {0}", this);
            //TODO: Enter implementation here
        }
    }
}
