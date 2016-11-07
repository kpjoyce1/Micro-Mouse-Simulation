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
    class MazeNode
    {
        private Vector2 _location;

        public Vector2 Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private MazeEdge _edgeHead;

        public MazeEdge EdgeHead
        {
            get { return _edgeHead; }
            set { _edgeHead = value; }
        }

        private MazeNode _next;

        public MazeNode Next
        {
            get { return _next; }
            set { _next = value; }
        }

        private MazeNode _parent;

        public MazeNode Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        private int _distance;

        public int Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        private bool _mark;

        public bool Mark
        {
            get { return _mark; }
            set { _mark = value; }
        }

        private int _scanValue;

        public int ScanValue
        {
            get { return _scanValue; }
            set { _scanValue = value; }
        }

        private float g;

        public float G
        {
            get { return g; }
            set { g = value; }
        }

        private float h;

        public float H
        {
            get { return h; }
            set { h = value; }
        }

        private float f;

        public float F
        {
            get { return f; }
            set { f = value; }
        }


        private Color _color;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private Stack<MazeNode> _edgeStack;

        public Stack<MazeNode> EdgeStack
        {
            get { return _edgeStack; }
            set { _edgeStack = value; }
        }

        private MazeNode pathParent;

        public MazeNode PathParent
        {
            get { return pathParent; }
            set { pathParent = value; }
        }
        private bool pathFlag;

        public bool PathFlag
        {
            get { return pathFlag; }
            set { pathFlag = value; }
        }



        public  MazeNode(Vector2 location)
        {
            _distance = -1;
            _mark = false;        
            _location = location;
            _color = Color.Red;
            _edgeStack = new Stack<MazeNode>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {                                    
            spriteBatch.Draw(Game1.pixel, MazeGraph.toScreenCoordinates(_location), null, _color, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

           // spriteBatch.DrawString(Game1.font, ScanValue.ToString(), new Vector2(0, -5) + _location * Game1.MapUnit + _location * 8f + Vector2.One * (30 + 6), Color.Red, 0f, Vector2.Zero,
           //                             1f, SpriteEffects.None, 1f);
            if (_edgeHead != null)
            {
                MazeEdge edge = _edgeHead;
                while (edge != null)
                {
                    
                    if (edge.Target.Location.X - _location.X >= 0 && edge.Target.Location.Y - _location.Y >= 0)
                    {
                        spriteBatch.Draw(Game1.pixel, _location * Game1.MapUnit + _location * 8f + Vector2.One * (30 + 6), null, Color.Red, 0f, Vector2.Zero,
                                         new Vector2((float)(edge.Target.Location.X - _location.X) * (Game1.MapUnit + 8) + 1, (float)(edge.Target.Location.Y - _location.Y) * (Game1.MapUnit + 8) + 1),
                                         SpriteEffects.None, 1f);
                    }
                    else
                    {
                        spriteBatch.Draw(Game1.pixel, edge.Target.Location * Game1.MapUnit + edge.Target.Location * 8f + Vector2.One * (30 + 6), null, Color.Red, 0f, Vector2.Zero,
                                         new Vector2((float)(_location.X - edge.Target.Location.X) * (Game1.MapUnit + 8) + 1, (float)(_location.Y - edge.Target.Location.Y ) * (Game1.MapUnit + 8) + 1),
                                         SpriteEffects.None, 1f);
                    }
                    edge = edge.Next;
                }
            }
        }
    }
}
