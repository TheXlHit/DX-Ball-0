using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BreakOut_01.Elements
{
    public class FPSCounter
    {
        float _fpsTotal = 0;
        float _fps = 0;
        float _fpsSecond = 0;

        public void ComputeFPS()
        {
            if (_fpsSecond >= 1000)
            {
                _fps = _fpsTotal;
                _fpsTotal = 0;
                _fpsSecond = 0;
            }
            _fpsTotal++;
        }

        public void UpdateFPS(GameTime gameTime)
        {
            _fpsSecond += gameTime.ElapsedGameTime.Milliseconds;
        }

        public float FPS
        {
            get
            {
                return _fps - 3;
            }
        }
    }
}
