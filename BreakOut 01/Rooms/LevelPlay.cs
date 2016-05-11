using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using BreackOutLevelEditor;
using System.IO;
using Un4seen.Bass;
using BreakOut_01.Elements;

namespace BreakOut_01.Rooms
{
    public class LevelPlay
    {
        #region Objects

        ContentManager Content;
        GraphicsDevice graphic;
        private KeyboardState oldKeyState, currentKeyState;
        private MouseState oldState, currentState;

        int LevelIndex = 0;
        Level[] LoadedLevel = new Level[0];
        Dictionary<string, Texture2D> _Sprite = new Dictionary<string, Texture2D>();

        public List<HitObject> Objects = new List<HitObject>();
        public int MaxCount = 0;
        public int MaxCountBreakable = 0;
        List<HiddenBall> ExplosiveObjects = new List<HiddenBall>();

        ScoreDisplay Score;
        public BallObject Ball;
        public PlayerPaddle Player;
        Elements.MapInfo Map;
        Elements.LifeBar Life;
        Elements.ButtonClass btnBack;
        Elements.ButtonClass btnContinue;

        public float MultiplicatorBallSpeed = 1f;
        public float Seconder = 0;

        #region Pause

        Texture2D PauseOverlay;
        SpriteFont Font;
        SpriteFont LittleFont;

        #endregion

        int TimerSet = 50;
        HitObject last = new HitObject("", null, null, new Vector2(0,0), new Vector2(0, 0), null, null, new Color[] { Color.Black });

        #endregion

        #region EXT Loading

        private Dictionary<string, Texture2D> LoadComp()
        {
            Dictionary<string, Texture2D> temp = new Dictionary<string, Texture2D>();

            temp.Add("Default", Content.Load<Texture2D>("Default"));
            temp.Add("DefaultX2", Content.Load<Texture2D>("DefaultX2"));
            temp.Add("Type01", Content.Load<Texture2D>("Type01"));
            temp.Add("Type02", Content.Load<Texture2D>("Type02"));
            temp.Add("Type03", Content.Load<Texture2D>("Type03"));
            temp.Add("Type04", Content.Load<Texture2D>("Type04"));
            temp.Add("Type05", Content.Load<Texture2D>("Type05"));
            temp.Add("Type06", Content.Load<Texture2D>("Type06"));
            temp.Add("Type07", Content.Load<Texture2D>("Type07"));
            temp.Add("Type08", Content.Load<Texture2D>("Type08"));
            temp.Add("Type09", Content.Load<Texture2D>("Type09"));
            temp.Add("Type10", Content.Load<Texture2D>("Type10"));
            temp.Add("Type11", Content.Load<Texture2D>("Type11"));
            temp.Add("Type12", Content.Load<Texture2D>("Type12"));
            temp.Add("Type13", Content.Load<Texture2D>("Type13"));
            temp.Add("Type14", Content.Load<Texture2D>("Type14"));
            temp.Add("paddle", Content.Load<Texture2D>("paddle"));

            return temp;
        }

        private Texture2D GetSprite(string data)
        {
            if (_Sprite.ContainsKey(data))
            {
                return _Sprite[data];
            }
            return _Sprite["Default"];
        }

        #endregion

        public LevelPlay(ContentManager Content, GraphicsDevice graphic)
        {
            this.Content = Content;
            this.graphic = graphic;
            _Sprite = LoadComp();
            Font = Content.Load<SpriteFont>("InfoDisplay");
            LittleFont = Content.Load<SpriteFont>("MapCreator");

            LoadedLevel = WriterClass.ReadFromFile("level_0.bin", graphic);
            Score = new Elements.ScoreDisplay(Font, graphic);
            Map = new MapInfo(Content.Load<SpriteFont>("MapInfo"), Content.Load<SpriteFont>("MapCreator"), graphic);
            Vector2 ballSize = new Vector2(graphic.Viewport.Width / 80f, graphic.Viewport.Height / 45f);
            Ball = new BallObject(Content.Load<Texture2D>("BallImg"), new Vector2(graphic.Viewport.Width / 2 - ballSize.X / 2, graphic.Viewport.Height - 32 - ballSize.Y / 2), ballSize);
            Player = new PlayerPaddle(Content, graphic, new Vector2(graphic.Viewport.Width / 2 - graphic.Viewport.Width / 25.6f, graphic.Viewport.Height - 10 - graphic.Viewport.Height / 22.5f), new Vector2(graphic.Viewport.Width / 12.8f, graphic.Viewport.Height / 36f));
            Life = new LifeBar(Player.Texture, graphic);

            btnContinue = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, graphic.Viewport.Height / 2f), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190f, 48f), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnContinue.Text = "Continue";

            btnBack = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, btnContinue.Location.Y + Game1.scaleTool.ScaleY(74f)), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190f, 48f), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnBack.Text = "Back to Menu";

            PauseOverlay = new Texture2D(graphic, 1, 1);
            PauseOverlay.SetData(new Color[] { new Color(Color.Black, 190) });

            LoadLevel();
        }

        #region Collision

        private List<HitObject> CheckCollision(HitObject obj)
        {
            List<HitObject> temp = new List<HitObject>();
            HitObject[] tempArray = Objects.ToArray();

            foreach (HitObject h in tempArray)
            {
                if (h != obj && h.objType == "Type02")
                {
                    Rectangle r1 = new Rectangle(new Point((int)h.Position.X - 2, (int)h.Position.Y - 2), new Point((int)h.Size.X + 4, (int)h.Size.Y + 4));
                    Rectangle r2 = new Rectangle(new Point((int)obj.Position.X - 2, (int)obj.Position.Y - 2), new Point((int)obj.Size.X + 4, (int)obj.Size.Y + 4));
                    if (r1.Intersects(r2))
                    {
                        temp.Add(h);
                    }
                }
            }
            return temp;
        }

        private void PlayerCollision()
        {
            Rectangle r1 = new Rectangle(Ball.Position.ToPoint(), Ball.Size.ToPoint());
            Rectangle r2 = new Rectangle(Player.Position.ToPoint(), Player.Size.ToPoint());
            if (r1.Intersects(r2))
            {
                Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Boing.wav");
                Ball.Movement = new Vector2(GetVelocity(Ball.Position, Player.Position, Player.Size), Ball.Movement.Y);
                Ball.RevertMovement(BallObject.BallDirection.Up);
            }
        }

        private CollisionResult[] CheckCollision()
        {
            HitObject[] temp = Objects.ToArray();
            List<CollisionResult> output = new List<CollisionResult>();
            foreach (HitObject h in temp)
            {
                Rectangle _Block = new Rectangle(h.Position.ToPoint(), h.Size.ToPoint());
                Rectangle _Ball = new Rectangle(Ball.Position.ToPoint(), Ball.Size.ToPoint());
                Point _BallCenter = new Point((int)(Ball.Position.X + (Ball.Size.X / 2)), (int)(Ball.Position.Y + (Ball.Size.Y / 2)));

                if (_Block.Intersects(_Ball))
                {
                    //Hor
                    if (_BallCenter.X + 1 >= _Block.X && _BallCenter.X - 1 <= (_Block.X + _Block.Width))
                    {
                        //Up
                        if (_BallCenter.Y <= _Block.Y + (_Block.Height / 2))
                        {
                            BallObject.BallDirection b = BallObject.BallDirection.Up;
                            output.Add(new CollisionResult(Ball, h, b));
                        }
                        //Down
                        else if (_BallCenter.Y >= _Block.Y + (_Block.Height / 2))
                        {
                            BallObject.BallDirection b = BallObject.BallDirection.Down;
                            output.Add(new CollisionResult(Ball, h, b));
                        }
                    }
                    //Ver
                    else if (_BallCenter.X - 1 < _Block.X || _BallCenter.X + 1 > _Block.X + (_Block.Width / 2))
                    {
                        //Right
                        if (_BallCenter.X <= _Block.X)
                        {
                            BallObject.BallDirection b = BallObject.BallDirection.Right;
                            output.Add(new CollisionResult(Ball, h, b));
                        }
                        //Left
                        else if (_BallCenter.X >= _Block.X + _Block.Width)
                        {
                            BallObject.BallDirection b = BallObject.BallDirection.Left;
                            output.Add(new CollisionResult(Ball, h, b));
                        }
                    }
                }
            }
            return output.ToArray();
        }

        private CollisionResult[] CheckCollisionHidden()
        {
            HitObject[] temp = Objects.ToArray();
            HiddenBall[] temp2 = ExplosiveObjects.ToArray();
            List<CollisionResult> newOut = new List<CollisionResult>();

            foreach (HitObject h in temp)
            {
                foreach (HiddenBall b in temp2)
                {
                    Rectangle r1 = new Rectangle(h.Position.ToPoint(), h.Size.ToPoint());
                    Rectangle r2 = new Rectangle(b.Position.ToPoint(), b.Size.ToPoint());
                    if (r1.Intersects(r2))
                    {
                        newOut.Add(new CollisionResult(null, h, BallObject.BallDirection.Up));
                    }
                }
            }
            return newOut.ToArray();
        }

        #endregion

        #region Calculations

        private float GetVelocity(Vector2 posBall, Vector2 pos2, Vector2 pos2Size)
        {
            float ballX = posBall.X - pos2.X;
            float precent = ((ballX * 100f) / pos2Size.X) / 100f;
            float newMovX = (6 * precent) - 3;
            return newMovX;
        }

        public int GetBreakableCount(List<HitObject> obj)
        {
            int t = 0;
            foreach (HitObject h in obj)
            {
                if (h.objType != "Type09")
                {
                    t++;
                }
            }
            return t;
        }

        public bool TestForSpecialBlocks(List<HitObject> obj)
        {
            foreach (HitObject h in obj)
            {
                if (h.objType == "sp_obj_01")
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Level Options

        private void LoadLevel()
        {
            Level l = LoadedLevel[LevelIndex];
            GameObject[] t = l.Objects.ToArray();
            Objects.Clear();
            foreach (GameObject g in t)
            {
                float sizeX = graphic.Viewport.Width / 30.47619047619048f;
                float sizeY = graphic.Viewport.Height / 36f;

                float PosX = (g.Location.X / 42f) * sizeX;
                float PosY = (g.Location.Y / 20f) * sizeY;

                PosX += (graphic.Viewport.Width - (graphic.Viewport.Width / 1.26984126984127f)) / 2;
                PosY += (graphic.Viewport.Height - (graphic.Viewport.Height / 1.384615384615385f)) / 2 - (graphic.Viewport.Height / 14.4f);

                Color[] col = new Color[g.CustomColor.Length];

                for (int i = 0; i < col.Length; i++)
                {
                    col[i] = new Color(g.CustomColor[i].R, g.CustomColor[i].G, g.CustomColor[i].B);
                }

                Objects.Add(new Elements.HitObject(g.objType, GetSprite(g.objType), GetSprite("DefaultX2"), new Vector2(PosX, PosY), new Vector2(sizeX, sizeY), Content, graphic, col));
            }

            if (!LoadedLevel[LevelIndex].RatingFSK18 || Program.IsShowFSK18)
            {
                Map.MapCreator = l.Creator;
                Map.MapName = l.Name + (TestForSpecialBlocks(Objects) ? " [Image]" : "") + (l.RatingFSK18 ? " [Quastionable]" : "");
                MaxCountBreakable = GetBreakableCount(Objects);
                MaxCount = Objects.Count;
            }
            else if (LoadedLevel[LevelIndex].RatingFSK18 && !Program.IsShowFSK18)
            {
                Objects.Clear();
            }
        }

        public void Restart()
        {
            if (LoadedLevel[LevelIndex].RatingFSK18)
            {
                LevelIndex = 1;
            }
            else
            {
                LevelIndex = 0;
            }
            
            Ball.IsStuck = true;
            Player.Position = new Vector2(graphic.Viewport.Width / 2 - 50, graphic.Viewport.Height - 10 - 32);
            Life.Life = 5;
            Score.Score = 0;

            Player.SetBallPos();

            LoadLevel();
        }

        #endregion

        public void Draw(SpriteBatch batch)
        {
            HitObject[] temp = Objects.ToArray();
            foreach (HitObject g in temp)
            {
                g.Draw(batch);
            }

            if (Game1.ShowHitbox)
            {
                HiddenBall[] tempBall = ExplosiveObjects.ToArray();
                foreach (HiddenBall b in tempBall)
                {
                    b.Draw(batch);
                }
            }

            Ball.Draw(batch);
            Player.Draw(batch);

            Score.Draw(batch);
            Map.Draw(batch);
            Life.Draw(batch);

            if (Program.IsTournament)
            {
                Vector2 lenght = LittleFont.MeasureString("DX-Ball-0 TE");
                lenght = Game1.ScaleVector2(lenght, graphic.Viewport.Height / 720f);
                batch.DrawString(LittleFont, "DX-Ball-0 TE", new Vector2(graphic.Viewport.Width / 2 - lenght.X / 2, 15), Color.White, 0f, new Vector2(0, 0), graphic.Viewport.Height / 720f, SpriteEffects.None, 0f);
            }

            if (Game1.CurrentGameState == Game1.GameState.LevelPause)
            {
                batch.Draw(PauseOverlay, new Rectangle(0, 0, graphic.Viewport.Width, graphic.Viewport.Height), Color.White);
                Vector2 lenght = Font.MeasureString("Paused");
                lenght = Game1.ScaleVector2(lenght, graphic.Viewport.Height / 720f);
                batch.DrawString(Font, "Paused", new Vector2(graphic.Viewport.Width / 2 - lenght.X / 2, graphic.Viewport.Height / 2 - lenght.Y / 2 - (graphic.Viewport.Height / 7.2f)), Color.White, 0f, new Vector2(0, 0), graphic.Viewport.Height / 720f, SpriteEffects.None, 0f);
                if (!Program.IsTournament)
                {
                    btnBack.Draw(batch);
                }
                btnContinue.Draw(batch);
            }
        }

        public void Update(KeyboardState key, MouseState mouse, GameTime gameTime)
        {
            oldKeyState = currentKeyState;
            currentKeyState = key;

            #region Player

            Seconder += Ball.IsStuck ? 0f : gameTime.ElapsedGameTime.Milliseconds;
            if (Seconder >= 25000)
            {
                if (MultiplicatorBallSpeed < 1.6f)
                {
                    MultiplicatorBallSpeed += 0.2f;
                }
                Seconder = 0;
            }

            if (!Ball.IsStuck)
            {
                Ball.Position.Y -= (graphic.Viewport.Height / (720 / Ball.Movement.Y)) * MultiplicatorBallSpeed;
                Ball.Position.X += (graphic.Viewport.Width / (1280 / Ball.Movement.X)) * MultiplicatorBallSpeed;
            }
            else
            {
                Ball.Position = Player.BallPos;
            }

            if (key.IsKeyDown(Keys.Left)) { Player.Move(PlayerPaddle.Direction.Left); }
            if (key.IsKeyDown(Keys.Right)) { Player.Move(PlayerPaddle.Direction.Right); }

            if (currentKeyState.IsKeyDown(Keys.R) && oldKeyState.IsKeyUp(Keys.R) && Game1.IsDebug)
            {
                Ball.IsStuck = true;
                Ball.IsVisible = true;
                Ball.Position = Player.BallPos;
            }

            if (currentKeyState.IsKeyDown(Keys.N) && oldKeyState.IsKeyUp(Keys.N))
            {
                if (Game1.IsDebug)
                {
                    Ball.IsStuck = true;
                    Objects.Clear();
                }
            }

            if (key.IsKeyDown(Keys.Space))
            {
                if (Ball.IsStuck)
                {
                    Ball.Movement = new Vector2(GetVelocity(Ball.Position, Player.Position, Player.Size), Player.BallSpeed = 5f);
                    Ball.IsStuck = false;
                }
            }

            PlayerCollision();

            if (Player.Position.X <= 0) { Player.Position.X = 0; }
            if (Player.Position.X + Player.Size.X >= graphic.Viewport.Width) { Player.Position.X = graphic.Viewport.Width - Player.Size.X; }

            if (Ball.Position.X < -20 - Ball.Size.X || Ball.Position.X > graphic.Viewport.Width + 20)
            {
                Ball.IsStuck = true;
                Ball.IsVisible = true;
                Ball.Position = Player.BallPos;
            }

            if (Ball.Position.Y < -20 - Ball.Size.Y)
            {
                Ball.IsStuck = true;
                Ball.IsVisible = true;
                Ball.Position = Player.BallPos;
            }

            //Life Lost
            if (Ball.Position.Y > graphic.Viewport.Height + 20)
            {
                if (!Game1.IsDebug)
                {
                    if (Life.Kill())
                    {
                        Game1.GameOver.SetScore(Score.Score, DateTime.Now.Ticks);
                        Game1.GameOver.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\GameOver.mp3");
                        Game1.CurrentGameState = Game1.GameState.GameOver;
                    }
                    else
                    {
                        Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Padexplo.wav", 70);
                    }
                }
                Ball.IsStuck = true;
                Ball.IsVisible = true;
                Seconder = 0;
                MultiplicatorBallSpeed = 1f;
                Ball.Position = Player.BallPos;
            }

            #endregion

            #region HitObjects Collision


            CollisionResult[] result = CheckCollision();
            foreach(CollisionResult res in result)
            {
                if (res.hitObject.objType == "Type09")
                {
                    Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Ao-laser.wav");
                }
                if (res.hitObject.objType == "Type07")
                {
                    Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Glass.wav");
                }
                HitObjectOption o = res.hitObject.Destroy();
                if (o != null)
                {
                    if (o.IsExplosive)
                    {
                        ExplosiveObjects.Add(new HiddenBall(new Vector2(o.obj.Position.X - 8, o.obj.Position.Y - 8), new Vector2(o.obj.Size.X + 16, o.obj.Size.Y + 16)));

                        Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Xploshor.wav");
                        Objects.Remove(o.obj);
                        if (!Game1.IsDebug)
                        {
                            Score.Score += 100;
                        }
                    }
                    else
                    {
                        Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Wowpulse.wav");
                        Objects.Remove(o.obj);
                        if (!Game1.IsDebug)
                        {
                            Score.Score += 100;
                        }
                    }
                }
            }
            if (result.Length > 0)
            {
                result[0].ballObject.RevertMovement(result[0].Direction);
            }

            if (Ball.Position.Y <= 1)
            {
                Ball.RevertMovement(BallObject.BallDirection.Down);
                Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Bassdrum.wav", 100);
            }

            if (Ball.Position.X <= 1)
            {
                Ball.RevertMovement(BallObject.BallDirection.Left); Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Bassdrum.wav", 100);
            }

            if (Ball.Position.X + Ball.Size.X >= graphic.Viewport.Width - 1)
            {
                Ball.RevertMovement(BallObject.BallDirection.Right); Game1.bass.Play(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Bassdrum.wav", 100);
            }

            #endregion

            #region Explosive Collision

            foreach (CollisionResult c in CheckCollisionHidden())
            {
                if (c.hitObject.objType == "Type02")
                {
                    ExplosiveObjects.Add(new HiddenBall(new Vector2(c.hitObject.Position.X - 8, c.hitObject.Position.Y - 8), new Vector2(c.hitObject.Size.X + 16, c.hitObject.Size.Y + 16)));
                }
                Objects.Remove(c.hitObject);
                if (!Game1.IsDebug)
                {
                    Score.Score += 100;
                }
            }
            HiddenBall[] temp = ExplosiveObjects.ToArray();
            foreach (HiddenBall ball in temp)
            {
                ball.Time--;
                if (ball.Time <= 0)
                {
                    ExplosiveObjects.Remove(ball);
                }
            }

            #endregion

            #region Next Level Logic

            if (GetBreakableCount(Objects) <= 0)
            {
                TimerSet--;
                Ball.IsVisible = false;
                Ball.IsStuck = true;
                if (TimerSet <= 0)
                {
                    LevelIndex++;
                    if (LevelIndex > LoadedLevel.Length - 1)
                    {
                        Game1.GameOver.SetScore(Score.Score, DateTime.Now.Ticks);
                        Game1.GameOver.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\GameOver.mp3");
                        Game1.CurrentGameState = Game1.GameState.GameOver;
                    }
                    else
                    {
                        TimerSet = 50;
                        LoadLevel();
                        Ball.IsStuck = true;
                        Ball.IsVisible = true;
                        Ball.Position = Player.BallPos;
                        Seconder = 0;
                        MultiplicatorBallSpeed = 1f;
                    }
                }
            }

            #endregion
        }

        public void UpdatePause(MouseState mouse)
        {
            oldState = currentState;
            currentState = mouse;

            #region Pause

            if (Game1.CurrentGameState == Game1.GameState.LevelPause)
            {
                if (!Program.IsTournament)
                {
                    btnBack.Update(mouse);
                }
                btnContinue.Update(mouse);

                if (currentState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
                {
                    if (btnBack.IsHoverd && !Program.IsTournament)
                    {
                        Game1.roomMenu.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Main.mp3");
                        Game1.CurrentGameState = Game1.GameState.MainMenu;
                    }

                    if (btnContinue.IsHoverd)
                    {
                        Game1.CurrentGameState = Game1.GameState.Level;
                    }
                }
            }

            #endregion
        }
    }

    public class CollisionResult
    {
        public HitObject hitObject;
        public BallObject ballObject;
        public BallObject.BallDirection Direction;

        public CollisionResult(BallObject ball, HitObject hit, BallObject.BallDirection Direction)
        {
            this.ballObject = ball;
            this.hitObject = hit;
            this.Direction = Direction;
        }
    }
}
