using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mooege.Core.GS.AI.PatherDebug
{
    public partial class PatherDebug : Form
    {
        private Actors.Actor actor;
        private List<Common.Types.Math.Vector3D> vectorPathList;
        private readonly Brush _unwalkableBrush = Brushes.Red;
        private readonly Pen _unwalkablePen = new Pen(Color.Red, 1.0f);
        private readonly Brush _walkableBrush = Brushes.Blue;
        private readonly Pen _walkablePen = new Pen(Color.Blue, 1.0f);
        public PatherDebug()
        {
            InitializeComponent();
        }

        public PatherDebug(Actors.Actor actor, List<Common.Types.Math.Vector3D> vectorPathList)
        {
            // TODO: Complete member initialization
            this.actor = actor;
            this.vectorPathList = vectorPathList;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Point[] path = new Point[vectorPathList.Count];
            int i = 0;
            foreach (var x in vectorPathList)
            {
                path[i++] = new Point((int)(x.X - actor.CurrentScene.Bounds.Left),(int)(x.Y - actor.CurrentScene.Bounds.Top));
            }
            Rectangle[] mesh = new Rectangle[actor.CurrentScene.NavZone.NavCells.Count];
            i=0;
            foreach(var n in actor.CurrentScene.NavZone.NavCells)
            {
                if(n.Flags.HasFlag(Mooege.Common.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
                mesh[i++] = new Rectangle((int)n.Bounds.Location.X,(int)n.Bounds.Location.Y,(int)n.Bounds.Size.Width,(int)n.Bounds.Size.Height);
            }
            e.Graphics.DrawRectangles(_unwalkablePen, mesh);
            e.Graphics.DrawLines(_walkablePen, path);
        }
    }
}
