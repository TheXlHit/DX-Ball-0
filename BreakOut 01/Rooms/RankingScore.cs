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

namespace BreakOut_01.Rooms
{
    public class RankingScore
    {
        ContentManager Content;
        GraphicsDevice graphic;

        #region Elements

        Elements.ButtonClass btnMainMenu;
        Elements.ButtonClass btnPageDown;
        Elements.ButtonClass btnPageUp;
        MouseState oldState, currentState;

        #endregion

        Texture2D Logo;
        Texture2D BackColorLight;
        Texture2D BackColorDark;

        int _BGmusic = 0;
        SpriteFont Font;
        SpriteFont FontLittle;

        int Page = 1;

        public RankingScore(ContentManager Content, GraphicsDevice graphic)
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

            BackColorDark = new Texture2D(graphic, 1, 1);
            BackColorLight = new Texture2D(graphic, 1, 1);

            BackColorDark.SetData(new Color[] { new Color(Color.Black, 160) });
            BackColorLight.SetData(new Color[] { new Color(Color.Black, 120) });

            btnMainMenu = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, graphic.Viewport.Height / 1.237113402061856f), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190, 48), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnMainMenu.Text = "Back";

            btnPageDown = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, 582), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(50, 24), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnPageDown.Text = "->";

            btnPageUp = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, 582), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(50, 24), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnPageUp.Text = "<-";
        }

        public void Update(MouseState mouse)
        {
            btnMainMenu.Update(mouse);
            btnPageUp.Update(mouse);
            btnPageDown.Update(mouse);

            oldState = currentState;
            currentState = mouse;

            if (currentState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                if (btnMainMenu.IsHoverd)
                {
                    Game1.bass.Stop(_BGmusic);
                    Game1.roomMenu.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Main.mp3");
                    Game1.CurrentGameState = Game1.GameState.MainMenu;
                }
                if (btnPageUp.IsHoverd)
                {
                    Page = Page > 1 ? Page - 1 : 1;
                }
                if (btnPageDown.IsHoverd)
                {
                    Page = Page < 10 ? Page + 1 : 10;
                }
            }
        }

        public void Show(string file)
        {
            _BGmusic = Game1.bass.Play(file, 70, true);
        }

        public void Draw(SpriteBatch batch)
        {
            float w = graphic.Viewport.Width / 1.7462482946794f;
            float h = graphic.Viewport.Height / 4.615384615384615f;
            batch.Draw(Logo, new Rectangle((int)(graphic.Viewport.Width / 2 - w / 2 + .5f), graphic.Viewport.Height / 12, (int)w, (int)h), Color.White);

            #region Scores

            List<Score> SortedList = Game1.scrList.GetOrdered();

            bool IsLightedBackColor = false;
            int Offset = 4;
            int Y = (int)(graphic.Viewport.Height / 2.571428571428571f + .5f);
            float lastY = 0;
            Vector2 vec = new Vector2(0,0);

            for (int i = 0; i < 10; i++)
            {
                int pageAdd = 10 * (Page - 1);
                if (i + pageAdd < SortedList.Count)
                {
                    string User = SortedList[i + pageAdd].User.PadRight(26, ' ').Substring(0, 25);
                    string Date = new DateTime().AddTicks(SortedList[i + pageAdd].Date).ToString().PadRight(21, ' ').Substring(0, 20);
                    string HighScore = SortedList[i + pageAdd].HighScore.ToString().PadRight(11, ' ').Substring(0, 10);
                    string Rank = "#" + (i + pageAdd + 1).ToString().PadRight(4, ' ').Substring(0, 3);

                    string text = Rank + " " + User + " " + Date + "    " + HighScore;
                    vec = FontLittle.MeasureString(text);
                    vec = Game1.ScaleVector2(vec, graphic.Viewport.Height / 720f);

                    batch.Draw(IsLightedBackColor ? BackColorLight : BackColorDark, new Rectangle((int)(graphic.Viewport.Width / 2 - vec.X / 2) - Offset, (int)(Y + (i * (vec.Y + 4))) - Offset, (int)(vec.X), (int)(vec.Y)), Color.White);
                    batch.DrawString(FontLittle, text, new Vector2(graphic.Viewport.Width / 2 - vec.X / 2, Y + (i * (vec.Y + 4))), Color.White, 0f, new Vector2(0, 0), graphic.Viewport.Height / 720f, SpriteEffects.None, 0f);
                }
                else
                {
                    string User = "---".PadRight(26, ' ').Substring(0, 25);
                    string Date = "---".PadRight(21, ' ').Substring(0, 20);
                    string HighScore = "0".PadRight(11, ' ').Substring(0, 10);
                    string Rank = "#" + (i + pageAdd + 1).ToString().PadRight(4, ' ').Substring(0, 3);

                    string text = Rank + " " + User + " " + Date + "    " + HighScore;
                    vec = FontLittle.MeasureString(text);
                    vec = Game1.ScaleVector2(vec, graphic.Viewport.Height / 720f);

                    batch.Draw(IsLightedBackColor ? BackColorLight : BackColorDark, new Rectangle((int)(graphic.Viewport.Width / 2 - vec.X / 2) - Offset, (int)(Y + (i * (vec.Y + 4))) - Offset, (int)(vec.X), (int)(vec.Y)), Color.White);
                    batch.DrawString(FontLittle, text, new Vector2(graphic.Viewport.Width / 2 - vec.X / 2, Y + (i * (vec.Y + 4))), Color.White, 0f, new Vector2(0, 0), graphic.Viewport.Height / 720f, SpriteEffects.None, 0f);
                }
                IsLightedBackColor = IsLightedBackColor ? false : true;
                lastY = Y + (i * (vec.Y + 4));
            }

            lastY += graphic.Viewport.Height / 28.8f;

            btnPageUp.Location = new Vector2(graphic.Viewport.Width / 2 - vec.X / 2 - 4, lastY);
            btnPageDown.Location = new Vector2(graphic.Viewport.Width / 2 - vec.X / 2 + (vec.X - btnPageUp.Size.X - 2), lastY);

            btnPageUp.Draw(batch);
            btnPageDown.Draw(batch);

            #endregion

            btnMainMenu.Draw(batch);
        }
    }
}
