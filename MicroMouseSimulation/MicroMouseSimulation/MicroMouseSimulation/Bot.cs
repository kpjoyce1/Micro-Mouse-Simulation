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
    class Bot
    {
        enum Status
        {
            Scanning,
            Returning,
            Calculating,
            SpeedRun
        }

        private Status _status;

        private Texture2D _texture;

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        private Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private Vector2 _velocity;

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        private int _width;

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private Color _color;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private MazeGraph _mazeData;

        public MazeGraph MazeData
        {
            get { return _mazeData; }
            set { _mazeData = value; }
        }

        private Vector2 _mazePosition;

        public Vector2 MazePosition
        {
            get { return _mazePosition; }
            set { _mazePosition = value; }
        }

        private MazeNode[] _shortestPath;

        public MazeNode[] ShortestPath
        {
            get { return _shortestPath; }
            set { _shortestPath = value; }
        }


        public Bot(Texture2D image, Vector2 position, int width, int height, Color tint)        
        {
            _texture = image;
            _position = position;
            _color = tint;
            _width = width;
            _height = height;
            _status = Status.Scanning;
            _mazeData = new MazeGraph();
        }

        bool rightBlocked;
        bool leftBlocked;
        bool topBlocked;
        bool bottomBlocked;

        public void Update(KeyboardState ks, KeyboardState lastks, Color[,] PixelMap)
        {
            // 7,7  7,8 8,7 8,8 are considered the center
            PixelDetect(PixelMap);
            
            //MazePosition is relative coordinate system
            _mazePosition = new Vector2((int)((_position.X - (int)(_position.X / Game1.MapUnit)*8f) / Game1.MapUnit), (int)(_position.Y - (int)(_position.Y / Game1.MapUnit) * 8f) / Game1.MapUnit);



            if (_status == Status.Scanning)
            {
                //Added Edges to graph
                if (!rightBlocked)
                {
                    _mazeData.AddEdge(_mazePosition, _mazePosition + new Vector2(1, 0));
                }

                if (!leftBlocked)
                {
                    _mazeData.AddEdge(_mazePosition, _mazePosition - new Vector2(1, 0));
                }

                if (!topBlocked)
                {
                    _mazeData.AddEdge(_mazePosition, _mazePosition - new Vector2(0, 1));
                }

                if (!bottomBlocked)
                {
                    _mazeData.AddEdge(_mazePosition, _mazePosition + new Vector2(0, 1));
                }

                //Mark the node as visited
                _mazeData.Visited(_mazePosition);
                              

            }
            else if (_status == Status.Calculating)
            {
                _shortestPath = _mazeData.shortestPath(Vector2.Zero, Vector2.One * 8f);
            }
            //Keyboard movement
            int movement = Game1.MapUnit + 8;                

            if (ks.IsKeyDown(Keys.Left) && lastks.IsKeyUp(Keys.Left) &&  !leftBlocked)
            {
                _position.X -= movement;
            }
            else if (ks.IsKeyDown(Keys.Right) && lastks.IsKeyUp(Keys.Right) && !rightBlocked)
            {
                _position.X += movement;
            }
            else if (ks.IsKeyDown(Keys.Up) && lastks.IsKeyUp(Keys.Up) && !topBlocked)
            {
                _position.Y -= movement;
            }
            else if (ks.IsKeyDown(Keys.Down) && lastks.IsKeyUp(Keys.Down) &&  !bottomBlocked)
            {
                _position.Y += movement;
            }


        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, _color, 0f, Vector2.Zero, new  Vector2(_width, _height), SpriteEffects.None, 1f);
            _mazeData.Draw(spriteBatch);
        }        


        public void PixelDetect(Color[,] PixelMap)
        {
            int sensorDistance = Game1.MapUnit / 2;

            leftBlocked = false;
            rightBlocked = false;
            topBlocked = false;
            bottomBlocked = false;

            for (int currDistance = 1; currDistance < sensorDistance; currDistance++) //simulate a sensor 
            {
                if (_position.X - currDistance < 0)
                {
                    leftBlocked = true;
                }
                else if (_position.X + currDistance + _width >= Game1.MapWidth)
                {
                    rightBlocked = true;
                }
                else
                {
                    if (PixelMap[(int)_position.X - currDistance, (int)_position.Y].A != 0 || PixelMap[(int)_position.X - currDistance, (int)_position.Y + _height].A != 0)
                    {
                        leftBlocked = true;
                    }
                    if (PixelMap[(int)_position.X + _width + currDistance, (int)_position.Y].A != 0 || PixelMap[(int)_position.X + _width + currDistance, (int)_position.Y + _height].A != 0)
                    {
                        rightBlocked = true;
                    }
                }

                if (_position.Y - currDistance < 0)
                {
                    topBlocked = true;
                }
                else if (_position.Y + currDistance + _height >= Game1.MapHeight)
                {
                    bottomBlocked = true;
                }
                else
                {
                    if (PixelMap[(int)_position.X, (int)_position.Y + _height + currDistance].A != 0 || PixelMap[(int)_position.X + _width, (int)_position.Y + _height + currDistance].A != 0)
                    {
                        bottomBlocked = true;
                    }

                    if (PixelMap[(int)_position.X, (int)_position.Y - currDistance].A != 0 || PixelMap[(int)_position.X + _width, (int)_position.Y - currDistance].A != 0)
                    {
                        topBlocked = true;
                    }
                }
            }
        }

    }
}
