using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroMouseSimulation
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D MazeTexture;
        Color[,] PixelMap;
        Color[] Colors;

        Bot microMouse;
        KeyboardState ks, lastks;

        TimeSpan robotUpdate;

        public static int MapHeight;
        public static int MapWidth;
        public static int MapUnit;
        public static Texture2D pixel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {           
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            MazeTexture = Content.Load<Texture2D>("Maze");
            MapHeight = MazeTexture.Height;
            MapWidth = MazeTexture.Width;
            MapUnit = 56;  
                 
            Colors = new Color[MapWidth * MapHeight];
            MazeTexture.GetData<Color>(Colors);

            PixelMap = new Color[MapWidth, MapHeight];

            for (int x = 0; x < MazeTexture.Width; x++)                
            {
                for (int y = 0; y < MazeTexture.Height; y++)
                {
                    PixelMap[x, y] = Colors[x + y * MazeTexture.Width];
                }
            }
            
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            microMouse = new Bot(pixel, new Vector2( 8 + 14, 8 + 14), 28, 28, Color.Red);

            graphics.PreferredBackBufferHeight = MazeTexture.Height;
            graphics.PreferredBackBufferWidth = MazeTexture.Width;
            graphics.ApplyChanges();

        }

        protected override void UnloadContent()
        {
            
        }

    
        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            ks = Keyboard.GetState();

            robotUpdate += gameTime.ElapsedGameTime;
            if (robotUpdate > TimeSpan.FromSeconds(1))
            {
                microMouse.Update(ks, lastks, PixelMap);
                robotUpdate = TimeSpan.Zero;
            }
            base.Update(gameTime);
            lastks = ks;
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(MazeTexture, Vector2.Zero, Color.White);
            microMouse.Draw(spriteBatch);       
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
