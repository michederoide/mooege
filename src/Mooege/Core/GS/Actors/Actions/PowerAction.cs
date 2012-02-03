/*
 * Copyright (C) 2011 mooege project
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
using Mooege.Core.GS.Powers;
using Mooege.Core.GS.Ticker;
using Mooege.Core.GS.Common.Types.TagMap;
using Mooege.Core.GS.Actors.Movement;
using Mooege.Net.GS.Message;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Core.GS.Common.Types.Misc;

namespace Mooege.Core.GS.Actors.Actions
{
    public class PowerAction : ActorAction
    {
        const float MaxTargetRange = 60f;
        const float PathUpdateDelay = 1f;

        private Actor _target;
        private PowerScript _power;
        private bool _powerRan;
        private TickTimer _powerFinishTimer;
        private float _baseAttackRadius;
        private ActorMover _ownerMover;
        private TickTimer _pathUpdateTimer;

        private List<Vector3D> _path;
        private AI.Pather.PathRequestTask _pathRequestTask;

        public PowerAction(Actor owner, int powerSNO)
            : base(owner)
        {
            _power = PowerLoader.CreateImplementationForPowerSNO(powerSNO);
            _power.World = owner.World;
            _power.User = owner;
            _powerRan = false;
            _baseAttackRadius = this.Owner.ActorData.Cylinder.Ax2 + _power.EvalTag(PowerKeys.AttackRadius) + 1.5f;
            _ownerMover = new ActorMover(owner);
        }

        public override void Start(int tickCounter)
        {
            this.Started = true;
            this.Update(tickCounter);
        }

        public override void Update(int tickCounter)
        {
            // if power executed, wait for attack/cooldown to finish.
            if (_powerRan)
            {
                if (_powerFinishTimer.TimedOut)
                    this.Done = true;

                return;
            }
            
            // try to get nearest target if no target yet acquired
            if (_target == null)
            {
                _target = this.Owner.GetPlayersInRange(MaxTargetRange).OrderBy(
                    (player) => PowerMath.Distance2D(player.Position, this.Owner.Position))
                    .FirstOrDefault();
                    //.FirstOrDefault(x => x.Attributes[GameAttribute.Untargetable] == false);
                    // If target is marked untargetable then we shouldnt consider him for targeting - DarkLotus
            }

            if (_target != null)
            {
                float targetDistance = PowerMath.Distance2D(_target.Position, this.Owner.Position);

                // if target has moved out of range, deselect it as the target
                if (targetDistance > MaxTargetRange)
                {
                    _target = null;
                }
                else if (targetDistance < _baseAttackRadius + _target.ActorData.Cylinder.Ax2)  // run power if within range
                {
                    // stop any movement
                    this.Owner.Move(this.Owner.Position, MovementHelpers.GetFacingAngle(this.Owner, _target));
                    //this.Owner.TranslateFacing(_target.Position, true);

                    this.Owner.World.PowerManager.RunPower(this.Owner, _power, _target, _target.Position);
                    _powerFinishTimer = new SecondsTickTimer(this.Owner.World.Game,
                        _power.EvalTag(PowerKeys.AttackSpeed) + _power.EvalTag(PowerKeys.CooldownTime));
                    _powerRan = true;
                }
                else
                {
                    if (_pathRequestTask == null)
                        _pathRequestTask = Owner.World.Game.Pathfinder.GetPath(Owner, Owner.Position, _target.Position); // called once to create task
                    if (!_pathRequestTask.PathFound)
                        return;

                    // No path found, so end Action.
                    if (_pathRequestTask.Path.Count < 1)
                        return;
                    if(_path == null)
                    _path = _pathRequestTask.Path;

                    if (_pathUpdateTimer == null || _pathUpdateTimer.TimedOut)
                    {
                        _pathUpdateTimer = new SecondsTickTimer(this.Owner.World.Game, PathUpdateDelay);     
                        //_pathRequestTask = null;
                        Vector3D movePos = PowerMath.TranslateDirection2D(this.Owner.Position, _path[0], this.Owner.Position,
                            this.Owner.WalkSpeed * (_pathUpdateTimer.TimeoutTick - this.Owner.World.Game.TickCounter));
                        this.Owner.TranslateFacing(movePos, false);
                        _ownerMover.Move(movePos, this.Owner.WalkSpeed, new Net.GS.Message.Definitions.ACD.ACDTranslateNormalMessage
                        {
                            TurnImmediately = false,
                            AnimationTag = this.Owner.AnimationSet == null ? 0 : this.Owner.AnimationSet.GetAnimationTag(Mooege.Common.MPQ.FileFormats.AnimationTags.Walk)
                        });
                        _path.RemoveAt(0);
                        if (_path.Count == 0)
                        {
                            _pathRequestTask = null;
                            _path = null;
                            return;
                        }

                        
                        //_path.Clear();
                    }



                    /*// update or create path movement
                    if (_pathUpdateTimer == null || _pathUpdateTimer.TimedOut)
                    {
                        _pathUpdateTimer = new SecondsTickTimer(this.Owner.World.Game, PathUpdateDelay);

                        // move the space between each path update
                        Vector3D movePos = PowerMath.TranslateDirection2D(this.Owner.Position, _target.Position, this.Owner.Position,
                            this.Owner.WalkSpeed * (_pathUpdateTimer.TimeoutTick - this.Owner.World.Game.TickCounter));
                        if (!this.Owner.World.CheckLocationForFlag(movePos, Mooege.Common.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
                        {
                            var xdiff = movePos.X - this.Owner.Position.X;
                            var ydiff = movePos.Y - this.Owner.Position.Y;
                            movePos.Y = Owner.Position.Y;
                            // make sure mesh is a non walking one...
                            // could use gridsquares if hit left move one south etc
                            movePos.X = Owner.Position.X + xdiff;
                            foreach (var mesh in this.Owner.CurrentScene.NavZone.NavCells)// need to check scenes prob
                            {
                                if ((mesh.Bounds.Contains(movePos.X - Owner.CurrentScene.Position.X, movePos.Y - Owner.CurrentScene.Position.Y)) && !mesh.Flags.HasFlag(Mooege.Common.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
                                {
                                    movePos.X = Owner.Position.X;
                                    ydiff *= 1.4f;
                                    break;
                                }

                            }
                            movePos.Y += ydiff;
                            foreach (var mesh in this.Owner.CurrentScene.NavZone.NavCells)// need to check scenes prob
                            {
                                if ((mesh.Bounds.Contains(movePos.X - Owner.CurrentScene.Position.X, movePos.Y - Owner.CurrentScene.Position.Y)) && !mesh.Flags.HasFlag(Mooege.Common.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
                                {
                                    movePos.Y = Owner.Position.Y;
                                    movePos.X += (xdiff * 0.4f);
                                    break;
                                }
                            }
                            
                            //var localmovepos = new Vector3D(movePos);
                            System.Windows.Rect oldPosRectSceneLocal = new System.Windows.Rect(Owner.Position.X - Owner.CurrentScene.Position.X, Owner.Position.Y- Owner.CurrentScene.Position.Y, Owner.Bounds.Width, Owner.Bounds.Height);
                            System.Windows.Rect movePosSceneLocal = new System.Windows.Rect(movePos.X - Owner.CurrentScene.Position.X, movePos.Y - Owner.CurrentScene.Position.Y, Owner.Bounds.Width, Owner.Bounds.Height);
                            movePos.X -= (float)this.Owner.CurrentScene.Bounds.Location.X;
                            movePos.Y -= (float)this.Owner.CurrentScene.Bounds.Location.Y;
                            Circle mob = new Circle(movePos.X - (float)Owner.CurrentScene.Position.X, movePos.Y - (float)Owner.CurrentScene.Position.Y, (float)this.Owner.Bounds.Width);
                            
                            foreach (var mesh in this.Owner.CurrentScene.NavZone.NavCells)// need to check scenes prob
                            {
                                /*if (mob.Intersects(mesh.Bounds))
                                {
                                    if(PowerMath.CircleInBeam(mob,mesh.Bounds.TopLeft,mesh.Bounds.BottomLeft,1f))
                                    {
                                    
                                    }
                                }
                                if ((mesh.Bounds.Contains(movePos.X, movePos.Y)) && mesh.Flags.HasFlag(Mooege.Common.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
                                {
                                    if ((oldPosRectSceneLocal.Left >= mesh.Bounds.Right && movePosSceneLocal.Left < mesh.Bounds.Right)) // Right  collisions
                                    {
                                        ydiff = 0;
                                        xdiff = xdiff * 1.5f;
                                        break;
                                    }
                                    if ((oldPosRectSceneLocal.Right < mesh.Bounds.Left && movePosSceneLocal.Right >= mesh.Bounds.Left))//Left
                                    {
                                        ydiff = 0;
                                        xdiff = xdiff * 1.5f;
                                        break;
                                    }
                                    if ((oldPosRectSceneLocal.Top >= mesh.Bounds.Bottom && movePosSceneLocal.Top < mesh.Bounds.Bottom) || (oldPosRectSceneLocal.Bottom < mesh.Bounds.Top && movePosSceneLocal.Bottom >= mesh.Bounds.Top)) // Bottom then Top
                                    {

                                        xdiff = 0;
                                        ydiff = ydiff * 1.5f;
                                        break;
                                    }
                                    /*if (movePos.X > mesh.Min.X || movePos.X < mesh.Max.X)
                                    {
                                        xdiff = 0;
                                        break;
                                    }
                                    else if (movePos.Y > mesh.Min.Y || movePos.Y < mesh.Max.Y)
                                    {
                                        ydiff = 0;
                                        break;
                                    }
                                }
                                
                            }
                            movePos.X = this.Owner.Position.X + xdiff;
                            movePos.Y = this.Owner.Position.Y + ydiff;
                        }
                        


                        this.Owner.TranslateFacing(movePos,false);//_target.Position, false);

                        _ownerMover.Move(movePos, this.Owner.WalkSpeed, new Net.GS.Message.Definitions.ACD.ACDTranslateNormalMessage
                        {
                            TurnImmediately = false,
                            AnimationTag = this.Owner.AnimationSet == null ? 0 : this.Owner.AnimationSet.GetAnimationTag(Mooege.Common.MPQ.FileFormats.AnimationTags.Walk)
                        });
                    }*/
                    else
                    {
                        if(_ownerMover.Velocity != null)
                        _ownerMover.Update();
                        //if (_ownerMover.Arrived)
                          //  _ownerMover = new ActorMover(this.Owner);
                    }
                }
            }
        }

        public override void Cancel(int tickCounter)
        {
            // TODO: make this per-power instead?
            if (_powerRan)
                this.Owner.World.PowerManager.CancelAllPowers(this.Owner);

            this.Done = true;
        }

        public Mooege.Core.GS.Map.Scene GetSceneAt(Vector3D pos)
        {

            foreach (var s in this.Owner.World.Scenes)
            {
                if (s.Value.Bounds.Contains(pos.X, pos.Y))
                    return s.Value;
            }
            return null;
        }
        public Vector3D GetwalkableVector(Vector3D oldloc,Vector3D destloc)
        {
            Mooege.Core.GS.Map.Scene startscene = GetSceneAt(oldloc);
            Mooege.Core.GS.Map.Scene destscene = GetSceneAt(destloc);
            
            if (startscene.DynamicID == destscene.DynamicID)
            {
                foreach (var mesh in startscene.NavZone.NavCells)
                {
                    
                    /*if (mesh.Bounds.Contains(oldloc.AsPoint()) && mesh.Bounds.Contains(destloc.AsPoint()))
                        return destloc;
                    else if (mesh.Bounds.Contains(oldloc.AsPoint()))
                    {

                    }
                    else
                    {

                    }*/
                }

            }
            else
            {
                // cross scene
            }
            return oldloc;
        }
    }
}
