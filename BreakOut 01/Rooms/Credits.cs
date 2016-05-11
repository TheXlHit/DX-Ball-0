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
    public class Credits
    {
        ContentManager Content;
        GraphicsDevice graphic;
        Texture2D Logo;
        Texture2D _Credits;

        Elements.ButtonClass btnBack;
        int _BGmusic = 0;

        MouseState oldState, currentState;

        public Credits(ContentManager Content, GraphicsDevice graphic)
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

            _Credits = Content.Load<Texture2D>("dxballCredits");

            btnBack = new Elements.ButtonClass(new Vector2(graphic.Viewport.Width / 2 - Game1.scaleTool.ScaleX(190f) / 2, graphic.Viewport.Height / 1.142857142857143f), Content.Load<Texture2D>("btnSprite"), Game1.scaleTool.ScaledVector2(190, 48), Content.Load<SpriteFont>("ButtonFontClassic"));
            btnBack.Text = "Back";
        }

        public void Show(string file)
        {
            _BGmusic = Game1.bass.Play(file, 70, true);
        }

        internal void Update(KeyboardState key, MouseState mouse)
        {
            btnBack.Update(mouse);

            oldState = currentState;
            currentState = mouse;

            if (currentState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                if (btnBack.IsHoverd)
                {
                    Game1.bass.Stop(_BGmusic);
                    Game1.roomMenu.Show(System.IO.Directory.GetCurrentDirectory() + @"\Sounds\Main.mp3");
                    Game1.CurrentGameState = Game1.GameState.MainMenu;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            btnBack.Draw(spriteBatch);

            spriteBatch.Draw(_Credits, new Rectangle(0, 0, graphic.Viewport.Width, graphic.Viewport.Height), Color.White);
            float w = graphic.Viewport.Width / 1.7462482946794f;
            float h = graphic.Viewport.Height / 4.615384615384615f;
            spriteBatch.Draw(Logo, new Rectangle((int)(graphic.Viewport.Width / 2 - w / 2 + .5f), graphic.Viewport.Height / 12, (int)w, (int)h), Color.White);
        }
    }
}
