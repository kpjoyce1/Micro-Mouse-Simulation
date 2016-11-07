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
        public enum Status
        {
            Scanning,
            Returning,
            Calculating,
            Done,
            SpeedRun
        }

        private Status _status;
        public Status Stat { get { return _status; } set { _status = value; } }

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

        private Stack<MazeNode> _shortestPath;

        public Stack<MazeNode> ShortestPath
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
            _wayPoint = Vector2.One * -1f;
            _layer = 0.01f;
        }

        //actual movement blocked
        bool rightBlocked;
        bool leftBlocked;
        bool topBlocked;
        bool bottomBlocked;

        public bool RightWall;
        public bool LeftWall;
        public bool TopWall;
        public bool BottomWall;
        public bool enroute = false;
        public bool nextStep = true;

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
            if (!RightWall)
            {
                _mazeData.AddEdge(_mazePosition, _mazePosition + new Vector2(1, 0));
                _mazeData.AddEdge(_mazePosition + new Vector2(1, 0), _mazePosition);
            }
            if (!LeftWall)
            {
                _mazeData.AddEdge(_mazePosition, _mazePosition - new Vector2(1, 0));
                _mazeData.AddEdge(_mazePosition - new Vector2(1, 0), _mazePosition);
            }

            if (!TopWall)
            {
                _mazeData.AddEdge(_mazePosition, _mazePosition - new Vector2(0, 1));
                _mazeData.AddEdge(_mazePosition - new Vector2(0, 1), _mazePosition);
            }

            if (!BottomWall)
            {
                _mazeData.AddEdge(_mazePosition, _mazePosition + new Vector2(0, 1));
                _mazeData.AddEdge(_mazePosition + new Vector2(0, 1), _mazePosition);
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

            if ( !enroute && ks.IsKeyDown(Keys.K) && lastks.IsKeyUp(Keys.K))
            {
                _wayPoint = _mazeData.nextNodeInScanDepthFirst(this);
                enroute = true;
                _shortestPath = _mazeData.getAPath(MazeGraph.toMazeCoorinates(_position), _wayPoint);
                _wayPoint = _shortestPath.Pop().Location;
            }
            else if(nextStep)
            {
                if (_status == Status.Scanning && _mazeData.GetNode(MazeGraph.toMazeCoorinates(Position)).ScanValue <= 1)
                {
                    _status = Status.Returning;
                    _mazeData.ChangeScanValues(MazeGraph.toMazeCoorinates(_position));
                    enroute = true;
                }
                else if (_status == Status.Returning && _mazeData.GetNode(MazeGraph.toMazeCoorinates(Position)).ScanValue <= 1)
                {
                    _status = Status.Calculating;
                    _shortestPath = _mazeData.shortestPath(MazeGraph.toMazeCoorinates(_position), new Vector2(7, 7));

                }
                else if(_status == Status.Calculating && MazeGraph.toMazeCoorinates(_position) == 7f*Vector2.One)
                {
                    _status = Status.Done;
                }
                if (_status == Status.Scanning)
                {
                    _wayPoint = _mazeData.nextNodeInScanDepthFirst(this);
                    enroute = true;
                    _shortestPath = _mazeData.getAPath(MazeGraph.toMazeCoorinates(_position), _wayPoint);
                    _wayPoint = _shortestPath.Pop().Location;
                    nextStep = false;
                }
                else if(_status == Status.Returning)
                {
                    _wayPoint = _mazeData.nextNodeInScanDepthFirst(this);
                    enroute = true;
                    _shortestPath = _mazeData.getAPath(MazeGraph.toMazeCoorinates(_position), _wayPoint);
                    _wayPoint = _shortestPath.Pop().Location;
                    nextStep = false;
                }
                else if(_status == Status.Calculating)
                {
                    _wayPoint = _shortestPath.Pop().Location;
                    nextStep = false;
                    enroute = true;
                }

            }



            if (enroute)
            {

                if (Vector2.Distance(MazeGraph.toScreenCoordinates(_wayPoint), Position) < 1 && _shortestPath.Count == 0)
                {
                    enroute = false;
                    nextStep = true;

                }
                else if (Vector2.Distance(MazeGraph.toScreenCoordinates(_wayPoint), Position) < 1 && _shortestPath.Count != 0)
                {
                    _wayPoint = _shortestPath.Pop().Location;
                }
                else
                {
                    MoveToGridPosition(_wayPoint);
                }
            }


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            _mazeData.Draw(spriteBatch);
        }

        public void Scanning()
        {


        }

        public void MoveToGridPosition(Vector2 GridPosition)
        {
            if (Position != MazeGraph.toScreenCoordinates(GridPosition))
            {
                Vector2 deltaPosition = ((MazeGraph.toScreenCoordinates(GridPosition)) - Position);
                deltaPosition.Normalize();
                Position += deltaPosition;
                Position = MazeGraph.toScreenCoordinates(GridPosition);
            }
            //Position = MazeGraph.toScreenCoordinates(GridPosition);  
        }

        public void PixelDetect(Color[,] PixelMap)
        {
            
            int sensorDistance = (int)(1f * Game1.MapUnit);
            int bodyCollisionDistance = 6;
            leftBlocked = false;
            rightBlocked = false;
            topBlocked = false;
            bottomBlocked = false;

            LeftWall = false;
            RightWall = false;
            TopWall = false;
            BottomWall = false;

            for (int currDistance = 1; currDistance < sensorDistance; currDistance++) //simulate a sensor 
            {
                if ((_position.X - _width / 2) - currDistance < 0)
                {
                    LeftWall = true;
                    if (currDistance < bodyCollisionDistance)
                    {
                        leftBlocked = true;
                    }
                }
                else
                {
                    if (PixelMap[(int)(_position.X - _width / 2) - currDistance, (int)(_position.Y)].A != 0 || PixelMap[(int)(_position.X - _width / 2) - currDistance, (int)(_position.Y - _height / 2)].A != 0 || PixelMap[(int)(_position.X - _width / 2) - currDistance, (int)(_position.Y + _height / 2)].A != 0)
                    {
                        LeftWall = true;
                        if (currDistance < bodyCollisionDistance)
                        {
                            leftBlocked = true;
                        }
                    }
                }

                if ((_position.X + _width / 2) + currDistance >= Game1.MapWidth)
                {
                    RightWall = true;
                    if (currDistance < bodyCollisionDistance)
                    {
                        rightBlocked = true;
                    }
                }
                else
                {
                    if (PixelMap[(int)(_position.X + _width / 2) + currDistance, (int)(_position.Y)].A != 0 || PixelMap[(int)(_position.X + _width / 2) + currDistance, (int)(_position.Y - _height / 2)].A != 0 || PixelMap[(int)(_position.X + _width / 2) + currDistance, (int)(_position.Y + _height / 2)].A != 0)
                    {
                        RightWall = true;
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
                    TopWall = true;
                }
                else
                {
                    if (PixelMap[(int)(_position.X), (int)(_position.Y - _height / 2) - currDistance].A != 0 || PixelMap[(int)(_position.X - _width / 2), (int)(_position.Y - _height / 2) - currDistance].A != 0 || PixelMap[(int)(_position.X + _width / 2), (int)(_position.Y - _height / 2) - currDistance].A != 0)
                    {
                        TopWall = true;
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
                    BottomWall = true;
                }
                else
                {
                    if (PixelMap[(int)(_position.X), (int)(_position.Y + _height / 2) + currDistance].A != 0 || PixelMap[(int)(_position.X - _width / 2), (int)(_position.Y + _height / 2) + currDistance].A != 0 || PixelMap[(int)(_position.X + _width / 2), (int)(_position.Y + _height / 2) + currDistance].A != 0)
                    {
                        BottomWall = true;
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
