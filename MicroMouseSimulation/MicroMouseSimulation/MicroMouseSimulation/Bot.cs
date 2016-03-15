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
    class Bot : AnimatedSprite
    {
        enum Status
        {
            Scanning,
            Returning,
            Calculating,
            SpeedRun
        }

        private Status _status;

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

        private Vector2 _wayPoint;

        public Vector2 WayPoint
        {
            get { return _wayPoint; }
            set { _wayPoint = value; }
        }

        public Bot(Texture2D image, Vector2 position, Color tint,
                    float rotation, Vector2 origin, Vector2 scale)
            : base(image, position, tint, rotation, origin, scale)        
        {
            _status = Status.Scanning;
            _direction = Direction.Down;
            _mazeData = new MazeGraph();

            _layer = 0.01f;
        }

        //actual movement blocked
        bool rightBlocked;
        bool leftBlocked;
        bool topBlocked;
        bool bottomBlocked;

        bool rightWall;
        bool leftWall;
        bool topWall;
        bool bottomWall;

        public void Update(GameTime gameTime, KeyboardState ks, KeyboardState lastks, Color[,] PixelMap)
        {
            base.Update(gameTime);

            _width = _frame.Width;
            _height = _frame.Height;

            _origin = new Vector2(_width / 2, _height / 2);
            
            //MazePosition is relative coordinate system
            _mazePosition = MazeGraph.toMazeCoorinates(_position);

            // 7,7  7,8 8,7 8,8 are considered the center
            PixelDetect(PixelMap);

            //Added Edges to graph
            if (!rightWall)
            {
                _mazeData.AddEdge(_mazePosition, _mazePosition + new Vector2(1, 0));
                _mazeData.AddEdge( _mazePosition + new Vector2(1, 0), _mazePosition);
            }

            if (!leftWall)
            {
                _mazeData.AddEdge(_mazePosition, _mazePosition - new Vector2(1, 0));
                _mazeData.AddEdge(_mazePosition - new Vector2(1, 0), _mazePosition);
            }

            if (!topWall)
            {
                _mazeData.AddEdge(_mazePosition, _mazePosition - new Vector2(0, 1));
                _mazeData.AddEdge(_mazePosition - new Vector2(0, 1), _mazePosition);
            }

            if (!bottomWall)
            {
                _mazeData.AddEdge(_mazePosition, _mazePosition + new Vector2(0, 1));
                _mazeData.AddEdge(_mazePosition + new Vector2(0, 1), _mazePosition);
            }         

            if(ks.IsKeyDown(Keys.K) && lastks.IsKeyUp(Keys.K))
            {
                _wayPoint = _mazeData.nextNodeInScan(_mazePosition);
            }

            if (ks.IsKeyDown(Keys.L) && lastks.IsKeyUp(Keys.L))
            {
                _shortestPath = _mazeData.shortestPath(Vector2.Zero, _mazePosition);
            }

            //Keyboard movement
            int movement = (Game1.MapUnit + 8) / 30;

            if (ks.IsKeyDown(Keys.Left) && !leftBlocked)
            {
                _position.X -= movement;
                if (_direction != Direction.Left)
                {
                    _direction = Direction.Left;
                    _currentFrame = 0;
                }
            }
            else if (ks.IsKeyDown(Keys.Right) && !rightBlocked)
            {
                _position.X += movement;
                if (_direction != Direction.Right)
                {
                    _direction = Direction.Right;
                    _currentFrame = 0;
                }
            }
            else if (ks.IsKeyDown(Keys.Up) && !topBlocked)
            {
                _position.Y -= movement;
                if (_direction != Direction.Up)
                {
                    _direction = Direction.Up;
                    _currentFrame = 0;
                }
            }
            else if (ks.IsKeyDown(Keys.Down) && !bottomBlocked)
            {
                _position.Y += movement;
                if (_direction != Direction.Down)
                {
                    _direction = Direction.Down;
                    _currentFrame = 0;
                }
            }


        }

        public override  void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            _mazeData.Draw(spriteBatch);
        }

        public void Scanning()
        {


        }

        public void PixelDetect(Color[,] PixelMap)
        {
            
            int sensorDistance = (int)(1f * Game1.MapUnit);
            int bodyCollisionDistance = 6;
            leftBlocked = false;
            rightBlocked = false;
            topBlocked = false;
            bottomBlocked = false;

            leftWall = false;
            rightWall = false;
            topWall = false;
            bottomWall = false;

            for (int currDistance = 1; currDistance < sensorDistance; currDistance++) //simulate a sensor 
            {
                if ((_position.X - _width / 2) - currDistance < 0)
                {
                    leftWall = true;
                    if (currDistance < bodyCollisionDistance)
                    {
                        leftBlocked = true;
                    }
                }
                else
                {
                    if (PixelMap[(int)(_position.X - _width / 2) - currDistance, (int)(_position.Y)].A != 0 || PixelMap[(int)(_position.X - _width / 2) - currDistance, (int)(_position.Y - _height / 2)].A != 0 || PixelMap[(int)(_position.X - _width / 2) - currDistance, (int)(_position.Y + _height / 2)].A != 0)
                    {
                        leftWall = true;
                        if (currDistance < bodyCollisionDistance)
                        {
                            leftBlocked = true;
                        }
                    }
                }

                if ((_position.X + _width / 2) + currDistance >= Game1.MapWidth)
                {
                    rightWall = true;
                    if (currDistance < bodyCollisionDistance)
                    {
                        rightBlocked = true;
                    }
                }
                else
                {
                    if (PixelMap[(int)(_position.X + _width / 2) + currDistance, (int)(_position.Y)].A != 0 || PixelMap[(int)(_position.X + _width / 2) + currDistance, (int)(_position.Y - _height / 2)].A != 0 || PixelMap[(int)(_position.X + _width / 2) + currDistance, (int)(_position.Y + _height / 2)].A != 0)
                    {
                        rightWall = true;
                        if (currDistance < bodyCollisionDistance)
                        {
                            rightBlocked = true;
                        }
                    }
                }

                if ((_position.Y - _height / 2) - currDistance < 0)
                {
                    if (currDistance < bodyCollisionDistance)
                    {
                        topBlocked = true;
                    }
                    topWall = true;
                }
                else
                {
                    if (PixelMap[(int)(_position.X), (int)(_position.Y - _height / 2) - currDistance].A != 0 || PixelMap[(int)(_position.X - _width / 2), (int)(_position.Y - _height / 2) - currDistance].A != 0 || PixelMap[(int)(_position.X + _width / 2), (int)(_position.Y - _height / 2) - currDistance].A != 0)
                    {
                        topWall = true;
                        if (currDistance < bodyCollisionDistance)
                        {
                            topBlocked = true;
                        }
                    }
                }

                if ((_position.Y + _height / 2) + currDistance >= Game1.MapHeight)
                {
                    if (currDistance < bodyCollisionDistance)
                    {
                        bottomBlocked = true;
                    }
                    bottomWall = true;
                }
                else
                {
                    if (PixelMap[(int)(_position.X), (int)(_position.Y + _height / 2) + currDistance].A != 0 || PixelMap[(int)(_position.X - _width / 2), (int)(_position.Y + _height / 2) + currDistance].A != 0 || PixelMap[(int)(_position.X + _width / 2), (int)(_position.Y + _height / 2) + currDistance].A != 0)
                    {
                        bottomWall = true;
                        if (currDistance < bodyCollisionDistance)
                        {
                            bottomBlocked = true;
                        }
                    }
                }

            }
        }
    }
}
