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
        Random gen = new Random();
        public static SpriteFont font;
        Texture2D MazeTexture;
        Color[,] PixelMap;
        Color[] Colors;

        TimeSpan timer;

        Bot mataMouse;
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
            System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);

        }

        protected override void LoadContent()
        {
            

            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");

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
            Texture2D mouseTexture = Content.Load<Texture2D>("MataMouse");
            mataMouse = new Bot(mouseTexture, new Vector2( 8 + 14 + 12, 8 + 14 + 13), Color.White, 0f, Vector2.Zero, Vector2.One);

            List<Rectangle> frontFrames = new List<Rectangle> { new Rectangle(12, 20, 24, 24),
                                                           new Rectangle(60, 22, 24, 22),
                                                           new Rectangle(108, 20, 24, 24),
                                                           new Rectangle(156, 22 , 24, 22)};
            mataMouse.LoadFrames(frontFrames, AnimatedSprite.Direction.Down,  8f);

            List<Rectangle> backFrames = new List<Rectangle> { new Rectangle(12, 164, 24, 24),
                                                           new Rectangle(60, 166, 24, 22),
                                                           new Rectangle(108, 164, 24, 24),
                                                           new Rectangle(156, 166 , 24, 22)};
            mataMouse.LoadFrames(backFrames, AnimatedSprite.Direction.Up, 8f);

            List<Rectangle> rightFrames = new List<Rectangle> { new Rectangle(4, 118, 38, 22),
                                                           new Rectangle(54, 118, 38, 22),
                                                           new Rectangle(100, 118, 38, 22),
                                                           new Rectangle(150, 118 , 38, 22)};
            mataMouse.LoadFrames(rightFrames, AnimatedSprite.Direction.Right, 8f);

            List<Rectangle> leftFrames = new List<Rectangle> { new Rectangle(6, 70, 38, 24),
                                                           new Rectangle(52, 70, 38, 24),
                                                           new Rectangle(102, 70, 38, 24),
                                                           new Rectangle(148, 70 , 38, 24)};
            mataMouse.LoadFrames(leftFrames, AnimatedSprite.Direction.Left, 8f);


            graphics.PreferredBackBufferHeight = MazeTexture.Height - 100;
            graphics.PreferredBackBufferWidth = MazeTexture.Width;
            graphics.ApplyChanges();

            mataMouse.Stat = Bot.Status.Done;

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
            if (robotUpdate > TimeSpan.FromMilliseconds(1))
            {
                mataMouse.Update(gameTime, ks, lastks, PixelMap);
                robotUpdate = TimeSpan.Zero;
            }

            if(mataMouse.Stat != Bot.Status.Done)
            {
                timer += gameTime.ElapsedGameTime;
            }
            base.Update(gameTime);
            lastks = ks;

            if(ks.IsKeyDown(Keys.P))
            {
                mataMouse.Stat = Bot.Status.Scanning;
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(MazeTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            mataMouse.Draw(spriteBatch);
            spriteBatch.DrawString(font, timer.ToString(), Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
