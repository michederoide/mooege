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
using System.Linq;
using System.Text;
using Mooege.Common.Logging;

namespace Mooege.Core.GS.Games.Scripts
{
    public class Script: ScriptBase, IScript
    {
        protected static readonly Logger Logger = new Logger("Script");

        /// <summary>
        /// SNOId.
        /// </summary>
        public int SNOId { get; protected set; }

        /// <summary>
        /// Script Name
        /// </summary>
        public string ScriptName { get; protected set; }

        /// <summary>
        /// Game this script is attached to
        /// TODO: Maybe refactor this as scripts should all e loaded
        /// </summary>
        public Game game { get; protected set; }

        #region c-tors

        public Script(Game game, int SNOId, string ScriptName)
        {
            this.SNOId = SNOId;
            this.ScriptName = ScriptName;
        }

        public Script(Game game, Mooege.Common.MPQ.Asset script)
        {
        }

        #endregion


        public override string ToString()
        {
            return String.Format("{{ Script: {0} [Id: {1}] }}", this.ScriptName, this.SNOId);
        }


        public override void Execute()
        {
            Logger.Trace("Executing script: {0}", this);
        }
    }

    public abstract class ScriptBase : IScript
    {
        public abstract void Execute();
    }

    public interface IScript
    {
        void Execute();
    }
}
