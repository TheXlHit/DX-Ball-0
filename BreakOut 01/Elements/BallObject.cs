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
    public class BallObject
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Size;

        public bool IsVisible = true;
        public bool IsStuck = true;

        private Vector2 movNeg = new Vector2();
        private Vector2 movPos = new Vector2();

        private Vector2 movement = new Vector2(0, 0);

        public Vector2 Movement {
            get {
                return movement;
            }
            set {
                movement = value;
                movPos = value;
                movNeg.X = value.X * -1;
                movNeg.Y = value.Y * -1;
            }
        }

        public BallObject(Texture2D Texture, Vector2 Pos, Vector2 Size)
        {
            this.Texture = Texture;
            this.Position = Pos;
            this.Size = Size;
        }

        public BallObject()
        { }

        public void StartMove()
        {
            int tMovX = new Random().Next(-51, 52);
            int tMovY = new Random().Next(0, 52);
            Movement = new Vector2(((float)tMovX / 50f), 2f);
        }

        public void Draw(SpriteBatch batch)
        {
            if (IsVisible)
            {
                batch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White);
                if (Game1.ShowHitbox)
                {
                    Game1.DrawRectangle(batch, Game1.HitLighting, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White);
                }
            }
        }

        internal void RevertMovement(BallDirection Direction)
        {
            switch (Direction)
            {
                case BallDirection.Up:
                    {
                        movement.Y = Math.Abs(movement.Y);
                        //Position.Y -= 2f;
                    }
                    break;
                case BallDirection.Down:
                    {
                        movement.Y = Math.Abs(movement.Y) * -1;
                        //Position.Y -= 2f;
                    }
                    break;
                case BallDirection.Left:
                    {
                        movement.X = Math.Abs(movement.X);
                        //Position.X += 2f;
                    }
                    break;
                case BallDirection.Right:
                    {
                        movement.X = Math.Abs(movement.X) * -1;
                        //Position.X += 2f;
                    }
                    break;
            }
        }

        public enum BallDirection
        {
            None = 0,
            Up,
            Down,
            Left,
            Right,
        };
    }
}
