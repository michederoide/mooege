/*
 * Copyright (C) 2011 - 2012 mooege project - http://www.mooege.org
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using Mooege.Common.Logging;
using Mooege.Common.MPQ;
using Mooege.Core.GS.Games;

namespace Mooege.Core.GS.Games.Scripts
{
    /// <summary>
    /// Script manager class. Based on Quest manager implementation.
    /// </summary>
    public class ScriptManager : IEnumerable<Script>
    {
        private Dictionary<int, Script> scripts = new Dictionary<int, Script>();
        private static readonly Logger Logger = new Logger("ScriptManager");

        public Game game { get; private set; }

        /// <summary>
        /// Accessor for scripts
        /// </summary>
        /// <param name="snoQuest">snoId of the script to retrieve</param>
        /// <returns></returns>
        public Script this[int snoScript]
        {
            get 
            {
                if (!scripts.ContainsKey(snoScript))
                {
                    Logger.Warn("Tried to get unimplemented script with sno: {0}", snoScript);
                    return new Script(game, snoScript, "UnimplementedScript");
                }
                return scripts[snoScript]; 
            }
        }

        /// <summary>
        /// Creates a new ScriptManager and attaches it to a game. Scripts are shared among all players in a game
        /// </summary>
        /// <param name="game">The game is used to broadcast game messages to</param>
        public ScriptManager(Game game)
        {
            this.game = game;
            //No reason to use it as scripts are not stored anymore but in case we want to reconstruct properly the SNO DB
            var asset = MPQStorage.Data.Assets[Common.Types.SNO.SNOGroup.Script];
            foreach (var script in asset)
                scripts.Add(script.Key, new Script(game, script.Value));

            InitScripts(game);
        }

        /// <summary>
        /// Storage and init of current script implementations
        /// TODO: Refactor this with proper asset implementation
        /// </summary>
        private void InitScripts(Game game)
        {            
            scripts.Add(200883, new _200883(game));
        }

        public IEnumerator<Script> GetEnumerator()
        {
            return scripts.Values.GetEnumerator();
        }


        public bool HasCurrentScript(int snoScript, int Step)
        {
            if (scripts.ContainsKey(snoScript))
                    return true;

            return false;
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
