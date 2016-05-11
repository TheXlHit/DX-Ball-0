using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace BreakOut_01.Rooms
{
    public class MainMenuElement
    {
        ContentManager Content;
        GraphicsDevice graphic;

        #region Elements

        Elements.ButtonClass btnPlay;
        Elements.ButtonClass btnCredtits;
        Elements.ButtonClass btnExit;
        Elements.ButtonClass btnHighScore;

        Texture2D Logo;
        public bool IsExit = false;
        int _BGmusic = 0;

        MouseState oldState, currentState;

        #endregion

        public MainMenuElement(ContentManager Content, GraphicsDevice graphic)
        {
            this.Content = Content;
            this.graphic = graphic;

            if (!Program.IsTournament) {
                Logo = Content.Load<Texture2D>("dxball");
            }
            else {
                Logo = Content.Load<Texture2D>("dxballTE");
            }

            int YOffset = (int)Game1.scaleTool.ScaleY(74f);

            int Y = (int)(graphic.Viewport.Height / 2f + .5f);
            btnPlay = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, Y),Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190f, 48f), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnPlay.Text = "Play";
            Y += YOffset;

            btnHighScore = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, Y), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190f, 48f), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnHighScore.Text = "Highscore List";
            if (Game1.scrList.Count > 0)
            {
                Y += YOffset;
            }

            btnCredtits = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, Y), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190f, 48f), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnCredtits.Text = "Credits";
            Y += YOffset;

            btnExit = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, Y), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190f, 48f), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnExit.Text = "Exit";
            Y += YOffset;
        }

        public void Show(string file)
        {
            _BGmusic = Game1.bass.Play(file, 70, true);
        }

        public void Resort()
        {
            int Y = (int)(graphic.Viewport.Height / 2f + .5f);
            int YOffset = (int)Game1.scaleTool.ScaleY(74f);

            btnPlay.Location.Y = Y;
            Y += YOffset;

            btnHighScore.Location.Y = Y;
            if (Game1.scrList.Count > 0)
            {
                Y += YOffset;
            }

            btnCredtits.Location.Y = Y;
            Y += YOffset;

            btnExit.Location.Y = Y;
            Y += YOffset;
        }

        internal void Update(KeyboardState key, MouseState mouse)
        {
            btnPlay.Update(mouse);
            btnCredtits.Update(mouse);
            btnExit.Update(mouse);

            oldState = currentState;
            currentState = mouse;

            if (Game1.scrList.Count > 0)
            {
                btnHighScore.Update(mouse);
            }

            if (currentState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                if (btnHighScore.IsHoverd)
                {
                    Game1.bass.Stop(_BGmusic);
                    Game1.Ranking.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\GameOver.mp3");
                    Game1.CurrentGameState = Game1.GameState.Ranking;
                }

                if (btnPlay.IsHoverd)
                {
                    Game1.bass.Stop(_BGmusic);
                    Game1.CurrentGameState = Game1.GameState.Level;
                    Game1.levelPlay.Restart();
                }

                if (btnCredtits.IsHoverd)
                {
                    Game1.bass.Stop(_BGmusic);
                    Game1._Credits.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Credits.mp3");
                    Game1.CurrentGameState = Game1.GameState.Credits;
                }

                if (btnExit.IsHoverd)
                {
                    IsExit = true;
                }
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            btnPlay.Draw(spriteBatch);
            btnCredtits.Draw(spriteBatch);
            btnExit.Draw(spriteBatch);
            if (Game1.scrList.Count > 0)
            {
                btnHighScore.Draw(spriteBatch);
            }

            float w = graphic.Viewport.Width / 1.7462482946794f;
            float h = graphic.Viewport.Height / 4.615384615384615f;
            spriteBatch.Draw(Logo, new Rectangle((int)(graphic.Viewport.Width / 2 - w / 2 + .5f), graphic.Viewport.Height / 12, (int)w, (int)h), Color.White);
        }
    }
}
