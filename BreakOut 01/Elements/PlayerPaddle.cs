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
    public class PlayerPaddle
    {
        public Vector2 Position;
        private Vector2 OriginPosition;
        public float BallSpeed = 3f;
        public Vector2 BallPos;
        public Vector2 Size;
        public Texture2D Texture;

        public float Speed = 8;

        private ContentManager Content;
        private GraphicsDevice graphic;


        public PlayerPaddle(ContentManager Content, GraphicsDevice graphic, Vector2 Pos, Vector2 Size)
        {
            Position = OriginPosition = Pos;
            BallPos = new Vector2(Size.X / 2 + Pos.X - 4, Pos.Y - 18);
            this.Size = Size;
            this.graphic = graphic;
            this.Content = Content;
            this.Texture = Content.Load<Texture2D>("paddle");

            Speed = graphic.Viewport.Width / (1280 / Speed);
        }

        public void SetBallPos()
        {
            BallPos = new Vector2(Size.X / 2 + Position.X - 4, Position.Y - graphic.Viewport.Height / 40f);
        }

        public void Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    {
                        Position.X -= Speed;
                        SetBallPos();
                    }
                    break;
                case Direction.Right:
                    {
                        Position.X += Speed;
                        SetBallPos();
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White);
            if (Game1.ShowHitbox)
            {
                Game1.DrawRectangle(batch, Game1.HitLighting, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White);
            }
        }

        public enum Direction
        {
            None,
            Left,
            Right,
        };
    }
}
