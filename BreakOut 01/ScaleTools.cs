using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace BreakOut_01
{
    public class ScaleTools
    {
        GraphicsDevice graphic;
        int DefaultX = 800;
        int DefaultY = 600;

        public ScaleTools(GraphicsDevice graphic, int DefaultX, int DefaultY)
        {
            this.graphic = graphic;
            this.DefaultX = DefaultX;
            this.DefaultY = DefaultY;
        }

        public GraphicsDevice SetGraphicsDevice
        {
            set
            {
                graphic = value;
            }
        }

        public float ScaleX(float value)
        {
            return graphic.Viewport.Width / (DefaultX / value);
        }

        public float ScaleY(float value)
        {
            return graphic.Viewport.Height / (DefaultY / value);
        }

        public float GetWindowScaleX
        { get { return graphic.Viewport.Width / DefaultX; } }

        public float GetWindowScaleY
        { get { return graphic.Viewport.Height / DefaultY; } }

        public Vector2 GetWindowScale
        { get { return new Vector2(GetWindowScaleX, GetWindowScaleY); } }

        public Vector2 ScaledVector2(float x, float y)
        {
            return new Vector2(ScaleX(x), ScaleY(y));
        }

        public Vector2 ScaledVector2(float x)
        {
            return new Vector2(ScaleX(x), ScaleY(x));
        }
    }

    public class Resolution
    {
        public float Width = 800;
        public float Height = 600;
    }
}
