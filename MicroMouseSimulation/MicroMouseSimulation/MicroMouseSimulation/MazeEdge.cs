using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroMouseSimulation
{
    class MazeEdge
    {
        private MazeNode _target;

        public MazeNode Target
        {
            get { return _target; }
            set { _target = value; }
        }

        private MazeEdge _next;

        public MazeEdge Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public MazeEdge(MazeNode target)
        {
            _target = target;
        }

        public MazeEdge(MazeNode target, MazeEdge next)
        {
            _target = target;
            _next = next;
        }

    }
}
