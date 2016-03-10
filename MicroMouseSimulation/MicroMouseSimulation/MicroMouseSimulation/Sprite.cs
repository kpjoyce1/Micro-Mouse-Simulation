using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MicroMouseSimulation
{
    public class Sprite
    {
        protected Texture2D _texture;

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        protected Vector2 _origin;

        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        protected Vector2 _scale;

        public Vector2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        protected Vector2 _position;


        protected Color _tint;

        public Color Tint
        {
            get { return _tint; }
            set { _tint = value; }
        }

        protected float _rotation;

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        protected float _layer;

        public float Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }



        public Sprite() { }

        public Sprite(Texture2D texture, Vector2 position, Color tint)
        {
            _texture = texture;
            _position = position;
            _tint = tint;
        }

        public Sprite(Texture2D texture, Vector2 position, Color tint, float rotation, Vector2 origin, Vector2 scale)
            : this(texture, position, tint)
        {
            _rotation = rotation;
            _origin = origin;
            _scale = scale;         
        }

        public void Update()
        {

        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, _tint, _rotation, _origin, _scale, SpriteEffects.None, _layer);
        }
    }
}
