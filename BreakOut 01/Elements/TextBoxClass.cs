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
    public class TextBoxClass
    {
        Texture2D BG;
        Texture2D Border;
        SpriteFont Font;

        GraphicsDevice graphic;
        ContentManager Content;

        public string Text = "";
        private bool isActive = false;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        int BlinkDelay = 30;
        int Blink = 0;
        bool isBlinking = false;

        private KeyboardState lastKey, key;
        private MouseState lastMouse, mouse;

        public Vector2 Pos;
        private Vector2 size;

        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        public TextBoxClass(ContentManager Content, GraphicsDevice graphics, Vector2 Pos, Vector2 Size)
        {
            this.graphic = graphics;
            this.Content = Content;

            BG = new Texture2D(graphics, 1, 1);
            BG.SetData(new Color[] { new Color(Color.Black, 120) });

            Border = new Texture2D(graphics, 1, 1);
            Border.SetData(new Color[] { Color.White });

            Font = Content.Load<SpriteFont>("MapInfo");

            this.Size = Size;
            this.Pos = Pos;
        }

        public void Update(MouseState mouse, KeyboardState key)
        {
            lastKey = this.key;
            this.key = key;

            lastMouse = this.mouse;
            this.mouse = mouse;

            float ScaleFullX = (float)Game1.WindowViewWidth / (float)Game1.WindowWidth;
            float ScaleFullY = (float)Game1.WindowViewHeight / (float)Game1.WindowHeight;
            ScaleFullX = Game1.IsFullScreen ? ScaleFullX : 1f;
            ScaleFullY = Game1.IsFullScreen ? ScaleFullY : 1f;
            if (this.mouse.LeftButton == ButtonState.Pressed && this.lastMouse.LeftButton == ButtonState.Released)
            {
                if (new Rectangle(this.mouse.X, this.mouse.Y, 1, 1).Intersects(new Rectangle((int)(Pos.X * ScaleFullY), (int)(Pos.Y * ScaleFullY), (int)(Size.X * ScaleFullY), (int)(Size.Y * ScaleFullY))))
                {
                    IsActive = true;
                }
                else
                {
                    isActive = false;
                }
            }

            if (this.mouse.RightButton == ButtonState.Pressed && this.lastMouse.RightButton == ButtonState.Released)
            {
                if (new Rectangle(this.mouse.X, this.mouse.Y, 1, 1).Intersects(new Rectangle((int)(Pos.X * ScaleFullY), (int)(Pos.Y * ScaleFullY), (int)(Size.X * ScaleFullY), (int)(Size.Y * ScaleFullY))))
                {
                    Text = "";
                    IsActive = true;
                }
                else
                {
                    isActive = false;
                }
            }

            if (IsActive)
            {
                Keys[] k = this.key.GetPressedKeys();
                foreach (Keys K in k)
                {
                    if (this.key.IsKeyDown(K) && lastKey.IsKeyUp(K) && (IsKeyAChar(K) || IsKeyADigit(K)))
                    {
                        if (Text.Length < (Size.X / (Game1.ScaleVector2(Font.MeasureString("M"), graphic.Viewport.Height / 720).X + 2)))
                        {
                            Text += K.ToString();
                        }
                    }
                    else if (this.key.IsKeyDown(Keys.Back) && lastKey.IsKeyUp(Keys.Back))
                    {
                        if (Text.Length > 0)
                        {
                            Text = Text.Substring(0, Text.Length - 1);
                        }
                    }
                }
            }
            Blinking();
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(BG, new Rectangle((int)Pos.X, (int)Pos.Y, (int)Size.X, (int)Size.Y), Color.White);
            Game1.DrawRectangle(batch, Border, new Rectangle((int)Pos.X, (int)Pos.Y, (int)Size.X, (int)Size.Y), Color.White);
            Vector2 vec = Font.MeasureString(Text);
            vec = Game1.ScaleVector2(vec, graphic.Viewport.Height / 720);
            batch.DrawString(Font, Text, new Vector2(Pos.X + (Size.X / 2 - vec.X / 2), Pos.Y + (Size.Y / 2 - vec.Y / 2) + 3), Color.White,
                0f, new Vector2(0), graphic.Viewport.Height / 720, SpriteEffects.None, 0f);
            if (isBlinking)
            {
                batch.Draw(Border, new Rectangle((int)(Pos.X + (Size.X / 2 - vec.X / 2)) + (int)vec.X + 2, (int)(Pos.Y + (Size.Y / 2 - vec.Y / 2)), 1, (int)vec.Y), Color.White);
            }
        }

        private void Blinking()
        {
            if (isActive)
            {
                if (Text.Length < (Size.X / Game1.ScaleVector2(Font.MeasureString("M"), graphic.Viewport.Height / 720).X + 5))
                {
                    Blink++;
                    if (Blink >= BlinkDelay)
                    {
                        isBlinking = isBlinking ? false : true;
                        Blink = 0;
                    }
                }
                else
                {
                    Blink = 0;
                    isBlinking = false;
                }
            }
            else
            {
                Blink = 0;
                isBlinking = false;
            }
        }

        public static bool IsKeyAChar(Keys key)
        {
            return key >= Keys.A && key <= Keys.Z;
        }

        public static bool IsKeyADigit(Keys key)
        {
            return (key >= Keys.D0 && key <= Keys.D9);
        }
    }
}
