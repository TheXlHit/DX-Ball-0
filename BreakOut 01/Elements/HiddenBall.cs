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
    public class HiddenBall : BallObject
    {
        public int Time = 2;

        public HiddenBall(Vector2 pos, Vector2 size)
        {
            this.Position = pos;
            this.Size = size;
        }

        public HiddenBall(BallObject ball)
        {
            this.Position = ball.Position;
            this.Size = ball.Size;
        }

        public new void Draw(SpriteBatch batch)
        {
            if (Game1.ShowHitbox)
            {
                Game1.DrawRectangle(batch, Game1.HitLighting, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White);
            }
        }
    }
}
