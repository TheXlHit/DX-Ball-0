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
    public class GameOver
    {
        ContentManager Content;
        GraphicsDevice graphic;

        #region Elements

        Elements.ButtonClass btnMainMenu;
        MouseState oldState, currentState;
        Elements.TextBoxClass textBox;

        #endregion

        Texture2D Logo;
        int _BGmusic = 0;
        int Score = 0;
        long TimeTicks = 0;
        SpriteFont Font;
        SpriteFont FontLittle;

        public GameOver(ContentManager Content, GraphicsDevice graphic)
        {
            this.Content = Content;
            this.graphic = graphic;

            if (!Program.IsTournament)
            {
                Logo = Content.Load<Texture2D>("dxball");
            }
            else
            {
                Logo = Content.Load<Texture2D>("dxballTE");
            }
            Font = Content.Load<SpriteFont>("InfoDisplay");
            FontLittle = Content.Load<SpriteFont>("MapInfo");

            btnMainMenu = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, graphic.Viewport.Height / 1.2f), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190, 48), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnMainMenu.Text = "Back to Mainmenu";

            textBox = new Elements.TextBoxClass(Content, graphic, new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(100f), graphic.Viewport.Height / 2 + Game1.scaleTool.ScaleY(90f)), Game1.scaleTool.ScaledVector2(200f, 28f));
            textBox.Text = "UNKNOWN";
        }

        public void SetScore(int Score, long Time)
        {
            this.Score = Score;
            this.TimeTicks = Time;
        }

        public void Show(string file)
        {
            _BGmusic = Game1.bass.Play(file, 70, true);
        }

        public void Update(KeyboardState key, MouseState mouse)
        {
            btnMainMenu.Update(mouse);
            if (Game1.scrList.IsInRanking(Score))
            {
                textBox.Update(mouse, key);
            }

            oldState = currentState;
            currentState = mouse;

            if (currentState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                if (btnMainMenu.IsHoverd)
                {
                    Game1.scrList.AddScore(new BreackOutLevelEditor.Score(Score, textBox.Text, TimeTicks));
                    Game1.bass.Stop(_BGmusic);
                    Game1.roomMenu.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Main.mp3");
                    Game1.CurrentGameState = Game1.GameState.MainMenu;
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            float w = graphic.Viewport.Width / 1.7462482946794f;
            float h = graphic.Viewport.Height / 4.615384615384615f;
            batch.Draw(Logo, new Rectangle((int)(graphic.Viewport.Width / 2 - w / 2 + .5f), graphic.Viewport.Height / 12, (int)w, (int)h), Color.White);

            Vector2 lenght = Game1.ScaleVector2(Font.MeasureString("Game Over"), Game1.scaleTool.GetWindowScaleY);
            batch.DrawString(Font, "Game Over", new Vector2(graphic.Viewport.Width / 2 - lenght.X / 2, graphic.Viewport.Height / 2 - lenght.Y / 2 + Game1.scaleTool.ScaleY(10f)), Color.White,
                0f, new Vector2(0, 0), Game1.scaleTool.GetWindowScaleY, SpriteEffects.None, 0f);

            lenght = Game1.ScaleVector2(FontLittle.MeasureString("Your Score: " + Score), Game1.scaleTool.GetWindowScaleY);
            batch.DrawString(FontLittle, "Your Score: " + Score, new Vector2(graphic.Viewport.Width / 2 - lenght.X / 2, graphic.Viewport.Height / 2 - lenght.Y / 2 + Game1.scaleTool.ScaleY(60f)), Color.White,
                0f, new Vector2(0, 0), Game1.scaleTool.GetWindowScaleY, SpriteEffects.None, 0f);

            if (Game1.scrList.IsInRanking(Score))
            {
                textBox.Draw(batch);
            }

            btnMainMenu.Draw(batch);
        }
    }
}
