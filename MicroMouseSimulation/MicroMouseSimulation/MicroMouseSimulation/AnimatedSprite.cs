using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MicroMouseSimulation
{
    class AnimatedSprite : Sprite
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public Direction _direction;

        protected List<Rectangle> _frontFrames;

        public List<Rectangle> FrontFrames
        {
            get { return _frontFrames; }
            set { _frontFrames = value; }
        }

        protected List<Rectangle> _backFrames;

        public List<Rectangle> BackFrames
        {
            get { return _backFrames; }
            set { _backFrames = value; }
        }

        protected List<Rectangle> _leftFrames;

        public List<Rectangle> LeftFrames
        {
            get { return _leftFrames; }
            set { _leftFrames = value; }
        }

        protected List<Rectangle> _rightFrames;

        public List<Rectangle> RightFrames
        {
            get { return _rightFrames; }
            set { _rightFrames = value; }
        }

        protected Rectangle _frame;

        public Rectangle Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }
        

        protected int _currentFrame;

        public int CurrentFrame
        {
            get { return _currentFrame; }
            set { _currentFrame = value; }
        }

        private TimeSpan _animationTimer;

        public TimeSpan AnimationTimer
        {
            get { return _animationTimer; }
            set { _animationTimer = value; }
        }

        protected float _framesPerSecond;

        public float FramesPerSecond
        {
            get { return _framesPerSecond; }
            set { _framesPerSecond = value; }
        }

        protected SpriteEffects _currentEffect;

        public SpriteEffects CurrentEffect
        {
            get { return _currentEffect; }
            set { _currentEffect = value; }
        }

        public AnimatedSprite(Texture2D texture, Vector2 position, Color tint, float rotation, Vector2 origin, Vector2 scale)
            : base(texture,  position,  tint,  rotation,  origin, scale)
        {
            _currentEffect = SpriteEffects.None;   
        }

        public virtual void LoadFrames(List<Rectangle> frames, Direction direction, float framesPerSecond)
        {
            switch(direction)
            {
                case Direction.Down:
                    _frontFrames = frames;
                    break;
                case Direction.Up:
                    _backFrames = frames;
                    break;
                case Direction.Right:
                    _rightFrames = frames;
                    break;
                case Direction.Left:
                    _leftFrames = frames;
                    break;
            }
            _framesPerSecond = framesPerSecond;
        }

        public virtual void Update(GameTime gameTime)
        {
            _animationTimer += gameTime.ElapsedGameTime;

            if(_animationTimer > TimeSpan.FromMilliseconds( 1000f / _framesPerSecond))
            {
                CurrentFrame++;
                _animationTimer = TimeSpan.Zero;
            }
            
            switch(_direction)
            {
                case Direction.Down:
                    if (CurrentFrame >= _frontFrames.Count)
                    {
                        CurrentFrame = 0;
                    }
                    _frame = _frontFrames[_currentFrame];
                    break;

                case Direction.Up:
                    if (CurrentFrame >= _backFrames.Count)
                    {
                        CurrentFrame = 0;
                    }
                    _frame = _backFrames[_currentFrame];
                    break;

                case Direction.Left:
                    if (CurrentFrame >= _leftFrames.Count)
                    {
                        CurrentFrame = 0;
                    }
                    _frame = _leftFrames[_currentFrame];
                    break;

                case Direction.Right:
                    if (CurrentFrame >= _rightFrames.Count)
                    {
                        CurrentFrame = 0;
                    }
                    _frame = _rightFrames[_currentFrame];
                    break;
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, _frame, _tint, _rotation, _origin, _scale, _currentEffect, _layer);            
        }

    }
}
