using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Un4seen.Bass;
using System.IO;
using System;

namespace BreakOut_01
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int WindowWidth = 1280;
        public static int WindowHeight = 720;

        public const int DefaultWindowWidth = 1280;
        public const int DefaultWindowHeight = 720;

        public static int WindowViewWidth = 1280;
        public static int WindowViewHeight = 720;
        public static BassWrapper bass;
        public static BreackOutLevelEditor.ScoreList scrList = new BreackOutLevelEditor.ScoreList();
        private RenderTarget2D Render;
        public static ScaleTools scaleTool;

        #region Debug

        public static bool ShowHitbox = false;
        public static bool ShowBlockImage = true;
        public static bool ShowAdvancedBlockImage = true;
        public static bool ShowDebug = false;
        public static bool IsDebug = false;
        public static bool IsVSync = true;
        public static bool IsFullScreen = true;
        public static bool IsFixedTimeStamp = true;
        public SpriteFont Font;
        public static Texture2D HitLighting;
        public static Texture2D DebugBackground;
        int ExitVal = 0;

        static Elements.FPSCounter fpsCounter = new Elements.FPSCounter();

        #endregion

        private KeyboardState oldKeyState, currentKeyState;

        public enum GameState : int
        {
            MainMenu,
            Credits,
            Level,
            LevelPause,
            LevelSelect,
            Ranking,
            GameOver,
        }

        public static GameState CurrentGameState = GameState.MainMenu;

        #region Rooms

        public static Rooms.MainMenuElement roomMenu;
        public static Rooms.LevelPlay levelPlay;
        public static Rooms.LevelSelect Select;
        public static Rooms.Credits _Credits;
        public static Rooms.GameOver GameOver;
        public static Rooms.RankingScore Ranking;
        public Elements.BackgroundAnimation _BG;

        #endregion

        public Game1()
        {
            Elements.ExternelHelperClass.RestoreDefault();

            BassNet.Registration("Username", "Register Code");
            bass = new BassWrapper();
            graphics = new GraphicsDeviceManager(this);
            graphics.HardwareModeSwitch = false;
            Content.RootDirectory = "Content";

            if (Program.FullscreenResolution != null)
            {
                WindowWidth = (int)Program.FullscreenResolution.Width;
                WindowHeight = (int)Program.FullscreenResolution.Height;
            }

            if (System.IO.File.Exists("Scores_0.bin"))
            {
                scrList = BreackOutLevelEditor.WriterClass.ReadScoresFromFile("Scores_0.bin");
            }

            Window.Title = Program.IsTournament ? "DX-Ball-0 TE 1.0" : "DX-Ball-0 Final 1.3";

            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.SynchronizeWithVerticalRetrace = IsVSync;
            graphics.ApplyChanges();
            IsFullScreen = Program.IsFullscrenStartup;
            graphics.IsFullScreen = IsFullScreen;
            this.IsFixedTimeStep = IsFixedTimeStamp;

            this.IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            WindowViewWidth = GraphicsDevice.DisplayMode.Width;
            WindowViewHeight = GraphicsDevice.DisplayMode.Height;
            scaleTool = new ScaleTools(GraphicsDevice, DefaultWindowWidth, DefaultWindowHeight);
            
            HitLighting = new Texture2D(GraphicsDevice, 1, 1);
            HitLighting.SetData(new Color[] { Color.White });

            roomMenu = new Rooms.MainMenuElement(Content, GraphicsDevice);
            levelPlay = new Rooms.LevelPlay(Content, GraphicsDevice);
            Select = new Rooms.LevelSelect(Content, GraphicsDevice);
            _Credits = new Rooms.Credits(Content, GraphicsDevice);
            GameOver = new Rooms.GameOver(Content, GraphicsDevice);
            _BG = new Elements.BackgroundAnimation(Content, GraphicsDevice);
            Ranking = new Rooms.RankingScore(Content, GraphicsDevice);

            DebugBackground = new Texture2D(GraphicsDevice, 1, 1);
            DebugBackground.SetData(new Color[] { new Color(Color.Black, 150) });

            Font = Content.Load<SpriteFont>("Debug");

            Render = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            Select.UsedLevel = BreackOutLevelEditor.WriterClass.ReadFromFile("level_0.bin", GraphicsDevice);
            roomMenu.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Main.mp3");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            bass.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            oldKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            _BG.Update();

            #region Debug

            if (currentKeyState.IsKeyDown(Keys.F1) && oldKeyState.IsKeyUp(Keys.F1))
            {
                ShowDebug = ShowDebug ? false : true;
            }

            if (currentKeyState.IsKeyDown(Keys.F2) && oldKeyState.IsKeyUp(Keys.F2))
            {
                ShowHitbox = ShowHitbox ? false : true;
            }

            if (currentKeyState.IsKeyDown(Keys.F3) && oldKeyState.IsKeyUp(Keys.F3))
            {
                ShowBlockImage = ShowBlockImage ? false : true;
            }

            if (currentKeyState.IsKeyDown(Keys.F4) && oldKeyState.IsKeyUp(Keys.F4))
            {
                ShowAdvancedBlockImage = ShowAdvancedBlockImage ? false : true;
            }

            if (currentKeyState.IsKeyDown(Keys.F5) && oldKeyState.IsKeyUp(Keys.F5))
            {
                graphics.SynchronizeWithVerticalRetrace = graphics.SynchronizeWithVerticalRetrace ? false : true;
                graphics.ApplyChanges();
                IsVSync = graphics.SynchronizeWithVerticalRetrace;
            }

            if (currentKeyState.IsKeyDown(Keys.F6) && oldKeyState.IsKeyUp(Keys.F6) && !Program.IsTournament)
            {
                IsDebug = IsDebug ? false : true;
            }

            if (currentKeyState.IsKeyDown(Keys.F7) && oldKeyState.IsKeyUp(Keys.F7) && !Program.IsTournament)
            {
                IsFixedTimeStamp = IsFixedTimeStamp ? false : true;
                this.IsFixedTimeStep = IsFixedTimeStamp;
            }

            if (currentKeyState.IsKeyDown(Keys.F11) && oldKeyState.IsKeyUp(Keys.F11))
            {
                graphics.ToggleFullScreen();
                WindowViewWidth = GraphicsDevice.DisplayMode.Width;
                WindowViewHeight = GraphicsDevice.DisplayMode.Height;
                IsFullScreen = graphics.IsFullScreen;
            }

            if (currentKeyState.IsKeyDown(Keys.F12) && oldKeyState.IsKeyUp(Keys.F12))
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Screenshots"))
                { Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Screenshots"); }

                using (FileStream fs = new FileStream(Directory.GetCurrentDirectory() + @"\Screenshots\Img_" + DateTime.Now.ToString("MM-dd-yy H;mm;ss") + ".png", FileMode.Create))
                {
                    Render.SaveAsPng(fs, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                }
            }

            #endregion

            #region KeyBoard

            if (currentKeyState.IsKeyDown(Keys.Escape) && oldKeyState.IsKeyUp(Keys.Escape))
            {
                if (CurrentGameState == GameState.Level || CurrentGameState == GameState.LevelPause)
                {
                    if (CurrentGameState == GameState.LevelPause) { CurrentGameState = GameState.Level; }
                    else if (CurrentGameState == GameState.Level) { CurrentGameState = GameState.LevelPause; }
                }
                else
                {
                    Exit();
                }
            }

            if (currentKeyState.IsKeyDown(Keys.Escape))
            {
                ExitVal += gameTime.ElapsedGameTime.Milliseconds;
                if (ExitVal >= 2000 && !Program.IsTournament)
                {
                    Exit();
                }
            }
            else
            {
                ExitVal = 0;
            }

            #endregion

            #region Update

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    {
                        roomMenu.Update(Keyboard.GetState(), Mouse.GetState());
                        if (roomMenu.IsExit)
                        {
                            Exit();
                        }
                    }
                    break;
                case GameState.Level:
                    {
                        levelPlay.Update(Keyboard.GetState(), Mouse.GetState(), gameTime);
                    }
                    break;
                case GameState.LevelPause:
                    {
                        levelPlay.UpdatePause(Mouse.GetState());
                    }
                    break;
                case GameState.Credits:
                    {
                        _Credits.Update(Keyboard.GetState(), Mouse.GetState());
                    }
                    break;
                case GameState.GameOver:
                    {
                        GameOver.Update(Keyboard.GetState(), Mouse.GetState());
                    }
                    break;
                case GameState.Ranking:
                    {
                        Ranking.Update(Mouse.GetState());
                        this.IsMouseVisible = true;
                    }
                    break;
            }

            #endregion

            fpsCounter.UpdateFPS(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetRenderTarget(Render);

            spriteBatch.Begin();

            _BG.Draw(spriteBatch);

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    {
                        roomMenu.Draw(spriteBatch);
                        this.IsMouseVisible = true;
                    }
                    break;
                case GameState.Level:
                    {
                        levelPlay.Draw(spriteBatch);
                        this.IsMouseVisible = false;
                    }
                    break;
                case GameState.LevelPause:
                    {
                        levelPlay.Draw(spriteBatch);
                        this.IsMouseVisible = true;
                    }
                    break;
                case GameState.LevelSelect:
                    {
                        Select.Draw(spriteBatch);
                        this.IsMouseVisible = true;
                    }
                    break;
                case GameState.Credits:
                    {
                        _Credits.Draw(spriteBatch);
                        this.IsMouseVisible = true;
                    }
                    break;
                case GameState.GameOver:
                    {
                        GameOver.Draw(spriteBatch);
                        this.IsMouseVisible = true;
                    }
                    break;
                case GameState.Ranking:
                    {
                        Ranking.Draw(spriteBatch);
                        this.IsMouseVisible = true;
                    }
                    break;
            }

            if (ShowDebug)
            {
                int y = (int)(GraphicsDevice.Viewport.Height / 72f + .5f);
                if (CurrentGameState == GameState.LevelPause || CurrentGameState == GameState.Level) { y = (int)(GraphicsDevice.Viewport.Height / 11.07692307692308f + .5f); }
                DrawDebug(spriteBatch, Font, new Vector2(scaleTool.ScaleX(10f), y), scaleTool.GetWindowScaleY, GraphicsDevice);
            }

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin();
            spriteBatch.Draw(Render, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();

            fpsCounter.ComputeFPS();
            base.Draw(gameTime);
        }

        public static Vector2 ScaleVector2(Vector2 vec, float scale)
        {
            vec.Y = vec.Y * scale;
            vec.X = vec.X * scale;

            return vec;
        }

        #region Debug

        public static void DrawDebug(SpriteBatch batch, SpriteFont Font, Vector2 Position, float scale, GraphicsDevice graphic)
        {
            float Y = 0;
            float Height = graphic.Viewport.Height / 48f;
            string text = "";
            Vector2 vec = CalculateMax(Font, scale, graphic);
            batch.Draw(DebugBackground, new Rectangle((int)Position.X - 2, (int)Position.Y - 2, (int)vec.X + 4, (int)vec.Y + (int)Y + 4), Color.White);

            text = "[F2]  Show Hitbox:            " + ShowHitbox.ToString();
            vec = Font.MeasureString(text);
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;

            text = "[F3]  Show Block Images:      " + ShowBlockImage.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;

            text = "[F4]  Show Advanced Images:   " + ShowAdvancedBlockImage.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;

            text = "[F5]  Enable V-Sync:          " + IsVSync.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;

            text = "[F6]  Enable Debug:           " + IsDebug.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;
            
            text = "[F7]  Enable Fixed Time Step: " + IsFixedTimeStamp.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;

            text = "[F11] Enable Fullscreen:      " + IsFullScreen.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;

            text = "[F12] Take Screenshot";
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;
            Y += Height;

            text = "Game Resolution: " + WindowWidth + "x" + WindowHeight;
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;

            text = "Window Viewport: " + WindowViewWidth + "x" + WindowViewHeight;
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            Y += Height;

            text = "FPS:             " + fpsCounter.FPS;
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));

            #region InGame

            if (CurrentGameState == GameState.Level || CurrentGameState == GameState.LevelPause)
            {
                Y += Height;
                Y += Height;

                text = "Max Blocks: " + levelPlay.MaxCountBreakable + "/" + levelPlay.MaxCount;
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
                Y += Height;

                text = "Blocks:     " + levelPlay.GetBreakableCount(levelPlay.Objects) + "/" + levelPlay.Objects.Count;
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
                Y += Height;
                Y += Height;

                text = "Player Position: [" + levelPlay.Player.Position.X + "px | " + levelPlay.Player.Position.Y + "px]";
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
                Y += Height;

                text = "Ball Position:   [" + levelPlay.Ball.Position.X + "px | " + levelPlay.Ball.Position.Y + "px]";
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
                Y += Height;

                text = "Ball Movement:   [" + (levelPlay.Ball.IsStuck ? "0" : (scaleTool.ScaleX(levelPlay.Ball.Movement.X) * levelPlay.MultiplicatorBallSpeed) + "") + "px | " + (levelPlay.Ball.IsStuck ? "0" : (scaleTool.ScaleY(levelPlay.Ball.Movement.Y) * levelPlay.MultiplicatorBallSpeed) + "") + "px]";
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
                Y += Height;

                text = "Ball Speed Up:   [" + (25000 - levelPlay.Seconder) + " ms]";
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                DrawDebugString(batch, text, Font, new Vector2(Position.X, Position.Y + Y));
            }

            #endregion

            vec = ScaleVector2(vec, scaleTool.GetWindowScaleY);

            DrawRectangle(batch, HitLighting, new Rectangle((int)Position.X - 2, (int)Position.Y - 2, (int)vec.X + 4, (int)vec.Y + (int)Y + 4), Color.White);
        }

        public static Vector2 CalculateMax(SpriteFont Font, float scale, GraphicsDevice graphic)
        {
            Vector2 vec = new Vector2(0, 0);
            float Y = 0;
            float Height = graphic.Viewport.Height / 48f;
            string text = "";

            text = "[F2]  Show Hitbox:            " + ShowHitbox.ToString();
            vec = Font.MeasureString(text);
            Y += Height;

            text = "[F3]  Show Block Images:      " + ShowBlockImage.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;

            text = "[F4]  Show Advanced Images:   " + ShowAdvancedBlockImage.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;

            text = "[F5]  Enable V-Sync:          " + IsVSync.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;

            text = "[F6]  Enable Debug:           " + IsDebug.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;

            text = "[F7]  Enable Fixed Time Step: " + IsFixedTimeStamp.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;

            text = "[F11] Enable Fullscreen:      " + IsFullScreen.ToString();
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;

            text = "[F12] Take Screenshot";
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;
            Y += Height;

            text = "Game Resolution: " + WindowWidth + "x" + WindowHeight;
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;

            text = "Window Viewport: " + WindowViewWidth + "x" + WindowViewHeight;
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            Y += Height;

            text = "FPS:             " + fpsCounter.FPS;
            vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;

            #region InGame

            if (CurrentGameState == GameState.Level || CurrentGameState == GameState.LevelPause)
            {
                Y += Height;
                Y += Height;

                text = "Max Blocks: " + levelPlay.MaxCountBreakable + "/" + levelPlay.MaxCount;
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                Y += Height;

                text = "Blocks:     " + levelPlay.GetBreakableCount(levelPlay.Objects) + "/" + levelPlay.Objects.Count;
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                Y += Height;
                Y += Height;

                text = "Player Position: [" + levelPlay.Player.Position.X + "px | " + levelPlay.Player.Position.Y + "px]";
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                Y += Height;

                text = "Ball Position:   [" + levelPlay.Ball.Position.X + "px | " + levelPlay.Ball.Position.Y + "px]";
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                Y += Height;

                text = "Ball Movement:   [" + (levelPlay.Ball.IsStuck ? "0" : (scaleTool.ScaleX(levelPlay.Ball.Movement.X) * levelPlay.MultiplicatorBallSpeed) + "") + "px | " + (levelPlay.Ball.IsStuck ? "0" : (scaleTool.ScaleY(levelPlay.Ball.Movement.Y) * levelPlay.MultiplicatorBallSpeed) + "") + "px]";
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
                Y += Height;

                text = "Ball Speed Up:   [" + (25000 - levelPlay.Seconder) + " ms]";
                vec = Font.MeasureString(text).X > vec.X ? Font.MeasureString(text) : vec;
            }

            #endregion

            vec = ScaleVector2(vec, scaleTool.GetWindowScaleY);

            return new Vector2(vec.X, vec.Y + Y);
        }

        public static void DrawDebugString(SpriteBatch batch, string Text, SpriteFont Font, Vector2 Position)
        {
            batch.DrawString(Font, Text, new Vector2(Position.X, Position.Y), Color.White,
                0f, new Vector2(0, 0), scaleTool.GetWindowScaleY, SpriteEffects.None, 0f);
        }

        public static void DrawRectangle(SpriteBatch batch, Texture2D texture, Rectangle rectangle, Color color)
        {
            batch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 1), color);
            batch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, 1), color);
            batch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Top, 1, rectangle.Height), color);
            batch.Draw(texture, new Rectangle(rectangle.Right, rectangle.Top, 1, rectangle.Height + 1), color);
        }

        #endregion
    }
}
