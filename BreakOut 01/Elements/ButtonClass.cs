using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace BreakOut_01.Elements
{
    public class ButtonClass
    {
        public string Text = "";
        public Vector2 Location;
        public Vector2 Size;
        public Texture2D Texture;
        public SpriteFont Font;
        public Color ForeColor = Color.White;
        private Color DrawAlpha = Color.White;
        private bool isHoverd = false;
        public bool IsHoverd
        {
            get
            {
                if (!isHoverd)
                {
                    return false;
                }
                isHoverd = false;
                return true;
            }
        }

        public ButtonClass(Vector2 Location, Texture2D Texture, Vector2 Size, SpriteFont Font)
        {
            Text = "Button";
            this.Location = Location;
            this.Texture = Texture;
            this.Size = Size;
            this.Font = Font;
        }

        public ButtonClass(Vector2 Location, Texture2D Texture, Vector2 Size, SpriteFont Font, string Text)
        {
            this.Text = Text;
            this.Location = Location;
            this.Texture = Texture;
            this.Size = Size;
            this.Font = Font;
        }


        public void Update(MouseState MouseST)
        {
            float ScaleFullX = (float)Game1.WindowViewWidth / (float)Game1.WindowWidth;
            float ScaleFullY = (float)Game1.WindowViewHeight / (float)Game1.WindowHeight;

            ScaleFullX = Game1.IsFullScreen ? ScaleFullX : 1f;
            ScaleFullY = Game1.IsFullScreen ? ScaleFullY : 1f;

            if (new Rectangle(MouseST.X, MouseST.Y, 1, 1).Intersects(new Rectangle((int)(Location.X * ScaleFullX), (int)(Location.Y * ScaleFullY), (int)(Size.X * ScaleFullX), (int)(Size.Y * ScaleFullY))))
            {
                DrawAlpha = Color.Yellow;
                isHoverd = true;
            }
            else
            {
                DrawAlpha = Color.White;
                isHoverd = false;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            float ScaleWitdh = Game1.WindowWidth / Size.X;
            float ScaleHeight = Game1.WindowHeight / Size.Y;

            spriteBatch.Draw(Texture, new Rectangle((int)Location.X, (int)Location.Y, (int)(Game1.WindowWidth / ScaleWitdh), (int)(Game1.WindowHeight / ScaleHeight)), DrawAlpha);
            Vector2 size = Game1.ScaleVector2(Font.MeasureString(Text), Game1.scaleTool.GetWindowScaleY);
            spriteBatch.DrawString(Font, Text, new Vector2(Location.X + Size.X / 2 - size.X / 2, Location.Y + Size.Y / 2 - size.Y / 2), ForeColor,
                0f, new Vector2(0, 0), Game1.scaleTool.GetWindowScaleY, SpriteEffects.None, 0f);
            if (Game1.ShowHitbox)
            {
                Game1.DrawRectangle(spriteBatch, Game1.HitLighting, new Rectangle((int)(Location.X), (int)(Location.Y), (int)(Size.X), (int)(Size.Y)), Color.White);
            }
        }
    }
}
