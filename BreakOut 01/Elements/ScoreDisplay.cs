using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace BreakOut_01.Elements
{
    public class ScoreDisplay
    {
        private SpriteFont Font;
        private GraphicsDevice Device;
        public int Score = 0;

        public ScoreDisplay(SpriteFont font, GraphicsDevice device)
        {
            this.Font = font;
            this.Device = device;
        }

        public void Draw(SpriteBatch batch)
        {
            if (Font != null)
            {
                Vector2 lenght = Game1.ScaleVector2(Font.MeasureString(Score.ToString("00000000000")), Game1.scaleTool.GetWindowScaleY);
                batch.DrawString(Font, Score.ToString("00000000000"), new Vector2(Device.Viewport.Width - (Device.Viewport.Width / 106.66666666666666666666666666667f + lenght.X), Device.Viewport.Height / 90), Color.White,
                0f, new Vector2(0, 0), Game1.scaleTool.GetWindowScaleY, SpriteEffects.None, 0f);
            }
        }
    }

    public class MapInfo
    {
        private SpriteFont FontMap;
        private SpriteFont FontCreator;
        public string MapName = "Sandstorm";
        public string MapCreator = "Darude";
        GraphicsDevice graphic;
        private int Y = 20;

        public MapInfo(SpriteFont fontMap, SpriteFont fontCreator, GraphicsDevice graphic)
        {
            this.FontMap = fontMap;
            this.FontCreator = fontCreator;
            this.graphic = graphic;
            Y = (int)(graphic.Viewport.Height / 36f + .5f);
        }

        public void Draw(SpriteBatch batch)
        {
            if (FontMap != null && FontCreator != null)
            {
                float X = graphic.Viewport.Width / 128f;
                batch.DrawString(FontMap, MapName, new Vector2(X, Y), Color.White, 0f, new Vector2(0, 0), graphic.Viewport.Height / 720, SpriteEffects.None, 0f);
                batch.DrawString(FontCreator, "Map by " + MapCreator, new Vector2(X, Y + 20 * (graphic.Viewport.Height / 720)), Color.White, 0f, new Vector2(0, 0), graphic.Viewport.Height / 720, SpriteEffects.None, 0f);
            }
        }
    }

    public class LifeBar
    {
        Texture2D Player;
        GraphicsDevice graphic;
        public int Life = 5;

        public LifeBar(Texture2D Paddle, GraphicsDevice graphic, int Life = 5)
        {
            Player = Paddle;
            this.graphic = graphic;
            this.Life = Life;
        }

        public bool Kill()
        {
            Life--;
            if (Life < 0)
            {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch batch)
        {
            int X = 0;
            for(int i = 0; i < Life; i++)
            {
                batch.Draw(Player, new Rectangle((int)(graphic.Viewport.Width / 128f + .5f) + X, (int)(graphic.Viewport.Height / 72f + .5f), (int)(graphic.Viewport.Width / 55.65217391304348f + .5f), (int)(graphic.Viewport.Height / 120 + .5f)), Color.White);
                X += (int)(graphic.Viewport.Width / 47.40740740740741 + .5f);
            }
        }
    }

    public class BackgroundAnimation
    {
        ContentManager Content;
        GraphicsDevice graphic;
        Texture2D Wallpaper;
        Vector2 Location = new Vector2(0, 0);
        float X = 0f;

        public BackgroundAnimation(ContentManager Content, GraphicsDevice graphic)
        {
            this.Content = Content;
            this.graphic = graphic;
            Wallpaper = Content.Load<Texture2D>("BGAnim");
        }

        public void Update()
        {
            if (Game1.CurrentGameState != Game1.GameState.Level)
            {
                X -= graphic.Viewport.Width / 640f;
                if (X <= -(graphic.Viewport.Width / 0.835509138381201f))
                {
                    X = 0;
                }
                Location.X = (int)(X + .5f);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (Game1.CurrentGameState != Game1.GameState.Level && Game1.CurrentGameState != Game1.GameState.LevelPause)
            {
                batch.Draw(Wallpaper, new Rectangle((int)Location.X, (int)Location.Y, (int)(graphic.Viewport.Width / 0.4551920341394026f + .5f), graphic.Viewport.Height), Color.White);
            }
        }
    }
}
